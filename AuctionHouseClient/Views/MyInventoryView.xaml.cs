using AuctionHouseClient.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.IO;

namespace AuctionHouseClient.Views
{
    /// <summary>
    /// Interaction logic for MyInventoryView.xaml
    /// </summary>
    public partial class MyInventoryView : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Visibility invVis;
        public Visibility InvVis
        {
            get
            {
                return invVis;
            }
            set
            {
                invVis = value;
                OnPropertyChanged("invVis");
            }
        }

        public Visibility bankVis;
        public Visibility BankVis
        {
            get
            {
                return bankVis;
            }
            set
            {
                bankVis = value;
                OnPropertyChanged("bankVis");
            }
        }

        public Visibility wtsVis;
        public Visibility WtsVis
        {
            get
            {
                return wtsVis;
            }
            set
            {
                wtsVis = value;
                OnPropertyChanged("wtsVis");
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

        private string bankSearch;
        public string BankSearch
        {
            get
            {
                return bankSearch;
            }

            set
            {
                bankSearch = value;
                OnPropertyChanged("bankSearch");
            }
        }

        private string inventorySearch;
        public string InventorySearch
        {
            get
            {
                return inventorySearch;
            }

            set
            {
                inventorySearch = value;
                OnPropertyChanged("inventorySearch");
            }
        }

        private string wtsSearch;
        public string WtsSearch
        {
            get
            {
                return wtsSearch;
            }

            set
            {
                wtsSearch = value;
                OnPropertyChanged("wtsSearch");
            }
        }
        private ObservableCollection<BagItem> bag;
        public ObservableCollection<BagItem> Bag
        {
            get
            {
                return bag;
            }
            set
            {
                bag = value;
                OnPropertyChanged("Bag");
            }
        }
        private ObservableCollection<BagItem> actualBag;

        private ObservableCollection<BankItem> bank;
        public ObservableCollection<BankItem> Bank
        {
            get
            {
                return bank;
            }
            set
            {
                bank = value;
                OnPropertyChanged("Bank");
            }
        }

        private ObservableCollection<BankItem> actualBank;
        private ObservableCollection<InventoryItem> wts;
        public ObservableCollection<InventoryItem> Wts
        { 
            get
            {
                return wts;
            }
            set
            {
                wts = value;
                OnPropertyChanged("Wts");
            }
        }
        private ObservableCollection<InventoryItem> actualWts;

        DBConn db;

        public MyInventoryView(DBConn _db)
        {
            db = _db;
            Regular = db.GetRegularCurrency();
            Premium = db.GetPremiumCurrency();
            wtsSearch = "";
            bankSearch = "";
            inventorySearch = "";
            RefreshInventory();
            InitializeComponent();
            this.DataContext = this;
        }
        private void RefreshWts()
        {
            Wts = new ObservableCollection<InventoryItem>();
            actualWts = new ObservableCollection<InventoryItem>();
            foreach (InventoryItem i in actualBag)
            {
                if (i.Wts == true)
                {
                    Wts.Add(i);
                    actualWts.Add(i);
                }
            }
            foreach (InventoryItem i in actualBank)
            {
                if (i.Wts == true)
                {
                    Wts.Add(i);
                    actualWts.Add(i);
                }
            }
        }

        public void RefreshInventory()
        {
            Bag = new ObservableCollection<BagItem>(db.GetBag());
            actualBag = new ObservableCollection<BagItem>(Bag);
            Bank = new ObservableCollection<BankItem>(db.GetBank());
            actualBank = new ObservableCollection<BankItem>(Bank);
            RefreshWts();
        }

        private void FromInventoryToBank(object sender, RoutedEventArgs e)
        {
            MenuItem mi = e.Source as MenuItem;
            InventoryItem ba = mi.DataContext as InventoryItem;
            if (ba.ContainedItem.name.ToLower().Contains(bankSearch.ToLower())) Bank.Add((BankItem)ba);
            db.MoveToBank((BagItem)ba);
            actualBank.Add((BankItem)ba);
            actualBag.Remove((BagItem)ba);
            Bag.Remove((BagItem)ba);
        }

        private void FromBankyToInventory(object sender, RoutedEventArgs e)
        {
            MenuItem mi = e.Source as MenuItem;
            InventoryItem ba = mi.DataContext as InventoryItem;
            if (ba.ContainedItem.name.ToLower().Contains(inventorySearch.ToLower())) Bag.Add((BagItem)ba);
            db.MoveToBag((BankItem)ba);
            actualBag.Add((BagItem)ba);
            actualBank.Remove((BankItem)ba);
            Bank.Remove((BankItem)ba);
        }

        private void MarkAsWTS(object sender, RoutedEventArgs e)
        {
            MenuItem mi = e.Source as MenuItem;
            InventoryItem ba;
            if (mi.DataContext.GetType() == new BagItem().GetType()) ba = mi.DataContext as BagItem;
            else ba = mi.DataContext as BankItem;
            ContextMenu cm = mi.Parent as ContextMenu;
            ba.Wts = true;
            if (ba.ContainedItem.name.ToLower().Contains(wtsSearch.ToLower())) Wts.Add(ba);
            actualWts.Add(ba);
        }

        private void UnmarkAsWTS(object sender, RoutedEventArgs e)
        {
            MenuItem mi = e.Source as MenuItem;
            InventoryItem ga = mi.DataContext as InventoryItem;
            actualWts.Remove(ga);
            Wts.Remove(ga);
            ga.Wts = false;
        }

        private void DragFrom(object sender, MouseButtonEventArgs e)
        {
            Image dataObj = sender as Image;
            Grid g = dataObj.Parent as Grid;
            InventoryItem ga;
            if (dataObj.DataContext.GetType() == new BankItem().GetType()) ga = dataObj.DataContext as BankItem;
            else if (dataObj.DataContext.GetType() == new BagItem().GetType()) ga = dataObj.DataContext as BagItem;
            else ga = dataObj.DataContext as InventoryItem;
            object[] o = new object[] { ga, g };
            DragDrop.DoDragDrop(dataObj, o, DragDropEffects.All);
        }

        private void DropToEvent(object sender, DragEventArgs e)
        {
            ScrollViewer s = sender as ScrollViewer;
            DataObject obj = e.Data as DataObject;
            string[] dataFormats = obj.GetFormats();
            object[] o = obj.GetData(dataFormats[0]) as object[];
            InventoryItem ba;
            if (o[0].GetType() == new BagItem().GetType()) ba = o[0] as BagItem;
            else if (o[0].GetType() == new BankItem().GetType()) ba = o[0] as BankItem;
            else ba = o[0] as InventoryItem;
            Grid g = o[1] as Grid;

            //Add to lists
            //Add to WTS
            if (s.Name == "WTSScroll" && g.Name != "WTSGrid")
            {
                ba.Wts = true;
                RefreshWts();
            }
            //Add to Bank
            else if (s.Name == "BankScroll" && g.Name == "InvGrid")
            {
                db.MoveToBank((BagItem)ba);
                RefreshInventory();
            }
            //Add to Bag
            else if (s.Name == "InvScroll" && g.Name == "BankGrid")
            {
                db.MoveToBag((BankItem)ba);
                RefreshInventory();
            }

            //Remove from lists
            //Remove from WTS
            if (g.Name == "WTSGrid")
            {
                ba.Wts = false;
                RefreshWts();
            }
        }

        private void ClearWTS(object sender, RoutedEventArgs e)
        {
            db.ClearWts();
            Wts.Clear();
        }

        private void OnPropertyChanged(string v)
        {
            if (this.PropertyChanged != null)
            {
                if (v == "bankSearch") FilterBank();
                if (v == "inventorySearch") FilterInventory();
                if (v == "wtsSearch") FilterWts();
                PropertyChanged(this, new PropertyChangedEventArgs(v));
            }
        }

        private void FilterBank()
        {
            if (bankSearch.Length == 0) BankVis = Visibility.Visible;
            else BankVis = Visibility.Collapsed;
            Bank.Clear();
            foreach (BankItem game in actualBank)
            {
                if (game.ContainedItem.name.ToLower().Contains(bankSearch.ToLower())) Bank.Add(game);
            }
        }

        private void FilterInventory()
        {
            if (inventorySearch.Length == 0) InvVis = Visibility.Visible;
            else InvVis = Visibility.Collapsed;
            Bag.Clear();
            foreach (BagItem game in actualBag)
            {
                if (game.ContainedItem.name.ToLower().Contains(inventorySearch.ToLower())) Bag.Add(game);
            }
        }

        private void FilterWts()
        {
            if (wtsSearch.Length == 0) WtsVis = Visibility.Visible;
            else WtsVis = Visibility.Collapsed;
            Wts.Clear();
            foreach (InventoryItem game in actualWts)
            {
                if (game.ContainedItem.name.ToLower().Contains(wtsSearch.ToLower())) Wts.Add(game);
            }
        }
    }
}
