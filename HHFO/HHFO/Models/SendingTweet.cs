using CoreTweet;
using HHFO.Models.Logic.EventAggregator.Tweet;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace HHFO.Models
{
    public class SendingTweet : BindableBase, IDisposable
    {
        private CompositeDisposable Disposable = new CompositeDisposable();

        public ReactiveProperty<long> InReplyTo { get; private set; } = new ReactiveProperty<long>(0);
        public ReactiveProperty<string> Text { get; set; } = new ReactiveProperty<string>("");

        /// <summary>
        /// 発言内容を初期から変えていない場合true。1文字以上入力するとfalse。発言全削除または発言成功でtrue。
        /// 自発言に発言をぶらさげるケースなど@ScreenNameなしでリプライを送るために使用。
        /// </summary>
        private bool IsInit = true;

        public SendingTweet()
        {
            Text.Subscribe(t => ClearInReplyTo())
                .AddTo(Disposable);
        }

        public void Reflesh()
        {
            InReplyTo.Value = 0;
            Text.Value = "";
            IsInit = true;
        }

        public void ClearInReplyTo()
        {
            if (!IsInit && string.IsNullOrEmpty(Text.Value))
            {
                InReplyTo.Value = 0;
                IsInit = true;
            }
        }

        public void AddReply(TweetInfo tweet)
        {
            var replyUsers = tweet.UserScreenNames.Select(name => "@" + name);
            Text.Value = string.Join(' ', replyUsers, Text.Value);
        }

        public void Dispose()
        {
            Disposable.Dispose();
        }

        public bool Send()
        {
            var token = Authorization.GetToken();
            try
            {
                if (InReplyTo.Value == 0)
                {
                    token.Statuses.Update(status: Text.Value);
                }
                else
                {
                    token.Statuses.Update(status: Text.Value, in_reply_to_status_id: InReplyTo.Value);  
                }
            }
            catch (TwitterException)
            {
                return false;
            }
            Reflesh();
            return true;
        }
    }
}
