using HHFO.Models.Data;
using HHFO.Models.Interface;
using HHFO.Models.Logic.EventAggregator.Tweets;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Reactive.Disposables;
using System.ComponentModel;

namespace HHFO.Models
{
    public class Messanger4Tweets: IMessanger<IList<Tweet>>
    {
        public ObservableCollection<Tweet> SelectedTweets { get; set; }
        public ReactivePropertySlim<Tweet> SelectedTweet { get; set; } = new ReactivePropertySlim<Tweet>();
        private CompositeDisposable Disposable { get; set; } = new CompositeDisposable();
        private ITweetsProvider TweetsProvider { get; set; }
        private IList<IObserver<IList<Tweet>>> Observers { get; } = new List<IObserver<IList<Tweet>>>();

        public Messanger4Tweets(ITweetsProvider tweetsProvider)
        {
            TweetsProvider = tweetsProvider;
            TweetsProvider.PropertyChanged += ChangeSelectTweets;
        }

        public void Dispose()
        {
            this.Disposable.Dispose();
        }

        public IDisposable Subscribe(IObserver<IList<Tweet>> observer)
        {
            if(!Observers.Contains(observer))
            {
                Observers.Add(observer);
            }
            return new Unsubscriber(Observers, observer);
        }

        private void ChangeSelectTweets(Object sender, PropertyChangedEventArgs e)
        {
            {
                if (e.PropertyName == nameof(TweetsProvider.Tweets))
                {
                    SelectedTweets = new ObservableCollection<Tweet>(TweetsProvider.Tweets);
                    SelectedTweet.Value = TweetsProvider.Tweets.LastOrDefault();
                }

                if (e.PropertyName == nameof(TweetsProvider.IsReply))
                {
                    foreach(var observer in Observers)
                    {
                        observer.OnNext(SelectedTweets);
                    }
                }
            };
        }

        private class Unsubscriber : IDisposable
        {
            private IList<IObserver<IList<Tweet>>> _observers;
            private IObserver<IList<Tweet>> _observer;

            public Unsubscriber(IList<IObserver<IList<Tweet>>> observers, IObserver<IList<Tweet>> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }

    }
}
