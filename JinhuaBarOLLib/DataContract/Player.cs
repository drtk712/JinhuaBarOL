using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel;
namespace JinhuaBarOLLib
{
    [DataContract]
    public class Player
    {
        public Player() { }
        public Player(string name)
        {
            this.name = name;
        }
        private string name;
        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string passWord;
        public string PassWord
        {
            get { return passWord; }
            set { passWord = value; }
        }
        private int totalRounds=0;
        public int TotalRounds
        {
            get { return totalRounds; }
            set { totalRounds = value; }
        }
        private int winRounds;
        public int WinRounds
        {
            get { return winRounds; }
            set { winRounds = value; }
        }
        private string roomName;
        [DataMember]
        public string RoomName
        {
            get { return roomName; }
            set { roomName = value; }
        }
        private int chips = 10000;
        [DataMember]
        public int Chips
        {
            get { return chips; }
            set { chips = value; }
        }
        private int myBet = 0;
        [DataMember]
        public int MyBet
        {
            get { return myBet; }
            set { myBet = value; }
        }
        private List<Card> cards = new List<Card>();
        [DataMember]
        public List<Card> Cards
        {
            get { return cards; }
            set
            {
                cards = value;
            }
        }
        private CardType cardType;
        [DataMember]
        public CardType CardType
        {
            get { return cardType; }
            set { cardType = value; }
        }
        private bool isSee = false;
        [DataMember]
        public bool IsSee
        {
            get { return isSee; }
            set { isSee = value; }
        }
        [DataMember]
        public string IsSee2String
        {
            get
            {
                if (isSee)
                    return "已看牌";
                else
                    return "未看牌";
            }
            set
            {
                return;
            }
        }
        private bool isGiveUp = false;
        [DataMember]
        public bool IsGiveUp
        {
            get { return isGiveUp; }
            set { isGiveUp = value; }
        }
        private ICallBack callBack;
        [DataMember]
        internal ICallBack CallBack
        {
            get { return callBack; }
            set { callBack = value; }
        }
        public virtual void Operate()
        {
            if (!isGiveUp)
            {
                callBack.ShowMessage("----------玩家" + name + "，请选择你的操作----------");
                ShowHand();
                string menu = "";
                menu += "----------1.跟注(剩余筹码" + chips + "，已投注" + myBet + ")\n";
                menu += "----------2.加注\n";
                if (isSee)
                    menu += "----------3.已经看牌----------\n";
                else
                    menu += "----------3.看牌-----------\n";
                menu += "----------4.弃牌-----------\n";
                menu += "----------5.开！-----------\n";
                callBack.ShowMessage(menu);
                //输入判断
                while (true)
                {
                    string input = callBack.PlayerControl();
                    if (input == "1")
                    {
                        if (OnCall())
                        {
                            break;
                        }
                    }
                    else if (input == "2")
                    {
                        if (OnAddBet())
                        {
                            break;
                        }
                    }
                    else if (input == "3")
                    {
                        if (isSee)
                        {
                            callBack.ShowMessage("你已看牌，咋滴，还想看别人的牌？垃圾");
                        }
                        else
                        {
                            OnSee();
                            callBack.ShowMessage("请继续操作");
                        }

                    }
                    else if (input == "4")
                    {
                        OnGiveUp();
                        break;
                    }
                    else if (input == "5")
                    {
                        OnOpen();
                        break;
                    }
                    else
                    {
                        callBack.ShowMessage("请输入1~4的数字，并按下回车");
                    }
                }
            }
            else
            {
                callBack.ShowMessage("您已弃牌，跳过操作");
            }
        }
        public delegate bool CallHandler(Player player);
        public event CallHandler Call;
        public bool OnCall()
        {
            return Call(this);
        }
        public delegate bool AddBetHandler(Player player);
        public event AddBetHandler AddBet;
        public bool OnAddBet()
        {
            return AddBet(this);
        }
        public delegate void SeeHandler(Player player);
        public event SeeHandler See;
        public void OnSee()
        {
            See(this);
        }
        public delegate void GiveUpHandler(Player player);
        public event GiveUpHandler GiveUp;
        public void OnGiveUp()
        {
            GiveUp(this);
        }
        public void ShowHand()
        {
            string result = "";
            if (isSee)
            {
                result += "┌─────┐ ┌─────┐ ┌─────┐\n";
                result += "│ " + cards[0].Number2String + "   │ │ " + cards[1].Number2String + "   │ │ " + cards[2].Number2String + "   │\n";
                result += "│  " + cards[0].Suit2Sharp + " │ │  " + cards[1].Suit2Sharp + " │ │  " + cards[2].Suit2Sharp + " │\n";
                result += "│     │ │     │ │     │\n";
                result += "└─────┘ └─────┘ └─────┘\n";
            }
            else
            {
                result += "┌─────┐ ┌─────┐ ┌─────┐\n";
                result += "│     │ │     │ │     │\n";
                result += "│  ?  │ │  ?  │ │  ?  │\n";
                result += "│     │ │     │ │     │\n";
                result += "└─────┘ └─────┘ └─────┘\n";
            }
            callBack.ShowMessage(result);
        }
        public delegate void OpenHandler(Player player);
        public event OpenHandler Open;
        public void OnOpen()
        {
            Open(this);
        }
        //重置上一局游戏信息
        public void Rest()
        {
            cards.Clear();
            isGiveUp = false;
            isSee = false;
            myBet = 0;
        }
        //判断自己的牌型
        public void JudgeCards()
        {
            bool isLeopard = false;
            bool isGoldeFlower = false;
            bool isJunko = false;
            bool isPair = false;
            //是否为豹子
            if (cards[0].Number == cards[1].Number && cards[0].Number == cards[2].Number)
            {
                isLeopard = true;
            }
            //是否为对子
            if (!isLeopard && (cards[0].Number == cards[1].Number || cards[0].Number == cards[2].Number || cards[1].Number == cards[2].Number))
            {
                isPair = true;
            }
            if (isLeopard)
            {
                cardType = CardType.Leopard;
                return;
            }
            else if (isPair)
            {
                cardType = CardType.Pair;
                return;
            }
            //是否为金花
            if (cards[0].Suit == cards[1].Suit && cards[0].Suit == cards[2].Suit)
            {
                isGoldeFlower = true;
            }
            //是否为顺子
            if ((cards[2].Number - cards[1].Number) == 1 && (cards[1].Number - cards[0].Number) == 1)
            {
                isJunko = true;
            }
            if (isGoldeFlower && isJunko)
            {
                cardType = CardType.Flush;
            }
            else if (isGoldeFlower)
            {
                cardType = CardType.GoldeFlower;
            }
            else if (isJunko)
            {
                cardType = CardType.Junko;
            }
            else
            {
                cardType = CardType.Single;
            }
        }

    }
}
