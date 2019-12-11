using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace JinhuaBarOLLib
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的类名“Service1”。
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode=ConcurrencyMode.Reentrant)]
    public class GameService : IGameService
    {
        Room roomList = new Room();
        static Dictionary<string, Player> playerDictionary = new Dictionary<string, Player>();
        public GameService()
        {

        }
        public bool Login(string name)
        {
            if (playerDictionary.ContainsKey(name))
            {
                playerDictionary[name].CallBack= OperationContext.Current.GetCallbackChannel<ICallBack>();
                playerDictionary[name].CallBack.ShowMessage("请输入密码");
                if (playerDictionary[name].CallBack.PlayerControl() == playerDictionary[name].PassWord)
                {
                    return true;
                }
            }
            else
            {
                Player player = new Player(name);
                player.CallBack = OperationContext.Current.GetCallbackChannel<ICallBack>();
                player.CallBack.ShowMessage("新注册用户，请设置您的密码：");
                player.PassWord = player.CallBack.PlayerControl();
                playerDictionary[name] = player;
                return true;
            }
            return false;
        }
        public string ShowInformation(string name)
        {
            Player player = playerDictionary[name];
            string information = "用户名：" + player.Name + "\n";
            information += "密码：" + player.PassWord + "\n";
            information += "筹码数：" + player.Chips + "\n";
            information += "总场次：" + player.TotalRounds + "\n";
            information += "总胜场：" + player.WinRounds + "\n";
            information += "胜率：" + (Convert.ToDouble(player.WinRounds)/Convert.ToDouble(player.TotalRounds)) + "%\n";
            return information;
        }
        public void CteateRoom(string name, bool isOffLine)
        {
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
            }
            else//PVP
            {

            }
            player.RoomName = player.Name;
            player.CallBack = OperationContext.Current.GetCallbackChannel<ICallBack>();
            roomList.Rooms.Add(player.RoomName, dealer);
            player.CallBack.ShowMessage("创建房间成功");
        }
        public string ShowRoomList()
        {
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
        public string JoinRoom(string name, string roomName)
        {
            Player player = playerDictionary[name];
            player.RoomName = roomName;
            player.CallBack = OperationContext.Current.GetCallbackChannel<ICallBack>();
            foreach (Player otherPlayer in roomList.Rooms[roomName].OrderedPlayers)
            {
                otherPlayer.CallBack.ShowMessage("玩家{0}加入房间" + player.Name);
            }
            roomList.Rooms[roomName].OrderedPlayers.Enqueue(player);
            return "加入房间成功，当前房间人数:" + roomList.Rooms[roomName].OrderedPlayers.Count;
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
