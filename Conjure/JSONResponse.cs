using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Conjure
{
    public class JSONResponse
    {
        public DataResponse data;
    }

    public class DataResponse
    {
        public string url;
    }
}
