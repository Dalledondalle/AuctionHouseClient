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
using System.Windows.Shapes;

namespace AuctionHouseClient.Views
{
    /// <summary>
    /// Interaction logic for SoldAuctionsPopUpView.xaml
    /// </summary>
    public partial class SoldAuctionsPopUpView : Window, INotifyPropertyChanged
    {
        ObservableCollection<Mail> mailList;
        ObservableCollection<Mail> MailList
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string v)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(v));
            }
        }

        public SoldAuctionsPopUpView(DBConn _db)
        {
            db = _db;
            InitializeComponent();
            DataContext = this;
        }

        private void ClaimAll_Click(object sender, RoutedEventArgs e)
        {
            mailList = new ObservableCollection<Mail>();
            mailList.Add(new Mail(db) { ContainedItem = db.GetItem(12), Claimed = false, Message = "Item one", RecievedDate = new DateTime(2020, 03, 11), Seen = false });
            mailList.Add(new Mail(db) { ContainedItem = db.GetItem(5), Claimed = false, Message = "Item two", RecievedDate = new DateTime(2020, 03, 09), Seen = false });
            mailList.Add(new Mail(db) { ContainedItem = db.GetItem(20), Claimed = true, Message = "Item three", RecievedDate = new DateTime(2020, 03, 07), Seen = true });
            //MailListView.ItemsSource = mails;
            foreach (Mail m in MailList)
            {
                Debug.WriteLine(m.ContainedItem.name);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
