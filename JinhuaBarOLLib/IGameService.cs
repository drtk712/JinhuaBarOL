using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
namespace JinhuaBarOLLib
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService1”。
    //[ServiceContract(SessionMode =SessionMode.Required,CallbackContract =typeof(ICallBack))]
    [ServiceContract(SessionMode = SessionMode.Required,CallbackContract =typeof(ICallBack))]
    public interface IGameService
    {
        // TODO: 在此添加您的服务操作
        [OperationContract]
        bool Login(string name);
        [OperationContract]
        string ShowInformation(string name);
        [OperationContract]//玩家创建房间
        void CteateRoom(string name, bool isOffLine);
        [OperationContract]
        string ShowRoomList(string name);
        [OperationContract]
        string JoinRoom(string name, string roomName);
        [OperationContract]
        string LeaveRoom(string name, string roomName);
        [OperationContract]
        string StartGame(string name);
        [OperationContract]
        string ResetGame(string name);
        [OperationContract]
        Player test(Player player);

    }

    public interface ICallBack
    {
        [OperationContract(IsOneWay = true)]
        void ShowMessage(string message);
        [OperationContract(IsOneWay = false)]
        string PlayerControl();
        [OperationContract(IsOneWay = true)]
        void ClearScreen();
    }
}
