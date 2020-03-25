using AuctionHouseClient.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    /// Interaction logic for BuyoutPopUpView.xaml
    /// </summary>
    public partial class BuyoutPopUpView : Window, INotifyPropertyChanged
    {
        private Auction auctionToBuy;
        public Auction AuctionToBuy 
        {
            get
            {
                return auctionToBuy;
            }
            set
            {
                auctionToBuy = value;
                OnPropertyChanged("AuctionToBuy");
            }
        }
        public string AuctionName
        {
            get
            {
                return AuctionToBuy.Name;
            }
        }

        public DBConn db { get; set; }

        private string amount;
        public string Amount
        {
            get
            {
                return amount;
            }
            set
            {
                int i;
                if (int.TryParse(value, out i))
                {
                    if (i < 1) amount = "1";
                    else if (i > auctionToBuy.Amount) amount = auctionToBuy.Amount.ToString();
                    else amount = value;
                }
                else if (value == "" || value == null) amount = "";
                OnPropertyChanged("Amount");
                OnPropertyChanged("Price");
            }
        }

        public int Price
        {
            get
            {
                if (amount != "" && amount != null) return auctionToBuy.Buyout * int.Parse(amount);
                else return auctionToBuy.Buyout;
            }
        }

        private void OnPropertyChanged(string v)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(v));
            }
        }
        public BuyoutPopUpView()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Buyout_Click(object sender, RoutedEventArgs e)
        {
            db.Buyout(auctionToBuy, int.Parse(Amount));
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
