using System;
using System.Collections.Generic;
using System.Text;

namespace HHFO.Models
{
    public class Media
    {
        public long Id { get; private set; }
        public string Type { get; private set; }
        public string MediaUrl { get; private set; }

        public Media(long id, string type, string mediaUrl)
        {
            Id = id;
            Type = type;
            MediaUrl = mediaUrl;
        }
    }
}
