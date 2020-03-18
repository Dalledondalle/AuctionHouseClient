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
    public partial class SoldAuctionsPopUpView : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        private ObservableCollection<Mail> mailList;
        public ObservableCollection<Mail> MailList
        {
            get
            {
                return mailList;
            }
            set
            {
                mailList = value;
                OnPropertyChanged("MailList");
            }
        }
        DBConn db;

        private void OnPropertyChanged(string v)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(v));
            }
        }

        public SoldAuctionsPopUpView(DBConn _db)
        {
            mailList = new ObservableCollection<Mail>();
            db = _db;
            InitializeComponent();
            DataContext = this;
        }

        private void ClaimAll_Click(object sender, RoutedEventArgs e)
        {
            ClaimAll();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ClaimAll()
        {
            foreach(Mail m in MailList)
            {
                if (!m.Claimed)
                {
                    db.ClaimSingle(m);
                    m.Seen = true;
                    m.Claimed = true;
                }
            }
            RefreshMail();
        }

        public void RefreshMail()
        {
            mailList.Clear();
            ObservableCollection<Mail> temp = db.GetMails();
            foreach(Mail m in temp)
            {
                mailList.Add(m);
            }
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

        private void ClaimSingle_Click(object sender, RoutedEventArgs e)
        {
            ClaimSingle(sender);
        }

        private void ClaimSingle(object sender)
        {
            Mail m = (((sender as Button).Parent as Grid).DataContext as Mail);
            db.ClaimSingle(m);
            m.Seen = true;
            m.Claimed = true;
            m.Alpha = 0.5f;
            RefreshMail();
        }
    }
}
