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
                Application.Run(new Main());
            }
            
        }
    }
}
public class DataBase
{
    private bool login;
    private SqlConnection sqlconn;
    public DataBase()
    {
        login = false;
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
        reader.Close();
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
}