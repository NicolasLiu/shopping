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
    public partial class submitorder : Form
    {
        private DataBase DB;
        private int itemid;
        private Item temp;
        public submitorder(DataBase db,int id)
        {
            DB = db;
            itemid = id;
            temp = DB.getitem(itemid);
            InitializeComponent();
            leftnum.Text = temp.num.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int num = Convert.ToInt32(numBox.Text);
            if (num > temp.num)
            {
                MessageBox.Show("库存不足", "提示");
            }
            else if (num <= 0)
            {
                MessageBox.Show("无效的数量", "提示");
            }
            else
            {
                int mark = DB.submitorder(DB.user.id, itemid, num);
                if (mark == 0)
                {
                    MessageBox.Show("下单成功", "提示");
                    this.Close();
                }
                else if (mark == 1)
                {
                    MessageBox.Show("余额不足", "提示");
                }
                else
                {
                    MessageBox.Show("库存不足", "提示");
                }

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
