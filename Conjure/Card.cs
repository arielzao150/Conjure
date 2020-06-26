using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conjure
{
    public class Card
    {
        public int CardID;
        public string Name = "Card";
        public string Nickname;
        public object Transform = new
        {
            posX = 0,
            posY = 0,
            posZ = 0,
            rotX = 0,
            rotY = 180,
            rotZ = 180,
            scaleX = 1,
            scaleY = 1,
            scaleZ = 1
        };

        public Card()
        {

        }

        public Card(string fileLine)
        {
            var qnt = fileLine.Trim(' ')[0].ToString();
            quantity = Convert.ToInt32(qnt);
            var name = fileLine.Substring(fileLine.IndexOf(' ') + 1);
            Nickname = name;
        }

        public Card(int id, string name)
        {
            CardID = id;
            Nickname = name;
        }

        [NonSerialized]
        public int quantity = 1;

        [NonSerialized]
        public List<Card> tokens = null;
    }
}
