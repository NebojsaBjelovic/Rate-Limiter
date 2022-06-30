using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class RateLimiterIP
    {
        private string ip;
        private SlidingWindow sw;

        public RateLimiterIP(string ip, ITimestamp timestamp, int requestLimit, int requestIntervalMs)
        {
            sw = new SlidingWindow(timestamp, requestLimit, requestIntervalMs);
            Ip = ip;
        }

        public string Ip
        {
            get { return ip; }
            set { ip = value; }
        }

        public SlidingWindow Sw
        {
            get { return sw; }
        }
    }
}
