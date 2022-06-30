using System;
using System.Diagnostics;

namespace RateLimiter
{
    public class Timestamp : ITimestamp
    {
        public long GetTimestamp()
        {
            return Stopwatch.GetTimestamp();
        }
    }
}
