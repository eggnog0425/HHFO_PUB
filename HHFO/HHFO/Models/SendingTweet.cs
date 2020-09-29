using CoreTweet;
using HHFO.Models.Data;
using HHFO.Models.Logic.EventAggregator.Tweets;
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
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace HHFO.Models
{
    public class SendingTweet : BindableBase, IDisposable
    {
        private CompositeDisposable Disposable = new CompositeDisposable();

        public ReactivePropertySlim<long> InReplyTo { get; private set; } = new ReactivePropertySlim<long>(0);
        public ReactivePropertySlim<string> InReplyToMessage { get; private set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> Text { get; set; } = new ReactivePropertySlim<string>("");
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
            InReplyToMessage.Value = "";
            IsInit = true;
        }

        public void ClearInReplyTo()
        {
            if (!IsInit && string.IsNullOrEmpty(Text.Value))
            {
                InReplyTo.Value = 0;
                InReplyToMessage.Value = "";
                IsInit = true;
            }
        }
        public void ClearInReplyToForce()
        {
                InReplyTo.Value = 0;
                InReplyToMessage.Value = "";
        }

        public void AddReply(IList<Tweet> tweets)
        {
            var cnt = tweets?.Count() ?? 0;
            if (cnt == 0)
            {
                return;
            }
            if (cnt == 1)
            {
#pragma warning disable CS8602 // 僕のnull合体演算子にVisualStudioがついてこれなかった
                var t = tweets[0];
#pragma warning restore CS8602 // 
                InReplyTo.Value = t.IsRetweetedTweet
                    ? t.RetweetedId
                    : t.Id;
                InReplyToMessage .Value= t.FullText;
            }

            var reply = tweets.Select(t => "@" + t.ScreenName)
                .Distinct()
                .Append(Text.Value);
            Text.Value = string.Join(' ', reply);
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
