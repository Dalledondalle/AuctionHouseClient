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
using System.Windows.Shapes;

namespace AuctionHouseClient.Views
{
    public partial class PostingPopUpView : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        public DBConn db;
        private string amountTxtBox;
        public string AmountTxtBox
        {
            get
            {
                return amountTxtBox;
            }
            set
            {
                int i;
                if (int.TryParse(value, out i))
                {
                    if (i > itemToPost.amount) amountTxtBox = itemToPost.amount.ToString();
                    else if (i < 1) amountTxtBox = "1";
                    else amountTxtBox = value;
                }

                else if (value == "" || value == null) amountTxtBox = "";
                OnPropertyChanged("AmountTxtBox");
            }
        }
        private string buyoutTxtBox;
        public string BuyoutTxtBox
        {
            get
            {
                return buyoutTxtBox;
            }
            set
            {
                int i;
                if (int.TryParse(value, out i))
                {
                    if (i < 1) buyoutTxtBox = "1";
                    else buyoutTxtBox = value;
                }
                else if (value == "" || value == null) buyoutTxtBox = "";
                OnPropertyChanged("BuyoutTxtBox");
            }
        }
        private string bidTxtBox;
        public string BidTxtBox
        {
            get
            {
                return bidTxtBox;
            }
            set
            {
                int i;
                int j;
                if (int.TryParse(value, out i))
                {
                    if(int.TryParse(buyoutTxtBox, out j))
                    {
                        if (i > j) bidTxtBox = buyoutTxtBox;
                        else if (i < 0) bidTxtBox = "0";
                        else bidTxtBox = value;
                    }
                    else bidTxtBox = value;
                }
                else if (value == "" || value == null) bidTxtBox = "";
                OnPropertyChanged("BidTxtBox");
            }
        }
        private string itemName;
        public string ItemName
        {
            get
            {
                return itemName;
            }
            set
            {
                itemName = value;
                OnPropertyChanged("ItemName");
            }
        }
        public int ShortDuration { get; set; }
        public int MediumDuration { get; set; }
        public int LongDuration { get; set; }
        private int selectedIndex = 0;
        public int SelectedIndex 
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                selectedIndex = value;
                OnPropertyChanged("SelectedIndex");
            }
        }
        private int selectedItem;
        public int SelectedItem 
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }
        private GameItem itemToPost;
        public GameItem ItemToPost { 
            get 
            {
                return itemToPost;
            } 
            set
            {
                itemToPost = value;
                ItemName = value.name;
                AmountTxtBox = value.amount.ToString();
                OnPropertyChanged("ItemToPost");
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
        public PostingPopUpView()
        {
            ShortDuration = 12;
            MediumDuration = 24;
            LongDuration = 48;
            SelectedItem = 48;
            InitializeComponent();
            DataContext = this;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Post_Click(object sender, RoutedEventArgs e)
        {
            PostAuction();
            Close();
        }

        private void PostAuction()
        {
            int t;
            if (BidTxtBox != "") t = 0;
            else t = int.Parse(bidTxtBox);
            db.PostAuction(ItemToPost, int.Parse(AmountTxtBox), SelectedItem, int.Parse(BuyoutTxtBox), t);
        }

        private void ComboBox_Selected(object sender, RoutedEventArgs e)
        {
            ComboBoxItem c = sender as ComboBoxItem;
            TextBlock t = c.Content as TextBlock;
            if(t.Text != "")SelectedItem = int.Parse(t.Text);
        }
    }
}
