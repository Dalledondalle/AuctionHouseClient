using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionHouseClient.Shared
{
    public class Auction
    {
        public int AuctionId { get; set; }
        public int Bid { get; set; }
        public int Buyout { get; set; }
        public DateTime PostedDate { get; set; }
        public int Duration { get; set; }
        public int Amount { get; set; }
        public string SellerName { get; set; }
        public string Name { get; set; }
        public string Game { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public ObservableCollection<string> auctionInfo { get; set; }
        public GameItem item { get; set; }
        public DateTime ExperationDate { get; set; }
        public string ExperationDateStr { get; set; }
        public string TimeLeft 
        { 
            get
            {
                return TimeLeftCalc(PostedDate, Duration);
            } 
        }
        public int TimeLeftIndex { get; set; }
        public bool Premium { get; set; }
        public Auction()
        {
        }
        private string TimeLeftCalc(DateTime postedDate, int duration)
        {
            DateTime experationDate = postedDate.AddHours(duration);
            if (experationDate - DateTime.Now < DateTime.Now.AddHours(2) - DateTime.Now)
            {
                TimeLeftIndex = 0;
                return "short";
            } //Less than 2 hours left
            else if (experationDate - DateTime.Now < DateTime.Now.AddHours(12) - DateTime.Now)
            {
                TimeLeftIndex = 1;
                return "medium";
            }//Between 2 hours and 12 hours left
            else if (experationDate - DateTime.Now < DateTime.Now.AddHours(24) - DateTime.Now)
            {
                TimeLeftIndex = 2;
                return "long";
            }//Between 12 hours and 24 hours left
            else
            {
                TimeLeftIndex = 3;
                return "Very long";
            }
        }
    }
}
