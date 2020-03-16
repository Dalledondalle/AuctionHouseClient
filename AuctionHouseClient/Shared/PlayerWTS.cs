using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionHouseClient.Shared
{
    public class PlayerWTS : PlayerParent
    {
        public PlayerWTS(DBConn _db)
        {
            inventory = _db.GetWts();
        }
    }
}
