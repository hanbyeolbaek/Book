using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BookManager
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            Text = "사용자관리";

            dataGridView_Users.DataSource = DB.Users;
            dataGridView_Users.CurrentCellChanged += DataGridView_Users_CurrentCellChanged;

            //람다 button_Add 동작
            button_Add.Click += (sender, e) =>
            {
                try
                {
                    if (DB.Users.Exists((x) => x.Id == int.Parse(textBox_ID.Text)))
                    {
                        MessageBox.Show("사용자 ID가 중복됩니다.");

                        TextFile.UsersHistory("사용자 ID 중복", "추가");
                    }
                    else
                    {
                        if (textBox_ID.Text.Trim() == "")
                        {
                            MessageBox.Show("사용자 ID를 입력해주세요.");

                            TextFile.UsersHistory("사용자 ID 미입력", "추가");
                        }
                        else if (textBox_Name.Text.Trim() == "")
                        {
                            MessageBox.Show("사용자의 이름을 입력해주세요.");

                            TextFile.UsersHistory("사용자 이름 미입력", "추가");
                        }
                        else
                        {
                            Query_Insert();

                            DB.SelectDB();
                            dataGridView_Users.DataSource = null;
                            dataGridView_Users.DataSource = DB.Users;

                            MessageBox.Show($"\"{textBox_ID.Text}\" 사용자가 추가되었습니다.");

                            TextFile.UsersHistory($"{textBox_ID.Text}", "추가");
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("예기치 못한 오류가 발생하였습니다. 다시 시도해주세요.");

                    TextFile.UsersHistory("예기치 못한 오류 발생", "추가");
                }
            };

            button_Modify.Click += (sender, e) =>
            {
                try
                {
                    if (textBox_Name.Text.Trim() == "")
                    {
                        MessageBox.Show("사용자의 이름을 입력해주세요.");

                        TextFile.UsersHistory("사용자 이름 미입력", "수정");
                    }
                    else
                    {
                        Query_Modify();

                        DB.SelectDB();
                        dataGridView_Users.DataSource = null;
                        dataGridView_Users.DataSource = DB.Users;

                        MessageBox.Show($"\"{textBox_ID.Text}\" 사용자가 수정되었습니다.");

                        TextFile.UsersHistory($"{textBox_ID.Text}", "수정");
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("존재하지 않는 사용자입니다.");
                    
                    TextFile.UsersHistory("존재하지 않는 사용자", "수정");
                }
            };

            button_Delete.Click += (sender, e) =>
            {
                try
                {
                    if (textBox_ID.Text.Trim() == "")
                    {
                        MessageBox.Show("사용자 ID를 입력해주세요.");

                        TextFile.BooksHistory("사용자 ID 미입력", "삭제");
                    }
                    else
                    {
                        User user = DB.Users.Single((x) => x.Id.ToString() == (textBox_ID.Text));

                        Query_Delete();

                        DB.SelectDB();
                        dataGridView_Users.DataSource = null;
                        dataGridView_Users.DataSource = DB.Users;

                        MessageBox.Show($"\"{user.Id}\" 사용자가 삭제되었습니다.");

                        TextFile.UsersHistory($"{user.Id}", "삭제");
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("존재하지 않는 사용자입니다.");

                    TextFile.UsersHistory("존재하지 않는 사용자", "삭제");
                }
            };
        }

        private void Query_Insert()
        {
            try
            {
                DB.ConnectDB();
                string sqlcommand = "Insert into User_Manager (Id, Name) values (@p1, @p2)";
                SqlCommand cmd = new SqlCommand();

                cmd.Connection = DB.conn;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@p1", textBox_ID.Text);
                cmd.Parameters.AddWithValue("@p2", textBox_Name.Text);
                cmd.CommandText = sqlcommand;
                cmd.ExecuteNonQuery();
                DB.conn.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + Environment.NewLine + e.StackTrace);
                DB.conn.Close();
            }
        }

        private void Query_Modify()
        {
            try
            {
                DB.ConnectDB();
                string sqlcommand = "update User_Manager set Name = @p1 where Id = @p2";
                SqlCommand cmd = new SqlCommand();

                cmd.Connection = DB.conn;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@p1", textBox_Name.Text);
                cmd.Parameters.AddWithValue("@p2", textBox_ID.Text);
                cmd.CommandText = sqlcommand;
                cmd.ExecuteNonQuery();
                DB.conn.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + Environment.NewLine + e.StackTrace);
                DB.conn.Close();
            }
        }

        private void Query_Delete()
        {
            try
            {
                DB.ConnectDB();
                string sqlcommand = "delete from User_Manager where Id = @p1";
                SqlCommand cmd = new SqlCommand();

                cmd.Connection = DB.conn;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@p1", textBox_ID.Text);
                cmd.CommandText = sqlcommand;
                cmd.ExecuteNonQuery();
                DB.conn.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + Environment.NewLine + e.StackTrace);
                DB.conn.Close();
            }
        }

        private void DataGridView_Users_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                User user = dataGridView_Users.CurrentRow.DataBoundItem as User;
                textBox_ID.Text = user.Id.ToString();
                textBox_Name.Text = user.Name;
            }
            catch (Exception)
            {

            }
        }

        private void textBox_ID_KeyPress(object sender, KeyPressEventArgs e)
        {
            //숫자만 입력되도록 필터링
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))    //숫자와 백스페이스를 제외한 나머지를 바로 처리
            {
                e.Handled = true;
            }
        }
    }
}
