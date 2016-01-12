using System;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace shopping
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            DataBase db = new DataBase();
            Application.Run(new login(db));
            if(db.isLogin())
            {
                Application.Run(new Main(db));
            }
            
        }
    }
}
public class User
{
    public int id;
    public string username;
    public int role;
    public string name;
    public string phone;
    public string address;
    public double balance;
    public int status;
}
public class Shop
{
    public int id;
    public string name;
    public string description;
    public byte[] data;
}
public class Item
{
    public int id;
    public int shopid;
    public string name;
    public string description;
    public int num;
    public double price;
    public byte[] picture;
}
public class Order
{
    public int id;
    public string name;
    public double price;
    public int num;
    public string username;
    public string time;
    public int status;
}
public class shopsales
{
    public string shopname;
    public double sales;
}
public class DataBase
{
    private bool login;
    private SqlConnection sqlconn;
    public User user;
    public Shop shop;
    public DataBase()
    {
        login = false;
        user = new User();
    }
    public bool Connect()
    {
        sqlconn =  new SqlConnection("Server = .\\SQLEXPRESS; DataBase = shopping; uid = admin; pwd = admin");
        sqlconn.Open();
        return true;
    }
    public bool isLogin()
    {
        return login;
    }
    public int Login(string uid, string pwd)
    {
        SqlCommand cmd = sqlconn.CreateCommand();
        cmd.CommandText = "select * from s_user where username = @UID and password = @PWD";
        cmd.Parameters.AddWithValue("@UID", uid);
        MD5 md5 = MD5.Create();
        byte[] temp = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(pwd));
        string temp2 = "";
        foreach (byte b in temp)
        {
            temp2 += b.ToString("X");
        }
        cmd.Parameters.AddWithValue("@PWD", temp2); 
        SqlDataReader  reader = cmd.ExecuteReader();
        login = reader.Read();
        int status = 0;
        if(login)
        {
            user.role = reader.GetInt32(3);
            user.name = reader.GetString(4);
            user.phone = reader.GetString(5);
            user.address = reader.GetString(6);
            user.balance = reader.GetDouble(7);
            user.id = reader.GetInt32(0);
            status = reader.GetInt32(8);
            reader.Close();
            shop = new Shop();
            if (user.role == 2) 
            {
                pullShopInfo();
            }
        }
        else
        {
            reader.Close();
        }
        if (login && status == 0)
        {
            return 0;
        }
        else if (login && status == 1) 
        {
            login = false;
            return 1;
        }
        else
        {
            return 2;
        }
    }
    public bool Register(string uid, string pwd, int role, string name, string phone, string address)
    {
        SqlCommand cmd = sqlconn.CreateCommand();
        cmd.CommandText = "insert into s_user values(@UID,@PWD,@ROLE,@NAME,@PHONE,@ADDRESS,0,0)";
        cmd.Parameters.AddWithValue("@UID", uid);
        MD5 md5 = MD5.Create();
        byte[] temp = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(pwd));
        string temp2 = ""; 
        foreach(byte b in temp)
        {
            temp2 += b.ToString("X");
        }
        cmd.Parameters.AddWithValue("@PWD", temp2);
        cmd.Parameters.AddWithValue("@ROLE", role);
        cmd.Parameters.AddWithValue("@NAME", name);
        cmd.Parameters.AddWithValue("@PHONE", phone);
        cmd.Parameters.AddWithValue("@ADDRESS", address);
        bool mark = true;
        try
        {
            int result = cmd.ExecuteNonQuery();
        }
        catch
        {
            mark = false;
        }
       
        return mark;
    }
    public void pullUserInfo()
    {
        SqlCommand cmd = sqlconn.CreateCommand();
        cmd.CommandText = "select * from s_user where id = @ID";
        cmd.Parameters.AddWithValue("@ID", user.id);
        SqlDataReader reader = cmd.ExecuteReader();
        reader.Read();
        user.role = reader.GetInt32(3);
        user.name = reader.GetString(4);
        user.phone = reader.GetString(5);
        user.address = reader.GetString(6);
        user.balance = reader.GetDouble(7);
        user.id = reader.GetInt32(0);
        reader.Close();
    }
    public bool pushUserInfo()
    {
        SqlCommand cmd = sqlconn.CreateCommand();
        cmd.CommandText = "update s_user set name = @NAME, phone = @PHONE, role = @ROLE, address = @ADDRESS, balance = @BALANCE where id = @ID";
        cmd.Parameters.AddWithValue("@ID", user.id);
        cmd.Parameters.AddWithValue("@NAME", user.name);
        cmd.Parameters.AddWithValue("@PHONE", user.phone);
        cmd.Parameters.AddWithValue("@ROLE", user.role);
        cmd.Parameters.AddWithValue("@ADDRESS", user.address);
        cmd.Parameters.AddWithValue("@BALANCE", user.balance);
        int result = cmd.ExecuteNonQuery();
        return result == 1 ? true : false;
    }
    public void pullShopInfo()
    {
        SqlCommand cmd = sqlconn.CreateCommand();
        cmd.CommandText = "select S.* from s_user U,s_shop S where S.owner = U.id and U.id = @ID";
        cmd.Parameters.AddWithValue("@ID", user.id);
        SqlDataReader reader = cmd.ExecuteReader();
        reader.Read();
        shop.id = reader.GetInt32(0);
        shop.name = reader.GetString(2);
        shop.description = reader.IsDBNull(3) ? null : reader.GetString(3); 
        shop.data = reader.IsDBNull(4) ? null : (byte[])reader.GetValue(4);
        reader.Close();
    }
    public bool pushShopInfo()
    {
        SqlCommand cmd = sqlconn.CreateCommand();
        cmd.CommandText = "update s_shop set name = @NAME, description = @DESCRIPTION, picture = @DATA where id = @ID";
        cmd.Parameters.AddWithValue("@ID", shop.id);
        cmd.Parameters.AddWithValue("@NAME", shop.name);
        cmd.Parameters.AddWithValue("@DESCRIPTION", shop.description);
        cmd.Parameters.AddWithValue("@DATA", shop.data);
        int result = cmd.ExecuteNonQuery();
        return result == 1 ? true : false;
    }
    public bool additem(Item item)
    {
        SqlCommand cmd = sqlconn.CreateCommand();
        cmd.CommandText = "insert into s_item values(@SHOPID,@NAME,@DESCRIPTION,@NUM,@PRICE,@PICTURE,0);";
        cmd.Parameters.AddWithValue("@SHOPID", item.shopid);
        cmd.Parameters.AddWithValue("@NAME", item.name);
        cmd.Parameters.AddWithValue("@DESCRIPTION", item.description);
        cmd.Parameters.AddWithValue("@NUM", item.num);
        cmd.Parameters.AddWithValue("@PRICE", item.price);
        cmd.Parameters.AddWithValue("@PICTURE", item.picture);
        int result = cmd.ExecuteNonQuery();
        return result == 1 ? true : false;
    }
    private int countitems(int shopid)
    {
        SqlCommand cmd = sqlconn.CreateCommand();
        cmd.CommandText = "select count(*) from s_item where shopid = @SHOPID";
        cmd.Parameters.AddWithValue("@SHOPID", shopid);
        return (int)cmd.ExecuteScalar();
    }
    public Item[] showitems(int shopid)
    {
        SqlCommand cmd = sqlconn.CreateCommand();
        cmd.CommandText = "showitems @SHOPID";
        cmd.Parameters.AddWithValue("@SHOPID", shopid);
        int num = countitems(shopid);
        if (num == 0)
        {
            return null;
        }
        SqlDataReader reader = cmd.ExecuteReader();
        Item[] result = new Item[num];
        int i = 0;
        while (reader.Read())
        {
            result[i] = new Item();
            result[i].id = reader.GetInt32(0);
            result[i].shopid = shopid;
            result[i].name = reader.GetString(2);
            result[i].description = reader.IsDBNull(3) ? null : reader.GetString(3);
            result[i].num = reader.GetInt32(4);
            result[i].price = reader.GetDouble(5);
            result[i].picture = reader.IsDBNull(6) ? null : (byte[])reader.GetValue(6);
            i++;
        }
        reader.Close();
        return result;
    }
    public Item getitem(int itemid)
    {
        SqlCommand cmd = sqlconn.CreateCommand();
        cmd.CommandText = "select * from s_item where id = @ID";
        cmd.Parameters.AddWithValue("@ID", itemid);
        SqlDataReader reader = cmd.ExecuteReader();
        reader.Read();
        Item item = new Item();
        item.id = reader.GetInt32(0);
        item.shopid = reader.GetInt32(1);
        item.name = reader.GetString(2);
        item.description = reader.IsDBNull(3) ? null : reader.GetString(3);
        item.num = reader.GetInt32(4);
        item.price = reader.GetDouble(5);
        item.picture = reader.IsDBNull(6) ? null : (byte[])reader.GetValue(6);
        reader.Close();
        return item;
    }
    public bool updateitem(Item item)
    {
        SqlCommand cmd = sqlconn.CreateCommand();
        cmd.CommandText = "update s_item set name = @NAME, description = @DESCRIPTION, num = @NUM, price = @PRICE, picture = @PICTURE where id = @ID";
        cmd.Parameters.AddWithValue("@ID", item.id);
        cmd.Parameters.AddWithValue("@NAME", item.name);
        cmd.Parameters.AddWithValue("@DESCRIPTION", item.description);
        cmd.Parameters.AddWithValue("@NUM", item.num);
        cmd.Parameters.AddWithValue("@PRICE", item.price);
        cmd.Parameters.AddWithValue("@PICTURE", item.picture);
        int result = cmd.ExecuteNonQuery();
        return result == 1 ? true : false;
    }
    public bool deleteitem(int itemid)
    {
        SqlCommand cmd = sqlconn.CreateCommand();
        cmd.CommandText = "update s_item set status = 1 where id = @ID";
        cmd.Parameters.AddWithValue("@ID", itemid);
        int result = cmd.ExecuteNonQuery();
        return result == 1 ? true : false;
    }
    private int countshops()
    {
        SqlCommand cmd = sqlconn.CreateCommand();
        cmd.CommandText = "select count(*) from s_shop;";
        return (int)cmd.ExecuteScalar();
    }
    public Shop[] showshops()
    {
        SqlCommand cmd = sqlconn.CreateCommand();
        cmd.CommandText = "select * from s_shop";
        int num = countshops();
        if (num == 0)
        {
            return null;
        }
        SqlDataReader reader = cmd.ExecuteReader();
        Shop[] result = new Shop[num];
        int i = 0;
        while (reader.Read())
        {
            result[i] = new Shop();
            result[i].id = reader.GetInt32(0);
            result[i].name = reader.GetString(2);
            result[i].description = reader.IsDBNull(3) ? null : reader.GetString(3);
            result[i].data = reader.IsDBNull(4) ? null : (byte[])reader.GetValue(4);
            i++;
        }
        reader.Close();
        return result;
    }
    public int submitorder(int userid, int itemid, int num)
    {
        SqlCommand cmd = sqlconn.CreateCommand();
        cmd.CommandText = "submitorder @USERID,@ITEMID,@NUM;";
        cmd.Parameters.AddWithValue("@USERID", userid);
        cmd.Parameters.AddWithValue("@ITEMID", itemid);
        cmd.Parameters.AddWithValue("@NUM", num);
        int rtn = (int)cmd.ExecuteScalar();
        return rtn;
    }
    public Order[] showorders(int role, int id)
    {
        SqlCommand cmd = sqlconn.CreateCommand();
        if (role == 1) 
        {
            cmd.CommandText = "select O.id,I.name,I.price,O.num,U.name,O.time,O.status from s_order O,s_user U,s_item I where U.id = @ID and U.id = O.userid and O.itemid = I.id order by O.time;";
            cmd.Parameters.AddWithValue("@ID", id);
            SqlDataReader reader = cmd.ExecuteReader();
            List<Order> list = new List<Order>();
            while (reader.Read())
            {
                Order temp = new Order();
                temp.id = reader.GetInt32(0);
                temp.name = reader.GetString(1);
                temp.price = reader.GetDouble(2);
                temp.num = reader.GetInt32(3);
                temp.username = reader.GetString(4);
                temp.time = reader.GetDateTime(5).ToString();
                temp.status = reader.GetInt32(6);
                list.Add(temp);
            }
            reader.Close();
            return list.ToArray();
        }
        else
        {
            cmd.CommandText = "select O.id,I.name,I.price,O.num,U.name,O.time,O.status from s_order O,s_user U,s_item I,s_shop S where S.id = @ID and I.shopid = S.id and U.id = O.userid and O.itemid = I.id order by O.time;";
            cmd.Parameters.AddWithValue("@ID", id);
            SqlDataReader reader = cmd.ExecuteReader();
            List<Order> list = new List<Order>();
            while (reader.Read())
            {
                Order temp = new Order();
                temp.id = reader.GetInt32(0);
                temp.name = reader.GetString(1);
                temp.price = reader.GetDouble(2);
                temp.num = reader.GetInt32(3);
                temp.username = reader.GetString(4);
                temp.time = reader.GetDateTime(5).ToString();
                temp.status = reader.GetInt32(6);
                list.Add(temp);
            }
            reader.Close();
            return list.ToArray();
        }
        
    }
    public bool updateorder(int id,int status)
    {
        SqlCommand cmd = sqlconn.CreateCommand();
        cmd.CommandText = "update s_order set status = @STATUS where id = @ID;";
        cmd.Parameters.AddWithValue("@ID", id);
        cmd.Parameters.AddWithValue("@STATUS", status);
        int rtn = cmd.ExecuteNonQuery();
        return rtn == 1;
    }
    public User[] showusers()
    {
        SqlCommand cmd = sqlconn.CreateCommand();
        cmd.CommandText = "select * from s_user";
        SqlDataReader reader = cmd.ExecuteReader();
        List<User> list = new List<User>();
        while (reader.Read())
        {
            User temp = new User();
            temp.id = reader.GetInt32(0);
            temp.username = reader.GetString(1);
            temp.role = reader.GetInt32(3);
            temp.name = reader.GetString(4);
            temp.phone = reader.GetString(5);
            temp.address = reader.GetString(6);
            temp.balance = reader.GetDouble(7);
            temp.status = reader.GetInt32(8);
            list.Add(temp);
        }
        reader.Close();
        return list.ToArray();
    }
    public bool updateuser(int id, int status)
    {
        SqlCommand cmd = sqlconn.CreateCommand();
        cmd.CommandText = "update s_user set status = @STATUS where id = @ID;";
        cmd.Parameters.AddWithValue("@ID", id);
        cmd.Parameters.AddWithValue("@STATUS", status);
        int rtn = cmd.ExecuteNonQuery();
        return rtn == 1;
    }
    public shopsales[] statistics()
    {
        SqlCommand cmd = sqlconn.CreateCommand();
        cmd.CommandText = "select * from statistics_view;";
        SqlDataReader reader = cmd.ExecuteReader();
        List<shopsales> list = new List<shopsales>();
        while (reader.Read())
        {
            shopsales temp = new shopsales();
            temp.shopname = reader.GetString(0);
            temp.sales = reader.GetDouble(1);
            list.Add(temp);
        }
        reader.Close();
        return list.ToArray();
    }
}