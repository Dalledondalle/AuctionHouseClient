using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Data;
using System.Configuration;
using System.Collections.ObjectModel;

namespace AuctionHouseClient.Shared
{
    public class DBConn
    {
        static public SqlConnection conn;
        private SqlDataReader dataReader;
        private string username;
        public string Username
        {
            get
            {
                return username;
            }
        }

        private int userID;
        public int UserID
        {
            get
            {
                return userID;
            }
        }

        //Update for new DB
        public DBConn()
        {
            conn = new SqlConnection(Cnnval("AHDB"));
            conn.Open();
        }

        //Update for new DB
        public string Cnnval(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        //Updated for new DB
        public bool LoginWith(string username, string password)
        {
            if (username != "" && password != "")
            {
                SqlCommand command = new SqlCommand("exec login @Username, @Password", conn);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", password);
                dataReader = command.ExecuteReader();
                if (dataReader.Read())
                {
                    this.username = dataReader.GetValue(0).ToString();
                    this.userID = int.Parse(dataReader.GetValue(1).ToString());
                    command.Dispose();
                    dataReader.Close();
                    return true;
                }
                command.Dispose();
                dataReader.Close();
            }
            return false;
        }

        //Updated for new DB
        public void ClearWts()
        {
            SqlCommand command = new SqlCommand("exec ClearWts @userid", conn);
            command.Parameters.AddWithValue("@userid", userID);
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
            command.Dispose();
        }

        //Updated for new DB
        public bool DoesUserExist(string username)
        {
            //OpenConn();
            SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM loginInfo WHERE (username = @user)", conn);
            command.Parameters.AddWithValue("@user", username);
            int UserExist = (int)command.ExecuteScalar();

            if (UserExist > 0)
            {
                command.Dispose();
                return true;
            }
            else
            {
                command.Dispose();
                return false;
            }
        }

        //Updated for new DB
        public void CreateNewUser(string username, string password)
        {
            //OpenConn();
            SqlCommand command = new SqlCommand("exec NewUser @Username = @user, @Password = @pass", conn);
            command.Parameters.AddWithValue("@user", username);
            command.Parameters.AddWithValue("@pass", password);
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
            command.Dispose();
        }

        //Updated for new DB
        private void OpenConn()
        {
            if (conn != null && conn.State == ConnectionState.Closed) conn.Open();
        }

        //Updated for new DB
        public string GetUsername(int userid)
        {
            //OpenConn();
            SqlCommand command = new SqlCommand("exec getusername @userid", conn);
            command.Parameters.AddWithValue("@userid", userid);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    string _username = reader.GetValue(0).ToString();
                    reader.Close();
                    command.Dispose();
                    return _username;
                }
            }
            command.Dispose();
            return "user not found";
        }

        //Updated for new DB
        public void InsertIntoTable(int itemID, int amount = 1, bool bank = false)
        {
            //OpenConn();
            //SqlCommand command = new SqlCommand("Insert into inventory (userid, itemid, amount, bank, wts) values(@userid, @gameitemid, @amount, @bank, 0)", conn);
            //command.Parameters.AddWithValue("@userid", userID);
            //command.Parameters.AddWithValue("@gameitemid", itemID);
            //command.Parameters.AddWithValue("@amount", amount);
            //command.Parameters.AddWithValue("@bank", bank);
            //command.CommandType = CommandType.Text;
            //command.ExecuteNonQuery();
            //command.Dispose();
            return;
        }

        //Updated for new DB (Need optimizing)
        public GameItem GetItem(int id)
        {
            //OpenConn();
            GameItem gameitem = null;
            string sql = $"SELECT * FROM gameitems where id = {id}";
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            da.SelectCommand.CommandTimeout = 120;
            DataSet ds = new DataSet();
            da.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                string name = ds.Tables[0].Rows[0][1].ToString();
                string game = ds.Tables[0].Rows[0][2].ToString();
                string category = ds.Tables[0].Rows[0][3].ToString();
                string description = ds.Tables[0].Rows[0][4].ToString();
                int stamina = GetValueFromSql(ds.Tables[0].Rows[0][5]);
                int intellect = GetValueFromSql(ds.Tables[0].Rows[0][6]);
                int agility = GetValueFromSql(ds.Tables[0].Rows[0][7]);
                int strength = GetValueFromSql(ds.Tables[0].Rows[0][8]);
                int haste = GetValueFromSql(ds.Tables[0].Rows[0][9]);
                int crit = GetValueFromSql(ds.Tables[0].Rows[0][10]);
                int vers = GetValueFromSql(ds.Tables[0].Rows[0][11]);
                int spellpower = GetValueFromSql(ds.Tables[0].Rows[0][12]);

                gameitem = new GameItem(id, name, game, category, description, stamina, intellect, agility, strength, haste, crit, vers, spellpower);
            }
            int GetValueFromSql(object o)
            {
                if (o.ToString() != "") return int.Parse(o.ToString());
                return 0;
            }
            return gameitem;
        }

        //Updated for new DB
        public void MoveToBank(BagItem bagitem)
        {
            //OpenConn();
            SqlCommand cmd = new SqlCommand("exec MoveToBank @id", conn);
            cmd.Parameters.AddWithValue("@id", bagitem.id);
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        //Updated for new DB
        public void MoveToBag(BankItem bankitem)
        {
            //OpenConn();
            SqlCommand cmd = new SqlCommand("exec MoveToBag @id", conn);
            cmd.Parameters.AddWithValue("@id", bankitem.id);
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        //Updated for new DB
        public void UnMarkForWts(object inventoryItem)
        {
            SqlCommand cmd;
            InventoryItem ba;
            if (inventoryItem.GetType() == new BagItem().GetType())
            {
                ba = (BagItem)inventoryItem;
                cmd = new SqlCommand("exec MarkBag @id, @wts", conn);
                cmd.Parameters.AddWithValue("@id", ba.id);
                cmd.Parameters.AddWithValue("@wts", ba.Wts);
            }
            else
            {
                ba = (BankItem)inventoryItem;
                cmd = new SqlCommand("exec MarkBank @id, @wts", conn);
                cmd.Parameters.AddWithValue("@id", ba.id);
                cmd.Parameters.AddWithValue("@wts", ba.Wts);
            }
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        //Updated for new DB
        public ObservableCollection<Auction> SearchAuction(string search)
        {
            //OpenConn();
            ObservableCollection<Auction> collection = new ObservableCollection<Auction>();
            SqlCommand command = new SqlCommand("exec SearchAuction @Search, @premium", conn);
            command.Parameters.AddWithValue("@Search", search);
            command.Parameters.AddWithValue("@premium", 0);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = 0;
                    if (reader.GetValue(0) != null) id = int.Parse(reader.GetValue(0).ToString());

                    int bid = 0;
                    if (reader.GetValue(1) != null) bid = int.Parse(reader.GetValue(1).ToString());

                    int buyout = 0;
                    if (reader.GetValue(2) != null) buyout = int.Parse(reader.GetValue(2).ToString());

                    DateTime date = (DateTime)reader.GetValue(3);

                    int duration = 0;
                    if (reader.GetValue(4) != null) duration = int.Parse(reader.GetValue(4).ToString());

                    int amount = (int)reader.GetValue(5);

                    string seller = (string)reader.GetValue(6);

                    int itemid = (int)reader.GetValue(7);

                    string itemName = (string)reader.GetValue(8);

                    string game = (string)reader.GetValue(9);

                    string category = (string)reader.GetValue(10);

                    string description = (string)reader.GetValue(11);

                    int sta = 0;
                    if (reader.GetValue(12).ToString() != "" && reader.FieldCount > 12) sta = int.Parse(reader.GetValue(12).ToString());

                    int intel = 0;
                    if (reader.GetValue(13).ToString() != "" && reader.FieldCount > 13) intel = int.Parse(reader.GetValue(13).ToString());

                    int agi = 0;
                    if (reader.GetValue(14).ToString() != "" && reader.FieldCount > 14) agi = int.Parse(reader.GetValue(14).ToString());

                    int str = 0;
                    if (reader.GetValue(15).ToString() != "" && reader.FieldCount > 15) str = int.Parse(reader.GetValue(15).ToString());

                    int haste = 0;
                    if (reader.GetValue(16).ToString() != "" && reader.FieldCount > 16) haste = int.Parse(reader.GetValue(16).ToString());

                    int crit = 0;
                    if (reader.GetValue(17).ToString() != "" && reader.FieldCount > 17) crit = int.Parse(reader.GetValue(17).ToString());

                    int vers = 0;
                    if (reader.GetValue(18).ToString() != "" && reader.FieldCount > 18) vers = int.Parse(reader.GetValue(18).ToString());

                    int spellpower = 0;
                    if (reader.GetValue(19).ToString() != "" && reader.FieldCount > 19) spellpower = int.Parse(reader.GetValue(19).ToString());


                    GameItem g = new GameItem(itemid, itemName, game, category, description,
                                                sta, intel, agi, str, haste, crit, vers, spellpower);
                    Auction a = new Auction();
                    a.Amount = amount;
                    a.item = g;
                    a.Name = a.item.name;
                    a.AuctionId = id;
                    a.Bid = bid;
                    a.Buyout = buyout;
                    a.PostedDate = date;
                    a.Duration = duration;
                    a.SellerName = seller;

                    collection.Add(a);
                }
            }
            command.Dispose();
            return collection;
        }

        //Updated for new DB
        public ObservableCollection<Auction> GetPostedList()
        {
            //OpenConn();
            ObservableCollection<Auction> collection = new ObservableCollection<Auction>();
            SqlCommand command = new SqlCommand("exec PostedAuctions @userid", conn);
            command.Parameters.AddWithValue("@userid", UserID);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = 0;
                    if (reader.GetValue(0) != null) id = int.Parse(reader.GetValue(0).ToString());

                    int bid = 0;
                    if (reader.GetValue(1) != null) bid = int.Parse(reader.GetValue(1).ToString());

                    int buyout = 0;
                    if (reader.GetValue(2) != null) buyout = int.Parse(reader.GetValue(2).ToString());

                    DateTime date = (DateTime)reader.GetValue(3);

                    int duration = 0;
                    if (reader.GetValue(4) != null) duration = int.Parse(reader.GetValue(4).ToString());

                    int amount = (int)reader.GetValue(5);

                    string seller = (string)reader.GetValue(6);

                    int itemid = (int)reader.GetValue(7);

                    string itemName = (string)reader.GetValue(8);

                    string game = (string)reader.GetValue(9);

                    string category = (string)reader.GetValue(10);

                    string description = (string)reader.GetValue(11);

                    int sta = 0;
                    if (reader.GetValue(12).ToString() != "" && reader.FieldCount > 12) sta = int.Parse(reader.GetValue(12).ToString());

                    int intel = 0;
                    if (reader.GetValue(13).ToString() != "" && reader.FieldCount > 13) intel = int.Parse(reader.GetValue(13).ToString());

                    int agi = 0;
                    if (reader.GetValue(14).ToString() != "" && reader.FieldCount > 14) agi = int.Parse(reader.GetValue(14).ToString());

                    int str = 0;
                    if (reader.GetValue(15).ToString() != "" && reader.FieldCount > 15) str = int.Parse(reader.GetValue(15).ToString());

                    int haste = 0;
                    if (reader.GetValue(16).ToString() != "" && reader.FieldCount > 16) haste = int.Parse(reader.GetValue(16).ToString());

                    int crit = 0;
                    if (reader.GetValue(17).ToString() != "" && reader.FieldCount > 17) crit = int.Parse(reader.GetValue(17).ToString());

                    int vers = 0;
                    if (reader.GetValue(18).ToString() != "" && reader.FieldCount > 18) vers = int.Parse(reader.GetValue(18).ToString());

                    int spellpower = 0;
                    if (reader.GetValue(19).ToString() != "" && reader.FieldCount > 19) spellpower = int.Parse(reader.GetValue(19).ToString());


                    GameItem g = new GameItem(itemid, itemName, game, category, description,
                                                sta, intel, agi, str, haste, crit, vers, spellpower);
                    Auction a = new Auction();
                    a.Amount = amount;
                    a.Name = itemName;
                    a.item = g;
                    a.AuctionId = id;
                    a.Bid = bid;
                    a.Buyout = buyout;
                    a.PostedDate = date;
                    a.Duration = duration;
                    a.SellerName = seller;

                    collection.Add(a);
                }
            }
            command.Dispose();
            return collection;
        }

        //Updated for new DB
        public ObservableCollection<BankItem> GetBank()
        {
            //OpenConn();
            ObservableCollection<BankItem> collection = new ObservableCollection<BankItem>();
            SqlCommand command = new SqlCommand("Exec GetBank @userid", conn);
            command.Parameters.AddWithValue("@userid", userID);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    collection.Add(new BankItem
                    {
                        id = (int)reader.GetValue(0),
                        Itemid = (int)reader.GetValue(2),
                        Amount = (int)reader.GetValue(3),
                        Wts = (bool)reader.GetValue(4),
                        db = this
                    });
                }
            }
            command.Dispose();
            foreach (BankItem b in collection)
            {
                b.RefreshItem();
            }
            return collection;
        }

        //Updated for new DB
        public ObservableCollection<BagItem> GetBag()
        {
            //OpenConn();
            ObservableCollection<BagItem> collection = new ObservableCollection<BagItem>();
            SqlCommand command = new SqlCommand("Exec GetBag @userid", conn);
            command.Parameters.AddWithValue("@userid", userID);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    collection.Add(new BagItem
                    {
                        id = (int)reader.GetValue(0),
                        Itemid = (int)reader.GetValue(2),
                        Amount = (int)reader.GetValue(3),
                        Wts = (bool)reader.GetValue(4),
                        db = this
                    });
                }
            }
            command.Dispose();
            foreach (BagItem b in collection)
            {
                b.RefreshItem();
            }
            return collection;
        }

        //Updated for new DB
        public void PostAuction(InventoryItem g, int PostingAmount, int duration, int buyout, int bid = 0)
        {
            //OpenConn();
            SqlCommand cmd = new SqlCommand("exec PostAuction @id, @userid, @itemid, @AmountToPost, @MaxAmount, @buyout, @bid, @duration, @bank, @premium", conn);
            if (g.GetType() == new BagItem().GetType()) cmd.Parameters.AddWithValue("@Bank", false);
            else cmd.Parameters.AddWithValue("@Bank", true);
            cmd.Parameters.AddWithValue("@premium", false);
            cmd.Parameters.AddWithValue("@id", g.id);
            cmd.Parameters.AddWithValue("@userid", userID);
            cmd.Parameters.AddWithValue("@itemid", g.ContainedItem.id);
            cmd.Parameters.AddWithValue("@MaxAmount", g.Amount);
            cmd.Parameters.AddWithValue("@AmountToPost", PostingAmount);
            cmd.Parameters.AddWithValue("@Buyout", buyout);
            cmd.Parameters.AddWithValue("@Bid", bid);
            cmd.Parameters.AddWithValue("@Duration", duration);
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        public ObservableCollection<Mail> GetMails()
        {
            ObservableCollection<Mail> collect = new ObservableCollection<Mail>();
            SqlCommand command = new SqlCommand("exec getMails @userid", conn);
            command.Parameters.AddWithValue("@userid", userID);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    collect.Add(new Mail(this)
                    {
                        id = int.Parse(reader.GetValue(0).ToString()),
                        Itemid = int.Parse(reader.GetValue(2).ToString()),
                        amount = int.Parse(reader.GetValue(3).ToString()),
                        RecievedDate = (DateTime)reader.GetValue(4),
                        Message = (string)reader.GetValue(5),
                        Claimed = (bool)reader.GetValue(6),
                        Seen = (bool)reader.GetValue(7)
                    });
                }
            }
            foreach (Mail m in collect)
            {
                m.RefreshItem();
            }
            return collect;
        }

        public void ClaimSingle(Mail mail)
        {
            SqlCommand cmd = new SqlCommand("exec ClaimMail @mailid", conn);
            cmd.Parameters.AddWithValue("@mailid", mail.id);
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        public void CancelAuction(Auction auction)
        {
            SqlCommand cmd = new SqlCommand("exec CancelAuction @auctionID , @msg", conn);
            cmd.Parameters.AddWithValue("@auctionID", auction.AuctionId);
            cmd.Parameters.AddWithValue("@msg", "Auction Canceled");
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        public void MarkBag(InventoryItem inventoryItem)
        {
            SqlCommand cmd = new SqlCommand("exec MarkBag @id", conn);
            cmd.Parameters.AddWithValue("@id", inventoryItem.id);
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        public void MarkBank(InventoryItem inventoryItem)
        {
            SqlCommand cmd = new SqlCommand("exec MarkBank @id", conn);
            cmd.Parameters.AddWithValue("@id", inventoryItem.id);
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        public PremiumCurrency GetPremiumCurrency()
        {
            PremiumCurrency t = new PremiumCurrency();
            SqlCommand command = new SqlCommand("exec GetPremium @userid", conn);
            command.Parameters.AddWithValue("@userid", userID);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    t = new PremiumCurrency()
                    {
                        Amount = (int)reader.GetValue(0)
                    };
                }
            }
            return t;
        }

        public RegularCurrency GetRegularCurrency()
        {
            //OpenConn();
            SqlCommand command = new SqlCommand("exec GetRegular @userid", conn);
            command.Parameters.AddWithValue("@userid", userID);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new RegularCurrency() { Amount = (int)reader.GetValue(0) };
                }
            }
            command.Dispose();
            return new RegularCurrency() { Amount = 0 };
        }

        public void Buyout(Auction auction, int amount)
        {
            SqlCommand cmd = new SqlCommand("exec Buyout @auctionID, @userid, @amount, @premium", conn);
            cmd.Parameters.AddWithValue("@auctionID", auction.AuctionId);
            cmd.Parameters.AddWithValue("@userid", userID);
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@premium", auction.Premium);
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        public void Bid(Auction auction, int amount)
        {
            SqlCommand cmd = new SqlCommand("exec Bid @userid, @auctionid, @bidamount", conn);
            cmd.Parameters.AddWithValue("@userid", userID);
            cmd.Parameters.AddWithValue("@auctionid", auction.AuctionId);
            cmd.Parameters.AddWithValue("@bidamount", amount);
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }
    }
}
