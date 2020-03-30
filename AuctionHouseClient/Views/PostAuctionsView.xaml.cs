using AuctionHouseClient.Shared;
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
    /// Interaction logic for PostAuctions.xaml
    /// </summary>
    public partial class PostAuctionsView : UserControl, INotifyPropertyChanged
    {
        DBConn db;

        ObservableCollection<InventoryItem> Wts;

        private static double postBottomOpacity;
        public static double PostBottomOpacity
        {
            get
            {
                return postBottomOpacity;
            }
            set
            {
                if (value != postBottomOpacity)
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


        private ObservableCollection<Post> listToPost;
        public ObservableCollection<Post> ListToPost
        { 
            get
            {
                return listToPost;
            }
            set
            {
                listToPost = value;
                OnPropertyChanged("ListToPost");
            }
        }


        private ObservableCollection<Post> listNotToPost;
        public ObservableCollection<Post> ListNotToPost 
        { 
            get
            {
                return listNotToPost;
            }
            set
            {
                listNotToPost = value;
                OnPropertyChanged("ListNotToPost");
            }
        }

        private Post selectedItem;
        public Post SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
                if (value.CanPost == true)
                {
                    PostBottomOpacity = 1.0f;
                    PostEnabled = true;
                }
                else
                {
                    PostBottomOpacity = 0.5f;
                    PostEnabled = false;
                }
                OnPropertyChanged("SelectedItem");
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


        public event PropertyChangedEventHandler PropertyChanged;
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        public PostAuctionsView(DBConn _db)
        {
            SelectedItem = new Post() { CanPost = false };
            db = _db;
            InitializeComponent();
        }
        public void MakeWts()
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

        private void ScanAll()
        {
            ListToPost = new ObservableCollection<Post>();
            ListNotToPost = new ObservableCollection<Post>();
            foreach(InventoryItem i in Wts)
            {
                ObservableCollection<Auction> t = db.SearchAuction(i.ContainedItem.name);
                if (t.Count > 0)
                {
                    ListToPost.Insert(0, new Post
                    {
                        ItemToPost = i,
                        Bid = t[0].Bid.ToString(),
                        Buyout = t[0].Buyout.ToString(),
                        Premium = t[0].Premium,
                        CanPost = true
                    });
                }
                else
                {
                    ListToPost.Add(new Post
                    {
                        ItemToPost = i,
                        Bid = "",
                        Buyout = "No Items",
                        Premium = false,
                        CanPost = false
                    });
                }
            }
        }

        private void ScanAll_Click(object sender, RoutedEventArgs e)
        {
            ScanAll();
        }

        private void Post_Click(object sender, RoutedEventArgs e)
        {
            PostItem();
        }

        private void PostAll_Click(object sender, RoutedEventArgs e)
        {
            PostAll();
        }

        private void PostItem()
        {
            if(selectedItem.CanPost) db.PostAuction(SelectedItem.ItemToPost, SelectedItem.ItemToPost.Amount, 48, int.Parse(SelectedItem.Buyout), int.Parse(SelectedItem.Buyout));
        }

        private void PostAll()
        {
            foreach(Post p in ListToPost)
            {
                if(selectedItem.CanPost) db.PostAuction(p.ItemToPost, p.ItemToPost.Amount, 48, int.Parse(p.Buyout), int.Parse(p.Buyout));
            }
        }
    }
}
