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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            Text = "도서 관리";

            dataGridView_book.DataSource = DataManager.Books;
            
        }

        private void button_add_Click(object sender, EventArgs e)
        {
            try
            {
                if(DataManager.Books.Exists((x) => x.Isbn == textBox_isbn.Text))
                {
                    MessageBox.Show("이미 존재하는 도서입니다.");

                    TextFile.BooksHistory("이미 존재하는 도서", "추가");
                }
                else if(textBox_bookName.Text.Trim() == "")
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
                    Book book = new Book()
                    {
                        Isbn = textBox_isbn.Text,
                        Name = textBox_bookName.Text,
                        Publisher = textBox_publisher.Text,
                        Page = int.Parse(textBox_page.Text)
                    };
                    DataManager.Books.Add(book);

                    dataGridView_book.DataSource = null;
                    dataGridView_book.DataSource = DataManager.Books;
                    DataManager.Save();

                    MessageBox.Show($"\"{book.Name}\" 도서가 추가되었습니다.");

                    TextFile.BooksHistory($"{book.Name}", "추가");
                }
            }
            catch(Exception)
            {

            }
        }

        private void button_modify_Click(object sender, EventArgs e)
        {
            try
            {
                Book book = DataManager.Books.Single((x) => x.Isbn == textBox_isbn.Text);
                if (textBox_bookName.Text.Trim() == "")
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
                    book.Name = textBox_bookName.Text;
                    book.Publisher = textBox_publisher.Text;
                    book.Page = int.Parse(textBox_page.Text);

                    dataGridView_book.DataSource = null;
                    dataGridView_book.DataSource = DataManager.Books;
                    DataManager.Save();

                    MessageBox.Show($"\"{book.Name}\" 도서가 수정되었습니다.");

                    TextFile.BooksHistory($"{book.Name}", "수정");
                }
            }
            catch(Exception)
            {
                MessageBox.Show("존재하지 않는 도서입니다.");

                TextFile.BooksHistory("존재하지 않는 도서", "수정");
            }

        }

        private void button_delete_Click(object sender, EventArgs e)
        {
            try
            {
                Book book = DataManager.Books.Single((x) => x.Isbn == textBox_isbn.Text);
                DataManager.Books.Remove(book);

                MessageBox.Show($"\"{book.Name}\" 도서가 삭제되었습니다.");

                TextFile.BooksHistory($"{book.Name}", "삭제");

                dataGridView_book.DataSource = null;
                dataGridView_book.DataSource = DataManager.Books;
                DataManager.Save();
            }
            catch(Exception)
            {
                MessageBox.Show("존재하지 않는 도서입니다.");

                TextFile.BooksHistory("존재하지 않는 도서", "삭제");
            }
        }

        private void dataGridView_book_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                Book book = dataGridView_book.CurrentRow.DataBoundItem as Book;
                textBox_isbn.Text = book.Isbn;
                textBox_bookName.Text = book.Name;
                textBox_publisher.Text = book.Publisher;
                textBox_page.Text = book.Page.ToString();
            }
            catch(Exception)
            {

            }

        }

        private void textBox_page_KeyPress(object sender, KeyPressEventArgs e)
        {
            //숫자만 입력되도록 필터링
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))    //숫자와 백스페이스를 제외한 나머지를 바로 처리
            {
                e.Handled = true;
            }
        }
    }
}
