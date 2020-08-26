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
using MahApps.Metro.Controls;
using HHFO.Models.UI;

namespace HHFO.ViewModels
{
    public partial class ShellViewModel : BindableBase, IDisposable
    {
        private CompositeDisposable Disposable { get; }

        public string Title { get; } = new SettingUtils().GetCommonSetting().Title;
        private Authorization authorization { get; set; }
        private IListPublisher ListPublisher { get; set; }

        public ReactiveCommand<System.Windows.Input.MouseButtonEventArgs> OpenList { get; }
        public ReactiveCommand OnLoaded { get; }
        public ReactiveCommand OpenBrowser { get; }
        public ReactiveCommand InitialAuth { get; }
        public ReactiveCommand<RoutedEventArgs> ExpandedLists { get; }
        public ReactiveCommand OpenTweetFlyOut { get; }

        public ReactiveProperty<bool> IsOpenTweetFlyOut { get; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> IsOpenAuthFlyOut { get; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<String> Pin { get; set; } = new ReactiveProperty<string>();
        public ReactiveProperty<Visibility> PinError { get; } = new ReactiveProperty<Visibility>(Visibility.Collapsed);
        public ReactiveProperty<Visibility> MenuVisibility { get; } = new ReactiveProperty<Visibility>(Visibility.Hidden);
        public ReactiveProperty<TweetInfo> SelectedTweet { get; private set; } = new ReactiveProperty<TweetInfo>();
        public ReactiveProperty<string> InReplyTo { get; private set; } = new ReactiveProperty<string>("");

        public ReactiveProperty<string> Tweet { get; set; } = new ReactiveProperty<string>("");
        public ObservableCollection<CoreTweet.List> Lists { get; private set; } = new ObservableCollection<CoreTweet.List>();
        public ReactiveProperty<double> TweetAreaHeight { get; private set; } = new ReactiveProperty<double>(0.0d);

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
            SelectedTweet.Subscribe(e => OpenTweetFlyOutAction(e));
            Tweet.Subscribe(t => ClearInReplyTo());
        }

        private void OpenListAction(MouseButtonEventArgs e)
        {
            ListPublisher.Id = long.Parse(((TextBlock)e.Source).Tag?.ToString() ?? "0");
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
            IsOpenTweetFlyOut.Value = !IsOpenTweetFlyOut.Value;
        }
        
        public void OpenTweetFlyOutAction(TweetInfo tweet)
        {
            if (tweet == null)
            {
                return;
            }

            IsOpenTweetFlyOut.Value = true;
            
            if (tweet.InReplyToScreenName == null)
            {
                string.Join(' ', tweet.UserScreenName, Tweet.Value);
            }
            else
            {
                string.Join(' ', tweet.UserScreenName, tweet.InReplyToScreenName, Tweet.Value);
            }
            
        }

        public void ClearInReplyTo(bool saveInReplyTo = false)
        {
            if (!saveInReplyTo && string.IsNullOrWhiteSpace(Tweet.Value))
            {
                InReplyTo.Value = "";
            }
            
        }
    }
}
