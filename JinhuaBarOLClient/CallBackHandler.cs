using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinhuaBarOLClient
{
    class CallBackHandler : GameService.IGameServiceCallback
    {
        public void ClearScreen()
        {
            Console.Clear();
        }

        public string PlayerControl()
        {
            return Console.ReadLine();
        }

        public void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
