using System;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
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

    public int role;
    public string name;
    public string phone;
    public string address;
    public double balance;
    public int id;
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
    public bool Login(string uid, string pwd)
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
        if(login)
        {
            user.role = reader.GetInt32(3);
            user.name = reader.GetString(4);
            user.phone = reader.GetString(5);
            user.address = reader.GetString(6);
            user.balance = reader.GetDouble(7);
            user.id = reader.GetInt32(0);
            reader.Close();
            if (user.role == 2) 
            {
                shop = new Shop();
                pullShopInfo();
            }
        }        
        return login;
    }
    public bool Register(string uid, string pwd, int role, string name, string phone, string address)
    {
        SqlCommand cmd = sqlconn.CreateCommand();
        cmd.CommandText = "insert into s_user values(@UID,@PWD,@ROLE,@NAME,@PHONE,@ADDRESS,0)";
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
        cmd.CommandText = "insert into s_item values(@SHOPID,@NAME,@DESCRIPTION,@NUM,@PRICE,@PICTURE);";
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
        }
        reader.Close();
        return result;
    }
}