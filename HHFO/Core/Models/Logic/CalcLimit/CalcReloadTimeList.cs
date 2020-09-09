using CoreTweet;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HHFO.Models.Logic.API
{
    public sealed class CalcReloadTimeList : CalcReloadTimeBase
    {
        private CalcReloadTimeList(int cntTabs, RateLimit rateLimit) : base(cntTabs, rateLimit) {}
        private static CalcReloadTimeList Instance = null;
            
        /// <summary>
        /// 手動更新とかで余分に更新する可能性があるので少しバッファをとる
        /// </summary>
        private const int Buffer = 2;

        public static CalcReloadTimeList GetInstance(int cntTabs, RateLimit rateLimit)
        {

            if (Instance != null)
            {
                    Instance.ApiLimit = rateLimit.Limit;
                    return Instance;
            }
            Instance = new CalcReloadTimeList(cntTabs, rateLimit);

            return Instance;
        }

    public override TimeSpan CalcReloadTime(RateLimit rateLimit)
        {
            var currentTime = DateTimeOffset.Now.ToUniversalTime();
            var remainingTime= rateLimit.Reset - currentTime;

            // 切り捨てたいからint/intでシンプルに
            var totalRemaining = rateLimit.Remaining / CntTabs;
            if (totalRemaining == 0)
            {
                if (TimeSpan.Zero < (rateLimit.Reset - DateTimeOffset.Now))
                {
                    return CompareMinReloadTime(remainingTime.Add(TimeSpan.FromSeconds(1.5d)));
                }
                else
                {
                    var limitDictionary = Authorization.GetToken().Application.RateLimitStatus("lists").GetValueOrDefault("lists");
                    if (limitDictionary.TryGetValue("/lists/statuses", out var limit))
                    {
                        return CompareMinReloadTime(CalcReloadTime(limit));
                    }
                }
            }
            else if (totalRemaining <= Buffer)
            {
                return CompareMinReloadTime(TimeSpan.FromTicks(remainingTime.Ticks / totalRemaining));
            }
            return CompareMinReloadTime(TimeSpan.FromTicks(remainingTime.Ticks / (totalRemaining - Buffer)));
        }

        private TimeSpan CompareMinReloadTime(TimeSpan reloadTime)
        {
            return MinReloadTime < reloadTime
                ? reloadTime
                : MinReloadTime;
        }
    }
}