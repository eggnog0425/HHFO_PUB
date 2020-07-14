using CoreTweet;
using CoreTweet.Core;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace HHFO.Models
{
    public abstract class AbstractMenu: BindableBase
    {
        public abstract string HomeLabel { get; protected set; }
        public abstract string ListLabel { get; protected set; }
        public abstract int MenuWidth { get; protected set; }
        public abstract void SpreadMenu();
        public abstract void ShrinkMenu();
        public abstract IReadOnlyList<CoreTweet.List> Lists { get; protected set; }
        public abstract void FetchList();
    }
}
