using Conjure.API_Responses;
using System.Collections.Generic;

namespace Conjure
{
    public class ScryfallJSONResponse
    {
        public string scryfall_uri;
        public Dictionary<string, string> image_uris;
        public List<ScryfallCardFace> card_faces;
    }
}
