using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service" in code, svc and config file together.
public class Service : IService
{
	public Dictionary<string, int> MapReduce(String[] words)
	{
		Dictionary<string, int> result = new Dictionary<string, int>();

		for(int i=0; i<words.Length; i++)
		{
			if (!result.ContainsKey(words[i])) result.Add(words[i], 1);
			else result[words[i]] += 1;
		}

		return result;
	}

	public List<Tuple<String, int>> Map(String[] words)
	{
		List<Tuple<String, int>> mapped = new List<Tuple<string, int>>();

		for (int i = 0; i < words.Length; i++)
		{
			mapped.Add(new Tuple<String, int>(words[i], 1));
		}

		return mapped;
	}

	public Dictionary<String, int> Reduce(List<Tuple<String, int>> mapped)
	{
		Dictionary<String, int> reduced = new Dictionary<string, int>();

		foreach (var item in mapped)
		{
			if (!reduced.ContainsKey(item.Item1)) reduced.Add(item.Item1, item.Item2);
			else reduced[item.Item1] += item.Item2;
		}

		return reduced;
	}

	public Dictionary<String, int> Combine(Dictionary<String, int>[] reduced)
	{
		Dictionary<String, int> combined = new Dictionary<string, int>();

		for (int i = 0; i < reduced.Length; i++)
		{
			foreach (var item in reduced[i])
			{
				if (!combined.ContainsKey(item.Key)) combined.Add(item.Key, item.Value);
				else combined[item.Key] += item.Value;
			}
		}

		return combined;
	}
}
