using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using HHFO.Models.Data;

namespace HHFO.Models.UI
{
    public class MediaTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ImageTemplate { get; set; }
        public DataTemplate MovieTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var media = item as Media;
            switch (media.Type)
            {
                case "video":
                    return MovieTemplate;

                case "photo":
                case "animation_gif":
                default:
                    return ImageTemplate;
            }
        }
    }
}
