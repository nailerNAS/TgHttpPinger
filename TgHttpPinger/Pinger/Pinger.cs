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

            return pingResult;
        }

        public static bool IsValidUrl(string url)
        {
            bool result = Uri.TryCreate(url, UriKind.Absolute, out Uri uri);
            return result && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
    }
}
