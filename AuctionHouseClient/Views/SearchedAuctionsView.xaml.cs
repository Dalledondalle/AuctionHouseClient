using AuctionHouseClient.Shared;
using AuctionHouseClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
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

    public partial class SearchedAuctionsView : UserControl, INotifyPropertyChanged
    {
        public delegate void myEventDelegaqte(object sender, SwitchViewEventArgs e);
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        public ObservableCollection<Auction> searchedList;
        public ObservableCollection<Auction> SearchedList 
        {
            get
            {
                return searchedList;
            }
            set
            {
                searchedList = value;
                OnPropertyChanged("SearchedList");
            }
        }
        public event myEventDelegaqte SwitchView;
        public event PropertyChangedEventHandler PropertyChanged;
        private string searchIt;

        private Auction selectedItem;
        public Auction SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
            }
        }

        private PostingPopUpView PostPopUpView;
        public bool QuickSearch { get; set; }
        public InventoryItem itemToLookUp { get; set; }

        private static double postBottomOpacity;
        public static double PostBottomOpacity
        {
            get
            {
                return postBottomOpacity;
            }
            set
            {
                if(value != postBottomOpacity)
                {
                    postBottomOpacity = value;
                    NotifyStaticPropertyChanged("PostBottomOpacity");
                }
            }
        }

        private static bool postEnabled;
        public static bool PostEnabled 
        {
            get
            {
                return postEnabled;
            }
            set
            {
                if (value != postEnabled)
                {
                    postEnabled = value;
                    NotifyStaticPropertyChanged("PostEnabled");
                }
            }
        }

        private RegularCurrency regular;
        public RegularCurrency Regular
        {
            get
            {
                return regular;
            }
            set
            {
                regular = value;
                OnPropertyChanged("Regular");
            }
        }
        private PremiumCurrency premium;
        public PremiumCurrency Premium
        {
            get
            {
                return premium;
            }
            set
            {
                premium = value;
                OnPropertyChanged("Premium");
            }
        }

        BuyoutPopUpView buyoutPopUpView;

        public DBConn db { get; set; }

        private Dictionary<string, bool> sorted;
        public string SearchIt
        {
            get { return searchIt; }
            set
            {
                searchIt = value;
                OnPropertyChanged("SearchIt");
            }
        }
        private void OnPropertyChanged(string v)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(v));
            }
        }
        private static void NotifyStaticPropertyChanged(string v)
        {
            if (StaticPropertyChanged != null)
                StaticPropertyChanged(null, new PropertyChangedEventArgs(v));
        }
        public SearchedAuctionsView(DBConn _db)
        {
            db = _db;
            PostBottomOpacity = 1.0;
            sorted = new Dictionary<string, bool>();
            sorted.Add("name", false);
            sorted.Add("time left", false);
            sorted.Add("bid", false);
            sorted.Add("buyout", false);
            sorted.Add("seller", false);
            sorted.Add("amount", false);
            SearchedList = new ObservableCollection<Auction>();
            Regular = db.GetRegularCurrency();
            Premium = db.GetPremiumCurrency();
            InitializeComponent();
            //DataContext = new SearchedAuctionsModel();
        }

        private void BackToSearch_Click(object sender, RoutedEventArgs e)
        {
            myEventDelegaqte t = SwitchView;
            if (t != null)
            {
                t.Invoke(this, new SwitchViewEventArgs { newView = ViewSelection.search, SearchedString = SearchIt });
            }
        }

        void Search_Click(object sender, RoutedEventArgs e)
        {
            QuickSearch = false;
            Search(SearchIt);
        }

        public void Search(string _s)
        {
            if(QuickSearch)
            {
                PostBottomOpacity = 1.0;
                PostEnabled = true;
            }
            else
            {
                PostBottomOpacity = 0.3;
                PostEnabled = false;
            }
            SearchedList.Clear();
            if (_s == null) _s = "";
            SearchedList = db.SearchAuction(_s);
        }

        private void Buy_Click(object sender, RoutedEventArgs e)
        {
            buyoutPopUpView = new BuyoutPopUpView();
            buyoutPopUpView.AuctionToBuy = SelectedItem;
            buyoutPopUpView.Show();
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Search(SearchIt);
            }
        }

        private void SortBy(string arg)
        {
            arg = arg.ToLower();
            List<Auction> temp = SearchedList.ToList<Auction>();
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
            if (arg.ToLower() == "time left")
            {
                if (sorted[arg])
                {
                    temp.Reverse();
                    sorted[arg] = false;
                }
                else
                {
                    temp.Sort(delegate (Auction a1, Auction a2) { return a1.TimeLeftIndex.CompareTo(a2.TimeLeftIndex); });
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
            SearchedList.Clear();
            foreach (Auction a in temp)
            {
                SearchedList.Add(a);
            }
        }

        private void Bid_Click(object sender, RoutedEventArgs e)
        {
            return;
        }

        private void Grid_Click(object sender, MouseButtonEventArgs e)
        { 
            SortBy(((sender as Grid).Children[0] as TextBlock).Text);
        }

        private void FixDic(string s)
        {
            for (int i = 0; i < sorted.Count; i++)
            {
                string t = sorted.ElementAt(i).Key;
                if (t != s) sorted[t] = false;
            }
        }

        private void Post_Click(object sender, RoutedEventArgs e)
        {
            OpenPostWindow();
        }

        private void OpenPostWindow()
        {
            PostPopUpView = new PostingPopUpView();
            PostPopUpView.ItemToPost = itemToLookUp;
            PostPopUpView.BuyoutTxtBox = SearchedList.Any() ? SearchedList.Min(Auction => Auction.Buyout).ToString() : "0";
            PostPopUpView.BidTxtBox = "0";
            PostPopUpView.db = db;
            PostPopUpView.Show();
        }
    }
}
