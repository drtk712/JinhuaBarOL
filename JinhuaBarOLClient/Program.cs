using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using JinhuaBarOLClient.GameService;
using JinhuaBarOLLib;

namespace JinhuaBarOLClient
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                Console.OutputEncoding = Encoding.UTF8;
                InstanceContext instanceContext = new InstanceContext(new CallBackHandler());
                GameServiceClient client = new GameServiceClient(instanceContext);

                Console.WriteLine("欢迎来到本游戏，请输入你的昵称：");
                string name = Console.ReadLine();
                while (!client.Login(name))
                {
                    Console.WriteLine("请重新输入");
                }
                Console.WriteLine(client.ShowInformation(name));
                Console.WriteLine(client.ShowRoomList());
                Console.ReadKey();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
