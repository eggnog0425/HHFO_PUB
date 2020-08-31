using CoreTweet;
using ImTools;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Documents;

namespace HHFO.Models
{
    public class Tweet : BindableBase, IEquatable<Tweet>
    {
        // 常に元tweetの値が入る項目群
        public long Id { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public long UserId { get; private set; }
        public string ScreenName { get; private set; }
        public bool IsRetweetedTweet { get; private set; }

        // Retweetの場合のみ入る項目群
        public long RetweetedId { get; private set; }
        public DateTimeOffset RetweetedCreatedAt {get; private set;}
        public long RetweetedUserId { get; private set; }
        public string RetweetedUserScreenName { get; private set; }

        // Retweetの場合はRetweet先のstatusの値が入る項目群
        /// <summary>
        /// ListView用のtweet本文
        /// </summary>
        public string PlainFullText { get; private set; }

        /// <summary>
        /// tweet詳細用にHyperLinkの設定を追加したtweet本文
        /// </summary>
        public Paragraph FullText { get; private set; }
        public Tweet QuotedTweet { get; private set; }
        public bool HasLinks { get; private set; }
        public string[] Links { get; private set; }
        public bool HasMedias { get; private set; }
        public string Via { get; private set;}
        public string MediaType { get; private set; }
        public string[] MediaUrls { get; private set; }

        public Tweet(Status status)
        {
            Id = status.Id;
            CreatedAt = status.CreatedAt;
            UserId = status.User.Id ?? 0;
            ScreenName = status.User.ScreenName;

            IsRetweetedTweet = status.RetweetedStatus != null;
            if (IsRetweetedTweet)
            {
                var retweetedStatus = status.RetweetedStatus;
                RetweetedCreatedAt = retweetedStatus.CreatedAt;
                RetweetedUserId = retweetedStatus.User.Id ?? 0;
                RetweetedUserScreenName = retweetedStatus.User.ScreenName;
                AddFieldValues(retweetedStatus);
            }
            else
            {
                AddFieldValues(status);
            }
        }
        public void AddFieldValues(Status status)
        {
            PlainFullText = status.FullText;
            QuotedTweet = status.QuotedStatus == null
                ? null
                : new Tweet(status.QuotedStatus);
            Via = status.Source;
            var links = status.Entities.Urls;
            HasLinks = status.Entities.Urls != null;
            var hyperlinks = Enumerable.Empty<Link>();
            if (HasLinks)
            {
                hyperlinks = hyperlinks.Append(links.Select(l => new Link(l.ExpandedUrl, l.Indices[0], l.Indices[1])));
            }

            var medias = status.ExtendedEntities?.Media;
            HasMedias = medias != null;
            if (HasMedias)
            {
                MediaType = medias[0].Type;
                MediaUrls = medias.Select(m => m.MediaUrlHttps).ToArray();
                // MediaのURLは該当tweetの詳細画面に飛ぶURLになっているので最初の要素で置換
                hyperlinks = hyperlinks.Append(new Link(medias[0].DisplayUrl, medias[0].Indices[0], medias[0].Indices[1]));
            }

            var hashTags = status.Entities?.HashTags;
            if (hashTags != null)
            {
                hyperlinks = hyperlinks.Append(hashTags.Select(tag => new Link(tag.Text, tag.Indices[0], tag.Indices[1])));
            }
            FullText = new Paragraph();
            return;
        }

        // TODO パラグラフ作成実行(実際はShellVMでコンバータ使ってやることになると思う)
        /*
        private Paragraph MakeParagraph(IEnumerable<Link> links)
        {
            if (links.Count() == 0)
            {
                return new Paragraph();
            }
            var paragraph = new Paragraph();
            var sortedLinks = links.OrderBy(l => l.Start).ToList();
            var first = sortedLinks[0];
            if (first.Start == 0)
            {
                var hyperLink = new Hyperlink(new Run(PlainFullText.Substring(0, first.End)));
                hyperLink.NavigateUri =new Uri(first.AfterText);
                paragraph.Inlines.Add(hyperLink);
            }
        }
        */

        // override object.Equals
        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return this.Id == (obj as Tweet).Id;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return Id.ToString().GetHashCode();
        }

        public bool Equals([AllowNull] Tweet other)
        {
            return this.Id == other?.Id;
        }
    }
}
