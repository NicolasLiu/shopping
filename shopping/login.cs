using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace shopping
{
    public partial class login : Form
    {
        private DataBase DB;
        public login(DataBase db)
        {
            this.DB = db;
            DB.Connect();
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int login = DB.Login(textBox1.Text,textBox2.Text);
            if (login == 0)
            {
                this.Close();
            }
            else if (login == 1)
            {
                MessageBox.Show("账号被冻结", "提示");
            }
            else if(login == 2)
            {
                MessageBox.Show("用户名或密码错误", "提示");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            loginflowLayout.Hide();
            registertableLayout.Show();
        }

        private void registerTrue_Click(object sender, EventArgs e)
        {
            if (registerUid.Text.Trim() == string.Empty || registerPwd.Text == string.Empty || registerName.Text.Trim() == string.Empty || registerPhone.Text.Trim() == string.Empty || registerAddress.Text.Trim() == string.Empty) 
            {
                MessageBox.Show("信息不能为空", "提示");
                return;
            }
            int role = radioButton1.Checked ? 2 : 1;  
            bool result = DB.Register(registerUid.Text.Trim(), registerPwd.Text, role, registerName.Text.Trim(), registerPhone.Text.Trim(), registerAddress.Text.Trim());
            registertableLayout.Hide();
            loginflowLayout.Show();
            if (result)
            {
                MessageBox.Show("注册成功", "提示");
            }
            else
            {
                MessageBox.Show("注册失败", "提示");
            }
        }

        private void registerFalse_Click(object sender, EventArgs e)
        {

            registertableLayout.Hide();
            loginflowLayout.Show();
        }
    }

}
