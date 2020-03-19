using AuctionHouseClient.Shared;
using AuctionHouseClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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
    /// Interaction logic for SearchAuctionsView.xaml
    /// </summary>
    public partial class SearchAuctionsView : UserControl, INotifyPropertyChanged
    {
        public delegate void myEventDelegaqte(object sender, SwitchViewEventArgs e);

        public event myEventDelegaqte SwitchView;
        public event PropertyChangedEventHandler PropertyChanged;

        private InventoryItem itemToLookUp;

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

        DBConn db;

        public ObservableCollection<InventoryItem> Wts { get; set; }

        private string searchIt;

        public string SearchIt
        {
            get { return searchIt; }
            set
            {
                searchIt = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SearchIt"));
            }
        }

        private void OnPropertyChanged(string v)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(v));
            }
        }
        public SearchAuctionsView(DBConn _db)
        {
            db = _db;
            Regular = db.GetRegularCurrency();
            Premium = db.GetPremiumCurrency();
            InitializeComponent();
            MakeWts();
            //DataContext = new SearchAuctionModel();
        }

        private void MakeWts()
        {
            Wts = new ObservableCollection<InventoryItem>();
            ObservableCollection<InventoryItem> t1 = new ObservableCollection<InventoryItem>(db.GetBag());
            ObservableCollection<InventoryItem> t2 = new ObservableCollection<InventoryItem>(db.GetBank());
            foreach (InventoryItem inventory in t1)
            {
                if (inventory.Wts == true) Wts.Add(inventory);
            }
            foreach (InventoryItem inventory in t2)
            {
                if (inventory.Wts == true) Wts.Add(inventory);
            }
        }

        public void Searching_Clicked(object sender, RoutedEventArgs e)
        {
            switchToSearched();
        }

        private void switchToSearched()
        {
            myEventDelegaqte t = SwitchView;
            if (t != null) t.Invoke(this, new SwitchViewEventArgs { SearchedString = SearchIt, db = this.db, QuickSearch = false });
        }

        private void QuickSearch()
        {
            myEventDelegaqte t = SwitchView;
            if (t != null) t.Invoke(this, new SwitchViewEventArgs { SearchedString = SearchIt, db = this.db, QuickSearch = true, itemToLookUp = itemToLookUp });
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                switchToSearched();
            }
        }

        public void RefreshWts(DBConn _db)
        {
            db = _db;
            MakeWts();
        }

        private void Item_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image i = sender as Image;
            itemToLookUp = i.DataContext as InventoryItem;
            SearchIt = itemToLookUp.ContainedItem.name;
            QuickSearch();
        }

        private void UnmarkForWts(MenuItem mi)
        {
            InventoryItem ga = mi.DataContext as InventoryItem;
            Wts.Remove(ga);
            ga.Wts = false;
        }

        private void UnmarkeMenu_Click(object sender, RoutedEventArgs e)
        {
            UnmarkForWts(e.Source as MenuItem);
        }
    }
}
