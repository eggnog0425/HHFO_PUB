using System;
using System.Collections.Generic;
using System.Text;

namespace HHFO.Models.UI
{
    public class Link
    {
        public string AfterText { get; }
        public int Start { get; }
        public int End { get; }

        public Link(string text, int start, int end)
        {
            AfterText = text;
            Start = start;
            End = end;
        }

    }
}
