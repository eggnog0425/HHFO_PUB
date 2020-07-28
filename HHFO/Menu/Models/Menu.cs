using CoreTweet.Core;
using ImTools;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace HHFO.Models
{
    public class Menu: AbstractMenu
    {
        private int width;
        private IReadOnlyList<CoreTweet.List> lists;

        public Menu()
        {
            width = 1;
            Lists = new List<CoreTweet.List>();
        }
        public override int Width
        {
            get => width;
            protected set => SetProperty(ref this.width, value, nameof(Width));
        }

        public override IReadOnlyList<CoreTweet.List> Lists
        {
            get => lists;
            protected set => SetProperty(ref lists, value, nameof(Lists));
        }
        public override void FetchList()
        {
            var token = Authorization.GetToken();
            Lists = token.Lists.List();
            //this.RaisePropertyChanged(nameof(Lists));
        }
    }
}
