using DryIoc.Messages;
using HHFO.Models;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Windows;
using Unity;
using CoreTweet;
using CoreTweet.Rest;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Windows.Controls;
using HHFO.Core.Models;

namespace HHFO.ViewModels
{
    class ShellViewModel : BindableBase, IDisposable
    {
        private CompositeDisposable Disposable { get; }

        public string Title { get; } = new SettingUtils().GetCommonSetting().Title;
        private Authorization authorization { get; set; }
        private AbstractMenu Menu;
        [Unity.Dependency]
        public IListProvider ListProvider { get; set; }

        public ReactiveCommand<System.Windows.Input.MouseButtonEventArgs> OpenList { get; }
        public ReactiveCommand OnLoaded { get; }
        public ReactiveCommand OpenBrowser { get; }
        public ReactiveCommand InitialAuth { get; }
        public ReactiveCommand OpenTweetSpace { get; }
        public ReactiveCommand<RoutedEventArgs> ExpandedLists { get; } = new ReactiveCommand<RoutedEventArgs>();

        public ReactiveProperty<bool> OpenFlyOut { get; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<String> Pin { get; set; } = new ReactiveProperty<string>();
        public ReactiveProperty<Visibility> PinError { get; } = new ReactiveProperty<Visibility>(Visibility.Collapsed);
        public ReactiveProperty<Visibility> MenuVisibility { get; } = new ReactiveProperty<Visibility>(Visibility.Hidden);
        public ReactiveProperty<Visibility> TweetVisibility { get; } = new ReactiveProperty<Visibility>(Visibility.Collapsed);
        public ReactiveProperty<string> Tweet { get; } = new ReactiveProperty<string>("test11");
        public ReadOnlyReactiveProperty<IReadOnlyList<CoreTweet.List>> Lists { get; }

        public ShellViewModel(IRegionManager regionManager, AbstractMenu menu)
        {
            Menu = menu;
            Lists = this.Menu.ObserveProperty(m => m.Lists)
                .ToReadOnlyReactiveProperty();

            Disposable = new CompositeDisposable();
            OnLoaded = new ReactiveCommand()
                .AddTo(Disposable);
            OpenBrowser = new ReactiveCommand()
                .AddTo(Disposable);
            InitialAuth = new ReactiveCommand()
                .AddTo(Disposable);
            OpenTweetSpace = new ReactiveCommand()
                .AddTo(Disposable);
            OpenList = new ReactiveCommand<System.Windows.Input.MouseButtonEventArgs>()
                .AddTo(Disposable);

            ExpandedLists.Subscribe(_ => Menu.FetchList());
            OnLoaded.Subscribe(_ => Loaded());
            OpenBrowser.Subscribe(_ => OpenBrowserAction());
            InitialAuth.Subscribe(_ => Auth());
            OpenTweetSpace.Subscribe(_ => OpenTweetSpaceAction());
            OpenList.Subscribe(e =>
            {
                ListProvider.Id = ((TextBlock)e.Source).Tag.ToString();
                ListProvider.Publish();
            });
        }

        public void Dispose()
        {
            this.Disposable.Dispose();
        }

        private void Loaded()
        {
            if (!Authorization.Authed())
            {
                OpenFlyOut.Value = true;
            }
            else
            {
                MenuVisibility.Value = Visibility.Visible;
            }
        }

        private void OpenBrowserAction()
        {
            authorization = new Authorization();
            authorization.OpenAuthPage();
        }

        private void Auth()
        {
            if (authorization != null && authorization.InitialAuth(Pin.Value))
            {
                OpenFlyOut.Value = false;
                MenuVisibility.Value = Visibility.Visible;
            }
            else
            {
                PinError.Value = Visibility.Visible;
            }
        }

        private void OpenTweetSpaceAction()
        {
            lock (TweetVisibility) 
            { 
                if (TweetVisibility.Value == Visibility.Visible)
                {
                    TweetVisibility.Value = Visibility.Collapsed;
                }
                else
                {
                    TweetVisibility.Value = Visibility.Visible;
                }
            }
        }
    }
}
