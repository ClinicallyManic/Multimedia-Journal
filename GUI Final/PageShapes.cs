using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace GUI_Final
{
    public class PageShapes
    {
        public PageShapes() { }
        public PageShapes(PageShapes other) 
        {
            this.media = other.media;
            this.Box = other.Box;
            this.image = other.image;
            this.rectangle = other.rectangle;
            this.words = other.words;
        }
        public int Box { get; set; }
        public MediaElement media { get; set; }
        public ImageBrush image { get; set; }
        public Windows.UI.Xaml.Shapes.Rectangle rectangle { get; set; }
        public TextBox words { get; set; }
    }
}
