using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BookManager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Text = "도서관 관리";

            DB.SelectDB();

            //전체 도서 수
            label_allBookCount.Text = DB.Books.Count.ToString();
            //사용자 수
            label_allUserCount.Text = DB.Users.Count.ToString();
            //대출중인 도서의 수
            label_allBorrowedBook.Text = DB.Books.Where((x) => x.isBorrowed).Count().ToString();
            //연체중인 도서의 수
            label_allDelayedBook.Text = DB.Books.Where((x) =>
            {
                return x.isBorrowed && x.BorrowedAt.AddDays(7) < DateTime.Now;
            }).Count().ToString();

            //데이터 그리드 설정
            dataGridView_BookManager.DataSource = DB.Books;
            dataGridView_UserManager.DataSource = DB.Users;

            dataGridView_BookManager.CurrentCellChanged += DataGridView_BookManager_CurrentCellChanged;
        }

        private void DataGridView_BookManager_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                var bookData = dataGridView_BookManager.CurrentRow;
                textBox_isbn.Text = dataGridView_BookManager.CurrentRow.Cells[0].Value.ToString();
                textBox_bookName.Text = dataGridView_BookManager.CurrentRow.Cells[1].Value.ToString();
            }
            catch(Exception)
            {

            }
        }

        private void dataGridView_UserManager_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                var userData = dataGridView_UserManager.CurrentRow;
                textBox_id.Text = dataGridView_UserManager.CurrentRow.Cells[0].Value.ToString();
            }
            catch(Exception)
            {

            }
        }

        private void Query_Borrow(bool statusBorrow)
        {
            try
            {
                DB.ConnectDB();
                string sqlcommand = "update Book_Manager set UserId = @p1, UserName = @p2, isBorrowed = @p3, BorrowedAt = @p4 where Isbn = @p5";
                SqlCommand cmd = new SqlCommand();

                cmd.Connection = DB.conn;
                cmd.CommandType = CommandType.Text;

                if (statusBorrow == false)
                {
                    cmd.Parameters.AddWithValue("@p1", textBox_id.Text);
                    User user = DB.Users.Single((x) => x.Id.ToString() == textBox_id.Text);
                    cmd.Parameters.AddWithValue("@p2", user.Name);
                    Book book = DB.Books.Single((x) => x.Isbn.ToString() == textBox_isbn.Text);
                    cmd.Parameters.AddWithValue("@p3", !(book.isBorrowed));
                    cmd.Parameters.AddWithValue("@p4", DateTime.Now);
                    cmd.Parameters.AddWithValue("@p5", textBox_isbn.Text);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@p1", "");
                    cmd.Parameters.AddWithValue("@p2", "");
                    Book book = DB.Books.Single((x) => x.Isbn.ToString() == textBox_isbn.Text);
                    cmd.Parameters.AddWithValue("@p3", !(book.isBorrowed));
                    cmd.Parameters.AddWithValue("@p4", "");
                    cmd.Parameters.AddWithValue("@p5", textBox_isbn.Text);
                }
                cmd.CommandText = sqlcommand;
                cmd.ExecuteNonQuery();
                DB.conn.Close();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message + Environment.NewLine + e.StackTrace);
                DB.conn.Close();
            }
        }

        private void button_Borrow_Click(object sender, EventArgs e)
        {
            if(textBox_isbn.Text.Trim() == "")
            {
                MessageBox.Show("Isbn을 입력해주세요.");
                TextFile.ManageHistory("Isbn 미입력", "대여");
            }
            else if(textBox_bookName.Text.Trim() == "")
            {
                MessageBox.Show("책의 제목을 입력해주세요.");
                TextFile.ManageHistory("책 제목 미입력", "대여");
            }
            else if (textBox_id.Text.Trim() == "")
            {
                MessageBox.Show("사용자 ID를 입력해주세요.");
                TextFile.ManageHistory("사용자 ID 미입력", "대여");
            }
            else
            {
                try
                {
                    Book book = DB.Books.Single((x) => x.Isbn == textBox_isbn.Text);
                    if (book.isBorrowed)
                    {
                        MessageBox.Show("이미 대여 중인 도서입니다.");
                        TextFile.ManageHistory("대여된 도서", "대여");
                    }
                    else
                    {
                        User user = DB.Users.Single((x) => x.Id.ToString() == textBox_id.Text);

                        Query_Borrow(book.isBorrowed);

                        MessageBox.Show("\"" + book.Name + "\"이/가\"" + user.Name + "\"님께 대여되었습니다.");
                        TextFile.ManageHistory($"{book.Name}' '{user.Name}", "대여");
                        DB.SelectDB();
                        dataGridView_BookManager.DataSource = null;
                        dataGridView_BookManager.DataSource = DB.Books;
                        label_allBorrowedBook.Text = DB.Books.Where((x) => x.isBorrowed).Count().ToString();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("존재하지 않는 도서 또는 사용자입니다.");
                    TextFile.ManageHistory("존재하지 않는 도서", "대여");
                }
            }
        }

        private void button_Return_Click(object sender, EventArgs e)
        {
            if (textBox_isbn.Text.Trim() == "")
            {
                MessageBox.Show("Isbn을 입력해주세요.");
                TextFile.ManageHistory("Isbn 미입력", "반납");
            }
            else if (textBox_bookName.Text.Trim() == "")
            {
                MessageBox.Show("책의 제목을 입력해주세요.");
                TextFile.ManageHistory("책 제목 미입력", "반납");
            }
            else if (textBox_id.Text.Trim() == "")
            {
                MessageBox.Show("사용자 ID를 입력해주세요.");
                TextFile.ManageHistory("사용자 ID 미입력", "반납");
            }
            else
            {
                try
                {
                    Book book = DB.Books.Single((x) => x.Isbn == textBox_isbn.Text);
                    if (book.isBorrowed)
                    {
                        Query_Borrow(book.isBorrowed);

                        DB.SelectDB();
                        dataGridView_BookManager.DataSource = null;
                        dataGridView_BookManager.DataSource = DB.Books;

                        DateTime oldDay = book.BorrowedAt;
                        TimeSpan timeDiff = DateTime.Now - oldDay;
                        int diffDays = timeDiff.Days;
                        if (diffDays > 7)
                        {
                            MessageBox.Show("\"" + book.Name + "\"이/가 연체 상태로 반납되었습니다.");
                            TextFile.ManageHistory($"{book.Name}'", "연체 반납");
                            label_allBorrowedBook.Text = DB.Books.Where((x) => x.isBorrowed).Count().ToString();
                            label_allDelayedBook.Text = DB.Books.Where((x) =>
                            {
                                return x.isBorrowed && x.BorrowedAt.AddDays(7) < DateTime.Now;
                            }).Count().ToString();
                        }
                        else
                        {
                            MessageBox.Show("\"" + book.Name + "\"이/가 반납되었습니다.");
                            TextFile.ManageHistory($"{book.Name}'", "반납");
                            label_allBorrowedBook.Text = DB.Books.Where((x) => x.isBorrowed).Count().ToString();
                        }
                    }
                    else
                    {
                        MessageBox.Show("대여상태가 아닙니다.");
                        TextFile.ManageHistory("대여상태 아닌 도서", "반납");
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("존재하지 않는 도서 또는 사용자입니다.");
                    TextFile.ManageHistory("존재하지 않는 도서 또는 사용자", "반납");
                }
            }
        }

        private void 도서관리ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //new Form2().ShowDialog();
            Form2 temp = new Form2();
            temp.ShowDialog();
            dataGridView_BookManager.DataSource = null;
            dataGridView_BookManager.DataSource = DB.Books;

            //전체 도서 수
            label_allBookCount.Text = DB.Books.Count.ToString();
        }

        private void 사용자관리ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Form3().ShowDialog();
            dataGridView_UserManager.DataSource = null;
            dataGridView_UserManager.DataSource = DB.Users;
            dataGridView_BookManager.DataSource = null;
            dataGridView_BookManager.DataSource = DB.Books;

            //사용자 수
            label_allUserCount.Text = DB.Users.Count.ToString();
        }

        private void textBox_id_KeyPress(object sender, KeyPressEventArgs e)
        {
            //숫자만 입력되도록 필터링
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))    //숫자와 백스페이스를 제외한 나머지를 바로 처리
            {
                e.Handled = true;
            }
        }
    }
}
