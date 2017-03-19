using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;//выбираем провайдера для MS SQL
using System.IO;

namespace hr_depart
{
    public partial class Form1 : Form
    {
        string connection_string;
        SqlConnection conn;
        List<int> emp_id;
        int index;
        int empId;
        OpenFileDialog ofd = new OpenFileDialog();
        List<int> p_id;
        int idEmp;
        public Form1()
        {
            InitializeComponent();
            connection_string = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=D:\ШАГ\Project\ADO.net\HR\hr_depart\hr_depart\Database1.mdf;Integrated Security=True";
            conn = new SqlConnection(connection_string);
            emp_id = new List<int>();
            p_id = new List<int>();
        }

        //обновляем листбокс с фамилиями сотрудников
        private void updateEmp()
        {
            fieldClear();
            int index = comboBox1.SelectedIndex;
            string name = comboBox1.Items[index].ToString();
            SqlParameter depname = new SqlParameter();
            depname.ParameterName = "@depName";
            depname.SqlDbType = SqlDbType.NVarChar;
            depname.Direction = ParameterDirection.Input;
            depname.Value = name;
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "showEmployees";
            cmd.Parameters.Add(depname);

            conn.Open();
            //считываем инфо
            SqlDataReader reader = cmd.ExecuteReader();
            //пока ридер работает считываем все данные
            string buff;
            listBox1.Items.Clear();
            emp_id.Clear();
            while (reader.Read())
            {
                buff = String.Format("{0} {1} {2}",
                    reader["fname"],
                    reader["sname"],
                    reader["mnane"]
                    );
                listBox1.Items.Add(buff);
                emp_id.Add(Convert.ToInt32(reader["Id"]));

            }
            conn.Close();
            loadPositions();
            comboBox2.Text = "";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateEmp();

        }

        //при загрузке формы загружаем комбобокс департаментами
        private void Form1_Load(object sender, EventArgs e)
        {
            radioButton1.Checked=true;
            groupBox3.Visible = false;
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "EXECUTE showDepartments";
            conn.Open();
            //считываем инфо
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                comboBox1.Items.Add(reader["name"]);
            }
            conn.Close();
            comboBox1.SelectedIndex = 0;

            loadPositions();
            comboBox2.Text = "";
        }

        

        //Выбираем сотрудника из списка
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {
                index = listBox1.SelectedIndex;
                //id сотрудника
                empId = emp_id[index];
                SqlParameter showInfoEmp = new SqlParameter();
                showInfoEmp.ParameterName = "@id";
                showInfoEmp.SqlDbType = SqlDbType.Int;
                showInfoEmp.Direction = ParameterDirection.Input;
                showInfoEmp.Value = empId;
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "showInfo";
                cmd.Parameters.Add(showInfoEmp);

                conn.Open();
                //считываем инфо
                SqlDataReader reader = cmd.ExecuteReader();
                //пока ридер работает считываем все данные
                string[] data;
                textBox1.Clear();
                textBox3.Clear();
                textBox2.Clear();
                dateTimePicker1.Text = "";
                textBox6.Clear();
                textBox4.Clear();
                textBox5.Clear();

                while (reader.Read())
                {                    data = reader["bdate"].ToString().Split(new Char[] { '.', ' ' });
                    textBox1.Text = reader["fname"].ToString();

                    textBox3.Text = reader["sname"].ToString();
                    textBox2.Text = reader["mnane"].ToString();
                    dateTimePicker1.Value = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    textBox6.Text = reader["name"].ToString();
                    textBox4.Text = reader["order1"].ToString();
                    textBox5.Text = reader["order2"].ToString();
                    idEmp = (int)reader["Id"];
                   
                }
                reader.Close();
                SqlCommand comm = new SqlCommand();
                comm.Connection = conn;
                comm.CommandText = @"SELECT foto FROM Eployees WHERE Id=" + empId.ToString();
                
                    reader = comm.ExecuteReader();

                    MemoryStream ms = new MemoryStream();
                    int buffSize = 100;

                    byte[] buffer = new byte[buffSize];

                    while (reader.Read())
                    {
                        int retval;
                        int startIndex = 0;
                       
                            while (true)
                            {
                                retval = (int)reader.GetBytes(0, startIndex, buffer, 0, buffSize);
                                if (retval < 1) break;

                                ms.Write(buffer, 0, retval);
                                ms.Flush();
                                startIndex += retval;

                            }

                        }
                    

                        if (ms.Length < 1) return;

                        pictureBox1.Image = Image.FromStream(ms);
                    

                    ms.Close();

                    reader.Close();
                
                    conn.Close();
                
                comboBox2.Text = "";

            }
            catch { pictureBox1.Image = null; }
            conn.Close();
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
        //кнопка очистить
        private void button4_Click(object sender, EventArgs e)
        {
            fieldClear();
            comboBox2.Text = "";
        }

        //выбор фото
        private void button5_Click(object sender, EventArgs e)
        {

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(ofd.FileName);
            }
        }

        

        //загружаем должности
        private void loadPositions()
        {
            int index = comboBox1.SelectedIndex;
            string name = comboBox1.Items[index].ToString();
            SqlParameter posname = new SqlParameter();
            posname.ParameterName = "@posName1";
            posname.SqlDbType = SqlDbType.NVarChar;
            posname.Direction = ParameterDirection.Input;
            posname.Value = name;
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "showPositions1";
            cmd.Parameters.Add(posname);
           
            conn.Open();
            //считываем инфо
            SqlDataReader reader = cmd.ExecuteReader();
            //пока ридер работает считываем все данные
            comboBox2.Items.Clear();
            p_id.Clear();
            while (reader.Read())
            {               
                comboBox2.Items.Add(reader["name"]);
                p_id.Add(Convert.ToInt32(reader["Id"]));
            }
            conn.Close();
        }


        //выбираем должности
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox6.Text = comboBox2.Items[comboBox2.SelectedIndex].ToString();
        }

        //обновление фото
        private void updatePhoto()
        {
            MessageBox.Show("Выберите фото");
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                try
                {

                    Bitmap myBmp = new Bitmap(ofd.FileName);

                    Image.GetThumbnailImageAbort myCallBack = new Image.GetThumbnailImageAbort(ThumbnailCallBack);
                    Image imgPreview = myBmp.GetThumbnailImage(200, 200, myCallBack, IntPtr.Zero);

                    MemoryStream ms = new MemoryStream();
                    imgPreview.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

                    ms.Flush();
                    ms.Seek(0, SeekOrigin.Begin);

                    BinaryReader br = new BinaryReader(ms);

                    byte[] image = br.ReadBytes((int)ms.Length);

                    SqlCommand comm = new SqlCommand();
                    comm.Connection = conn;
                    comm.CommandText = String.Format("UPDATE Eployees set foto=@Image where Id={0}", idEmp);
                    comm.Parameters.Add("@Image", SqlDbType.Image, image.Length).Value = image;
                    conn.Open();
                    comm.ExecuteNonQuery();
                    conn.Close();
                    ms.Close();


                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {


                }
            }
            else
            {
                MessageBox.Show("Вы не выбрали новую фотографию");
            }
        }
        public bool ThumbnailCallBack()
        {
            return false;
        }
        
        //кнопка редактировать
        private void button2_Click(object sender, EventArgs e)
        {

            SqlParameter fname = new SqlParameter();
            fname.ParameterName = "@fname";
            fname.SqlDbType = SqlDbType.NVarChar;
            fname.Direction = ParameterDirection.Input;
            fname.Value = textBox1.Text;
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "updateEmployee";
            cmd.Parameters.Add(fname);

            SqlParameter mname = new SqlParameter();
            mname.ParameterName = "@mname";
            mname.SqlDbType = SqlDbType.NVarChar;
            mname.Direction = ParameterDirection.Input;
            mname.Value = textBox2.Text;
            cmd.Parameters.Add(mname);

            SqlParameter smane = new SqlParameter();
            smane.ParameterName = "@smane";
            smane.SqlDbType = SqlDbType.NVarChar;
            smane.Direction = ParameterDirection.Input;
            smane.Value = textBox3.Text;
            cmd.Parameters.Add(smane);

            SqlParameter bdate = new SqlParameter();
            bdate.ParameterName = "@bdate";
            bdate.SqlDbType = SqlDbType.Date;
            bdate.Direction = ParameterDirection.Input;
            bdate.Value = dateTimePicker1.Value;
            cmd.Parameters.Add(bdate);

            SqlParameter pos_id = new SqlParameter();
            pos_id.ParameterName = "@pos_id";
            pos_id.SqlDbType = SqlDbType.Int;
            pos_id.Direction = ParameterDirection.Input;
            if (comboBox2.SelectedIndex == -1) 
            { 
                MessageBox.Show("Обновите должность");
                textBox6.Clear();
                return;
            }
            pos_id.Value = p_id[comboBox2.SelectedIndex];
            cmd.Parameters.Add(pos_id);

            SqlParameter order1 = new SqlParameter();
            order1.ParameterName = "@order1";
            order1.SqlDbType = SqlDbType.NVarChar;
            order1.Direction = ParameterDirection.Input;
            order1.Value = textBox4.Text;
            cmd.Parameters.Add(order1);

            SqlParameter order2 = new SqlParameter();
            order2.ParameterName = "@order2";
            order2.SqlDbType = SqlDbType.NVarChar;
            order2.Direction = ParameterDirection.Input;
            order2.Value = textBox5.Text;
            cmd.Parameters.Add(order2);

            SqlParameter id = new SqlParameter();
            id.ParameterName = "@id";
            id.SqlDbType = SqlDbType.Int;
            id.Direction = ParameterDirection.Input;
            id.Value = idEmp;
            cmd.Parameters.Add(id);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            
            updatePhoto();
            updateEmp();
        }
        // кнопка добавить
        private void button1_Click(object sender, EventArgs e)
        {
            SqlParameter fname = new SqlParameter();
            fname.ParameterName = "@fname";
            fname.SqlDbType = SqlDbType.NVarChar;
            fname.Direction = ParameterDirection.Input;
            fname.Value = textBox1.Text;
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "insertEmployee";
            cmd.Parameters.Add(fname);

            SqlParameter mname = new SqlParameter();
            mname.ParameterName = "@mname";
            mname.SqlDbType = SqlDbType.NVarChar;
            mname.Direction = ParameterDirection.Input;
            mname.Value = textBox2.Text;
            cmd.Parameters.Add(mname);

            SqlParameter smane = new SqlParameter();
            smane.ParameterName = "@smane";
            smane.SqlDbType = SqlDbType.NVarChar;
            smane.Direction = ParameterDirection.Input;
            smane.Value = textBox3.Text;
            cmd.Parameters.Add(smane);

            SqlParameter bdate = new SqlParameter();
            bdate.ParameterName = "@bdate";
            bdate.SqlDbType = SqlDbType.Date;
            bdate.Direction = ParameterDirection.Input;
            bdate.Value = dateTimePicker1.Value;
            cmd.Parameters.Add(bdate);

            SqlParameter pos_id = new SqlParameter();
            pos_id.ParameterName = "@pos_id";
            pos_id.SqlDbType = SqlDbType.Int;
            pos_id.Direction = ParameterDirection.Input;
            if (comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Обновите должность");
                textBox6.Clear();
                return;
            }
            pos_id.Value = p_id[comboBox2.SelectedIndex];
            cmd.Parameters.Add(pos_id);

            SqlParameter order1 = new SqlParameter();
            order1.ParameterName = "@order1";
            order1.SqlDbType = SqlDbType.NVarChar;
            order1.Direction = ParameterDirection.Input;
            order1.Value = textBox4.Text;
            cmd.Parameters.Add(order1);

            SqlParameter order2 = new SqlParameter();
            order2.ParameterName = "@order2";
            order2.SqlDbType = SqlDbType.NVarChar;
            order2.Direction = ParameterDirection.Input;
            order2.Value = textBox5.Text;
            cmd.Parameters.Add(order2);
                       
            MessageBox.Show("Выберите фото");
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                try
                {

                    Bitmap myBmp = new Bitmap(ofd.FileName);

                    Image.GetThumbnailImageAbort myCallBack = new Image.GetThumbnailImageAbort(ThumbnailCallBack);
                    Image imgPreview = myBmp.GetThumbnailImage(200, 200, myCallBack, IntPtr.Zero);

                    MemoryStream ms = new MemoryStream();
                    imgPreview.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

                    ms.Flush();
                    ms.Seek(0, SeekOrigin.Begin);

                    BinaryReader br = new BinaryReader(ms);

                    byte[] image = br.ReadBytes((int)ms.Length);

                    cmd.Parameters.Add("@Image", SqlDbType.Image, image.Length).Value = image;
                    conn.Open();            
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    ms.Close();


                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {


                }
            }
            updateEmp();
        }

        //кнопка удалить 
        private void button3_Click(object sender, EventArgs e)
        {
            SqlParameter delEmp = new SqlParameter();
            delEmp.ParameterName = "@id";
            delEmp.SqlDbType = SqlDbType.NVarChar;
            delEmp.Direction = ParameterDirection.Input;
            delEmp.Value = idEmp;
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "deleteEmployees";
            cmd.Parameters.Add(delEmp);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            updateEmp();
        }
        //радио кнопка просмотр
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        //радио кнопка редактирование
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
            radioButton2.Checked = false;
            groupBox3.Visible = false;
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            Form2 frm = new Form2();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                string login = frm.Login;
                string password = frm.Password;

                SqlParameter enterLogin = new SqlParameter();
                enterLogin.ParameterName = "@login";
                enterLogin.SqlDbType = SqlDbType.NVarChar;
                enterLogin.Direction = ParameterDirection.Input;
                enterLogin.Value = login;
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "CheckLog";
                cmd.Parameters.Add(enterLogin);

                SqlParameter enterPassword = new SqlParameter();
                enterPassword.ParameterName = "@password";
                enterPassword.SqlDbType = SqlDbType.NVarChar;
                enterPassword.Direction = ParameterDirection.Input;
                enterPassword.Value = password;
                cmd.Parameters.Add(enterPassword);

                conn.Open();
                //считываем инфо
                SqlDataReader reader = cmd.ExecuteReader();
                //пока ридер работает считываем все данные
                string chackLog = "0";
                string chackPass = "0";
                while (reader.Read())
                {
                    chackLog = reader["login"].ToString();
                    chackPass = reader["password"].ToString();

                }
                conn.Close();
                if (login.Equals(chackLog) == true && password.Equals(chackPass) == true)
                {
                    radioButton1.Checked = false;
                    radioButton2.Checked = true;
                    groupBox3.Visible = true;
                }
                else
                {
                    MessageBox.Show("Логин или пароль неверны!");
                    radioButton1.Checked = true;
                    radioButton2.Checked = false;
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
