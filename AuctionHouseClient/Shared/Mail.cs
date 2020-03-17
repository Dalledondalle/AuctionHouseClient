using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionHouseClient.Shared
{
    public class Mail
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
        public bool Claimed { get; set; }
        private DateTime recievedDate;
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
