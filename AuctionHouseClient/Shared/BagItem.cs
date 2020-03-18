using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionHouseClient.Shared
{
    public class BagItem : InventoryItem
    {
        private bool wts;
        public bool Wts { 
            get
            {
                return wts;
            }
            set
            {
                wts = value;
                MarkForWts();
            }
        }
        private void MarkForWts()
        {
            db.MarkBag(this);
        }
    }
}
