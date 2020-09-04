using ControlzEx.Standard;
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
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
        public Tab CurrentTab { get; private set; }

        private FrameworkElement ParentElement { get; set; }
        private TabControl TabControl { get; set; }
        private ITweetPublisher TweetPublisher { get; set; }
        public ModifierKeys ModifierKeys { get; } = ModifierKeys.Control | ModifierKeys.Shift;

        private ReadOnlyReactiveProperty<long> ListId { get; }
        public ObservableCollection<Tab> Tabs { get; }
        public ReactiveProperty<bool> IsOpenCheckBoxArea { get; set; } = new ReactiveProperty<bool>(true);

        public ReactiveCommand<RoutedEventArgs> OnLoaded { get; }
        public ReactiveCommand<SelectionChangedEventArgs> OnCurrentTabChanged { get; }
        public ReactiveCommand<System.Windows.Input.MouseButtonEventArgs> OnTabClose { get; }
        public ReactiveCommand ReloadPast { get; }
        public ReactiveCommand<RoutedEventArgs> TabCloseCommand { get; }


        public TweetsViewModel(ListProvider ListIdProvider, ITweetPublisher tweetPublisher)
        {
            Disposable = new CompositeDisposable();
            this.ListProvider = ListIdProvider;
            ListId = this.ListProvider.Id.ToReadOnlyReactiveProperty();
            TweetPublisher = tweetPublisher;
            Tabs = new ObservableCollection<Tab>();

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

            ListId.Subscribe(e => OpenListTabAction(e))
                .AddTo(Disposable);
            OnLoaded.Subscribe(e => OnLoadedAction(e))
                .AddTo(Disposable);
            OnCurrentTabChanged.Subscribe(e => OnCurrentTabChangedAction())
                .AddTo(Disposable);
            OnTabClose.Subscribe(e => OnTabCloseAction(e))
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
            CurrentTab = (Tab)TabControl.SelectedItem;
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

        private void OpenListTabAction (long id)
        {
            if (id == 0)
            {
                return;
            }

            try
            {
                var tab = Tabs.FirstOrDefault(t => t.Id == id);
                if (tab == null)
                {
                    tab = new TabList(id, TweetPublisher);
                    Tabs.Add(tab);
                }
                SelectTabByVM(id);
            }
            catch (TwitterException)
            {
                MessageBox.Show("リストの取得に失敗しました。");
            }
        }

        private void SelectTabByVM(long id)
        {
            var i = 0;
            foreach (Tab item in TabControl.Items)
            {
                if (item.Id == id)
                {
                    TabControl.SelectedIndex = i;
                    return;
                }
                i++;
            }
        }

        public void Dispose()
        {
            this.Disposable.Dispose();
        }
    }

}
