using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionHouseClient.Shared
{
    public class Mail : INotifyPropertyChanged
    {
        public int id { get; set; }
        private int itemid;
        public int Itemid {
            get
            {
                return itemid;
            }
            set
            {
                itemid = value;
            } 
        }
        public int amount { get; set; }
        private GameItem containedItem;
        public GameItem ContainedItem 
        {
            get
            {
                return containedItem;
            }
            set
            {
                containedItem = value;
            }
        }
        private DBConn db;

        private bool claimed;
        public bool Claimed 
        {
            get
            {
                return !claimed;
            } 
            set
            {
                if (value == true) Alpha = 0.5f;
                else Alpha = 1.0f;
                claimed = value;
                NotifyStaticPropertyChanged("Claimed");
            }
        }
        private float alpha;
        public float Alpha
        {
            get
            {
                return alpha;
            } 
            set
            {
                alpha = value;
                NotifyStaticPropertyChanged("Alpha");
            }
        }
        private DateTime recievedDate;

        public event PropertyChangedEventHandler PropertyChanged;
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
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

        public DateTime RecievedDate {
            get
            {
                return recievedDate;
            }
            set
            {
                ShownDate = value.ToString("MM/dd/yyyy HH:mm");
                recievedDate = value;
            }
        }

        public string ShownDate { get; set; }
        public bool Seen { get; set; }
        public string Message { get; set; }

        public Mail(DBConn _db)
        {
            db = _db;
        }
        public void RefreshItem()
        {
            ContainedItem = db.GetItem(id);
        }
    }
}
