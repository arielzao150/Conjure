using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Conjure
{
    class Program
    {
        static void Main(string[] args)
        {
            string set = "";
            string key = "";
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-set")
                {
                    set = args[i + 1];
                }
                else if (args[i] == "-key")
                {
                    key = args[i + 1];
                }
            }

            Console.WriteLine("Conjuring...");

            // Open .xml
            

            try
            {
                var XMLDeck = LoadXML(set);

                var deck = new Deck(XMLDeck, set, key);

                using (StreamWriter file = File.CreateText(@$".\{set}.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    //serialize object directly into file stream
                    serializer.Serialize(file, new { ObjectStates = new[] { deck } });
                }

                Console.WriteLine("Conjure resolves.");
            }
            catch(Exception e)
            {
                Console.WriteLine("Conjure was countered!");
                Console.WriteLine("Cause: " + e.Message);
            }
        }

        private static List<CardXML> LoadXML(string set)
        {
            var XMLDeck = new List<CardXML>();

            XmlDocument doc = new XmlDocument();
            doc.Load($"{set}/{set}.xml");

            foreach(XmlNode node in doc.DocumentElement["cards"])
            {
                CardXML card = new CardXML((node["name"].InnerText), set+node["set"].Attributes["picURL"].Value);
                XMLDeck.Add(card);
            }
            return XMLDeck;
        }
    }
}
