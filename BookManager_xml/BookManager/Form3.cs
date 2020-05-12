using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

            dataGridView_Users.DataSource = DataManager.Users;
            dataGridView_Users.CurrentCellChanged += DataGridView_Users_CurrentCellChanged;

            //람다 button_Add 동작
            button_Add.Click += (sender, e) =>
            {
                try
                {
                    if(DataManager.Users.Exists((x) => x.Id == int.Parse(textBox_ID.Text)))
                    {
                        MessageBox.Show("사용자 ID가 겹칩니다.");

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
                            User user = new User()
                            {
                                Id = int.Parse(textBox_ID.Text),
                                Name = textBox_Name.Text
                            };
                            DataManager.Users.Add(user);

                            dataGridView_Users.DataSource = null;
                            dataGridView_Users.DataSource = DataManager.Users;
                            DataManager.Save();

                            MessageBox.Show($"사용자 \"{user.Name}\" 님이 추가되었습니다.");

                            TextFile.UsersHistory($"{user.Name}", "추가");
                        }
                    }
                }
                catch (Exception)
                {

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
                        User user = DataManager.Users.Single((x) => x.Id == int.Parse(textBox_ID.Text));
                        user.Name = textBox_Name.Text;

                        MessageBox.Show($"\"{user.Name}\" 으로 이름이 변경되었습니다.");

                        TextFile.UsersHistory($"{user.Name}", "수정");
                    }

                    try
                    {
                        Book book = DataManager.Books.Single((x) => x.UserId == int.Parse(textBox_ID.Text));
                        book.UserName = textBox_Name.Text;
                    }
                    catch (Exception)
                    {
                        
                    }

                    dataGridView_Users.DataSource = null;
                    dataGridView_Users.DataSource = DataManager.Users;
                    DataManager.Save();
                }
                catch (Exception)
                {
                    MessageBox.Show("존재하지 않는 사용자입니다.");
                    //MessageBox.Show("존재하지 않는 사용자입니다." + 
                    //    Environment.NewLine + exception.GetType() + 
                    //    Environment.NewLine + exception.Message + 
                    //    Environment.NewLine + exception.StackTrace);

                    TextFile.UsersHistory("존재하지 않는 사용자", "수정");
                }
            };

            button_Delete.Click += (sender, e) =>
            {
                try
                {
                    User user = DataManager.Users.Single((x) => x.Id == int.Parse(textBox_ID.Text));

                    MessageBox.Show($"사용자 \"{user.Name}\" 님이 삭제되었습니다.");

                    TextFile.UsersHistory($"{user.Name}", "삭제");

                    DataManager.Users.Remove(user);

                    dataGridView_Users.DataSource = null;
                    dataGridView_Users.DataSource = DataManager.Users;
                    DataManager.Save();
                }
                catch (Exception)
                {
                    MessageBox.Show("존재하지 않는 사용자입니다.");

                    TextFile.UsersHistory("존재하지 않는 사용자", "삭제");
                }
            };
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
