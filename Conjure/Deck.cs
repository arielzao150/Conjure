using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
        public object Transform = new {
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

        public Deck(List<Card> cards)
        {
            foreach(var card in cards)
            {
                int cardNum = ContainedObjects.Count + 1;
                Console.WriteLine(cardNum + "/" + cards.Count + " " + "Now adding: " + card.Name);
                var cardToAdd = new Card();
                cardToAdd.CardID = cardNum * 100;
                cardToAdd.Nickname = card.Name;
                ContainedObjects.Add(cardToAdd);

                DeckIDs.Add(cardToAdd.CardID);

                var cardImage = GetFromScryfall(card.Nickname);

                CustomDeck.Add(cardNum, cardImage);
            }
        }

        private DeckImage GetFromScryfall(string cardNickname)
        {
            string url = "https://api.scryfall.com";
            url = url + $"/cards/named?exact=" + cardNickname.ToLower().Replace(" ", "+");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        var faceURL = "";
                        var objText = reader.ReadToEnd();
                        ScryfallJSONResponse myojb = JsonConvert.DeserializeObject<ScryfallJSONResponse>(objText);
                        if (myojb.image_uris != null)
                            faceURL = myojb.image_uris["png"];
                        else
                        {
                            faceURL = myojb.card_faces[0].image_uris["png"].Split('?')[0];
                        }
                        return new DeckImage(faceURL);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
            
        public Deck(List<CardXML> XMLCards, string set, string key)
        {
            string cardback = "";
            foreach (var card in XMLCards)
            {
                int cardNum = ContainedObjects.Count + 1;
                Console.WriteLine(cardNum+"/"+XMLCards.Count+" "+"Now adding: " + card.name);
                var cardToAdd = new Card();
                cardToAdd.CardID = cardNum * 100;
                cardToAdd.Nickname = card.name;
                ContainedObjects.Add(cardToAdd);

                DeckIDs.Add(cardToAdd.CardID);

                var cardImage = new DeckImage();
                cardImage.FaceURL = UploadImageAsync(set+'/'+card.picURL, key).Result;
                
                // Verify if cardback is different
                if (File.Exists(set+"/cardback.jpg") || cardback.Length >= 1)
                {
                    cardImage.BackURL = cardback == "" ? UploadImageAsync(set+"/cardback.jpg", key).Result : cardback;
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
