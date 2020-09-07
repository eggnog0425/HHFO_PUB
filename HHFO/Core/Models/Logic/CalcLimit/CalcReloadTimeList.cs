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
        private CalcReloadTimeList Instance = null;

        /// <summary>
        /// 手動更新とかで余分に更新する可能性があるので少しバッファをとる
        /// </summary>
        private const int Buffer = 10;

        public CalcReloadTimeList GetInstance(int cntTabs, RateLimit rateLimit)
        {
            lock(Instance)
            {
                if (Instance != null)
                {
                    Instance.ApiLimitReset = rateLimit.Reset;
                    Instance.ApiLimitRemaining = rateLimit.Remaining;
                    Instance.ApiLimitReset = rateLimit.Reset;
                    return Instance;
                }
                Instance = new CalcReloadTimeList(cntTabs, rateLimit);
                Instance.NextReloadTime = Instance._nextReloadTime.ToReadOnlyReactiveProperty();
            }
            return Instance;
        }

        public override TimeSpan CalcReloadTime()
        {
            var currentTime = DateTimeOffset.Now.ToUniversalTime();
            var remainingTime= ApiLimitReset - currentTime;

            // 切り捨てたいからint/intでシンプルに
            var totalRemaining = ApiLimitRemaining / CntTabs;
            if (totalRemaining == 0)
            {
                _nextReloadTime.Value = remainingTime.Add(TimeSpan.FromSeconds(1.5d));
            }
            else if (totalRemaining <= Buffer)
            {
                _nextReloadTime.Value = TimeSpan.FromTicks(remainingTime.Ticks / totalRemaining);
            }
            else
            {
                _nextReloadTime.Value = TimeSpan.FromTicks(remainingTime.Ticks / (totalRemaining - Buffer));
            }
            return _nextReloadTime.Value;
        }
    }
}