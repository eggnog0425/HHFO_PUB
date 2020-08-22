using DryIoc.Messages;
using HHFO.Models;
using HHFO.Models.Logic.EventAggregator.Tweet;
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
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace HHFO.ViewModels
{
    public partial class ShellViewModel : BindableBase, IDisposable
    {
        private CompositeDisposable Disposable { get; }

        public string Title { get; } = new SettingUtils().GetCommonSetting().Title;
        private Authorization authorization { get; set; }
        public IListProvider ListProvider { get; set; }

        public ReactiveCommand<System.Windows.Input.MouseButtonEventArgs> OpenList { get; }
        public ReactiveCommand OnLoaded { get; }
        public ReactiveCommand OpenBrowser { get; }
        public ReactiveCommand InitialAuth { get; }
        public ReactiveCommand<RoutedEventArgs> ExpandedLists { get; } = new ReactiveCommand<RoutedEventArgs>();

        public static TweetRoutedCommand TweetRoutedCommand { get; } = new TweetRoutedCommand();
        public ReactiveProperty<bool> IsOpenTweetFlyOut { get; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> IsOpenAuthFlyOut { get; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<String> Pin { get; set; } = new ReactiveProperty<string>();
        public ReactiveProperty<Visibility> PinError { get; } = new ReactiveProperty<Visibility>(Visibility.Collapsed);
        public ReactiveProperty<Visibility> MenuVisibility { get; } = new ReactiveProperty<Visibility>(Visibility.Hidden);

        public ReactiveProperty<string> Tweet { get; } = new ReactiveProperty<string>("");
        public ObservableCollection<CoreTweet.List> Lists { get; private set; } = new ObservableCollection<CoreTweet.List>();
        public ReactiveProperty<double> TweetAreaHeight { get; private set; } = new ReactiveProperty<double>(0.0d);

        private bool IsOpenSetting { get; set; } = true;

        public ShellViewModel(IRegionManager regionManager, IListProvider listProvider)
        {
            Disposable = new CompositeDisposable();
            OnLoaded = new ReactiveCommand()
                .AddTo(Disposable);
            OpenBrowser = new ReactiveCommand()
                .AddTo(Disposable);
            InitialAuth = new ReactiveCommand()
                .AddTo(Disposable);
            OpenList = new ReactiveCommand<System.Windows.Input.MouseButtonEventArgs>()
                .AddTo(Disposable);
            ListProvider = listProvider;

            ExpandedLists.Subscribe(_ => FetchTwitterLists());
            OnLoaded.Subscribe(_ => Loaded());
            OpenBrowser.Subscribe(_ => OpenBrowserAction());
            InitialAuth.Subscribe(_ => Auth());
            OpenList.Subscribe(e => OpenListAction(e));
        }

        private void OpenListAction(MouseButtonEventArgs e)
        {
            ListProvider.Id = ((TextBlock)e.Source).Tag.ToString();
            ListProvider.Publish();
        }

        public void Dispose()
        {
            this.Disposable.Dispose();
        }

        private void Loaded()
        {
            if (!Authorization.Authed())
            {
                IsOpenAuthFlyOut.Value = true;
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
                IsOpenAuthFlyOut.Value = false;
                MenuVisibility.Value = Visibility.Visible;
            }
            else
            {
                PinError.Value = Visibility.Visible;
            }
        }

        public void OpenTweetFlyOutAction(object sender, ExecutedRoutedEventArgs args)
        {
                IsOpenTweetFlyOut.Value = !IsOpenTweetFlyOut.Value;
        }
        private void FetchTwitterLists()
        {
            try
            {
                Lists.Clear();
                var token = Authorization.GetToken();
                Lists.AddRange(token.Lists.List().AsEnumerable());
            }
            catch (TwitterException)
            {
                // APIリミットとかサーバエラーはどうしようもないので握りつぶす
            }
        }

        public void CanOpenTweetFlyOut(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = !IsOpenSetting;
    }
}
