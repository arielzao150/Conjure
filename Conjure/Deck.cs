using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Conjure
{
    public class Deck
    {
        public string Name = "DeckCustom";
        public List<Card> ContainedObjects = new List<Card>();
        public List<int> DeckIDs = new List<int>();
        public Dictionary<int, DeckImages> CustomDeck = new Dictionary<int, DeckImages>();
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

                var cardImage = new DeckImages();
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

        public async Task<string> UploadImageAsync(string imagePath, string key)
        {
            //return imagePath;
            HttpClient client = new HttpClient();

            var byteArrayContent = new StringContent(Convert.ToBase64String(GetFileByteArray(imagePath)));
            var content = new MultipartFormDataContent();
            content.Add(byteArrayContent, "image");

            var response = await client.PostAsync("https://api.imgbb.com/1/upload?key="+ key, content);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var responseJson = JsonConvert.DeserializeObject<JSONResponse>(responseString);
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
