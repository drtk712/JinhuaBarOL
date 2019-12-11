using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
namespace JinhuaBarOLLib
{
    [DataContract]
    class Room
    {
        private Dictionary<string, Dealer> rooms = new Dictionary<string, Dealer>();
        [DataMember]
        public Dictionary<string, Dealer> Rooms
        {
            get { return rooms; }
            set { rooms = value; }
        }
    }
}
