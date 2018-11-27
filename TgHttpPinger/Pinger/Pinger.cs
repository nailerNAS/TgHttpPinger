using Flurl.Http;
using System;
using System.Threading.Tasks;

namespace TgHttpPinger.Pinger
{
    public static class Pinger
    {
        public static async Task<PingResult> Ping(string url)
        {
            DateTime before = DateTime.Now;
            PingResult pingResult = new PingResult()
            {
                Url = url
            };

            try
            {
                var r = await url.WithTimeout(15).GetAsync();

                pingResult.Delay = (float)(DateTime.Now - before).TotalSeconds;
                pingResult.IsTimedOut = false;
                pingResult.StatusCode = (int)r.StatusCode;
            }

            catch(FlurlHttpTimeoutException ex)
            {
                pingResult.IsTimedOut = true;
            }

            catch(FlurlHttpException ex)
            {
                pingResult.Exception = ex.InnerException.InnerException.Message;
            }

            return pingResult;
        }

        public static string ParseUrl(string text)
        {
            string ret = "";

            if (!text.StartsWith("http")) ret += "http://";
            ret += text;

            bool result = Uri.TryCreate(ret, UriKind.Absolute, out Uri uri);

            if (result && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                return ret;
            }

            return string.Empty;
        }
    }
}
