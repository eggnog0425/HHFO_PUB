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
            protected set => SetProperty(ref this.width, value, "Width");
        }

        public override IReadOnlyList<CoreTweet.List> Lists
        {
            get => lists;
            protected set => SetProperty(ref lists, value, "Lists");
        }
        public override void FetchList()
        {
            //var token = Authorization.GetToken();
            //lists = token.Lists.List();
            var lists = Enumerable.Range(1, 20).Select(x => new CoreTweet.List() { Name = "リスト" + x }).ToList();
            Lists = new ListedResponse<CoreTweet.List>(lists);
            this.RaisePropertyChanged(nameof(Lists));
        }
    }
}
