using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hr_depart_entity
{
   
    public partial class Form1 : Form
    {
        List<int> emp_id;
        List<int> p_id;
        int idEmp;
        OpenFileDialog ofd = new OpenFileDialog();

        public Form1()
        {
            InitializeComponent();
            emp_id = new List<int>();
            p_id = new List<int>();

        }

        //загружаем должности
        private void loadPositions()
        {
            int index = comboBox1.SelectedIndex;
            string name = comboBox1.Items[index].ToString();
            comboBox2.Items.Clear();
            p_id.Clear();
            var db = new Database1Entities2();
            foreach (var item in db.Departments)
            {
                if (item.name.Equals(name))
                {
                    foreach (var i in db.Positions)
                    {
                        if (item.Id == i.dep_id)
                        {
                            comboBox2.Items.Add(i.name);
                            p_id.Add(Convert.ToInt32(i.Id));
                        }
                    }
                }
            }


        }

        //очистка полей
        private void fieldClear()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            pictureBox1.Image = null;

        }

        private void updateEmp()
        {
            fieldClear();
            int index = comboBox1.SelectedIndex;
            string name = comboBox1.Items[index].ToString();
            string buff;
            listBox1.Items.Clear();
            emp_id.Clear();
            var db = new Database1Entities2();
            //var db2 = new Database1Entities2();

            foreach (var i in db.Departments)
            {
                if (i.name.Equals(name))
                {

                    foreach (var j in db.Positions)
                    {
                        if (i.Id == j.dep_id)
                        {
                            foreach (var item in db.Eployees)
                            {
                                if (j.Id == item.pos_id)
                                {
                                    buff = String.Format("{0} {1} {2}",
                                   item.fname,
                                   item.sname,
                                   item.mnane);
                                    listBox1.Items.Add(buff);
                                    emp_id.Add(Convert.ToInt32(item.Id));
                                }
                            }
                        }
                    }
                }

            }
            loadPositions();
            comboBox2.Text = "";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
            groupBox3.Visible = false;
            var db = new Database1Entities2();
            foreach (var item in db.Departments)
            {
                comboBox1.Items.Add(item.name);
            }
            comboBox1.SelectedIndex = 0;
            comboBox2.Text = "";
        }
        public byte[] Content { get; set; }

        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateEmp();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int index = listBox1.SelectedIndex;

                int empId = emp_id[index];


                string[] data;
                fieldClear();
                var db = new Database1Entities2();
                foreach (var item in db.Eployees)
                {
                    if (item.Id == empId)
                    {
                        data = item.bdate.ToString().Split(new Char[] { '.', ' ' });
                        textBox1.Text = item.fname;
                        textBox3.Text = item.sname;
                        textBox2.Text = item.mnane;
                        dateTimePicker1.Value = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                        textBox4.Text = item.order1;
                        textBox5.Text = item.order2;
                        idEmp = item.Id;
                        foreach (var i in db.Positions)
                        {
                            if (item.pos_id == i.Id)
                            {
                                textBox6.Text = i.name;
                            }
                        }
                    }

                }


                //добавляем фото
                foreach (var j in db.Eployees)
                {
                    if (j.Id == empId)
                    {
                        pictureBox1.Image = byteArrayToImage(j.foto);
                    }
                }
                comboBox2.Text = "";
            }
            catch
            {
                pictureBox1.Image = null;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            fieldClear();
            comboBox2.Text = "";
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }
        //прозрачность
        public void MakeSeeThru()
        {
            this.Opacity = 0.83;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        
        private void fieldColorNone()
        {
            textBox1.BackColor = Color.Empty;
            textBox2.BackColor = Color.Empty;
            textBox3.BackColor = Color.Empty;
            textBox4.BackColor = Color.Empty;
            textBox5.BackColor = Color.Empty;
            textBox6.BackColor = Color.Empty;
            comboBox2.BackColor = Color.Empty;

        }

        private void fieldColor()
        {
            textBox1.BackColor = Color.WhiteSmoke;
            textBox2.BackColor = Color.WhiteSmoke;
            textBox3.BackColor = Color.WhiteSmoke;
            textBox4.BackColor = Color.WhiteSmoke;
            textBox5.BackColor = Color.WhiteSmoke;
            textBox6.BackColor = Color.WhiteSmoke;
            comboBox2.BackColor = Color.WhiteSmoke;

        }


        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }

        //выбираем должности
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox6.Text = comboBox2.Items[comboBox2.SelectedIndex].ToString();
        }
        
        public bool ThumbnailCallBack()
        {
            return false;
        }
        //редактировать
        private void button2_Click(object sender, EventArgs e)
        {

            if (comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Обновите должность");
                textBox6.Clear();
                return;
            }

            Database1Entities2 db = new Database1Entities2();
            var employ = db.Eployees.Where(c => c.Id == idEmp)
        .FirstOrDefault();
            employ.Id = idEmp;
            employ.fname = textBox1.Text;
            employ.mnane = textBox2.Text;
            employ.sname = textBox3.Text;
            employ.bdate = dateTimePicker1.Value;
            employ.pos_id = p_id[comboBox2.SelectedIndex];
            employ.order1 = textBox4.Text;
            employ.order2 = textBox5.Text;

            MessageBox.Show("Выберите фото");
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                Bitmap myBmp = new Bitmap(ofd.FileName);

                Image.GetThumbnailImageAbort myCallBack = new Image.GetThumbnailImageAbort(ThumbnailCallBack);
                Image imgPreview = myBmp.GetThumbnailImage(200, 200, myCallBack, IntPtr.Zero);
                employ.foto = imageToByteArray(imgPreview);


            }

            else
            {
                MessageBox.Show("Вы не выбрали новую фотографию");
            }
            db.SaveChanges();
            updateEmp();
        }


        //кнопка добавить
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Обновите должность");
                textBox6.Clear();
                return;
            }

            Database1Entities2 db = new Database1Entities2();
             MessageBox.Show("Выберите фото");
            byte[] b=null;
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                Bitmap myBmp = new Bitmap(ofd.FileName);

                Image.GetThumbnailImageAbort myCallBack = new Image.GetThumbnailImageAbort(ThumbnailCallBack);
                Image imgPreview = myBmp.GetThumbnailImage(200, 200, myCallBack, IntPtr.Zero);
                b=imageToByteArray(imgPreview);            
            }

            else
            {
                MessageBox.Show("Вы не выбрали новую фотографию");
            }
            Eployees employ = new Eployees
            {
                fname = textBox1.Text,
                mnane = textBox2.Text,
                sname = textBox3.Text,
                bdate = dateTimePicker1.Value,
                pos_id = p_id[comboBox2.SelectedIndex],
                order1 = textBox4.Text,
                order2 = textBox5.Text,
                foto = b
            };
            db.Eployees.Add(employ);
            db.SaveChanges();

            updateEmp();
        }
        //кнопка удалить
        private void button3_Click(object sender, EventArgs e)
        {
            Database1Entities2 db = new Database1Entities2();
            var emp = db.Eployees.Where(c => c.Id == idEmp).FirstOrDefault();
            db.Eployees.Remove(emp);
            db.SaveChanges();
            updateEmp();
        }

        //просмотр
        private void radioButton1_Click(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
            radioButton2.Checked = false;
            groupBox3.Visible = false;
            fieldColor();
        }

        //редактирование

        private void radioButton2_Click(object sender, EventArgs e)
        {
            fieldColorNone();

            Form2 frm = new Form2();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                string login = frm.Login;
                string password = frm.Password;
                Database1Entities2 db = new Database1Entities2();
                
                string chackLog = "0";
                string chackPass = "0";
                int flag = 0;
                foreach (var item in db.Logs)
                {
                    chackLog = item.login;
                    chackPass = item.password;
                    if (login.Equals(chackLog) == true && password.Equals(chackPass) == true)
                    {
                        radioButton1.Checked = false;
                        radioButton2.Checked = true;
                        groupBox3.Visible = true;
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0) {
                    MessageBox.Show("Логин или пароль неверны!");
                    radioButton1.Checked = true;
                    radioButton2.Checked = false;
                    fieldColor();
                }
                
            }
            else
            {
                radioButton1.Checked = true;
                radioButton2.Checked = false;
            }
        }
       
    }
}

