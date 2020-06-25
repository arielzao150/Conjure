using System;
using System.Collections.Generic;
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
            posY = 1,
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

        public Card(string nickname)
        {
            Name = nickname;
            Nickname = nickname;
        }
    }
}
