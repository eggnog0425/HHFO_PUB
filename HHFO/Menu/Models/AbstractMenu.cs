﻿using CoreTweet;
using CoreTweet.Core;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace HHFO.Models
{
    public abstract class AbstractMenu: BindableBase
    {
        public abstract int Width { get; protected set; }
        public abstract IReadOnlyList<CoreTweet.List> Lists { get; protected set; }
        public abstract void FetchList();
    }
}