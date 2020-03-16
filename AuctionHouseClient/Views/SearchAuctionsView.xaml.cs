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

        private GameItem itemToLookUp;

        DBConn db;

        public ObservableCollection<GameItem> wts { get; set; }

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


        public SearchAuctionsView(DBConn _db)
        {
            db = _db;
            InitializeComponent();
            wts = new PlayerWTS(_db).inventory;
            //DataContext = new SearchAuctionModel();
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
            ObservableCollection<GameItem> t1 = new ObservableCollection<GameItem>(new PlayerWTS(_db).inventory);
            wts.Clear();
            foreach (GameItem g in t1)
            {
                wts.Add(g);
            }
        }

        private void Item_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image i = sender as Image;
            itemToLookUp = i.DataContext as GameItem;
            SearchIt = itemToLookUp.name;
            QuickSearch();
        }

        private void UnmarkForWts(MenuItem mi)
        {
            GameItem ga = mi.DataContext as GameItem;
            wts.Remove(ga);
            db.UnMarkForWts(ga);
        }

        private void UnmarkeMenu_Click(object sender, RoutedEventArgs e)
        {
            UnmarkForWts(e.Source as MenuItem);
        }
    }
}
