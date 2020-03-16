using System;
using System.Collections.Generic;
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
using System.Data.SqlClient;
using System.Diagnostics;
using AuctionHouseClient.Shared;

namespace AuctionHouseClient.Views
{
    /// <summary>
    /// Interaction logic for AdminView.xaml
    /// </summary>
    public partial class AdminView : Window
    {
        public string Username { get; set; }
        private DBConn db;

        public AdminView(DBConn Db)
        {
            db = Db;
            InitializeComponent();
        }

        private void DebugTest_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(db.Username);
            for (int i = 0; i < new Random().Next(1, 11); i++)
            {
              db.InsertIntoTable(new Random().Next(3, 23), new Random().Next(1, 101), false);
              db.InsertIntoTable(new Random().Next(3, 23), new Random().Next(1, 101), true);
            }
        }
    }
}
