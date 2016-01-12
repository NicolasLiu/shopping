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
    public partial class editItem : Form
    {
        private DataBase DB;
        private int itemid;
        private Item temp;
        public editItem(DataBase db,int id)
        {
            DB = db;
            itemid = id;
            InitializeComponent();
            init();
        }
        private void init()
        {
            temp = DB.getitem(itemid);
            nameBox.Text = temp.name;
            desBox.Text = temp.description;
            numBox.Text = temp.num.ToString();
            priceBox.Text = temp.price.ToString();
            MemoryStream mstemp = new MemoryStream();
            mstemp.Write(temp.picture, 0, temp.picture.Length);
            pictureBox.Image = Image.FromStream(mstemp);
            mstemp.Close();
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            string picFilename;
            openFileDialog1.Filter = "图片文件(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF";
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                picFilename = openFileDialog1.FileName;
                FileStream fs = new FileStream(picFilename, FileMode.Open);
                Image img = Image.FromStream(fs);
                pictureBox.Image = img;
                MemoryStream ms = new MemoryStream();
                img.Save(ms, img.RawFormat);
                byte[] buffer = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(buffer, 0, buffer.Length);
                ms.Close();
                fs.Close();
                temp.picture = buffer;
            }
        }

        private void updatebutton_Click(object sender, EventArgs e)
        {
            temp.name = nameBox.Text;
            temp.description = desBox.Text;
            temp.num = Convert.ToInt32(numBox.Text);
            temp.price = Convert.ToDouble(priceBox.Text);
            DB.updateitem(temp);
            this.Close();
        }

        private void deletebutton_Click(object sender, EventArgs e)
        {
            DB.deleteitem(itemid);
            this.Close();
        }
    }
}
