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
        public DateTime RecievedDate { get; set; }
        public bool Seen { get; set; }
        public string Message { get; set; }

        public Mail(DBConn _db)
        {
            db = _db;
        }

    }
}
