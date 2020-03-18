using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionHouseClient.Shared
{
    public class InventoryItem
    {
        public bool Wts { get; set; }
        public DBConn db { get; set; }
        public int id { get; set; }
        public int Itemid { get; set; }
        public GameItem ContainedItem { get; set; }
        public int Amount { get; set; }
        public void RefreshItem()
        {
            ContainedItem = db.GetItem(Itemid);
        }
    }
}
