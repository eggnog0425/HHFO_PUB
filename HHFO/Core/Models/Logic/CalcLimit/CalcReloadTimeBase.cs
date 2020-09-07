using CoreTweet;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HHFO.Models.Logic.API
{
    public abstract class CalcReloadTimeBase : ICalcReloadTime
    {
        public int CntTabs;

        public abstract TimeSpan CalcReloadTime();

        protected int ApiLimit;

        /// <summary>
        /// 次にAPIがリセットされる時間(UTCのエポック秒)
        /// </summary>
        protected DateTimeOffset ApiLimitReset;

        /// <summary>
        /// リセットまでに叩ける回数
        /// </summary>
        protected int ApiLimitRemaining;


        protected ReactiveProperty<TimeSpan> _nextReloadTime = new ReactiveProperty<TimeSpan>();
        public ReadOnlyReactiveProperty<TimeSpan> NextReloadTime;

        public CalcReloadTimeBase(int cntTabs, RateLimit rateLimit)
        {
            CntTabs = cntTabs;
            ApiLimit = rateLimit.Limit;
            ApiLimitRemaining = rateLimit.Remaining;
            ApiLimitReset = rateLimit.Reset;
        }

        public void AddTab()
        {
            _ = Interlocked.Increment(ref CntTabs);
            CalcReloadTime();
        }

        public void AddTabs(int numAdd)
        {
            if (numAdd < 1)
            {
                return;
            }
            _ = Interlocked.Add(ref CntTabs, numAdd);
        }

        public void RemoveTab()
        {
            _ = Interlocked.Decrement(ref CntTabs);
        }

        public void RemoveTabs(int numRemove)
        {
            if (numRemove < 1)
            {
                return;
            }
            _ = Interlocked.Add(ref CntTabs, -numRemove);
        }
    }
}
