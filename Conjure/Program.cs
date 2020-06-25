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
#if RELEASE
                string[] lines = File.ReadAllLines(set + ".txt");
                var sideboard = lines.SkipWhile(x => !x.Contains("Sideboard")).ToList();
                var mainboard = lines.Take(lines.Length - 1 - sideboard.Count).ToList();

                var cards = new List<Card>(mainboard.Select(x => new Card(x.Substring(2))));
#endif
#if DEBUG
                var cards = new List<Card>() { new Card("Hadana's Climb") };
#endif


                var officialDeck = new Deck(cards);

                WriteToJsonFile(set, officialDeck);
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

        private static void WriteToJsonFile(string set, Deck deck)
        {
            using (StreamWriter file = File.CreateText(@$".\{set}.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, new { ObjectStates = new[] { deck } });
            }
        }
    }
}

