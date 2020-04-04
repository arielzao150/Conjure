using System;
using System.Collections.Generic;
using System.Text;

namespace Conjure
{
    public class CardXML
    {
        public string name { get; private set; }
        public string picURL { get; private set; }

        public CardXML(string name, string picURL)
        {
            this.name = name;
            this.picURL = picURL;
        }
    }
}
