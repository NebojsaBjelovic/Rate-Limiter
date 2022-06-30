using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public sealed class RateLimiterManager
    {
        private const int TooManyRequests = 429;

        private static volatile RateLimiterManager instance;
        private static object syncRoot = new object();

        private List<RateLimiterIP> ipRates = null;
        private int defaultRequestLimitCount;
        private int defaultRequestLimitMs;

        private RateLimiterManager()
        {
            ipRates = new List<RateLimiterIP>();
        }

        public static RateLimiterManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new RateLimiterManager();
                    }
                }

                return instance;
            }
        }

        public List<RateLimiterIP> IpRates
        {
            get { return ipRates; }
        }

        public int DefaultRequestLimitCount
        {
            get { return defaultRequestLimitCount; }
            set { defaultRequestLimitCount = value; }
        }

        public int DefaultRequestLimitMs
        {
            get { return defaultRequestLimitMs; }
            set { defaultRequestLimitMs = value; }
        }

        public void CheckIpAddress(string ip, ref int result)
        {
            if (!IpRates.Exists(r => r.Ip.Equals(ip)))
            {
                IpRates.Add(new RateLimiterIP(ip, new Timestamp(), DefaultRequestLimitCount, DefaultRequestLimitMs));
            }
            else
            {
                if (!IpRates.Find(r => r.Ip.Equals(ip)).Sw.RequestConforms())
                {
                    result = TooManyRequests;
                }
            }
        }
    }
}
