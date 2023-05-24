using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Data.OleDb;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace MSSQL_BANK
{
    public partial class Form1 : Form
    {
        private SqlConnection sqlConnection = null;
        private List<TextBox> textBoxes = new List<TextBox>();
        private List<Label> Labels = new List<Label>();
        private string str = "Валюты";
        private int n;
        private SqlDataAdapter dataAdapter = null;
        private DataSet dataSet = null;
        private List<string> lstTables = new List<string>();
        

        public Form1()
        {
            InitializeComponent();

        }



        private void Form1_Load(object sender, EventArgs e)
        {
  
            textBoxes.Add(textBox2);
            textBoxes.Add(textBox3);
            textBoxes.Add(textBox4);
            textBoxes.Add(textBox5);
            textBoxes.Add(textBox6);
            textBoxes.Add(textBox7);
            textBoxes.Add(textBox8);

            Labels.Add(label1);
            Labels.Add(label2);
            Labels.Add(label3);
            Labels.Add(label4);
            Labels.Add(label5);
            Labels.Add(label6);
            Labels.Add(label7);

            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["BankDB"].ConnectionString);
            sqlConnection.Open();

            comboBox1.Text = str;

            dataAdapter = new SqlDataAdapter("SELECT * FROM [" + str + "]", sqlConnection);

            dataSet = new DataSet();

            SqlCommand com = new SqlCommand("SELECT COUNT(*) FROM information_schema.COLUMNS WHERE TABLE_NAME = N'" + str + "'", sqlConnection);
            n = Convert.ToInt32(com.ExecuteScalar());

            for (int i = 1; i < n; i++)
            {
                Labels[i - 1].Text = lstTables[i].ToString();
            }

            dataAdapter.Fill(dataSet);

            dataGridView1.DataSource = dataSet.Tables[0];
            dataGridView1.Columns.RemoveAt(0);
            //dataGridView1.Columns.RemoveAt(1);

            SetVisibleTextBox(n);

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;
            updateGrid();
            //dataGridView1.Columns["Id"].ReadOnly = true;

            lstTables = new List<string>();
            SqlCommand cmd = new SqlCommand("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'" + str + "'", sqlConnection);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    lstTables.Add((string)reader[0]);
                }

            }



            if (sqlConnection.State == ConnectionState.Open)
            {
                //MessageBox.Show("Подключение установлено");
            }
            else
            {
                MessageBox.Show("Подключение не установлено!");
            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            lstTables = new List<string>();
            
            SqlCommand cmd = new SqlCommand("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'" + str + "'", sqlConnection);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    lstTables.Add((string)reader[0]);
                }
                
            }


            string[] temp = { "a", "b", "c", "d", "e", "f", "g" };
            string atributes = "";
            string atributes2 = "";
            //for (int i = 0; i < n; i++)
            //{

            //    if (i  !=  n - 1)
            //    {
            //        atributes += "N'" + lst[i].ToString() + "', ";
            //        atributes2 += "@N'" + lst[i].ToString() + "', ";
            //    }
            //    else
            //    {
            //        atributes += "N'" + lst[i].ToString() + "'";
            //        atributes2 += "@N'" + lst[i].ToString() + "'";
            //    }
            //}

            for (int i = 1; i < n; i++)
            {

                if (i != n - 1)
                {
                    atributes += "[" + lstTables[i].ToString() + "], ";
                    //atributes2 += "@[" + lstTables[i].ToString() + "], ";
                    atributes2 += "@" + temp[i - 1].ToString() + ", ";
                }
                else
                {
                    atributes += "[" + lstTables[i].ToString() + "]";
                    //atributes2 += "@[" + lstTables[i].ToString() + "]";
                    atributes2 += "@" + temp[i - 1].ToString();
                }
            }

            //atributes2 = "@a, @b, @c";




            SqlCommand com = new SqlCommand(
                $"INSERT INTO [" + str + "] (" + atributes + ") VALUES (" + atributes2 + ") "
                ,sqlConnection
                );
            bool emptyBox = false;
            for (int i = 1; i < n; i++)
            {
                if (textBoxes[i-1].Text != "")
                {
                    if (textBoxes[i - 1].Text.Contains(","))
                    {
                        textBoxes[i - 1].Text = textBoxes[i - 1].Text.Replace(',', '.');
                    }
                    com.Parameters.AddWithValue("@" + temp[i - 1].ToString(), textBoxes[i - 1].Text);
                }
                else
                {
                    emptyBox = true;
                }
            }

            //com.Parameters.AddWithValue("a", textBoxes[0].Text);
            //com.Parameters.AddWithValue("b", textBoxes[1].Text);
            //com.Parameters.AddWithValue("c", textBoxes[2].Text);

            if (!emptyBox)
            {
                try
                {
                    com.ExecuteNonQuery();
                    updateGrid();
                }
                catch (Exception)
                {
                    MessageBox.Show("Это строку невозможно вставить!");
                    //throw;
                }
            }
            else
            {
                MessageBox.Show("Это строку невозможно вставить!");
            }

            

            
            //for (int i = 0; i < textBoxes.Count; i++)
            //{
            //    if (textBoxes[i].Visible)
            //    textBoxes[i].Text = lstTables[i];
            //}

        }

        private void updateGrid()
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT * FROM [" + str + "]", sqlConnection);

            dataSet = new DataSet();

            dataAdapter.Fill(dataSet);

            dataGridView1.DataSource = dataSet.Tables[0];
            dataGridView1.Columns.RemoveAt(0);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            SetInvisibleTextBox();


            str = comboBox1.Text;

            dataAdapter = new SqlDataAdapter("SELECT * FROM [" + str + "]", sqlConnection);

            dataSet = new DataSet();

            SqlCommand com = new SqlCommand("SELECT COUNT(*) FROM information_schema.COLUMNS WHERE TABLE_NAME = N'" + str + "'",sqlConnection);
            n = Convert.ToInt32(com.ExecuteScalar());

            lstTables = new List<string>();
            SqlCommand cmd = new SqlCommand("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'" + str + "'", sqlConnection);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    lstTables.Add((string)reader[0]);
                }

            }

            for (int i = 1; i < n; i++)
            {
                Labels[i - 1].Text = lstTables[i].ToString();
            }

            dataAdapter.Fill(dataSet);

            dataGridView1.DataSource = dataSet.Tables[0];
            dataGridView1.Columns.RemoveAt(0);

            SetVisibleTextBox(n);



        }

        //private void SetVisibleTextBox(Control.ControlCollection control, int n)
        //{
        //    foreach (Control _control in control)
        //    {
        //        if (_control is TextBox && n > 0)
        //        {
        //            ((TextBox)_control).Visible = true;
        //            n--;
        //        }
                    
        //        if (_control.Controls.Count > 0)
        //        {
        //            SetVisibleTextBox(_control.Controls, n);
        //        }
        //    }
        //}

        private void SetVisibleTextBox(int n)
        {
            for (int i = 0; i < n - 1; i++)
            {
                textBoxes[i].Visible = true;
                Labels[i].Visible = true;
            }
        }


        private void SetInvisibleTextBox()
        {
            for (int i = 0; i < textBoxes.Count; i++)
            {
                textBoxes[i].Visible = false;
                Labels[i].Visible = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            

            //foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            //{
            //    dataGridView1.Rows.Remove(row);
            //}

            string IDs = string.Empty;
            if (dataGridView1.SelectedRows != null)
                for (int i = 0; i < dataGridView1.SelectedRows.Count; i++)
                {
                    IDs += dataGridView1.SelectedRows[i].Cells[0].Value.ToString();
                    if (i != dataGridView1.SelectedRows.Count - 1)
                        IDs += ",";
                }
            if (IDs == string.Empty)
            {
                MessageBox.Show("Выделите строки");
                return;
            }
            string query = String.Format("DELETE FROM [{0}] " +
                   "WHERE (ID IN ({1}));", str, IDs);

            dataAdapter.DeleteCommand = new SqlCommand(query, sqlConnection);
            try
            {
                dataAdapter.DeleteCommand.ExecuteNonQuery();
                updateGrid();
            }
            catch (Exception)
            {
                MessageBox.Show("Это строку удалить невозможно!");
                //throw;
            }
            




        }

        private void button3_Click(object sender, EventArgs e)
        {
            string IDs = string.Empty;
            if (dataGridView1.SelectedRows != null)
                for (int i = 0; i < dataGridView1.SelectedRows.Count; i++)
                {
                    IDs += dataGridView1.SelectedRows[i].Cells[0].Value.ToString();
                    if (i != dataGridView1.SelectedRows.Count - 1)
                        IDs += ",";
                }
            if (IDs == string.Empty)
            {
                MessageBox.Show("Выделите строки");
                return;
            }

            lstTables = new List<string>();
            SqlCommand cmd = new SqlCommand("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'" + str + "'", sqlConnection);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    lstTables.Add((string)reader[0]);
                }

            }
            string temp = "";
            bool emptyBox = false;
            for (int i = 1; i < n; i++)
            {
                if (textBoxes[i-1].Text != "")
                {
                    temp += "[" + lstTables[i].ToString() + "] = N'" + textBoxes[i - 1].Text + "'";
                    if (i != n - 1)
                        temp += ", ";
                }
                else
                {
                    emptyBox = true;
                }
                
            }


            string query = String.Format("UPDATE {0} " +
                   "SET  " + temp + 
                   " WHERE (ID IN ({1}));", str, IDs);

            dataAdapter.UpdateCommand = new SqlCommand(query, sqlConnection);


            
            if (!emptyBox)
            {
                try
                {
                    dataAdapter.UpdateCommand.ExecuteNonQuery();
                    updateGrid();
                }
                catch (Exception)
                {
                    MessageBox.Show("Это строку невозможно обновить!");
                    //throw;
                }
            }
            else
            {
                MessageBox.Show("Это строку невозможно обновить пустыми значениями!");
            }

            


        }

        private void button4_Click(object sender, EventArgs e)
        {
            string temp = "";

            bool flag = true;

            for (int i = 1; i < n; i++)
            {
                

                if (textBoxes[i - 1].Text != "")
                {
                    if (flag)
                    {
                        temp += "[" + lstTables[i] + "] = " + "N'" + textBoxes[i - 1].Text + "'";
                        flag = false;
                    }
                    else
                    {
                        temp += " AND [" + lstTables[i] + "] = " + "N'" + textBoxes[i - 1].Text + "'";
                    }
                    
                    

                }                

            }


            string query = "";

            if (!flag)
            {
                query = String.Format("SELECT * FROM {0} " +
                    " WHERE {1};", str, temp);
            }
            else
            {
                query = String.Format("SELECT * FROM {0} ", str);
            }

            

            SqlDataAdapter dataAdapter = new SqlDataAdapter(query, sqlConnection);

            //try
            //{
            //    dataAdapter.SelectCommand.ExecuteNonQuery();
            //    updateGrid();
            //}
            //catch (Exception)
            //{
            //    MessageBox.Show("Это строку удалить невозможно!");
            //    //throw;
            //}


            dataSet = new DataSet();

            dataAdapter.Fill(dataSet);

            dataGridView1.DataSource = dataSet.Tables[0];
            dataGridView1.Columns.RemoveAt(0);





        }

        private void button5_Click(object sender, EventArgs e)
        {

            string fileName = "Банк";
            //Объект документа пдф
            iTextSharp.text.Document doc = new iTextSharp.text.Document();

            //Создаем объект записи пдф-документа в файл
            PdfWriter.GetInstance(doc, new FileStream("pdfTables.pdf", FileMode.OpenOrCreate));

            //Открываем документ
            doc.Open();

            //Определение шрифта необходимо для сохранения кириллического текста
            //Иначе мы не увидим кириллический текст
            //Если мы работаем только с англоязычными текстами, то шрифт можно не указывать
            BaseFont baseFont = BaseFont.CreateFont("C:/Windows/Fonts/arial.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font font = new iTextSharp.text.Font(baseFont, iTextSharp.text.Font.DEFAULTSIZE, iTextSharp.text.Font.NORMAL);

            //Обход по всем таблицам датасета (хотя в данном случае мы можем опустить
            //Так как в нашей бд только одна таблица)
            for (int i = 0; i < dataSet.Tables.Count; i++)
            {
                //Создаем объект таблицы и передаем в нее число столбцов таблицы из нашего датасета
                PdfPTable table = new PdfPTable(dataSet.Tables[i].Columns.Count);

                //Добавим в таблицу общий заголовок
                PdfPCell cell = new PdfPCell(new Phrase("БД " + fileName + ", таблица " + str, font));

                cell.Colspan = dataSet.Tables[i].Columns.Count;
                cell.HorizontalAlignment = 1;
                //Убираем границу первой ячейки, чтобы балы как заголовок
                cell.Border = 0;
                table.AddCell(cell);

                //Сначала добавляем заголовки таблицы
                for (int j = 0; j < dataSet.Tables[i].Columns.Count; j++)
                {
                    cell = new PdfPCell(new Phrase(new Phrase(dataSet.Tables[i].Columns[j].ColumnName, font)));
                    //Фоновый цвет (необязательно, просто сделаем по красивее)
                    cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);
                }

                //Добавляем все остальные ячейки
                for (int j = 0; j < dataSet.Tables[i].Rows.Count; j++)
                {
                    for (int k = 0; k < dataSet.Tables[i].Columns.Count; k++)
                    {
                        table.AddCell(new Phrase(dataSet.Tables[i].Rows[j][k].ToString(), font));
                    }
                }
                //Добавляем таблицу в документ
                doc.Add(table);
            }
            //Закрываем документ
            doc.Close();

            MessageBox.Show("Pdf-документ сохранен");
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }







        //private void SetInvisibleTextBox(Control.ControlCollection control)
        //{
        //    foreach (Control _control in control)
        //    {
        //        if (_control is TextBox)
        //        {
        //            ((TextBox)_control).Visible = false;        
        //        }

        //        if (_control.Controls.Count > 0)
        //        {
        //            SetInvisibleTextBox(_control.Controls);
        //        }
        //    }
        //}



    }
}
