using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AuctionHouseClient.Shared
{
    public class GameItem
    {
        public bool wts { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string game { get; set; }
        public bool usesStats { get; }
        public string category { get; set; }
        public string imagePath { get; set; }
        public ImageSource image { get; }
        public int amount { get; set; }
        public Dictionary<string, int> stats { get; }
        public Dictionary<string, string> tooltipInfo { get; }

        public GameItem(int ID, string Name, string Game, string Category, string Description,
            int Stamina = 0, int Intellect = 0, int Agility = 0, int Strength = 0,
            int Haste = 0, int Crit = 0, int Vers = 0, int Spellpower = 0, bool Wts = false)
        {
            wts = Wts;
            stats = new Dictionary<string, int>();
            tooltipInfo = new Dictionary<string, string>();

            usesStats = false;

            id = ID;

            name = Name;
            tooltipInfo.Add("Name:", name);

            game = Game;
            tooltipInfo.Add("Game:", game);

            description = Description;
            tooltipInfo.Add("Description:", description);
            
            category = Category;
            imagePath = "/Images/"+ id.ToString() +".png";
            image = makeImage(id);
            if (game == "rpg" && category != "Consumable")
            {
                usesStats = true;
                if (Stamina > 0) stats.Add("Stamina:", Stamina);
                if (Stamina > 0) tooltipInfo.Add("Stamina:", Stamina.ToString());

                if (Intellect > 0) stats.Add("Intellect:", Intellect);
                if (Intellect > 0) tooltipInfo.Add("Intellect:", Intellect.ToString());

                if (Agility > 0) stats.Add("Agility:", Agility);
                if (Agility > 0) tooltipInfo.Add("Agility:", Agility.ToString());

                if (Strength > 0) stats.Add("Strength:", Strength);
                if (Strength > 0) tooltipInfo.Add("Strength:", Strength.ToString());

                if (Haste > 0) stats.Add("Haste:", Haste);
                if (Haste > 0) tooltipInfo.Add("Haste:", Haste.ToString());

                if (Crit > 0) stats.Add("Critical Strike:", Crit);
                if (Crit > 0) tooltipInfo.Add("Critical Strike:", Crit.ToString());

                if (Vers > 0) stats.Add("Versatility:", Vers);
                if (Vers > 0) tooltipInfo.Add("Versatility:", Vers.ToString());

                if (Spellpower > 0) stats.Add("Spell Power:", Spellpower);
                if (Spellpower > 0) tooltipInfo.Add("Spell Power:", Spellpower.ToString());
            }            
        }
        private ImageSource makeImage(int id)
        {
            Image img = new Image();
            img.BeginInit();
            img.Height = 50;
            img.Width = 50;
            img.Source = new BitmapImage(new Uri(@"\Images\" + id.ToString() + ".png", UriKind.Relative));
            return img.Source;
        }
    }
}
