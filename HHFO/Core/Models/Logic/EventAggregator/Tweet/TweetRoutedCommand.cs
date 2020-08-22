using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace HHFO.Models.Logic.EventAggregator.Tweet
{
    public class TweetRoutedCommand: RoutedCommand
    {
        public Tweet Tweet;
    }
}
