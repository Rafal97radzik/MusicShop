using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace MusicShop.Infrastructure
{
    public class Helpers
    {
        public static void CallUrl(string serviceUrl)
        {
            var req = HttpWebRequest.Create(@"https://localhost:44378"+serviceUrl);
            req.GetResponseAsync();
        }
    }
}