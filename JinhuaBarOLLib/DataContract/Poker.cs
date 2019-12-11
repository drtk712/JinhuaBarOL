using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinhuaBarOLLib
{
    public class Poker
    {
        Card[] olddeck = new Card[52];
        private Stack<Card> deck;
        public Stack<Card> Deck
        {
            get
            {
                return deck;
            }
        }
        public Poker()
        {
            int i = 0;
            for (int j = 2; j <= 14; j++)
            {
                olddeck[i] = new Card(Suit.Cube, j);
                i++;
            }
            for (int j = 2; j <= 14; j++)
            {
                olddeck[i] = new Card(Suit.Hearts, j);
                i++;
            }
            for (int j = 2; j <= 14; j++)
            {
                olddeck[i] = new Card(Suit.Plumblossom, j);
                i++;
            }
            for (int j = 2; j <= 14; j++)
            {
                olddeck[i] = new Card(Suit.Spades, j);
                i++;
            }
        }
        public void Shuffe()
        {
            deck = new Stack<Card>();
            Random random = new Random();
            int index;
            for (int i = olddeck.Length; i >= 1; i--)
            {
                index = random.Next(i);
                deck.Push(olddeck[index]);
                olddeck[index] = olddeck[i - 1];
                olddeck[i - 1] = deck.Peek();
            }
        }
    }
}
