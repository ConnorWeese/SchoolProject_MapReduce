using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService" in both code and config file together.
[ServiceContract]
public interface IService
{

	// TODO: Add your service operations here
	[OperationContract]
	List<Tuple<String, int>> Map(String[] words);

	[OperationContract]
	Dictionary<String, int> Reduce(List<Tuple<String, int>> mapped);

	[OperationContract]
	Dictionary<String, int> Combine(Dictionary<String, int>[] reduced);

	[OperationContract]
	Dictionary<string, int> MapReduce(String[] words);
}

// Use a data contract as illustrated in the sample below to add composite types to service operations.
[DataContract]
public class CompositeType
{
	bool boolValue = true;
	string stringValue = "Hello ";

	[DataMember]
	public bool BoolValue
	{
		get { return boolValue; }
		set { boolValue = value; }
	}

	[DataMember]
	public string StringValue
	{
		get { return stringValue; }
		set { stringValue = value; }
	}
}
