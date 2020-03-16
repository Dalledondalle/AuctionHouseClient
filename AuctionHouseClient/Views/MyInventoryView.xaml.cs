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
        public ObservableCollection<GameItem> inventory { get; set; }
        private ObservableCollection<GameItem> actualInventory;
        public ObservableCollection<GameItem> bank { get; set; }
        private ObservableCollection<GameItem> actualBank;
        public ObservableCollection<GameItem> wts { get; set; }
        private ObservableCollection<GameItem> actualWts;

        DBConn db;

        public MyInventoryView(DBConn _db)
        {
            db = _db;
            wtsSearch = "";
            bankSearch = "";
            inventorySearch = "";
            InitializeComponent();
            this.DataContext = this;
            inventory = new PlayerInventory(_db).inventory;
            actualInventory = new PlayerInventory(_db).inventory;
            bank = new PlayerBank(_db).inventory;
            actualBank = new PlayerBank(_db).inventory;
            wts = new PlayerWTS(_db).inventory;
            actualWts = new PlayerWTS(_db).inventory;
        }


        public void RefreshInventory(DBConn _db)
        {
            ObservableCollection<GameItem> t = new ObservableCollection<GameItem>(new PlayerInventory(_db).inventory);
            inventory.Clear();
            actualInventory.Clear();
            foreach (GameItem g in t)
            {
                inventory.Add(g);
                actualInventory.Add(g);
            }
            ObservableCollection<GameItem> t1 = new ObservableCollection<GameItem>(new PlayerBank(_db).inventory);
            bank.Clear();
            actualBank.Clear();
            foreach (GameItem g in t1)
            {
                bank.Add(g);
                actualBank.Add(g);
            }
            ObservableCollection<GameItem> t2 = new ObservableCollection<GameItem>(new PlayerWTS(_db).inventory);
            wts.Clear();
            actualWts.Clear();
            foreach (GameItem g in t2)
            {
                wts.Add(g);
                actualWts.Add(g);
            }
        }

        private void FromInventoryToBank(object sender, RoutedEventArgs e)
        {
            MenuItem mi = e.Source as MenuItem;
            GameItem ga = mi.DataContext as GameItem;
            if (ga.name.ToLower().Contains(bankSearch.ToLower())) bank.Add(ga);
            db.MoveToBank(ga);
            actualBank.Add(ga);
            actualBank.Remove(ga);
            inventory.Remove(ga);
        }

        private void FromBankyToInventory(object sender, RoutedEventArgs e)
        {
            MenuItem mi = e.Source as MenuItem;
            GameItem ga = mi.DataContext as GameItem;
            if (ga.name.ToLower().Contains(inventorySearch.ToLower())) inventory.Add(ga);
            db.MoveToBag(ga);
            actualInventory.Add(ga);
            actualBank.Remove(ga);
            bank.Remove(ga);
        }

        private void MarkAsWTS(object sender, RoutedEventArgs e)
        {
            MenuItem mi = e.Source as MenuItem;
            GameItem ga = mi.DataContext as GameItem;
            ContextMenu cm = mi.Parent as ContextMenu;
            if (GetCountOfId(actualInventory, ga) + GetCountOfId(actualBank, ga) > GetCountOfId(actualWts, ga))
            {
                if (ga.name.ToLower().Contains(wtsSearch.ToLower())) wts.Add(ga);
                db.MarkAsWts(ga);
                actualWts.Add(ga);
            }
        }

        private void UnmarkAsWTS(object sender, RoutedEventArgs e)
        {
            MenuItem mi = e.Source as MenuItem;
            GameItem ga = mi.DataContext as GameItem;
            actualWts.Remove(ga);
            wts.Remove(ga);
            db.UnMarkForWts(ga);
        }

        private void DragFrom(object sender, MouseButtonEventArgs e)
        {
            Image dataObj = sender as Image;
            Grid g = dataObj.Parent as Grid;
            GameItem ga = dataObj.DataContext as GameItem;
            object[] o = new object[] { ga, g };
            DragDrop.DoDragDrop(dataObj, o, DragDropEffects.All);
        }

        private void DropToEvent(object sender, DragEventArgs e)
        {
            ScrollViewer s = sender as ScrollViewer;
            DataObject obj = e.Data as DataObject;
            string[] dataFormats = obj.GetFormats();
            object[] o = obj.GetData(dataFormats[0]) as object[];
            GameItem ga = o[0] as GameItem;
            Grid g = o[1] as Grid;

            //Add to lists
            //Add to WTS
            if (s.Name == "WTSScroll" && g.Name != "WTSGrid")
            {
                if (GetCountOfId(actualInventory, ga) + GetCountOfId(actualBank, ga) > GetCountOfId(actualWts, ga))
                {
                    db.MarkAsWts(ga);
                    actualWts.Add(ga);
                    if (ga.name.ToLower().Contains(wtsSearch.ToLower())) wts.Add(ga);
                }
            }
            //Add to Bank
            else if (s.Name == "BankScroll" && g.Name == "InvGrid")
            {
                db.MoveToBank(ga);
                actualBank.Add(ga);
                if (ga.name.ToLower().Contains(bankSearch.ToLower())) bank.Add(ga);
            }
            //Add to Bag
            else if (s.Name == "InvScroll" && g.Name == "BankGrid")
            {
                db.MoveToBag(ga);
                actualInventory.Add(ga);
                if (ga.name.ToLower().Contains(inventorySearch.ToLower())) inventory.Add(ga);
            }

            //Remove from lists
            //Remove from WTS
            if (g.Name == "WTSGrid")
            {
                actualWts.Remove(ga);
                wts.Remove(ga);
                db.UnMarkForWts(ga);
            }
            //Remove from Bank
            else if (g.Name == "BankGrid" && s.Name == "InvScroll")
            {
                actualBank.Remove(ga);
                bank.Remove(ga);
            }
            //Remove from Inventory
            else if (g.Name == "InvGrid" && s.Name == "BankScroll")
            {
                actualInventory.Remove(ga);
                inventory.Remove(ga);
            }
        }

        private void ClearWTS(object sender, RoutedEventArgs e)
        {
            db.ClearWts();
            wts.Clear();
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
            bank.Clear();
            foreach (GameItem game in actualBank)
            {
                if (game.name.ToLower().Contains(bankSearch.ToLower())) bank.Add(game);
            }
        }

        private void FilterInventory()
        {
            if (inventorySearch.Length == 0) InvVis = Visibility.Visible;
            else InvVis = Visibility.Collapsed;
            inventory.Clear();
            foreach (GameItem game in actualInventory)
            {
                if (game.name.ToLower().Contains(inventorySearch.ToLower())) inventory.Add(game);
            }
        }

        private void FilterWts()
        {
            if (wtsSearch.Length == 0) WtsVis = Visibility.Visible;
            else WtsVis = Visibility.Collapsed;
            wts.Clear();
            foreach (GameItem game in actualWts)
            {
                if (game.name.ToLower().Contains(wtsSearch.ToLower())) wts.Add(game);
            }
        }

        private int GetCountOfId(ObservableCollection<GameItem> list, GameItem g)
        {
            int count = 0;
            foreach (GameItem G in list)
            {
                if (G.id == g.id && G.amount == g.amount) count++;
            }
            return count;
        }
    }
}
