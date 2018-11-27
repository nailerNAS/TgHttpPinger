namespace TgHttpPinger.Pinger
{
    public class PingResult
    {
        public string Url;
        public int StatusCode;
        public float Delay;
        public bool IsTimedOut;
        public string Exception;
    }
}
