using HHFO.Models;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity;

namespace HHFO.Menu.ViewModels
{
    public class MenuViewModel : BindableBase
    {

        public string Home { get; }
        public string List { get; }
        public ReactiveCommand<> clickList { get; private set; }


        public MenuViewModel()
        {
            // todo:module内でUnity使う方法調べる
            //this.Home = Menu?.Home;
            //this.List = Menu?.List;
            Home = "Home";
            List = "List";

        }

        public clickList()
        {

        }
    }
}
