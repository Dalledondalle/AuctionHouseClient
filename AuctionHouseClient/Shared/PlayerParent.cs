using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionHouseClient.Shared
{
    public abstract class PlayerParent
    {
        public ObservableCollection<GameItem> inventory = new ObservableCollection<GameItem>();
    }
}
