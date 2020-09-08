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
using System.Collections.Generic;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MahApps.Metro.Controls;

namespace HHFO.ViewModels
{
    public partial class ShellViewModel : BindableBase, IDisposable
    {
        private CompositeDisposable Disposable { get; }

        public string Title { get; } = new SettingUtils().GetCommonSetting().Title;
        private Authorization authorization { get; set; }
        private IListPublisher ListPublisher { get; set; }
        public ModifierKeys ModifierKeys { get; } = ModifierKeys.Control | ModifierKeys.Shift;

        public ReactiveCommand<System.Windows.Input.MouseButtonEventArgs> OpenList { get; }
        public ReactiveCommand OnLoaded { get; }
        public ReactiveCommand OpenBrowser { get; }
        public ReactiveCommand InitialAuth { get; }
        public ReactiveCommand<RoutedEventArgs> ExpandedLists { get; }
        public ReactiveCommand OpenTweetFlyOut { get; }
        public ReactiveCommand<KeyEventArgs> SendTweet { get; }

        public ReactivePropertySlim<bool> IsOpenTweetFlyOut { get; } = new ReactivePropertySlim<bool>(false);
        public ReactivePropertySlim<bool> IsOpenAuthFlyOut { get; } = new ReactivePropertySlim<bool>(false);
        public ReactivePropertySlim<String> Pin { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<Visibility> PinError { get; } = new ReactivePropertySlim<Visibility>(Visibility.Collapsed);
        public ReactivePropertySlim<Visibility> MenuVisibility { get; } = new ReactivePropertySlim<Visibility>(Visibility.Hidden);
        public ReactivePropertySlim<Visibility> SendErrorVisibility { get; } = new ReactivePropertySlim<Visibility>(Visibility.Collapsed);
        public SendingTweet Tweet { get; private set; } = new SendingTweet();

        public ObservableCollection<CoreTweet.List> Lists { get; private set; } = new ObservableCollection<CoreTweet.List>();

        private bool IsOpenSetting { get; set; } = false;

        public ShellViewModel(IRegionManager regionManager, IListPublisher listPublisher, ITweetProvider tweetProvider)
        {
            Disposable = new CompositeDisposable();
            ExpandedLists = new ReactiveCommand<RoutedEventArgs>()
                .AddTo(Disposable);
            OnLoaded = new ReactiveCommand()
                .AddTo(Disposable);
            OpenBrowser = new ReactiveCommand()
                .AddTo(Disposable);
            InitialAuth = new ReactiveCommand()
                .AddTo(Disposable);
            OpenList = new ReactiveCommand<System.Windows.Input.MouseButtonEventArgs>()
                .AddTo(Disposable);
            OpenTweetFlyOut = new ReactiveCommand()
                .AddTo(Disposable);
            SendTweet = new ReactiveCommand<KeyEventArgs>()
                .AddTo(Disposable);

            ListPublisher = listPublisher;

            ExpandedLists.Subscribe(_ => FetchTwitterLists())
                .AddTo(Disposable);
            OnLoaded.Subscribe(_ => Loaded())
                .AddTo(Disposable);
            OpenBrowser.Subscribe(_ => OpenBrowserAction())
                .AddTo(Disposable);
            InitialAuth.Subscribe(_ => Auth())
                .AddTo(Disposable);
            OpenList.Subscribe(e => OpenListAction(e))
                .AddTo(Disposable);
            OpenTweetFlyOut.Subscribe(_ => OpenTweetFlyOutAction())
                .AddTo(Disposable);
            tweetProvider.Tweet.Subscribe(e => OpenTweetFlyOutAction(e))
                .AddTo(Disposable);
            SendTweet.Subscribe(_ => SendTweetAction());
        }

        private void SendTweetAction()
        {
            if (Tweet.Send())
            {
                IsOpenTweetFlyOut.Value = false;
                SendErrorVisibility.Value = Visibility.Collapsed;
            } 
            else
            {
                SendErrorVisibility.Value = Visibility.Visible;
            }
        }

        private void OpenListAction(MouseButtonEventArgs e)
        {
            ListPublisher.Id.Value = long.Parse(((TextBlock)e.Source).Tag?.ToString() ?? "0");
            ListPublisher.Publish();
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

        public void OpenTweetFlyOutAction()       
        {
            var afterIsOpen = !IsOpenTweetFlyOut.Value && !IsOpenSetting;
            IsOpenTweetFlyOut.Value = afterIsOpen;
            if (!afterIsOpen)
            {
                SendErrorVisibility.Value = Visibility.Collapsed;
            }
        }
        
        public void OpenTweetFlyOutAction(TweetInfo tweet)
        {
            // 起動時に呼ばれちゃうようなのでnullの場合もガード
            if (tweet == null || IsOpenSetting)
            {
                return;
            }
            Tweet.AddReply(tweet);
            IsOpenTweetFlyOut.Value = true;
        }
    }
}
