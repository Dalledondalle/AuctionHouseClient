using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AuctionHouseClient.Shared
{
    public class Currency
    {
        public int Amount { get; set; }
        public string Name { get; set; }
        public string imagePath { get; set; }
        public ImageSource Image { get; }
        public Currency()
        {
            Image = makeImage();
        }
        private ImageSource makeImage()
        {
            Image img = new Image();
            img.BeginInit();
            img.Height = 50;
            img.Width = 50;
            img.Source = new BitmapImage(new Uri(@"\Images\" + Name + ".png", UriKind.Relative));
            return img.Source;
        }
    }
}
