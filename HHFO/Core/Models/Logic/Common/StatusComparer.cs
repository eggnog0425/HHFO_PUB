using CoreTweet;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace HHFO.Models.Logic.Common
{
    public class StatusComparer : IEqualityComparer<Status>
    {

        public bool Equals([AllowNull] Status x, [AllowNull] Status y)
        {
            return (x as Status).Id == (y as Status).Id;
        }

        public int GetHashCode([DisallowNull] Status obj)
        {
            return obj.Id.ToString().GetHashCode();
        }
    }
}
