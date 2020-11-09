using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/*
 * I probably spent way more time doing this incorrectly than I did doing it correctly,
 * if you want a good laugh then go down towards the bottom of this program and look at some
 * of the return values I was using lol.
 */

namespace Assignment_9
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        private Dictionary<String, int> map;
        private Dictionary<String, int> reduce;
        private Dictionary<String, int> combine;
        private static List<Dictionary<string,int>> threadResults;
        int n;

        protected void Page_Load(object sender, EventArgs e)
        {
            map = new Dictionary<string, int>();
            reduce = new Dictionary<string, int>();
            combine = new Dictionary<string, int>();
            threadResults = new List<Dictionary<string, int>>();
            n = 1;
        }

        //perform mapreduce
        protected void Button1_Click(object sender, EventArgs e)
        {
            ResultLabel.Text = "Result:";
            try
            {
                n = Int32.Parse(ThreadTextBox.Text);    //number of threads

                List<String> words = new List<string>();    //list of all the individual words in file
                StreamReader reader = new StreamReader(Server.MapPath("~/") + @"App_Data\\FileToCount.txt");    //file
                
                String line = reader.ReadLine();    //read the first line
                while (line != null)
                {
                    String[] w = line.Split(' ');   //split line at a space
                    for(int i=0; i<w.Length; i++)
                    {
                        words.Add(w[i]);    //add the word to the words list
                    }

                    line = reader.ReadLine();   //get the next line
                }

                reader.Close(); //close the file

                List<String[]> dividedWords = new List<string[]>();  //list of string arrays to hold the info for each thread

                //split the file for map
                for (int i=0; i<n; i++)
                {
                    /*
                     * Gives the starting position for each chunk of the list of words that each thread will take
                     *      i=0 -> 0
                     *      i=1 -> words.Count()/n
                     *      i=2 -> 2 * words.Count()/n
                     */
                    int start = i * (words.Count() / n);

                    List<String> temp = new List<string>();
                    for(int j=start; j<start + (words.Count() / n); j++)
                    {
                        temp.Add(words[j]);
                    }

                    dividedWords.Add(temp.ToArray());
                }

                /*for(int i=0; i<dividedWords.Count(); i++)
                {
                    for(int j=0; j<dividedWords[i].Length; j++)
                    {
                        Debug.WriteLine(dividedWords[i][j]);
                    }
                    Debug.WriteLine("-----------------------------------------------------------------------");
                }*/

                /*/big long name thing to hold some unknown number of tasks
                List<Task<List<Tuple<string, int>>>> mappings = new List<Task<List<Tuple<string, int>>>>();
                for(int i=0; i<n; i++)
                {
                    //perform the task which is asynchronus
                    Task<List<Tuple<string, int>>> temp = getMap(dividedWords[i]);
                    mappings.Add(temp); //add it to the list
                }

                //now that all the mapping tasks have been started, it is time to wait for them to complete
                List<List<Tuple<string, int>>> mappingResults = new List<List<Tuple<string, int>>>(); //list of lists to hold mapping returns
                for (int i=0; i<mappings.Count(); i++)
                {
                    var res = await mappings[i];
                    mappingResults.Add();
                }*/


                //call MapReduce function to call the web service
                reduce = MapReduce(dividedWords.ToArray());

                //display the result
                foreach(String key in reduce.Keys)
                {
                    ResultLabel.Text += "<br>" + key + "&emsp;" + reduce[key];
                }






                /*
                var mapped = getMapping(dividedWords);
                var map = mapped.Result;
                var reduced = getReduced(map);
                var red = reduced.Result;
                var comb = getCombined(red);
                combine = comb.Result;

                foreach(String key in combine.Keys)
                {
                    ResultLabel.Text += "\r\n" + key + "\t\t" + combine[key];
                }

                Debug.WriteLine("Finished");
                */

            }
            catch
            {
                ResultLabel.Text = "There has been an error, please try again";
            }
        }

        //upload file
        protected void Button3_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                try
                {
                    String filename = Path.GetFileName(FileUpload1.FileName);
                    FileUpload1.SaveAs(Server.MapPath("~/") + @"App_Data\\FileToCount.txt");
                }
                catch(Exception ex)
                {
                    ResultLabel.Text = ex.ToString();
                }
            }
            else
            {
                ResultLabel.Text = "Result: Failure";
            }
        }

        public Dictionary<string, int> MapReduce(String[][] words)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();

            List<Thread> threads = new List<Thread>();
            for(int i=0; i<words.Length; i++)
            {
                threadResults.Add(new Dictionary<string, int>());
            }

            //start threads
            for(int i=0; i<words.Length; i++)
            {
                Thread thread = new Thread(Foo);
                thread.Start(new Tuple<int, String[]>(i, words[i]));
                threads.Add(thread);
            }
            //wait for them to stop
            foreach(var t in threads)
            {
                t.Join();
            }

            for(int i=0; i<threadResults.Count(); i++)
            {
                foreach(String key in threadResults[i].Keys)
                {
                    if (!result.ContainsKey(key)) result.Add(key, threadResults[i][key]);
                    else result[key] += threadResults[i][key];
                }
            }

            return result;
        }

        private static void Foo(object data)
        {
            MapReduceService.ServiceClient ser = new MapReduceService.ServiceClient();
            Tuple<int, String[]> temp = (Tuple<int, String[]>)data;
            threadResults[temp.Item1] = ser.MapReduce(temp.Item2);
            ser.Close();
        }


        /*
         * 
         * All of this down here is old and a testament to my stupidity, just look at some of those return values
         * 
         */




        async Task<List<List<Tuple<string,int>>>> getMapping(List<String[]> dividedWords)
        {
            MapReduceService.ServiceClient ser = new MapReduceService.ServiceClient();

            //start all tasks in parallel
            List<Task<Tuple<string, int>[]>> mappings = new List<Task<Tuple<string, int>[]>>();
            for(int i=0; i<dividedWords.Count(); i++)
            {
                Debug.WriteLine("Starting strange task");
                Task<Tuple<string, int>[]> temp = Task<Tuple<string, int>[]>.Factory.StartNew(() => ser.Map(dividedWords[i]));
                Debug.WriteLine("Ending strange task and adding");
                mappings.Add(temp);
                Debug.WriteLine("Adding");
                //Debug.WriteLine("Starting Async Map");
                //mappings.Add(ser.MapAsync(dividedWords[i]));
            }

            //wait for all tasks to finish
            List<List<Tuple<string,int>>> result = new List<List<Tuple<string, int>>>();
            for (int i=0; i<mappings.Count(); i++)
            {
                Debug.WriteLine("About to await");
                var temp = mappings[i].Result;
                Debug.WriteLine("Awaited");
                List<Tuple<string, int>> res = new List<Tuple<string, int>>();
                for (int j = 0; j < temp.Length; j++)
                {
                    res.Add(temp[j]);
                }

                result.Add(res);
                Debug.WriteLine("Added one map");
            }
            Debug.WriteLine("Mapping Done");

            ser.Close();
            return result;
        }

        async Task<List<Dictionary<string,int>>> getReduced(List<List<Tuple<string, int>>> mapped)
        {
            MapReduceService.ServiceClient ser = new MapReduceService.ServiceClient();

            //start all tasks in parallel
            List<Task<Dictionary<string, int>>> reducings = new List<Task<Dictionary<string, int>>>();
            for(int i=0; i<mapped.Count(); i++)
            {
                reducings.Add(ser.ReduceAsync(mapped[i].ToArray()));
            }

            List<Dictionary<string, int>> result = new List<Dictionary<string, int>>();
            for(int i=0; i<reducings.Count(); i++)
            {
                result.Add(await reducings[i]);
            }
            Debug.WriteLine("Reducing Done");
            ser.Close();
            return result;
        }

        async Task<Dictionary<string,int>> getCombined(List<Dictionary<string,int>> reduced)
        {
            MapReduceService.ServiceClient ser = new MapReduceService.ServiceClient();

            Dictionary<string, int> result = await ser.CombineAsync(reduced.ToArray());
            Debug.WriteLine("Combining Done");
            ser.Close();
            return result;
        }
    }
}