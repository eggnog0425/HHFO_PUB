using CoreTweet;
using NLog;
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
        protected Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly TimeSpan MinReloadTime = TimeSpan.FromSeconds(5.0d);

        public int CntTabs;

        public abstract TimeSpan CalcReloadTime(RateLimit rateLimit);

        protected int ApiLimit;

        public CalcReloadTimeBase(int cntTabs, RateLimit rateLimit)
        {
            CntTabs = cntTabs;
        }

        public TimeSpan AddTab(RateLimit rateLimit)
        {
            _ = Interlocked.Increment(ref CntTabs);
            return CalcReloadTime(rateLimit);
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
        public void WhenManualUpdate(RateLimit rateLimit)
        {

        }

    }
}
