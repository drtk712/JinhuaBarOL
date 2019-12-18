using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace JinhuaBarOLLib
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的类名“Service1”。
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode=ConcurrencyMode.Reentrant)]
    public class GameService : IGameService
    {
        Room roomList = new Room();
        static Dictionary<string, Player> playerDictionary = new Dictionary<string, Player>();
        public GameService()
        {

        }
        //工具，方便在host端输出消息
        public static void Log(string message)
        {
            Console.WriteLine(DateTime.Now + "--------->" + message);
        }
        //用户注册或者登录
        public bool Login(string name)
        {
            //如果有账号，便进行登录
            if (playerDictionary.ContainsKey(name))
            {
                playerDictionary[name].CallBack= OperationContext.Current.GetCallbackChannel<ICallBack>();
                playerDictionary[name].CallBack.ShowMessage("请输入密码");
                if (playerDictionary[name].CallBack.PlayerControl() == playerDictionary[name].PassWord)
                {
                    Log(string.Format("玩家 {0} 登录成功", name));
                    return true;
                }
            }
            //没有账号的话进行注册
            else
            {
                Player player = new Player(name);
                player.CallBack = OperationContext.Current.GetCallbackChannel<ICallBack>();
                player.CallBack.ShowMessage("新注册用户，请设置您的密码：");
                player.PassWord = player.CallBack.PlayerControl();
                playerDictionary[name] = player;
                Log(string.Format("玩家 {0} 注册成功", name));
                return true;
            }
            Log(string.Format("玩家 {0} 登录失败", name));
            return false;
        }
        //查看个人信息
        public string ShowInformation(string name)
        {
            Log(string.Format("玩家 {0} 查看个人信息", name));
            Player player = playerDictionary[name];
            string information = "用户名：" + player.Name + "\n";
            information += "密码：" + player.PassWord + "\n";
            information += "筹码数：" + player.Chips + "\n";
            information += "总场次：" + player.TotalRounds + "\n";
            information += "总胜场：" + player.WinRounds + "\n";
            information += "胜率：" + (Convert.ToDouble(player.WinRounds)/Convert.ToDouble(player.TotalRounds));
            return information;
        }
        //创建房间，pvp或者pve
        public void CteateRoom(string name, bool isOffLine)
        {
            Log(string.Format("玩家 {0} 创建 {1} 房间", name,isOffLine?"PVE":"PVP"));
            Player player = playerDictionary[name];
            Dealer dealer = new Dealer();
            dealer.IsOffLine = isOffLine;
            if (isOffLine)//PVE
            {
                dealer.OrderedPlayers.Enqueue(player);
                for (int i = 1; i <= 4; i++)
                {
                    dealer.OrderedPlayers.Enqueue(new AIPlayer("robot" + i));
                }
                player.RoomName = player.Name;
                player.CallBack = OperationContext.Current.GetCallbackChannel<ICallBack>();
                roomList.Rooms.Add(player.RoomName, dealer);
                player.CallBack.ShowMessage("PVE创建房间成功，自动开始游戏");
                Log(string.Format("房间 {0} 开始游戏", player.RoomName));
                roomList.Rooms[player.RoomName].StartGame();
            }
            else//PVP
            {
                dealer.OrderedPlayers.Enqueue(player);
                player.RoomName = player.Name;
                player.CallBack = OperationContext.Current.GetCallbackChannel<ICallBack>();
                roomList.Rooms.Add(player.RoomName, dealer);
                player.CallBack.ShowMessage("创建PVP房间成功，请等待玩家加入");
            }
        }
        //查看所有房间
        public string ShowRoomList(string name)
        {
            Log(string.Format("玩家 {0} 查看房间列表", name));
            if (roomList.Rooms.Count == 0)
            {
                return "暂时没有房间";
            }
            string roomListString = "名称     人数     创建者\n";
            foreach (string room in roomList.Rooms.Keys)
            {
                roomListString += room + "    " + roomList.Rooms[room].OrderedPlayers.Count + "    " + roomList.Rooms[room].OrderedPlayers.Peek().Name + "\n";
            }
            return roomListString;
        }
        //玩家加入房间
        public string JoinRoom(string name, string roomName)
        {
            //不存在该房间
            if (roomList.Rooms[roomName] == null)
            {
                Log(string.Format("玩家 {0} 尝试加入房间 {1} 失败，原因：房间不存在", name,roomName));
                return "对不起，不存在该房间";
            }
            //存在该房间
            else
            {
                //房间未满
                if (roomList.Rooms[roomName].OrderedPlayers.Count <= 4)
                {
                    Player player = playerDictionary[name];
                    player.RoomName = roomName;
                    player.CallBack = OperationContext.Current.GetCallbackChannel<ICallBack>();
                    foreach (Player otherPlayer in roomList.Rooms[roomName].OrderedPlayers)
                    {
                        otherPlayer.CallBack.ShowMessage("玩家"+ player.Name + "加入房间");
                    }
                    roomList.Rooms[roomName].OrderedPlayers.Enqueue(player);

                    //如果房间正好满了，自动开始游戏
                    if (roomList.Rooms[roomName].OrderedPlayers.Count == 5)
                    {
                        Log(string.Format("玩家 {0} 尝试加入房间 {1} 成功", name, roomName));
                        Log(string.Format("房间 {0} 开始游戏", roomName));
                        roomList.Rooms[roomName].StartGame();
                        return "";
                    }
                    else
                    {
                        Log(string.Format("玩家 {0} 尝试加入房间 {1} 成功", name, roomName));
                        return "加入房间成功，当前房间人数： " + roomList.Rooms[roomName].OrderedPlayers.Count;
                    }
                }
                //房间已满
                else
                {
                    Log(string.Format("玩家 {0} 尝试加入房间 {1} 失败，原因：房间已满", name, roomName));
                    return "对不起，该房间已满";
                }

            }
        }
        public string LeaveRoom(string name, string roomName)
        {

            return "";
        }
        public string StartGame(string name)
        {
            Player player = playerDictionary[name];
            roomList.Rooms[player.RoomName].StartGame();
            return "";
        }
        public string ResetGame(string name)
        {
            
            return "";
        }
        public Player test(Player player)
        {
            player.RoomName = "test";
            return player;
        }
    }
}
