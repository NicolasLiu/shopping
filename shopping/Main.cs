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
        private byte[] temp_itempicture;
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
            panelNum = 4;
            panels = new System.Windows.Forms.Panel[panelNum];
            panels[0] = infopanel;
            panels[1] = myorderpanel;
            panels[2] = myshoppanel;
            panels[3] = allshoppanel;
            hideAllPanel();
        }

        private void hideAllPanel()
        {
            for(int i=0;i<panelNum;i++)
            {
                panels[i].Hide();
            }
        }

        private void updateItems(int shopid)
        {
            this.itemstableLayout.Controls.Clear();
            Item[] items = DB.showitems(shopid);
            if (items != null)
            {
                int itemnum = items.Length;
                for (int i = 0; i < itemnum; i++)
                {
                    FlowLayoutPanel panelitem = new FlowLayoutPanel();
                    panelitem.FlowDirection = FlowDirection.TopDown;
                    panelitem.Size = new System.Drawing.Size(120, 180);


                    PictureBox picbox = new PictureBox();
                    picbox.Name = "itempicturebox_" + items[i].id;
                    picbox.Size = new System.Drawing.Size(100, 100);
                    picbox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
                    MemoryStream mstemp = new MemoryStream();
                    mstemp.Write(items[i].picture, 0, items[i].picture.Length);
                    picbox.Image = Image.FromStream(mstemp);
                    mstemp.Close();
                    if (DB.user.role == 2)
                    {
                        picbox.Click += new System.EventHandler(this.itempicture_Click);
                    }
                    else
                    {
                        picbox.Click += new System.EventHandler(this.itempicture_Click2);
                    }
                    picbox.MouseEnter += new System.EventHandler(this.item_MouseEnter);
                    picbox.MouseLeave += new System.EventHandler(this.item_MouseLeave);
                    panelitem.Controls.Add(picbox);

                    Label labelitem = new Label();
                    labelitem.Text = "名称:" + items[i].name;
                    labelitem.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    panelitem.Controls.Add(labelitem);

                    Label labelitem2 = new Label();
                    labelitem2.Text = "数量:" + items[i].num;
                    labelitem2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    panelitem.Controls.Add(labelitem2);

                    Label labelitem3 = new Label();
                    labelitem3.Text = "单价:" + items[i].price;
                    labelitem3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    panelitem.Controls.Add(labelitem3);

                    this.itemstableLayout.Controls.Add(panelitem, i % this.itemstableLayout.ColumnCount, i / this.itemstableLayout.ColumnCount);

                }
            }
            
        }

        private void updateShops()
        {
            this.shopstableLayout.Controls.Clear();
            Shop[] shops = DB.showshops();
            int shopsnum = shops.Length;
            for (int i = 0; i < shopsnum; i++)
            {
                FlowLayoutPanel panelitem = new FlowLayoutPanel();
                panelitem.FlowDirection = FlowDirection.TopDown;
                panelitem.Size = new System.Drawing.Size(120, 140);


                PictureBox picbox = new PictureBox();
                picbox.Name = "shoppicturebox_" + shops[i].id;
                picbox.Size = new System.Drawing.Size(100, 100);
                picbox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
                
                if (shops[i].data != null)
                {
                    MemoryStream mstemp = new MemoryStream();
                    mstemp.Write(shops[i].data, 0, shops[i].data.Length);
                    picbox.Image = Image.FromStream(mstemp);
                    mstemp.Close();
                }
                
                picbox.Click += new System.EventHandler(this.shoppicture_Click);
                picbox.MouseEnter += new System.EventHandler(this.shop_MouseEnter);
                picbox.MouseLeave += new System.EventHandler(this.shop_MouseLeave);
                panelitem.Controls.Add(picbox);

                Label labelitem = new Label();
                labelitem.Text = "名称:" + shops[i].name;
                labelitem.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                panelitem.Controls.Add(labelitem);

                this.shopstableLayout.Controls.Add(panelitem, i % this.shopstableLayout.ColumnCount, i / this.shopstableLayout.ColumnCount);

            }
        }

        private void updateorder()
        {
            orderlistView.Items.Clear();
            Order[] orders;
            if (DB.user.role == 1) 
            {
                orders = DB.showorders(1, DB.user.id);
            }
            else
            {
                orders = DB.showorders(2, DB.shop.id);
            }
            for (int i = 0; i < orders.Length; i++) 
            {
                ListViewItem item = new ListViewItem();
                item.Text = orders[i].id.ToString();
                item.SubItems.Add(orders[i].name);
                item.SubItems.Add(orders[i].price.ToString());
                item.SubItems.Add(orders[i].num.ToString());
                item.SubItems.Add(orders[i].username);
                item.SubItems.Add(orders[i].time);
                string statusstring = "";
                if(orders[i].status == 0)
                {
                    statusstring = "待发货";
                }
                else if(orders[i].status == 1)
                {
                    statusstring = "已完成";
                }
                else if(orders[i].status == 2)
                {
                    statusstring = "已取消";
                }
                item.SubItems.Add(statusstring);
                orderlistView.Items.Add(item);
            }
            
        }

        private void item_MouseEnter(object sender, EventArgs e)
        {
            PictureBox item = (PictureBox)sender;
            item.Cursor = Cursors.Hand;
        }

        private void item_MouseLeave(object sender, EventArgs e)
        {
            PictureBox item = (PictureBox)sender;
            item.Cursor = Cursors.Default;
        }

        private void itempicture_Click(object sender, EventArgs e)
        {
            PictureBox item = (PictureBox)sender;
            string name = item.Name.ToString();
            string[] result = name.Split('_');
            int id = Convert.ToInt32(result[1]);
            Form edititemForm = new editItem(DB, id);
            edititemForm.ShowDialog();
            updateItems(DB.shop.id);
        }

        private void itempicture_Click2(object sender, EventArgs e)
        {
            PictureBox item = (PictureBox)sender;
            string name = item.Name.ToString();
            string[] result = name.Split('_');
            int id = Convert.ToInt32(result[1]);
            Form submitorderForm = new submitorder(DB, id);
            submitorderForm.ShowDialog();
            updateItems(DB.shop.id);
        }

        private void shop_MouseEnter(object sender, EventArgs e)
        {
            PictureBox item = (PictureBox)sender;
            item.Cursor = Cursors.Hand;
        }

        private void shop_MouseLeave(object sender, EventArgs e)
        {
            PictureBox item = (PictureBox)sender;
            item.Cursor = Cursors.Default;
        }

        private void shoppicture_Click(object sender, EventArgs e)
        {
            PictureBox shop = (PictureBox)sender;
            string name = shop.Name.ToString();
            string[] result = name.Split('_');
            int id = Convert.ToInt32(result[1]);
            updateItems(id);
            DB.shop.id = id;
            shopstableLayout.Hide();
            itemstableLayout.Dock = DockStyle.Fill;
            itemstableLayout.Show();
            
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
            updateorder();
            panels[1].Show();
        }

        private void myshopMenu_Click(object sender, EventArgs e)
        {
            hideAllPanel();
            //准备商店信息
            shopnameBox.Text = DB.shop.name;
            shopdesBox.Text = DB.shop.description;
            
            if (DB.shop.data != null)
            {
                MemoryStream ms = new MemoryStream();
                ms.Write(DB.shop.data, 0, DB.shop.data.Length);
                Image img = Image.FromStream(ms);
                ms.Close();
                shoppictureBox.Image = img;
            }
            
            //准备所有商品
            updateItems(DB.shop.id);
            panels[2].Show();
        }

        private void allshopMenu_Click(object sender, EventArgs e)
        {
            hideAllPanel();
            updateShops();
            itemstableLayout.Hide();
            shopstableLayout.Show();
            panels[3].Show();
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
                ms.Close();
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

        private void itempictureBox_Click(object sender, EventArgs e)
        {
            string picFilename;
            openFileDialog1.Filter = "图片文件(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF";
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                picFilename = openFileDialog1.FileName;
                FileStream fs = new FileStream(picFilename, FileMode.Open);
                Image img = Image.FromStream(fs);
                itempictureBox.Image = img;
                MemoryStream ms = new MemoryStream();
                img.Save(ms, img.RawFormat);
                byte[] buffer = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(buffer, 0, buffer.Length);
                ms.Close();
                fs.Close();
                temp_itempicture = buffer;
            }
        }

        private void additembutton_Click(object sender, EventArgs e)
        {
            Item item = new Item();
            item.shopid = DB.shop.id;
            item.name = itemnameBox.Text;
            item.description = itemdesBox.Text;
            item.num = Convert.ToInt32(itemnumBox.Text);
            item.price = Convert.ToDouble(itempriceBox.Text);
            item.picture = temp_itempicture;
            bool mark = DB.additem(item);
            if (mark)
            {
                MessageBox.Show("添加成功", "提示");
                updateItems(DB.shop.id);
            }
            else
            {
                MessageBox.Show("添加失败", "提示");
            }
        }

        private void orderlistView_DoubleClick(object sender, EventArgs e)
        {
            ListView listview = (ListView)sender;
            foreach (ListViewItem lvi in listview.SelectedItems)
            {
                if (DB.user.role == 1) 
                {
                    if (lvi.SubItems[6].Text.CompareTo("已取消") == 0 || lvi.SubItems[6].Text.CompareTo("已完成") == 0) 
                    {
                        continue;
                    }
                    if (DialogResult.Yes == MessageBox.Show("是否取消该订单?", "确认", MessageBoxButtons.YesNo))
                    {
                        int orderid = Convert.ToInt32(lvi.Text);
                        DB.updateorder(orderid, 2);
                        updateorder();
                    }
                }
                else
                {
                    if (lvi.SubItems[6].Text.CompareTo("已取消") == 0 || lvi.SubItems[6].Text.CompareTo("已完成") == 0)
                    {
                        continue;
                    }
                    if (DialogResult.Yes == MessageBox.Show("是否完成该订单?", "确认", MessageBoxButtons.YesNo))
                    {
                        int orderid = Convert.ToInt32(lvi.Text);
                        DB.updateorder(orderid, 1);
                        updateorder();
                    }
                }
                
                
            }
            
        }
    }
}
