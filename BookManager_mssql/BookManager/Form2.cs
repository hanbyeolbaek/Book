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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            Text = "도서 관리";

            dataGridView_book.DataSource = DB.Books;
        }

        private void dataGridView_book_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                var bookData = dataGridView_book.CurrentRow;
                textBox_isbn.Text = dataGridView_book.CurrentRow.Cells[0].Value.ToString();
                textBox_bookName.Text = dataGridView_book.CurrentRow.Cells[1].Value.ToString();
                textBox_publisher.Text = dataGridView_book.CurrentRow.Cells[2].Value.ToString();
                textBox_page.Text = dataGridView_book.CurrentRow.Cells[3].Value.ToString();
            }
            catch (Exception)
            {

            }
        }

        private void Query_Insert()
        {
            try
            {
                DB.ConnectDB();
                string sqlcommand = "Insert into Book_Manager (Isbn, Name, Publisher, Page, isBorrowed) values (@p1, @p2, @p3, @p4, @p5)";
                SqlCommand cmd = new SqlCommand();

                cmd.Connection = DB.conn;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@p1", textBox_isbn.Text);
                cmd.Parameters.AddWithValue("@p2", textBox_bookName.Text);
                cmd.Parameters.AddWithValue("@p3", textBox_publisher.Text);
                cmd.Parameters.AddWithValue("@p4", textBox_page.Text);
                Book book = new Book();
                cmd.Parameters.AddWithValue("@p5", book.isBorrowed);
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

        private void button_add_Click(object sender, EventArgs e)
        {
            if (DB.Books.Exists((x) => x.Isbn == textBox_isbn.Text))
            {
                MessageBox.Show("이미 존재하는 도서입니다.");

                TextFile.BooksHistory("이미 존재하는 도서", "추가");
            }
            else if (textBox_bookName.Text.Trim() == "")
            {
                MessageBox.Show("책의 제목을 입력해주세요.");

                TextFile.BooksHistory("제목 미입력", "추가");
            }
            else if (textBox_publisher.Text.Trim() == "")
            {
                MessageBox.Show("책의 출판사를 입력해주세요.");

                TextFile.BooksHistory("출판사 미입력", "추가");
            }
            else if (textBox_page.Text.Trim() == "")
            {
                MessageBox.Show("책의 페이지를 입력해주세요.");

                TextFile.BooksHistory("페이지 미입력", "추가");
            }
            else
            {
                try
                {
                    Query_Insert();

                    DB.SelectDB();
                    dataGridView_book.DataSource = null;
                    dataGridView_book.DataSource = DB.Books;

                    MessageBox.Show($"\"{textBox_bookName.Text}\" 도서가 추가되었습니다.");

                    TextFile.BooksHistory($"{textBox_bookName.Text}", "추가");
                }
                catch (Exception)
                {
                    MessageBox.Show("예기치 못한 오류가 발생하였습니다. 다시 시도해주세요.");

                    TextFile.BooksHistory("예기치 못한 오류 발생", "추가");
                }
            }
        }

        private void Query_Modify()
        {
            try
            {
                DB.ConnectDB();
                string sqlcommand = "update Book_Manager set Name = @p1, Publisher = @p2, Page = @p3 where Isbn = @p4";
                SqlCommand cmd = new SqlCommand();

                cmd.Connection = DB.conn;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@p1", textBox_bookName.Text);
                cmd.Parameters.AddWithValue("@p2", textBox_publisher.Text);
                cmd.Parameters.AddWithValue("@p3", textBox_page.Text);
                cmd.Parameters.AddWithValue("@p4", textBox_isbn.Text);
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

        private void button_modify_Click(object sender, EventArgs e)
        {
            if (textBox_isbn.Text.Trim() == "")
            {
                MessageBox.Show("Isbn을 입력해주세요.");

                TextFile.BooksHistory("Isbn 미입력", "수정");
            }
            else if (textBox_bookName.Text.Trim() == "")
            {
                MessageBox.Show("책의 제목을 입력해주세요.");

                TextFile.BooksHistory("제목 미입력", "수정");
            }
            else if (textBox_publisher.Text.Trim() == "")
            {
                MessageBox.Show("책의 출판사를 입력해주세요.");

                TextFile.BooksHistory("출판사 미입력", "수정");
            }
            else if (textBox_page.Text.Trim() == "")
            {
                MessageBox.Show("책의 페이지를 입력해주세요.");

                TextFile.BooksHistory("페이지 미입력", "수정");
            }
            else
            {
                try
                {
                    Query_Modify();

                    DB.SelectDB();
                    dataGridView_book.DataSource = null;
                    dataGridView_book.DataSource = DB.Books;

                    MessageBox.Show($"\"{textBox_bookName.Text}\" 도서가 수정되었습니다.");

                    TextFile.BooksHistory($"{textBox_bookName.Text}", "수정");
                }
                catch (Exception)
                {
                    MessageBox.Show("존재하지 않는 도서입니다.");

                    TextFile.BooksHistory("존재하지 않는 도서", "수정");
                }
            }
        }

        private void Query_Delete()
        {
            try
            {
                DB.ConnectDB();
                string sqlcommand = "delete from Book_Manager where Isbn = @p1";
                SqlCommand cmd = new SqlCommand();

                cmd.Connection = DB.conn;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@p1", textBox_isbn.Text);
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

        private void button_delete_Click(object sender, EventArgs e)
        {
            if (textBox_isbn.Text.Trim() == "")
            {
                MessageBox.Show("Isbn을 입력해주세요.");

                TextFile.BooksHistory("Isbn 미입력", "삭제");
            }
            else
            {
                try
                {
                    Book book = DB.Books.Single((x) => x.Isbn == textBox_isbn.Text);

                    Query_Delete();

                    DB.SelectDB();
                    dataGridView_book.DataSource = null;
                    dataGridView_book.DataSource = DB.Books;

                    MessageBox.Show($"\"{book.Name}\" 도서가 삭제되었습니다.");

                    TextFile.BooksHistory($"{book.Name}", "삭제");
                }
                catch (Exception)
                {
                    MessageBox.Show("존재하지 않는 도서입니다.");

                    TextFile.BooksHistory("존재하지 않는 도서", "삭제");
                }
            }
        }

        private void textBox_Page_KeyPress(object sender, KeyPressEventArgs e)
        {
            //숫자만 입력되도록 필터링
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))    //숫자와 백스페이스를 제외한 나머지를 바로 처리
            {
                e.Handled = true;
            }
        }
    }
}
