using ControlzEx.Standard;
using CoreTweet;
using HHFO.Core.Models;
using HHFO.Models;
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
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private CompositeDisposable Disposable { get; }
        private ListSubscriber ListSubscriber { get; set; }

        private TabControl TabControl { get; set; }
        private Grid Grid { get; set; }

        private ReadOnlyReactiveProperty<string> ListId { get; }
        public ObservableCollection<Tab> Tabs { get; }

        public ReactiveProperty<double> DataGridHeight { get; set; } = new ReactiveProperty<double>(1.0d);
        public ReactiveProperty<double> DataGridWidth { get; set; } = new ReactiveProperty<double>(1.0d);
        public const double CheckBoxHeight = 30.0d;
        public const double DataGridHeightMargin = 45.0d;

        public ReactiveCommand OnSizeChanged { get; }
        public ReactiveCommand<EventArgs> SelectStatuses { get; }
        public ReactiveCommand<RoutedEventArgs> OnLoaded { get; }

        private bool IsFilteredLink = false;
        private bool IsFilteredImages = false;
        private bool IsFilteredVideos = false;

        private Func<Status, bool> FilterLink = tweet => tweet.Entities.Urls != null && tweet.Entities.Urls.Length != 0;
        //private Func<Status, bool> FilterImages =  tweet => tweet.ExtendedEntities != null && tweet.ExtendedEntities.Media[0].Type == ";
        //private Func<Status, bool> FilterVideos

        public TweetsViewModel(ListSubscriber ListIdSubscriber)
        {
            Disposable = new CompositeDisposable();
            this.ListSubscriber = ListIdSubscriber;
            ListId = this.ListSubscriber.Id.ToReadOnlyReactiveProperty();
            Tabs = new ObservableCollection<Tab>();

            OnSizeChanged = new ReactiveCommand()
                .AddTo(Disposable);
            SelectStatuses = new ReactiveCommand<EventArgs>()
                .AddTo(Disposable);
            OnLoaded = new ReactiveCommand<RoutedEventArgs>()
                .AddTo(Disposable);

            OnSizeChanged.Subscribe(_ => OnSizeChangedAction());
            ListId.Subscribe(e => OpenListAction(e));
            OnLoaded.Subscribe(e => OnLoadedAction(e));
            SelectStatuses.Subscribe(e => SelectStatusesAction(e));
        }

        private void ChangeDataGridSize()
        {
            DataGridHeight.Value = Grid.ActualHeight - CheckBoxHeight - DataGridHeightMargin;
            DataGridWidth.Value = Grid.ActualWidth;
        }

        private void OnSizeChangedAction()
        {
            ChangeDataGridSize();
        }

        private void OnLoadedAction(RoutedEventArgs e)
        {
            Grid = (Grid)e.Source;
            ChangeDataGridSize();

            foreach (var item in Grid.Children)
            {
                if (item is TabControl)
                {
                    TabControl = (TabControl)item;
                    return;
                }
            }
        }

        private void SelectStatusesAction(EventArgs e)
        {
            e.GetType();
        }

        private void OpenListAction (string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return;
            }

            try
            {
                var tab = Tabs.FirstOrDefault(t => t.Id == id);
                if (tab == null)
                {
                    tab = new Tab(id);
                    Tabs.Add(tab);
                }
                SelectTabByVM(id);
            }
            catch (TwitterException)
            {
                MessageBox.Show("リストの取得に失敗しました。");
            }
        }

        private void SelectTabByVM(string id)
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
