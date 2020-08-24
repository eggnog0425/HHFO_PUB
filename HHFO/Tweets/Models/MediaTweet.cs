
using CoreTweet;
using System;
using System.Collections.Generic;
using System.Text;

namespace HHFO.Models
{
    public class MediaTweet
    {
        public long Id { get; private set; }

        public string MediaUrl {get; private set;}


        MediaTweet(long id, string mediaUrl)
        {
            Id = id;
            MediaUrl = mediaUrl;
        }
    }
}
