using CoreTweet;
using CoreTweet.Core;
using HHFO.Models;
using ImTools;
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

namespace HHFO.Menu.ViewModels
{
    public class MenuViewModel : BindableBase
    {
        public ReactiveProperty<String> HomeLabel { get; }
        public ReactiveProperty<String> ListsLabel { get; }
        public ReactiveProperty<bool> HasOpen { get; }
        public ReactiveProperty<int> MenuWidth { get; }
        public ReactiveProperty<IReadOnlyList<CoreTweet.List>> Lists { get; }

        private AbstractMenu Menu;
        public ReactiveCommand<RoutedEventArgs> ExpandedLists { get; } = new ReactiveCommand<RoutedEventArgs>();
        public ReactiveCommand<RoutedEventArgs> CollapsedLists { get; } = new ReactiveCommand<RoutedEventArgs>();

        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        public MenuViewModel(AbstractMenu Menu)
        {
            this.Menu = Menu;
            HomeLabel = this.Menu.ToReactivePropertyAsSynchronized(m => m.HomeLabel)
                                 .AddTo(this.Disposable);
            ListsLabel = this.Menu.ToReactivePropertyAsSynchronized(m => m.ListLabel)
                                 .AddTo(this.Disposable);
            MenuWidth = this.Menu.ToReactivePropertyAsSynchronized(m => m.MenuWidth)
                                 .AddTo(this.Disposable);
            Lists = this.Menu.ToReactivePropertyAsSynchronized(m=>m.Lists)
                                 .AddTo(this.Disposable);

            ExpandedLists.Subscribe(_ =>
            {
                FetchLists();
                SpreadMenu();
            });
            CollapsedLists.Subscribe(_ =>
            {
                ShrinkMenu();
            });
        }

        private void SpreadMenu()
        {
            Menu.SpreadMenu();
        }
        private void ShrinkMenu()
        {
            Menu.ShrinkMenu();
        }
        private void FetchLists()
        {
            Menu.FetchList();
        }
    }

}
