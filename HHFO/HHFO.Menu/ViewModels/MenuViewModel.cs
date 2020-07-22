using CoreTweet;
using CoreTweet.Core;
using HHFO.Models;
using ImTools;
using NLog;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HHFO.Menu.ViewModels
{
    public class MenuViewModel : BindableBase
    {
        public string Home { get; } = "Home";
        public string List { get; } = "List";
        public string Auth { get; } = "Auth";
        public ReadOnlyReactiveProperty<int> Width { get; }
        public ReadOnlyReactiveProperty<IReadOnlyList<CoreTweet.List>> Lists { get; }

        private AbstractMenu Menu;
        public ReactiveCommand<RoutedEventArgs> ExpandedLists { get; } = new ReactiveCommand<RoutedEventArgs>();
        public ReactiveCommand<System.Windows.Input.MouseButtonEventArgs> Aaa { get; } = new ReactiveCommand<System.Windows.Input.MouseButtonEventArgs>();

        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        public MenuViewModel(AbstractMenu Menu)
        {
            this.Menu = Menu;
            Width = this.Menu.ObserveProperty(m => m.Width)
                .ToReadOnlyReactiveProperty();
            Lists = this.Menu.ObserveProperty(m => m.Lists)
                .ToReadOnlyReactiveProperty();

            ExpandedLists.Subscribe(_ =>
            {
                FetchLists();
            });

            Aaa.Subscribe(e => {
                Logger logger = LogManager.GetCurrentClassLogger();
                MessageBox.Show(((TextBlock)e.Source).Tag.ToString());
            });
        }
        private void FetchLists()
        {
            Menu.FetchList();
        }
    }

}
