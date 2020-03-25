using AuctionHouseClient.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AuctionHouseClient.Views
{
    /// <summary>
    /// Interaction logic for BiddingPopUpView.xaml
    /// </summary>
    public partial class BiddingPopUpView : Window, INotifyPropertyChanged
    {
        public DBConn Db { get; set; }
        public Auction AuctionToBidOn { get; set; }

        private string bid;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Bid
        {
            get
            {
                return bid;
            }
            set
            {
                int i;
                if (int.TryParse(value, out i))
                {
                    if (i <= AuctionToBidOn.Bid) bid = (AuctionToBidOn.Bid + 1).ToString();
                    else bid = i.ToString();
                }
            }
        }
        public BiddingPopUpView()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        private void OnPropertyChanged(string v)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(v));
            }
        }

        private void Bid_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(bid) < AuctionToBidOn.Buyout) Db.Bid(AuctionToBidOn, int.Parse(bid));
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    this.WindowState = WindowState.Normal;
                    Application.Current.MainWindow.Top = 3;
                }
                this.DragMove();
            }
        }
    }
}
