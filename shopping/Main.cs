using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace shopping
{
    public partial class Main : Form
    {
        private DataBase DB;
        private System.Windows.Forms.Panel[] panels;
        private int panelNum;
        public Main(DataBase db)
        {
            DB = db;
            InitializeComponent();
            init();
        }
        private void init()
        {
            switch(DB.user.role)
            {
                case 1:
                    infoMenu.Show();
                    allshopMenu.Show();
                    myorderMenu.Show();
                    break;
                case 2:
                    infoMenu.Show();
                    myshopMenu.Show();
                    myorderMenu.Show();
                    break;
                case 3:
                    manageMenu.Show();
                    break;
            }
            panelNum = 3;
            panels = new System.Windows.Forms.Panel[panelNum];
            panels[0] = infopanel;
            panels[1] = myorderpanel;
            panels[2] = myshoppanel;
            hideAllPanel();
        }
        private void hideAllPanel()
        {
            for(int i=0;i<panelNum;i++)
            {
                panels[i].Hide();
            }
        }
        private void infoMenu_Click(object sender, EventArgs e)
        {
            hideAllPanel();
            DB.pullUserInfo();
            if (DB.user.role == 1)
            {
                shenfenlabel.Text = "顾客";
            }
            else if(DB.user.role == 2)
            {
                shenfenlabel.Text = "商家";
            }
            nameBox.Text = DB.user.name;
            phoneBox.Text = DB.user.phone;
            addressBox.Text = DB.user.address;
            balanceBox.Text = DB.user.balance.ToString(); 
            panels[0].Show();
        }

        private void myorderMenu_Click(object sender, EventArgs e)
        {
            hideAllPanel();
            panels[1].Show();
        }

        private void myshopMenu_Click(object sender, EventArgs e)
        {
            hideAllPanel();
            shopnameBox.Text = DB.shop.name;
            shopdesBox.Text = DB.shop.description;
            MemoryStream ms = new MemoryStream();
            ms.Write(DB.shop.data,0, DB.shop.data.Length);
            Image img = Image.FromStream(ms);
            shoppictureBox.Image = img;
            panels[2].Show();
        }

        private void allshopMenu_Click(object sender, EventArgs e)
        {

        }

        private void manageMenu_Click(object sender, EventArgs e)
        {

        }

        private void save_button_Click(object sender, EventArgs e)
        {
            DB.user.name = nameBox.Text;
            DB.user.phone = phoneBox.Text;
            DB.user.address = addressBox.Text;
            DB.user.balance = Convert.ToDouble(balanceBox.Text);
            bool mark = DB.pushUserInfo();
            if(mark)
            {
                MessageBox.Show("保存成功", "提示");
            }
            else
            {
                MessageBox.Show("保存失败", "提示");
            }
        }

        private void shoppictureBox_Click(object sender, EventArgs e)
        {
            string picFilename;
            openFileDialog1.Filter = "图片文件(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF";
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog()==DialogResult.OK)
            {
                picFilename = openFileDialog1.FileName;
                FileStream fs = new FileStream(picFilename, FileMode.Open);
                Image img = Image.FromStream(fs);
                shoppictureBox.Image = img;
                MemoryStream ms = new MemoryStream();
                img.Save(ms,img.RawFormat);
                byte[] buffer = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(buffer, 0, buffer.Length);
                fs.Close();
                DB.shop.data = buffer;
            }
        }

        private void shopinfobutton_Click(object sender, EventArgs e)
        {
            DB.shop.name = shopnameBox.Text;
            DB.shop.description = shopdesBox.Text;
            bool mark = DB.pushShopInfo();
            if (mark)
            {
                MessageBox.Show("保存成功", "提示");
            }
            else
            {
                MessageBox.Show("保存失败", "提示");
            }
        }
    }
}
