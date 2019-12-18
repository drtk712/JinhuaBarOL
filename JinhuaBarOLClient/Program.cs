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
        
        public static InstanceContext instanceContext = new InstanceContext(new CallBackHandler());
        public static GameServiceClient client = new GameServiceClient(instanceContext);
        public static string name = "";
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            try
            {
                Console.WriteLine("*******欢迎来到本游戏******");
                Console.WriteLine("请先进行登录");
                Console.Write("请输入用户名： ");
                name = Console.ReadLine();
                while (!client.Login(name))
                {
                    Console.WriteLine("请重新输入");
                }

                Console.WriteLine("请选择你要进行的操作");
                Console.WriteLine("1.开始PVE游戏\n2.创建PVP房间\n3.加入PVP房间\n4.显示房间列表\n5.显示游戏规则\n6.查看个人信息\n7.退出");
                while (true)
                {
                    string option = Console.ReadLine();
                    if (option == "1")
                    {
                        StartPVE();
                        break;
                    }
                    else if (option == "2")
                    {
                        CreateRoom();
                        break;
                    }
                    else if (option == "3")
                    {
                        JoinRoom();
                        break;
                    }
                    else if (option == "4")
                    {
                        ShowRoomList();
                    }
                    else if (option == "5")
                    {
                        ShowRule();
                    }
                    else if (option == "6")
                    {
                        ShowInformation();
                    }
                    else if (option == "7")
                    {
                        Exit();
                        break;
                    }
                    Console.WriteLine("1.开始PVE游戏\n2.创建PVP房间\n3.加入PVP房间\n4.显示房间列表\n5.显示游戏规则\n6.查看个人信息\n7.退出");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                client.Abort();
                Console.ReadKey();
            }
        }
        public static void StartPVE()
        {
            client.CteateRoom(name, true);
        }
        public static void CreateRoom()
        {
            client.CteateRoom(name, false);
            Console.WriteLine("请等待玩家进入");
            while (true)
            {
                
            }
        }
        public static void JoinRoom()
        {
            Console.WriteLine("请输入房间名称");
            Console.WriteLine(client.JoinRoom(name, Console.ReadLine()));
            Console.WriteLine("请等待玩家进入");
            while (true)
            {

            }
        }
        public static void ShowRoomList()
        {
            Console.WriteLine(client.ShowRoomList(name));
        }
        public static void ShowInformation()
        {
            Console.WriteLine(client.ShowInformation(name));
        }
        public static void ShowRule()
        {
            Console.WriteLine(@"
             诈金花游戏规则
    1.
    2.
    3.
    4.
    5.
                                ");
        }
        public static void Exit()
        {
            client.Close();
            Environment.Exit(0);
        }
    }
}
