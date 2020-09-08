using CoreTweet;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HHFO.Models.Logic.API
{
    public interface ICalcReloadTime
    {
        public TimeSpan CalcReloadTime(RateLimit rateLimit);
        
    }
}
