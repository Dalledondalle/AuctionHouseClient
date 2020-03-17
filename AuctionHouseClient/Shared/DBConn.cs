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
        public void MarkAsWts(GameItem g)
        {
            //OpenConn();
            SqlCommand command = new SqlCommand("exec MarkForWts @userID, @itemID, @amount", conn);
            command.Parameters.AddWithValue("@userID", userID);
            command.Parameters.AddWithValue("@itemID", g.id);
            command.Parameters.AddWithValue("@amount", g.amount);
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
            command.Dispose();
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
            SqlCommand command = new SqlCommand("Insert into inventory (userid, itemid, amount, bank, wts) values(@userid, @gameitemid, @amount, @bank, 0)", conn);
            command.Parameters.AddWithValue("@userid", userID);
            command.Parameters.AddWithValue("@gameitemid", itemID);
            command.Parameters.AddWithValue("@amount", amount);
            command.Parameters.AddWithValue("@bank", bank);
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
            command.Dispose();
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
        public void MoveToBank(GameItem gameitem)
        {
            //OpenConn();
            SqlCommand cmd = new SqlCommand("exec MoveToBank @userid, @itemid, @amount", conn);
            cmd.Parameters.AddWithValue("@userid", userID);
            cmd.Parameters.AddWithValue("@itemid", gameitem.id);
            cmd.Parameters.AddWithValue("@amount", gameitem.amount);
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        //Updated for new DB
        public void MoveToBag(GameItem gameitem)
        {
            //OpenConn();
            SqlCommand cmd = new SqlCommand("exec MoveToBag @userid, @itemid, @amount", conn);
            cmd.Parameters.AddWithValue("@userid", userID);
            cmd.Parameters.AddWithValue("@itemid", gameitem.id);
            cmd.Parameters.AddWithValue("@amount", gameitem.amount);
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        //Updated for new DB
        public void UnMarkForWts(GameItem g)
        {
            //OpenConn();
            SqlCommand command = new SqlCommand("exec UnMarkForWts @userID, @itemID, @amount", conn);
            command.Parameters.AddWithValue("@userID", userID);
            command.Parameters.AddWithValue("@itemID", g.id);
            command.Parameters.AddWithValue("@amount", g.amount);
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
            command.Dispose();
        }

        //Updated for new DB
        public ObservableCollection<Auction> SearchAuction(string search)
        {
            //OpenConn();
            ObservableCollection<Auction> collection = new ObservableCollection<Auction>();
            SqlCommand command = new SqlCommand("exec SearchAuction @Search", conn);
            command.Parameters.AddWithValue("@Search", search);
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
                    g.amount = amount;
                    Auction a = new Auction(g, this);
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
                    g.amount = amount;
                    Auction a = new Auction(g, this);
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
        public ObservableCollection<GameItem> GetBank()
        {
            //OpenConn();
            ObservableCollection<GameItem> collection = new ObservableCollection<GameItem>();
            SqlCommand command = new SqlCommand("exec getbank @userid", conn);
            command.Parameters.AddWithValue("@userid", userID);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while(reader.Read())
                {
                    int id = 0;
                    if (reader.GetValue(0) != null) id = int.Parse(reader.GetValue(0).ToString());

                    string name = reader.GetValue(4).ToString();
                    string game = reader.GetValue(5).ToString();
                    string category = reader.GetValue(6).ToString();
                    string description = reader.GetValue(7).ToString();

                    int sta = 0;
                    if (reader.GetValue(8).ToString() != "" && reader.FieldCount > 8) sta = int.Parse(reader.GetValue(8).ToString());

                    int intel = 0;
                    if (reader.GetValue(9).ToString() != "" && reader.FieldCount > 9) intel = int.Parse(reader.GetValue(9).ToString());

                    int agi = 0;
                    if (reader.GetValue(10).ToString() != "" && reader.FieldCount > 10) agi = int.Parse(reader.GetValue(10).ToString());

                    int str = 0;
                    if (reader.GetValue(11).ToString() != "" && reader.FieldCount > 11) str = int.Parse(reader.GetValue(11).ToString());

                    int haste = 0;
                    if (reader.GetValue(12).ToString() != "" && reader.FieldCount > 12) haste = int.Parse(reader.GetValue(12).ToString());

                    int crit = 0;
                    if (reader.GetValue(13).ToString() != "" && reader.FieldCount > 13) crit = int.Parse(reader.GetValue(13).ToString());

                    int vers = 0;
                    if (reader.GetValue(14).ToString() != "" && reader.FieldCount > 14) vers = int.Parse(reader.GetValue(14).ToString());

                    int spellpower = 0;
                    if (reader.GetValue(15).ToString() != "" && reader.FieldCount > 15) spellpower = int.Parse(reader.GetValue(15).ToString());

                    int amount = (int)reader.GetValue(1);

                    bool wts = (bool)reader.GetValue(3);

                    GameItem g = new GameItem(id, name, game, category, description,
                                                sta, intel, agi, str, haste, crit, vers, spellpower);
                    g.amount = amount;
                    g.wts = wts;

                    collection.Add(g);
                }
            }
            command.Dispose();
            return collection;
        }

        //Updated for new DB
        public ObservableCollection<GameItem> GetBag()
        {
            //OpenConn();
            ObservableCollection<GameItem> collection = new ObservableCollection<GameItem>();
            SqlCommand command = new SqlCommand("exec getbag @userid", conn);
            command.Parameters.AddWithValue("@userid", userID);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = 0;
                    if (reader.GetValue(0) != null) id = int.Parse(reader.GetValue(0).ToString());

                    string name = reader.GetValue(4).ToString();
                    string game = reader.GetValue(5).ToString();
                    string category = reader.GetValue(6).ToString();
                    string description = reader.GetValue(7).ToString();

                    int sta = 0;
                    if (reader.GetValue(8).ToString() != "" && reader.FieldCount > 8) sta = int.Parse(reader.GetValue(8).ToString());

                    int intel = 0;
                    if (reader.GetValue(9).ToString() != "" && reader.FieldCount > 9) intel = int.Parse(reader.GetValue(9).ToString());

                    int agi = 0;
                    if (reader.GetValue(10).ToString() != "" && reader.FieldCount > 10) agi = int.Parse(reader.GetValue(10).ToString());

                    int str = 0;
                    if (reader.GetValue(11).ToString() != "" && reader.FieldCount > 11) str = int.Parse(reader.GetValue(11).ToString());

                    int haste = 0;
                    if (reader.GetValue(12).ToString() != "" && reader.FieldCount > 12) haste = int.Parse(reader.GetValue(12).ToString());

                    int crit = 0;
                    if (reader.GetValue(13).ToString() != "" && reader.FieldCount > 13) crit = int.Parse(reader.GetValue(13).ToString());

                    int vers = 0;
                    if (reader.GetValue(14).ToString() != "" && reader.FieldCount > 14) vers = int.Parse(reader.GetValue(14).ToString());

                    int spellpower = 0;
                    if (reader.GetValue(15).ToString() != "" && reader.FieldCount > 15) spellpower = int.Parse(reader.GetValue(15).ToString());

                    int amount = (int)reader.GetValue(1);

                    bool wts = (bool)reader.GetValue(3);

                    GameItem g = new GameItem(id, name, game, category, description,
                                                sta, intel, agi, str, haste, crit, vers, spellpower);
                    g.amount = amount;
                    g.wts = wts;

                    collection.Add(g);
                }
            }
            command.Dispose();
            return collection;
        }

        //Updated for new DB
        public ObservableCollection<GameItem> GetWts()
        {
            ObservableCollection<GameItem> collection = new ObservableCollection<GameItem>();
            //OpenConn();
            SqlCommand command = new SqlCommand("exec getwts @userid", conn);
            command.Parameters.AddWithValue("@userid", userID);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = 0;
                    if (reader.GetValue(0) != null) id = int.Parse(reader.GetValue(0).ToString());

                    string name = reader.GetValue(4).ToString();
                    string game = reader.GetValue(5).ToString();
                    string category = reader.GetValue(6).ToString();
                    string description = reader.GetValue(7).ToString();

                    int sta = 0;
                    if (reader.GetValue(8).ToString() != "" && reader.FieldCount > 8) sta = int.Parse(reader.GetValue(8).ToString());

                    int intel = 0;
                    if (reader.GetValue(9).ToString() != "" && reader.FieldCount > 9) intel = int.Parse(reader.GetValue(9).ToString());

                    int agi = 0;
                    if (reader.GetValue(10).ToString() != "" && reader.FieldCount > 10) agi = int.Parse(reader.GetValue(10).ToString());

                    int str = 0;
                    if (reader.GetValue(11).ToString() != "" && reader.FieldCount > 11) str = int.Parse(reader.GetValue(11).ToString());

                    int haste = 0;
                    if (reader.GetValue(12).ToString() != "" && reader.FieldCount > 12) haste = int.Parse(reader.GetValue(12).ToString());

                    int crit = 0;
                    if (reader.GetValue(13).ToString() != "" && reader.FieldCount > 13) crit = int.Parse(reader.GetValue(13).ToString());

                    int vers = 0;
                    if (reader.GetValue(14).ToString() != "" && reader.FieldCount > 14) vers = int.Parse(reader.GetValue(14).ToString());

                    int spellpower = 0;
                    if (reader.GetValue(15).ToString() != "" && reader.FieldCount > 15) spellpower = int.Parse(reader.GetValue(15).ToString());

                    int amount = (int)reader.GetValue(1);

                    bool wts = (bool)reader.GetValue(3);

                    GameItem g = new GameItem(id, name, game, category, description,
                                                sta, intel, agi, str, haste, crit, vers, spellpower);
                    g.amount = amount;
                    g.wts = wts;

                    collection.Add(g);
                }
            }
            command.Dispose();
            return collection;
        }

        //Updated for new DB
        public void PostAuction(GameItem g, int PostingAmount, int duration, int buyout, int bid = 0)
        {
            //OpenConn();
            SqlCommand cmd = new SqlCommand("exec PostAuction @userid, @itemid, @AmountToPost, @MaxAmount, @buyout, @bid, @duration", conn);
            cmd.Parameters.AddWithValue("@userid", userID);
            cmd.Parameters.AddWithValue("@itemid", g.id);
            cmd.Parameters.AddWithValue("@MaxAmount", g.amount);
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
                    collect.Add(new Mail(this) {    id = int.Parse(reader.GetValue(0).ToString()),
                                                    Itemid = int.Parse(reader.GetValue(2).ToString()),
                                                    amount = int.Parse(reader.GetValue(3).ToString()),
                                                    RecievedDate = (DateTime)reader.GetValue(4),
                                                    Message = (string)reader.GetValue(5),
                                                    Claimed = (bool)reader.GetValue(6),
                                                    Seen = (bool)reader.GetValue(7)
                    }) ;
                }
            }
            foreach(Mail m in collect)
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
    }
}
