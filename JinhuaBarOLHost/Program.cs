using JinhuaBarOLLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace JinhuaBarOLHost
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost selfHost = new ServiceHost(typeof(GameService));
            try
            {
                selfHost.Open();
                Console.WriteLine("The service is ready.");
                Console.WriteLine("Press <Enter> to terminate the service.");
                Console.WriteLine();
                Console.ReadKey();
                selfHost.Close();
            }
            catch (CommunicationException ce)
            {
                Console.WriteLine("An exception occurred: {0}", ce.Message);
                selfHost.Abort();
            }
        }
    }
}
