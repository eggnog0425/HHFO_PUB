using ControlzEx.Standard;
using CoreTweet;
using HHFO.Models;
using HHFO.Models.Logic.API;
using HHFO.Models.Logic.EventAggregator.Tweets;
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
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private CompositeDisposable Disposable { get; set; }
        private ListProvider ListProvider { get; set; }
        public TabBase CurrentTab { get; private set; }

        private TabControl TabControl { get; set; }

        private ReadOnlyReactivePropertySlim<long> ListId { get; }
        public ObservableCollection<TabBase> Tabs { get; }
        public ReactivePropertySlim<bool> IsOpenCheckBoxArea { get; set; } = new ReactivePropertySlim<bool>(true);
        public ModifierKeys ModifierKeys { get; } = ModifierKeys.Control | ModifierKeys.Shift;

        public ReactiveCommand<RoutedEventArgs> OnLoaded { get; }
        public ReactiveCommand<SelectionChangedEventArgs> OnCurrentTabChanged { get; }
        public ReactiveCommand ReloadPast { get; }
        public ReactiveCommand<RoutedEventArgs> TabCloseCommand { get; }
        private static ConcurrentDictionary<int, TabBase> SurrogateKeyDictionary { get; } = new ConcurrentDictionary<int, TabBase>();
        private static CalcReloadTimeBase ListReloadTimeCalclator = null;
        private ITabFactory TabFactory { get; set; }

        public TweetsViewModel(ListProvider ListIdProvider, ITabFactory tabFactory)
        {
            Disposable = new CompositeDisposable();

            TabFactory = tabFactory;
            this.ListProvider = ListIdProvider;
            ListId = this.ListProvider.Id.ToReadOnlyReactivePropertySlim();
            Tabs = new ObservableCollection<TabBase>();

            // TODO 前回起動時のタブを呼び出す処理を追加

            OnLoaded = new ReactiveCommand<RoutedEventArgs>()
                .AddTo(Disposable);
            OnCurrentTabChanged = new ReactiveCommand<SelectionChangedEventArgs>()
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
            TabCloseCommand.Subscribe(e => OnTabClose(e))
                .AddTo(Disposable);
        }

        private void ReloadPastAction()
        {
            if (CurrentTab == null)
            {
                return;
            }
            CurrentTab.ReloadPastAsync();
        }

        private void OnTabClose(RoutedEventArgs e)
        {
            if (e.Source is Button b)
            {
                var target = Tabs.FirstOrDefault(t => t.SurrogateKey == (int)b.Tag);
                if (target != null)
                {
                    Tabs.Remove(target);
                    target.Dispose();
                }
            }
        }

        private void OnCurrentTabChangedAction()
        {
            CurrentTab = (TabBase)TabControl.SelectedItem;
        }

        private void OnLoadedAction(RoutedEventArgs e)
        {
            var parentElement = (FrameworkElement)e.OriginalSource;

            foreach (var item in parentElement.GetChildObjects())
            {
                if (item is TabControl tc)
                {
                    TabControl = tc;
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
            var surrogateKey = GetTabIndex();
            TabBase tab = null;
            try
            {
                tab = await TabFactory.CreateListTab(id, surrogateKey);
            }
            catch
            {
                ListProvider.Id.Value = 0;
                return;
            }
            AddTab(tab);
            if (ListReloadTimeCalclator == null)
            {
                var targetCnt = Tabs.Count() == 0
                    ? 1
                    : Tabs.Count(t => t.GetType() == typeof(TabList));
                ListReloadTimeCalclator = CalcReloadTimeList.GetInstance(targetCnt, tab.Limit);
                tab.RefleshTimer(ListReloadTimeCalclator.CalcReloadTime(tab.Limit));
            }
            else
            {
                tab.RefleshTimer(ListReloadTimeCalclator.AddTab(tab.Limit));
            }
            ListProvider.Id.Value = 0;
        }

        private void RefleshReloadTime(TimeSpan ts, Type T)
        {
            foreach(var tab in Tabs.Where(tab => tab.GetType() == T))
            {
                tab.RefleshTimer(ts);
            }
        }

        private void AddTab(TabBase tab)
        {
            lock (Tabs)
            {
                Tabs.Add(tab);
                SelectTabByVM(tab.SurrogateKey);
            }
            tab.Reloaded += HandleReloaded;
        }

        private int GetTabIndex()
        {
            var surrogateKey = SurrogateKeyDictionary.Count() == 0
                ? 0
                : SurrogateKeyDictionary.Keys.Max() + 1;
            while (!SurrogateKeyDictionary.TryAdd(surrogateKey, null))
            {
                surrogateKey++;
            }
            return surrogateKey;
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

        private async Task HandleReloaded(object sender, EventArgs e)
        {
            if (sender is TabList tab)
            {
                await Task.Run(() =>
                {
                    var limit = tab.Limit;
                    var nextReloadTime = tab.RefleshTimer(ListReloadTimeCalclator.CalcReloadTime(limit));
                    tab.RefleshDispApiInfo(nextReloadTime);
                });
            }
        }

        public void Dispose()
        {
            this.Disposable.Dispose();
        }
    }

}
