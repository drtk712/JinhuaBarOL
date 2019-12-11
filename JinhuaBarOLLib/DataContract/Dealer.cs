using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Threading;

namespace JinhuaBarOLLib
{
    [DataContract]
    public class Dealer
    {
        private Poker poker = new Poker();
        public Dealer()
        {
            poker.Shuffe();
        }
        private bool isOffLine;
        public bool IsOffLine
        {
            get { return isOffLine; }
            set { isOffLine = value; }
        }
        public Dealer(Player[] players)
        {
            foreach (Player player in players)
            {
                orderedPlayers.Enqueue(player);
            }
        }
        private Queue<Player> orderedPlayers = new Queue<Player>();
        [DataMember]
        public Queue<Player> OrderedPlayers
        {
            get
            {
                return orderedPlayers;
            }
            set
            {
                orderedPlayers = value;
            }
        }
        private int minBet = 20;
        [DataMember]
        public int MinBet
        {
            get { return minBet; }
            set { minBet = value; }
        }
        private int stepBet = 20;
        [DataMember]
        public int StepBet
        {
            get { return stepBet; }
            set { stepBet = value; }
        }
        private int sumBet = 0;
        [DataMember]
        public int SumBet
        {
            get { return sumBet; }
            set { sumBet = value; }
        }
        private int maxBet = 1000;
        [DataMember]
        public int MaxBet
        {
            get { return maxBet; }
            set { maxBet = value; }
        }
        private bool resetGame = false;
        [DataMember]
        public bool ResetGame
        {
            get { return resetGame; }
            set { resetGame = value; }
        }
        //发牌
        public void Licensing(Player player)
        {
            player.Cards.Add(poker.Deck.Pop());
            if (player.Cards.Count == 3)
            {
                player.Cards.Sort();
                player.JudgeCards();
            }
        }
        //玩家跟注
        public bool PlayerCall(Player player)
        {
            if (player.IsSee)
            {
                if (player.Chips < stepBet * 2)
                {
                    SendMessage(player,"对不起，你没有足够的筹码，只能弃牌或者全推");
                    return false;
                }
                else
                {
                    sumBet += stepBet * 2;
                    player.Chips -= stepBet * 2;
                    player.MyBet += stepBet * 2;
                    MassSendMessage("玩家" + player.Name + "跟注成功！");
                    return true;
                }
            }
            else
            {
                if (player.Chips < stepBet)
                {
                    SendMessage(player, "对不起，你没有足够的筹码，只能弃牌或者全推");
                    return false;
                }
                else
                {
                    sumBet += stepBet;
                    player.Chips -= stepBet;
                    player.MyBet += stepBet;
                    MassSendMessage("玩家" + player.Name + "跟注成功！");
                    return true;
                }
            }
        }
        public bool PlayerAddBet(Player player)
        {
            if (player.IsSee)
            {
                if (player.Chips < (stepBet + 20) * 2)
                {
                    SendMessage(player, "对不起，你没有足够的筹码加注");
                    return false;
                }
                else
                {
                    stepBet += 20;
                    sumBet += stepBet * 2;
                    player.Chips -= stepBet * 2;
                    player.MyBet += stepBet * 2;
                    MassSendMessage("玩家" + player.Name + "加注成功！");
                    return true;
                }
            }
            else
            {
                if (player.Chips < (stepBet + 20))
                {
                    SendMessage(player, "对不起，你没有足够的筹码加注");
                    return false;
                }
                else
                {
                    stepBet += 20;
                    sumBet += stepBet;
                    player.Chips -= stepBet;
                    player.MyBet += stepBet;
                    MassSendMessage("玩家" + player.Name + "加注成功！");
                    return true;
                }
            }
        }
        public void PlayerSee(Player player)
        {
            player.IsSee = true;
            string result = "┌─────┐ ┌─────┐ ┌─────┐\n";
            result += "│ " + player.Cards[0].Number2String + "   │ │ " + player.Cards[1].Number2String + "   │ │ " + player.Cards[2].Number2String + "   │\n";
            result += "│  " + player.Cards[0].Suit2Sharp + " │ │  " + player.Cards[1].Suit2Sharp + " │ │  " + player.Cards[2].Suit2Sharp + " │\n";
            result += "│     │ │     │ │     │\n";
            result += "└─────┘ └─────┘ └─────┘\n";
            SendMessage(player, result);
            MassSendMessage("玩家" + player.Name + "选择看牌");
        }
        public void PlayerGiveUp(Player player)
        {
            player.IsGiveUp = true;
            MassSendMessage("玩家" + player.Name + "选择弃牌");
        }
        public void PlayerOpen(Player player)
        {
            if (player.Chips < stepBet * 2)
            {
                sumBet += player.Chips;
                player.MyBet += player.Chips;
                player.Chips = 0;
                SendMessage(player, "全推！");
            }
            else
            {
                sumBet += stepBet * 2;
                player.Chips -= stepBet * 2;
                player.MyBet += stepBet * 2;
                SendMessage(player, "开牌！");
            }
            MassSendMessage("玩家" + player.Name + "选择开牌");
            Thread.Sleep(1000);
            PKWinner(player);
        }
        public void DealerOpen()
        {
            int count = 0;
            foreach (Player player in OrderedPlayers)
            {
                if (!player.IsGiveUp)
                {
                    count++;
                }
            }
            if (count == 1)
            {
                MassSendMessage("场上仅剩一名玩家未弃牌，该玩家获胜");
                Thread.Sleep(1000);
                JudgeWinner();
            }
        }
        public void PKWinner(Player playerPK)
        {
            while (true)
            {
                if (orderedPlayers.Peek() != playerPK)
                {
                    orderedPlayers.Enqueue(orderedPlayers.Dequeue());
                }
                else
                {
                    foreach (Player player in orderedPlayers)
                    {
                        if (playerPK == player)
                        {
                            continue;
                        }
                        if (!player.IsGiveUp)
                        {
                            MassSendMessage("玩家" + playerPK.Name + "开始与玩家" + player.Name + " PK");
                            Thread.Sleep(1000);
                            Player winner = null;
                            if (player.CardType > playerPK.CardType)//TODO:修改成个人PK
                            {
                                winner = player;
                            }
                            else if (player.CardType == playerPK.CardType)
                            {
                                if (player.CardType == CardType.Single || player.CardType == CardType.GoldeFlower)//单只or金花pk
                                {
                                    if (player.Cards[2].Number > playerPK.Cards[2].Number)
                                    {
                                        winner = player;
                                    }
                                    else if (player.Cards[2].Number == playerPK.Cards[2].Number)
                                    {
                                        if (player.Cards[1].Number > playerPK.Cards[1].Number)
                                        {
                                            winner = player;
                                        }
                                        else if (player.Cards[1].Number == playerPK.Cards[1].Number)
                                        {
                                            if (player.Cards[0].Number > playerPK.Cards[0].Number)
                                            {
                                                winner = player;
                                            }
                                            else if (player.Cards[0].Number == playerPK.Cards[0].Number)
                                            {
                                                //平局
                                            }
                                        }
                                    }
                                }
                                else if (player.CardType == CardType.Pair)//对子pk
                                {
                                    if (player.Cards[2].Number > playerPK.Cards[2].Number)
                                    {
                                        winner = player;
                                    }
                                    else if (player.Cards[2].Number == playerPK.Cards[2].Number)
                                    {
                                        if (player.Cards[0].Number > playerPK.Cards[0].Number)
                                        {
                                            winner = player;
                                        }
                                        else if (player.Cards[0].Number == playerPK.Cards[0].Number)
                                        {
                                            //平局
                                        }
                                    }
                                }
                                else if (player.CardType == CardType.Junko || player.CardType == CardType.Flush || player.CardType == CardType.Leopard)//顺子or同花顺or豹子pk
                                {
                                    if (player.Cards[0].Number > playerPK.Cards[0].Number)
                                    {
                                        winner = player;
                                    }
                                    else if (player.Cards[0].Number == playerPK.Cards[0].Number)
                                    {
                                        //平局
                                    }
                                }
                            }
                            if (winner == player)
                            {
                                MassSendMessage("玩家" + playerPK.Name + "与玩家" + player.Name + " PK失败");
                                playerPK.IsGiveUp = true;
                                break;
                            }
                            else
                            {
                                MassSendMessage("玩家" + playerPK.Name + "与玩家" + player.Name + " PK胜利");
                                player.IsGiveUp = true;
                            }
                        }
                        Thread.Sleep(1000);
                    }
                    break;
                }
            }
        }
        public void JudgeWinner()
        {
            while (true)
            {
                if (!orderedPlayers.Peek().IsGiveUp)
                {
                    resetGame = true;
                    orderedPlayers.Peek().Chips += sumBet;
                    orderedPlayers.Peek().WinRounds++;//胜场加一
                    string result = "--------------赢家是" + orderedPlayers.Peek().Name + "--------------\n";
                    result += "┌─────┐ ┌─────┐ ┌─────┐\n";
                    result += "│ " + orderedPlayers.Peek().Cards[0].Number2String + "   │ │ " + orderedPlayers.Peek().Cards[1].Number2String + "   │ │ " + orderedPlayers.Peek().Cards[2].Number2String + "   │\n";
                    result += "│  " + orderedPlayers.Peek().Cards[0].Suit2Sharp + " │ │  " + orderedPlayers.Peek().Cards[1].Suit2Sharp + " │ │  " + orderedPlayers.Peek().Cards[2].Suit2Sharp + " │\n";
                    result += "│     │ │     │ │     │\n";
                    result += "└─────┘ └─────┘ └─────┘\n";
                    MassSendMessage(result);

                    ShowResult();
                    break;
                }
                else
                {
                    orderedPlayers.Enqueue(orderedPlayers.Dequeue());
                }
            }
        }
        public void ShowResult()
        {
            foreach (Player player in orderedPlayers)
            {
                string result = "--------------玩家" + player.Name + "手牌--------------\n";
                result += "┌─────┐ ┌─────┐ ┌─────┐\n";
                result += "│ " + player.Cards[0].Number2String + "   │ │ " + player.Cards[1].Number2String + "   │ │ " + player.Cards[2].Number2String + "   │\n";
                result += "│  " + player.Cards[0].Suit2Sharp + " │ │  " + player.Cards[1].Suit2Sharp + " │ │  " + player.Cards[2].Suit2Sharp + " │\n";
                result += "│     │ │     │ │     │\n";
                result += "└─────┘ └─────┘ └─────┘\n";
                result += "-------------------------------------\n";
                MassSendMessage(result);

                Thread.Sleep(500);
            }
        }
        public void SendMessage(Player player,string message)
        {
            if ((player as AIPlayer) == null)
                player.CallBack.ShowMessage(message);
        }
        public void MassSendMessage(string message)
        {
            if (isOffLine)
            {
                foreach (Player player in orderedPlayers)
                {
                    if ((player as AIPlayer) == null)
                        player.CallBack.ShowMessage(message);
                }
            }
            else
            {
                foreach (Player player in orderedPlayers)
                    player.CallBack.ShowMessage(message);
            }
        }
        public void StartGame()
        {
            MassSendMessage("游戏初始化中。。。");
            Thread.Sleep(500);
            foreach (Player player in orderedPlayers)
            {
                player.Call += new Player.CallHandler(PlayerCall);
                player.AddBet += new Player.AddBetHandler(PlayerAddBet);
                player.See += new Player.SeeHandler(PlayerSee);
                player.GiveUp += new Player.GiveUpHandler(PlayerGiveUp);
                player.Open += new Player.OpenHandler(PlayerOpen);
                foreach (Player otherPlayer in orderedPlayers)
                {
                    if ((player as AIPlayer) != null)
                    {
                        if (otherPlayer != player)
                        {
                            (player as AIPlayer).OtherPlayers.Add(otherPlayer);
                        }
                    }
                }
            }
            MassSendMessage("游戏初始化完毕");
            OnGame();
        }
        public void OnGame()
        {
            int count = 1;
            while (true)
            {
                MassSendMessage("开始第" + count++ + "轮游戏\n庄家是：" + orderedPlayers.Peek().Name + "\n场上玩家筹码情况：\n");
                foreach (Player player in orderedPlayers)
                {
                    MassSendMessage("玩家：" + player.Name + " 筹码：" + player.Chips);
                }
                Thread.Sleep(1000);
                foreach (Player player in orderedPlayers)
                {
                    MassSendMessage("玩家" + player.Name + "下底注" + minBet);
                    sumBet += minBet;
                    player.Chips -= minBet;
                    player.MyBet += minBet;
                    player.TotalRounds++;//总局数加一
                    Thread.Sleep(200);
                }
                MassSendMessage("荷官开始洗牌...");
                poker.Shuffe();
                Thread.Sleep(1000);
                MassSendMessage("洗牌完毕，开始发牌...");
                for (int i = 1; i <= 3; i++)
                {
                    foreach (Player player in OrderedPlayers)
                    {
                        MassSendMessage("第" + i + "轮发牌,给玩家" + player.Name + "...");
                        Licensing(player);
                        Thread.Sleep(100);
                    }
                }
                MassSendMessage("发牌完毕，开始游戏...");
                Thread.Sleep(1000);
                int step = 0;
                Player[] playersInformation = orderedPlayers.ToArray();
                while (!resetGame)
                {
                    foreach (Player player in orderedPlayers)
                    {
                        if((player as AIPlayer)==null)
                            player.CallBack.ClearScreen();
                    }
                    MassSendMessage("第" + (step++ / 5 + 1) + "轮次");
                    #region 显示其他玩家信息
                    string line1 = "", line2 = "", line3 = "", line4 = "", line5 = "";
                    
                    foreach (Player player in playersInformation)
                    {
                        string temp = "";
                        line1 += " ********************* ";

                        temp = "玩家:" + player.Name;
                        line2 += "*";
                        for (int j = 0; j < (20 - temp.Length) / 2; j++)
                            line2 += " ";
                        line2 += temp;
                        for (int j = 0; j < (20 - temp.Length) / 2; j++)
                            line2 += " ";

                        if (player.IsGiveUp)
                        {
                            temp = "已弃牌";
                        }
                        else
                        {
                            temp = player.IsSee2String;
                        }
                        line3 += "*";
                        for (int j = 0; j < (20 - temp.Length) / 2; j++)
                            line3 += " ";
                        line3 += temp;
                        for (int j = 0; j < (20 - temp.Length) / 2; j++)
                            line3 += " ";

                        temp = "注数:" + player.MyBet;
                        line4 += "*";
                        for (int j = 0; j < (20 - temp.Length) / 2; j++)
                            line4 += " ";
                        line4 += temp;
                        for (int j = 0; j < (20 - temp.Length) / 2; j++)
                            line4 += " ";

                        line5 += " ********************* ";
                    }
                    MassSendMessage(line1 + "\n" + line2 + "\n" + line3 + "\n" + line4 + "\n" + line5);
                    #endregion
                    if (orderedPlayers.Peek().IsGiveUp)
                    {
                        MassSendMessage("玩家" + orderedPlayers.Peek().Name + "已弃牌，跳过操作");
                    }
                    else
                    {
                        orderedPlayers.Peek().Operate();
                    }
                    
                    orderedPlayers.Enqueue(orderedPlayers.Dequeue());
                    DealerOpen();
                    Thread.Sleep(500);
                }
                Reset();
                MassSendMessage("本轮结束，即将开始下轮游戏");
                Thread.Sleep(5000);
            }
        }
        public void Reset()
        {
            resetGame = false;
            stepBet = 20;
            sumBet = 0;
            foreach (Player player in orderedPlayers)
            {
                player.Rest();
                if ((player as AIPlayer) != null)
                {
                    (player as AIPlayer).OtherPlayersStrength = null;
                }
            }
        }
    }
}
