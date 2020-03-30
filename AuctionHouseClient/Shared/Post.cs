using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionHouseClient.Shared
{
    public class Post
    {
        public InventoryItem ItemToPost { get; set; }
        public string Buyout { get; set; }
        public string Bid { get; set; }
        public bool Premium { get; set; }
        public bool CanPost { get; set; }
    }
}
