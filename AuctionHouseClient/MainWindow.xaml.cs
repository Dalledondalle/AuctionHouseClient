using AuctionHouseClient.Shared;
using AuctionHouseClient.ViewModels;
using AuctionHouseClient.Views;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AuctionHouseClient
{
    public partial class MainWindow : Window
    {
        private bool searching = false;
        private SearchAuctionsView searchAuctionsView = null;
        private SearchedAuctionsView searchedAuctionsView = null;
        private PostAuctionsView postAuctionsView = null;
        private MyAuctionsView myAuctionsView = null;
        private MyInventoryView myInventoryView = null;
        private LoginView loginView = null;
        private DBConn dBConn = null;
        private AdminView adminView = null;
        private bool setupDone;


        public MainWindow()
        {
            InitializeComponent();
            FrameworkElement.StyleProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata
            {
                DefaultValue = FindResource(typeof(Window))
            });
            FrameworkElement.StyleProperty.OverrideMetadata(typeof(UserControl), new FrameworkPropertyMetadata
            {
                DefaultValue = FindResource(typeof(UserControl))
            });
            setupDone = false;
            dBConn = new DBConn();
            loginView = new LoginView(dBConn);
            DataContext = loginView;
        }

        private void Setup()
        {
            searchAuctionsView = new SearchAuctionsView(dBConn);
            searchedAuctionsView = new SearchedAuctionsView();
            postAuctionsView = new PostAuctionsView();
            myAuctionsView = new MyAuctionsView(dBConn);
            myInventoryView = new MyInventoryView(dBConn);
            searchedAuctionsView.SwitchView += searchAuctionsView_SwitchView;
            searchAuctionsView.SwitchView += searchedAuctionsView_SwitchView;
            setupDone = true;
        }

        private void searchedAuctionsView_SwitchView(object sender, SwitchViewEventArgs e)
        {
            searchedAuctionsView.SearchIt = e.SearchedString;
            searching = true;
            searchedAuctionsView.QuickSearch = e.QuickSearch;
            searchedAuctionsView.itemToLookUp = e.itemToLookUp;
            searchedAuctionsView.Search(e.SearchedString);
            DataContext = searchedAuctionsView;
        }

        private void searchAuctionsView_SwitchView(object sender, SwitchViewEventArgs e)
        {
            searching = false;
            searchAuctionsView.SearchIt = e.SearchedString;
            searchAuctionsView.RefreshWts(dBConn);
            DataContext = searchAuctionsView;
        }

        //MyAuctionsButton
        private void MyAuctions_Clicked(object sender, RoutedEventArgs e)
        {
            if (loginView.LoggedIn)
            {
                if (!setupDone) Setup();
                myAuctionsView.FillPosts();
                DataContext = myAuctionsView;                
            }
        }

        //PostAuctionsButton
        private void PostAuctions_Clicked(object sender, RoutedEventArgs e)
        {
            if (loginView.LoggedIn)
            {
                if (!setupDone) Setup();
                DataContext = postAuctionsView;
            }
        }

        //SearchAuctionsButton
        private void SearchAuctions_Clicked(object sender, RoutedEventArgs e)
        {
            if (loginView.LoggedIn)
            {
                if (!setupDone) Setup();
                searchedAuctionsView.db = dBConn;
                isSearching();
            }
        }

        //MyInventoryButton
        private void MyInventory_Clicked(object sender, RoutedEventArgs e)
        {
            if (loginView.LoggedIn)
            {
                if (!setupDone) Setup();
                myInventoryView.RefreshInventory(dBConn);
                DataContext = myInventoryView;
            }
        }

        private void isSearching()
        {
            if(searching)
            {
                DataContext = searchedAuctionsView;
            }
            else
            {
                searchAuctionsView.RefreshWts(dBConn);
                DataContext = searchAuctionsView;
            }
        }

        private void Admin_Clicked(object sender, RoutedEventArgs e)
        {
            if(!IsWindowOpen<AdminView>())
            {
                if(loginView.LoggedIn)
                {
                    adminView = new AdminView(dBConn);
                    adminView.Show();
                }                
            }
        }

        private void Close_Clicked(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Debug_Clicked(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(this.WindowState);
        }

        public static bool IsWindowOpen<T>(string name = "") where T : Window
        {
            return string.IsNullOrEmpty(name)
               ? Application.Current.Windows.OfType<T>().Any()
               : Application.Current.Windows.OfType<T>().Any(w => w.Name.Equals(name));
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

    public class SwitchViewEventArgs : EventArgs
    {
        private string searchedString;
        public DBConn db { get; set; }
        public bool QuickSearch { get; set; }
        public GameItem itemToLookUp { get; set; }
        public ViewSelection newView { get; set; }
        public string SearchedString
        {
            get { return searchedString; }
            set { searchedString = value;  }
        }
    }

    public enum ViewSelection
    {
       searching,
       search
    }
}
