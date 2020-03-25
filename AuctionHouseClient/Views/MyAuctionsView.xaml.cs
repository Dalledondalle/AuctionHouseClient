using AuctionHouseClient.Shared;
using AuctionHouseClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AuctionHouseClient.Views
{
    /// <summary>
    /// Interaction logic for MyAuctionsView.xaml
    /// </summary>
    public partial class MyAuctionsView : UserControl, INotifyPropertyChanged
    {
        private Dictionary<string, bool> sorted;
        private DBConn db;

        public event PropertyChangedEventHandler PropertyChanged;
        private SoldAuctionsPopUpView soldAuctionsPopUpView;

        private ObservableCollection<Auction> actualPostedList;

        private ObservableCollection<Auction> postedList;
        public ObservableCollection<Auction> PostedList 
        {
            get
            {
                return postedList;
            }
            set
            {
                postedList = value;
                OnPropertyChanged("PostedList");
            }
        }

        Auction selectedItem;
        public Auction SelectedItem {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
            }
        }
        private string filterWord;
        public string FilterWord
        {
            get
            {
                return filterWord;
            }
            set
            {
                filterWord = value;
                Filter();
                OnPropertyChanged("FilterWord");
            }
        }

        private void OnPropertyChanged(string v)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(v));
            }
        }

        private int totalBuyoutRegular;
        private int totalBidRegular;
        private int totalBuyoutPremium;
        private int totalBidPremium;

        public int TotalBuyoutRegular
        {
            get
            {
                return totalBuyoutRegular;
            }
            set
            {
                totalBuyoutRegular = value;
                OnPropertyChanged("TotalBuyoutRegular");
            }
        }
        public int TotalBidRegular
        {
            get
            {
                return totalBidRegular;
            }
            set
            {
                totalBidRegular = value;
                OnPropertyChanged("TotalBidRegular");
            }
        }
        public int TotalBuyoutPremium
        {
            get
            {
                return totalBuyoutPremium;
            }
            set
            {
                totalBuyoutPremium = value;
                OnPropertyChanged("TotalBuyoutPremium");
            }
        }
        public int TotalBidPremium
        {
            get
            {
                return totalBidPremium;
            }
            set
            {
                totalBidPremium = value;
                OnPropertyChanged("TotalBidPremium");
            }
        }

        public MyAuctionsView(DBConn _db)
        {
            db = _db;
            PostedList = new ObservableCollection<Auction>();
            sorted = new Dictionary<string, bool>();
            sorted.Add("name", false);
            sorted.Add("timeleft", false);
            sorted.Add("bid", false);
            sorted.Add("buyout", false);
            sorted.Add("seller", false);
            sorted.Add("amount", false);
            InitializeComponent();
            //this.DataContext = new MyAuctionsModel();
        }
        private void Grid_Click(object sender, MouseButtonEventArgs e)
        {
            SortBy(((sender as Grid).Children[0] as TextBlock).Text);
        }
        private void SortBy(string arg)
        {
            arg = arg.ToLower();
            List<Auction> temp = PostedList.ToList<Auction>();
            if (arg.ToLower() == "name")
            {
                if (sorted[arg])
                {
                    temp.Reverse();
                    sorted[arg] = false;
                }
                else
                {
                    temp.Sort(delegate (Auction a1, Auction a2) { return a1.Name.CompareTo(a2.Name); });
                    sorted[arg] = true;
                }
            }
            if (arg.ToLower() == "timeleft")
            {
                if (sorted[arg])
                {
                    temp.Reverse();
                    sorted[arg] = false;
                }
                else
                {
                    temp.Sort(delegate (Auction a1, Auction a2) { return a1.TimeLeft.CompareTo(a2.TimeLeft); });
                    sorted[arg] = true;
                }
            }
            if (arg.ToLower() == "bid")
            {
                if (sorted[arg])
                {
                    temp.Reverse();
                    sorted[arg] = false;
                }
                else
                {
                    temp.Sort(delegate (Auction a1, Auction a2) { return a1.Bid.CompareTo(a2.Bid); });
                    sorted[arg] = true;
                }
            }
            if (arg.ToLower() == "buyout")
            {
                if (sorted[arg])
                {
                    temp.Reverse();
                    sorted[arg] = false;
                }
                else
                {
                    temp.Sort(delegate (Auction a1, Auction a2) { return a1.Buyout.CompareTo(a2.Buyout); });
                    sorted[arg] = true;
                }
            }
            if (arg.ToLower() == "seller")
            {
                if (sorted[arg])
                {
                    temp.Reverse();
                    sorted[arg] = false;
                }
                else
                {
                    temp.Sort(delegate (Auction a1, Auction a2) { return a1.SellerName.CompareTo(a2.SellerName); });
                    sorted[arg] = true;
                }
            }
            if (arg.ToLower() == "amount")
            {
                if (sorted[arg])
                {
                    temp.Reverse();
                    sorted[arg] = false;
                }
                else
                {
                    temp.Sort(delegate (Auction a1, Auction a2) { return a1.Amount.CompareTo(a2.Amount); });
                    sorted[arg] = true;
                }
            }
            FixDic(arg);
            PostedList.Clear();
            foreach (Auction a in temp)
            {
                PostedList.Add(a);
            }
        }
        public void FillPosts()
        {
            totalBuyoutRegular = 0;
            totalBidRegular = 0;
            totalBuyoutPremium = 0;
            totalBidPremium = 0;
            postedList.Clear();
            ObservableCollection<Auction> t;
            t = db.GetPostedList();
            foreach (Auction a in t)
            {
                postedList.Add(a);
                if(a.Premium)
                {
                    totalBuyoutPremium += a.Buyout * a.Amount;
                    totalBidPremium += a.Bid * a.Amount;
                }
                else
                {
                    totalBuyoutRegular += a.Buyout * a.Amount;
                    totalBidRegular += a.Bid * a.Amount;
                }
            }
            actualPostedList = new ObservableCollection<Auction>(postedList);
        }
        private void FixDic(string s)
        {
            for (int i = 0; i < sorted.Count; i++)
            {
                string t = sorted.ElementAt(i).Key;
                if (t != s) sorted[t] = false;
            }
        }

        private void Mails_Click(object sender, RoutedEventArgs e)
        {
            OpenMailWindow();
        }

        private void OpenMailWindow()
        {
            soldAuctionsPopUpView = new SoldAuctionsPopUpView(db);
            soldAuctionsPopUpView.RefreshMail();
            soldAuctionsPopUpView.Show();
        }

        private void CancelAuction_Click(object sender, RoutedEventArgs e)
        {
            db.CancelAuction(SelectedItem);
            FillPosts();
        }

        private void Filter()
        {
            PostedList.Clear();
            foreach(Auction a in actualPostedList)
            {
                if (a.Name.ToLower().Contains(FilterWord.ToLower())) PostedList.Add(a);
            }
        }
    }
}
