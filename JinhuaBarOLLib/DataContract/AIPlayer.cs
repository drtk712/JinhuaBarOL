using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinhuaBarOLLib
{
    class AIPlayer : Player
    {
        public AIPlayer(string name) : base(name)
        {

        }
        private int baseStrength = 100;
        private double winRate = 0.5;
        //private double investRate;
        public double InvestRate
        {
            get { return Convert.ToDouble(MyBet) / Convert.ToDouble(MyBet + Chips); }
        }
        //private double absoluteWinRate;
        public double AbsoluteWinRate
        {
            get
            {
                switch (CardType)
                {
                    case CardType.Single: return 0.1901;
                    case CardType.Pair: return 0.5197;
                    case CardType.Junko: return 0.8006;
                    case CardType.GoldeFlower: return 0.976;
                    case CardType.Flush: return 0.9894;
                    case CardType.Leopard: return 0.9989;
                }
                return 0;
            }
        }
        private int strength;
        public int Strength
        {
            get { return strength; }
            set { strength = value; }
        }
        private List<Player> otherPlayers = new List<Player>();
        public List<Player> OtherPlayers
        {
            get { return otherPlayers; }
            set { otherPlayers = value; }
        }
        private Dictionary<Player, int> otherPlayersStrength;
        public Dictionary<Player, int> OtherPlayersStrength
        {
            get { return otherPlayersStrength; }
            set { otherPlayersStrength = value; }
        }
        public override void Operate()
        {
            if (IsGiveUp)
            {
                Console.WriteLine("玩家{0}已弃牌，跳过操作", Name);
                return;
            }
            GetStrength();
            if (otherPlayersStrength == null)
            {
                GetOtherPlayersStrength();
            }
            else
            {
                CalculateOtherPlayersStrength();
            }
            CalculateWinRate();
            //Console.WriteLine("-------------玩家{0}------------", Name);
            //See();
            #region AI判断
            if (IsSee)
            {
                winRate = (winRate + AbsoluteWinRate) / 2;
                if (winRate > 0.4)
                {
                    if (InvestRate < 0.2)
                    {
                        OnCall();
                    }
                    else if (InvestRate >= 0.2 && InvestRate < 0.6)
                    {
                        OnAddBet();
                    }
                    else
                    {
                        OnOpen();
                    }
                }
                else
                {
                    winRate = winRate * (1 - InvestRate);
                    if (winRate > 0.4)
                    {
                        OnCall();
                    }
                    else if (winRate > 0.25 && winRate <= 0.4)
                    {
                        OnAddBet();
                    }
                    else if (winRate > 0.18 && winRate <= 0.25)
                    {
                        if (InvestRate > 0.15)
                        {
                            OnOpen();
                        }
                        else
                        {
                            OnCall();
                        }
                    }
                    else if (winRate <= 0.18)
                    {
                        if (InvestRate > 0.4)
                        {
                            OnOpen();
                        }
                        else
                        {
                            OnGiveUp();
                        }
                    }
                }
            }
            else
            {
                if (InvestRate >= 0.01)
                {
                    IsSee = true;
                    Operate();
                    return;
                }
                else
                {
                    if (winRate * (1 - InvestRate) > 0.5)
                    {
                        OnAddBet();
                    }
                    else if (winRate * (1 - InvestRate) <= 0.5 && winRate * (1 - InvestRate) >= 0.4)
                    {
                        OnCall();
                    }
                    else
                    {
                        IsSee = true;
                        Operate();
                        return;
                    }
                }
            }
            #endregion
        }
        public void GetStrength()
        {
            Random random = new Random();
            int Strength = random.Next(3000, 5000);
            #region 计算出自己的牌力
            if (IsSee)
            {
                if (CardType == CardType.Leopard || CardType == CardType.Flush || CardType == CardType.Junko)
                {
                    Strength = (Convert.ToInt32(CardType) << 12) + (Cards[2].Number << 8);
                }
                else if (CardType == CardType.Pair)
                {
                    if (Cards[0].Number == Cards[1].Number)
                    {
                        Strength = (Convert.ToInt32(CardType) << 12) + (Cards[0].Number << 8) + (Cards[2].Number << 4);
                    }
                    else if (Cards[0].Number == Cards[2].Number)
                    {
                        Strength = (Convert.ToInt32(CardType) << 12) + (Cards[0].Number << 8) + (Cards[1].Number << 4);
                    }
                    else
                    {
                        Strength = (Convert.ToInt32(CardType) << 12) + (Cards[2].Number << 8) + (Cards[0].Number << 4);
                    }
                }
                else if (CardType == CardType.Single || CardType == CardType.GoldeFlower)
                {
                    Strength = (Convert.ToInt32(CardType) << 12) + (Cards[2].Number << 8) + (Cards[1].Number << 4) + Cards[0].Number;
                }
            }
            #endregion
            strength = Strength;
        }
        public void GetOtherPlayersStrength()
        {
            otherPlayersStrength = new Dictionary<Player, int>();
            foreach (Player player in otherPlayers)
            {
                Random random = new Random();
                int Strength = random.Next(3000, 5000);
                otherPlayersStrength.Add(player, Strength);
            }
        }
        public void CalculateOtherPlayersStrength()
        {
            foreach (Player player in otherPlayers)
            {
                if (!player.IsGiveUp)
                {
                    if (player.IsSee)
                    {
                        otherPlayersStrength[player] += baseStrength * 2 + player.MyBet * 2;
                    }
                    else
                    {
                        otherPlayersStrength[player] += baseStrength + player.MyBet;
                    }
                }
            }
        }
        public void CalculateWinRate()
        {
            double rate = 1.0;
            foreach (Player player in otherPlayers)
            {
                rate = Math.Min(rate, Convert.ToDouble(strength) / Convert.ToDouble(otherPlayersStrength[player] + strength));
            }
            winRate = rate;
        }
    }
}
