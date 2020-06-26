using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Conjure
{
    class Program
    {
        static void Main(string[] args)
        {
            string setName = "Conjure";
            string apiKey = "";
            string mode = "";
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-set")
                {
                    setName = args[i + 1];
                }
                else if (args[i] == "-key")
                {
                    apiKey = args[i + 1];
                }
                else if (args[i] == "-mode")
                {
                    mode = args[i + 1];
                }
            }
            switch(mode)
            {
                // Custom Cards Deck
                default:
                case "":
                case "1":
                    LoadCustomDeck(setName, apiKey);
                    break;

                // Official Cards Deck
                case "2":
                    LoadOfficialDeck(setName);
                    break;
            }
        }

        private static void LoadOfficialDeck(string set)
        {
            Console.WriteLine("Conjuring Official Deck...");
            try
            {
                // Load .txt.
                string[] lines = File.ReadAllLines(set + ".txt");
                var sideboard = lines.SkipWhile(x => !x.Contains("Sideboard")).ToList();
                var mainboard = sideboard.Count>0 ? lines.Take(lines.Length - 1 - sideboard.Count).ToList()
                    : lines.ToList();
                
                var mainboardCards = new List<Card>(mainboard.Where(x => !string.IsNullOrEmpty(x))
                    .Select(x => new Card(x)));

                if(sideboard.Count>0) sideboard.RemoveAt(0);
                var sideboardCards = new List<Card>(sideboard.Where(x => !string.IsNullOrEmpty(x))
                        .Select(x => new Card(x)));

                bool hasSideboard = false;

                if (sideboardCards.Count >= 1)
                {
                    hasSideboard = true;
                    if (sideboardCards.Count == 1 && sideboardCards[0].quantity == 1)
                    {
                        Console.WriteLine("Sideboard contains only 1 card, adding to mainboard");
                        mainboardCards.Add(sideboardCards[0]);
                        hasSideboard = false;
                    }
                }

#if DEBUG
                //var cards = new List<Card>();
                //cards.Add(new Card("Elbrus, the Binding Blade"));
                //cards.Add(new Card("Elbrus, the Binding Blade"));
#endif

                Console.WriteLine("Conjuring Mainboard...");
                var officialDeck = new Deck(mainboardCards);
                if(hasSideboard)
                {
                    Console.WriteLine("Conjuring Sideboard...");
                    var sideboardDeck = new Deck(sideboardCards);
                    
                    WriteToJsonFile(set, officialDeck, sideboardDeck);
                }
                else WriteToJsonFile(set, officialDeck);
            }
            catch (Exception e)
            {
                Console.WriteLine("Conjure was countered!");
                Console.WriteLine("Cause: " + e.Message);
            }
}

        private static void LoadCustomDeck(string set, string key)
        {
            Console.WriteLine("Conjuring Custom Deck...");
            try
            {
                var XMLDeck = LoadXML(set);

                var deck = new Deck(XMLDeck, set, key);

                WriteToJsonFile(set, deck);

                Console.WriteLine("Conjure resolves.");
            }
            catch (Exception e)
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

        private static void WriteToJsonFile(string set, params Deck[] ObjectStates)
        {
            int i = 0;
            foreach(var deck in ObjectStates)
            {
                deck.Transform = new
                {
                    posX = 1 + i,
                    posY = 0,
                    posZ = 0,
                    rotX = 0,
                    rotY = 180,
                    rotZ = 180,
                    scaleX = 1,
                    scaleY = 1,
                    scaleZ = 1
                };
                i += 2;
            }

            using (StreamWriter file = File.CreateText(@$".\{set}.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, new { ObjectStates });
            }
        }
    }
}

