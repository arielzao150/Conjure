using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Conjure
{
    public class Deck
    {
        public string Name = "DeckCustom";
        public List<Card> ContainedObjects = new List<Card>();
        public List<int> DeckIDs = new List<int>();
        public Dictionary<int, DeckImage> CustomDeck = new Dictionary<int, DeckImage>();
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

        public Deck(List<Card> cards)
        {
            var cardTotal = cards.Sum(x => x.quantity);
            string imgQuality = "png";
            foreach (var card in cards)
            {
                while (card.quantity >= 1)
                {
                    card.quantity--;
                    int cardNum = ContainedObjects.Count + 1;
                    Console.WriteLine(cardNum + "/" + cardTotal + " " + "Now adding: " + card.Nickname);
                    var cardToAdd = new Card(cardNum * 100, card.Nickname);

                    ScryfallJSONResponse cardFromScryfall = GetFromScryfall(card.Nickname);

                    if (cardFromScryfall.image_uris == null)
                    {
                        Console.WriteLine($"{cardToAdd.Nickname} is double faced, adding another card to list.");
                        cardTotal++;
                        var faceURL = cardFromScryfall.card_faces[0].image_uris[imgQuality];
                        var backURL = cardFromScryfall.card_faces[1].image_uris[imgQuality];
                        AddCardAndImage(cardToAdd, new DeckImage(faceURL));

                        
                        cardNum++;
                        Console.WriteLine(cardNum + "/" + cardTotal + " " + "Now adding: " + cardFromScryfall.card_faces[1].name);
                        cardToAdd = new Card(cardNum * 100, cardFromScryfall.card_faces[1].name);
                        AddCardAndImage(cardToAdd, new DeckImage(backURL, faceURL));
                        continue;
                    }

                    AddCardAndImage(cardToAdd, new DeckImage(cardFromScryfall.image_uris[imgQuality]));
                }
            }
        }

        private void AddCardAndImage(Card cardToAdd, DeckImage image)
        {
            ContainedObjects.Add(cardToAdd);
            DeckIDs.Add(cardToAdd.CardID);
            CustomDeck.Add(cardToAdd.CardID/100, image);
        }

        private ScryfallJSONResponse GetFromScryfall(string cardNickname)
        {
            string url = "https://api.scryfall.com";
            url = url + $"/cards/named?exact=" + cardNickname.ToLower().Replace(" ", "+");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var objText = reader.ReadToEnd();
                    ScryfallJSONResponse myojb = JsonConvert.DeserializeObject<ScryfallJSONResponse>(objText);
                    return myojb;
                }
            }
        }

        public Deck(List<CardXML> XMLCards, string set, string key)
        {
            string cardback = "";
            foreach (var card in XMLCards)
            {
                int cardNum = ContainedObjects.Count + 1;
                Console.WriteLine(cardNum + "/" + XMLCards.Count + " " + "Now adding: " + card.name);
                var cardToAdd = new Card();
                cardToAdd.CardID = cardNum * 100;
                cardToAdd.Nickname = card.name;
                ContainedObjects.Add(cardToAdd);

                DeckIDs.Add(cardToAdd.CardID);

                var cardImage = new DeckImage();
                cardImage.FaceURL = UploadImageAsync(set + '/' + card.picURL, key).Result;

                // Verify if cardback is different
                if (File.Exists(set + "/cardback.jpg") || cardback.Length >= 1)
                {
                    cardImage.BackURL = cardback == "" ? UploadImageAsync(set + "/cardback.jpg", key).Result : cardback;
                    cardback = cardImage.BackURL;
                }

                CustomDeck.Add(cardNum, cardImage);
            }
        }

        private async Task<string> UploadImageAsync(string imagePath, string key)
        {
            using var client = new HttpClient();
            var byteArrayContent = new StringContent(Convert.ToBase64String(GetFileByteArray(imagePath)));
            var content = new MultipartFormDataContent();
            content.Add(byteArrayContent, "image");

            var response = await client.PostAsync("https://api.imgbb.com/1/upload?key=" + key, content);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var responseJson = JsonConvert.DeserializeObject<ImgbbUploadJSONResponse>(responseString);
                return responseJson.data.url;
            }
            else throw new Exception("Not able to upload image");
        }


        private byte[] GetFileByteArray(string filename)
        {
            FileStream oFileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);

            // Create a byte array of file size.
            byte[] FileByteArrayData = new byte[oFileStream.Length];

            //Read file in bytes from stream into the byte array
            oFileStream.Read(FileByteArrayData, 0, System.Convert.ToInt32(oFileStream.Length));

            //Close the File Stream
            oFileStream.Close();

            return FileByteArrayData; //return the byte data
        }
    }
}
