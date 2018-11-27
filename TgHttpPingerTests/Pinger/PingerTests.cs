using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TgHttpPinger.Pinger.Tests
{
    [TestClass()]
    public class PingerTests
    {
        const string url = @"http://httpbin.org/";

        [TestMethod()]
        public void PingTest()
        {
            PingResult pingResult = Pinger.Ping(url).Result;

            Assert.IsFalse(pingResult.IsTimedOut);

            Console.WriteLine(
                $"{pingResult.Delay}\n" +
                $"{pingResult.IsTimedOut}\n" +
                $"{pingResult.StatusCode}\n" +
                $"{pingResult.Url}");
        }

        [TestMethod()]
        public void IsValidUrlTest()
        {
            Assert.IsTrue(Pinger.IsValidUrl(url));
            Assert.IsFalse(Pinger.IsValidUrl(""));
        }
    }
}