﻿using ControlzEx.Standard;
using CoreTweet;
using HHFO.Models;
using HHFO.Models.Logic.EventAggregator.Tweet;
using ImTools;
using MahApps.Metro.Controls;
using NLog;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.ObjectExtensions;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Unity;
using Status = CoreTweet.Status;

namespace HHFO.ViewModels
{
    public class TweetsViewModel : BindableBase, IDisposable
    {
        public ReactiveProperty<bool> IsExpandedHeader { get; private set; } = new ReactiveProperty<bool>(true);

        private static Logger logger = LogManager.GetCurrentClassLogger();
        private CompositeDisposable Disposable { get; set; }
        private ListProvider ListProvider { get; set; }
        public TabBase CurrentTab { get; private set; }

        private FrameworkElement ParentElement { get; set; }
        private TabControl TabControl { get; set; }
        private ITweetPublisher TweetPublisher { get; set; }
        public ModifierKeys ModifierKeys { get; } = ModifierKeys.Control | ModifierKeys.Shift;

        private ReadOnlyReactiveProperty<long> ListId { get; }
        public ObservableCollection<TabBase> Tabs { get; }
        public ReactiveProperty<bool> IsOpenCheckBoxArea { get; set; } = new ReactiveProperty<bool>(true);

        public ReactiveCommand<RoutedEventArgs> OnLoaded { get; }
        public ReactiveCommand<SelectionChangedEventArgs> OnCurrentTabChanged { get; }
        public ReactiveCommand<System.Windows.Input.MouseButtonEventArgs> OnTabClose { get; }
        public ReactiveCommand ReloadPast { get; }
        public ReactiveCommand<RoutedEventArgs> TabCloseCommand { get; }
        private static ConcurrentDictionary<int, TabBase> SurrogateKeyDictionary { get; } = new ConcurrentDictionary<int, TabBase>();


        public TweetsViewModel(ListProvider ListIdProvider, ITweetPublisher tweetPublisher)
        {
            Disposable = new CompositeDisposable();

            this.ListProvider = ListIdProvider;
            ListId = this.ListProvider.Id.ToReadOnlyReactiveProperty();
            TweetPublisher = tweetPublisher;
            Tabs = new ObservableCollection<TabBase>();

            OnLoaded = new ReactiveCommand<RoutedEventArgs>()
                .AddTo(Disposable);
            OnCurrentTabChanged = new ReactiveCommand<SelectionChangedEventArgs>()
                .AddTo(Disposable);
            OnTabClose = new ReactiveCommand<System.Windows.Input.MouseButtonEventArgs>()
                .AddTo(Disposable);
            ReloadPast = new ReactiveCommand()
                .AddTo(Disposable);
            TabCloseCommand = new ReactiveCommand<RoutedEventArgs>()
                .AddTo(Disposable);

            ListId.Subscribe(async e => await OpenListTabAction(e))
                .AddTo(Disposable);
            OnLoaded.Subscribe(e => OnLoadedAction(e))
                .AddTo(Disposable);
            OnCurrentTabChanged.Subscribe(e => OnCurrentTabChangedAction())
                .AddTo(Disposable);
            ReloadPast.Subscribe(_ => ReloadPastAction())
                .AddTo(Disposable);
            TabCloseCommand.Subscribe(e => TabClose(e))
                .AddTo(Disposable);
        }

        private void TabClose(RoutedEventArgs e)
        {

        }

        private void ReloadPastAction()
        {
            if (CurrentTab == null)
            {
                return;
            }
            CurrentTab.ReloadPastAsync();
        }

        private void OnTabCloseAction(MouseButtonEventArgs e)
        {
            var hoge = e.Source.GetType().ToString();
        }

        private void OnCurrentTabChangedAction()
        {
            CurrentTab = (TabBase)TabControl.SelectedItem;
        }

        private void OnLoadedAction(RoutedEventArgs e)
        {
            ParentElement = (FrameworkElement)e.OriginalSource;

            foreach (var item in ParentElement.GetChildObjects())
            {
                if (item is TabControl)
                {
                    TabControl = item as TabControl;
                    return;
                }
            }
        }

        private async Task OpenListTabAction(long id)
        {
            if (id == 0)
            {
                return;
            }
            
            await Task.Run(async () =>
            {
                try
                {
                    var surrogateKey = SurrogateKeyDictionary.Keys.Max();
                    var tab = await TabList.Create(id, surrogateKey);
                    lock (Tabs)
                    {
                        Tabs.Add(tab);
                    }
                    SelectTabByVM(surrogateKey);
                }
                catch (TwitterException)
                {
                        // TODO メッセージを発行する形に変更
                        //MessageBox.Show("リストの取得に失敗しました。");
                    }
            }).ConfigureAwait(false);
        }

        private void SelectTabByVM(int surrogateKey)
        {
            var d = Application.Current.Dispatcher;
            d.Invoke(() =>
            {
                var i = 0;
                foreach (TabBase item in TabControl.Items)
                {
                    if (item.SurrogateKey == surrogateKey)
                    {
                        TabControl.SelectedIndex = i;
                        return;
                    }
                    i++;
                }
            });
        }

        public void Dispose()
        {
            this.Disposable.Dispose();
        }
    }

}
