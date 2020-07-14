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
        // todo:最終的には文字列じゃなくてベクタアイコンにする
        private string homeLabel;
        private string listLabel;
        private int menuWidth;
        private IReadOnlyList<CoreTweet.List> lists;

        public Menu()
        {
            homeLabel = "Home";
            listLabel = "List";
            menuWidth = 1;
            Lists = new List<CoreTweet.List>();
        }
        public override int MenuWidth
        {
            get => menuWidth;
            protected set => SetProperty(ref this.menuWidth, value, "MenuWidth");
        }
        public override string HomeLabel
        {
            get => homeLabel;
            protected set => SetProperty(ref this.homeLabel, value, "Home");
        }
        public override string ListLabel
        {
            get => listLabel;
            protected  set => SetProperty(ref this.listLabel, value, "List");
        }

        public override IReadOnlyList<CoreTweet.List> Lists
        {
            get => lists;
            protected set => SetProperty(ref lists, value, "Lists");
        }

        public override void SpreadMenu()
        {
            MenuWidth++;
        }

        public override void ShrinkMenu()
        {
            MenuWidth--;
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
