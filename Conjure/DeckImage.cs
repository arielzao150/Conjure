using System;
using System.Collections.Generic;
using System.Text;

namespace Conjure
{
    public class DeckImage
    {
        public string FaceURL;
        public string BackURL = $"https://gamepedia.cursecdn.com/mtgsalvation_gamepedia/f/f8/Magic_card_back.jpg";
        public int NumHeight = 1;
        public int NumWidth = 1;
        public bool BackIsHidden = true;

        public DeckImage()
        {

        }

        public DeckImage(string faceURL)
        {
            FaceURL = faceURL;
        }
    }
}
