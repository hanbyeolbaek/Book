﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

            //전체 도서 수
            label_allBookCount.Text = DataManager.Books.Count.ToString();
            //사용자 수
            label_allUserCount.Text = DataManager.Users.Count.ToString();
            //대출중인 도서의 수
            label_allBorrowedBook.Text = DataManager.Books.Where((x) => x.isBorrowed).Count().ToString();
            //연체중인 도서의 수
            label_allDelayedBook.Text = DataManager.Books.Where((x) =>
            {
                return x.isBorrowed && x.BorrowedAt.AddDays(7) < DateTime.Now;
            }).Count().ToString();

            //데이터 그리드 설정
            dataGridView_BookManager.DataSource = DataManager.Books;
            dataGridView_UserManager.DataSource = DataManager.Users;
            dataGridView_BookManager.CurrentCellChanged += DataGridView_BookManager_CurrentCellChanged;
        }

        private void DataGridView_BookManager_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                Book book = dataGridView_BookManager.CurrentRow.DataBoundItem as Book;
                //Book book2 = (Book)dataGridView_BookManager.CurrentRow.DataBoundItem;
                textBox_isbn.Text = book.Isbn;
                textBox_bookName.Text = book.Name;
            }
            catch(Exception)
            {

            }
        }

        private void dataGridView_UserManager_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                User user = dataGridView_UserManager.CurrentRow.DataBoundItem as User;
                textBox_id.Text = user.Id.ToString();
            }
            catch(Exception)
            {

            }
        }
        private void button_Borrow_Click(object sender, EventArgs e)
        {
            //문자열 앞뒤 공백 제거, 데이터 중복성 제거
            if(textBox_isbn.Text.Trim() == "")
            {
                MessageBox.Show("Isbn을 입력해주세요.");

                TextFile.ManageHistory("Isbn 미입력", "대여");
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
                    Book book = DataManager.Books.Single((x) => x.Isbn == textBox_isbn.Text);
                    if(book.isBorrowed)
                    {
                        MessageBox.Show("이미 대여 중인 도서입니다.");

                        TextFile.ManageHistory("대여된 도서", "대여");
                    }
                    else
                    {
                        User user = DataManager.Users.Single((x) => x.Id.ToString() == textBox_id.Text);
                        book.UserId = user.Id;
                        book.UserName = user.Name;
                        book.isBorrowed = true;
                        book.BorrowedAt = DateTime.Now;

                        dataGridView_BookManager.DataSource = null;
                        dataGridView_BookManager.DataSource = DataManager.Books;
                        DataManager.Save();
                        
                        MessageBox.Show("\"" + book.Name + "\"이/가\"" + user.Name + "\"님께 대여되었습니다.");

                        TextFile.ManageHistory($"{book.Name}' '{user.Name}", "대여");

                        //대출중인 도서의 수
                        label_allBorrowedBook.Text = DataManager.Books.Where((x) => x.isBorrowed).Count().ToString();
                    }
                }
                catch(Exception)
                {
                    MessageBox.Show("존재하지 않는 도서 또는 사용자입니다.");

                    TextFile.ManageHistory("존재하지 않는 도서", "대여");
                }
            }
        }

        private void button_Return_Click(object sender, EventArgs e)
        {
            if(textBox_isbn.Text.Trim() == "")
            {
                MessageBox.Show("Isbn을 입력해주세요.");

                TextFile.ManageHistory("Isbn 미입력", "반납");
            }
            else
            {
                try
                {
                    Book book = DataManager.Books.Single((x) => x.Isbn == textBox_isbn.Text);
                    if(book.isBorrowed)
                    {
                        DateTime oldDay = book.BorrowedAt;
                        book.UserId = 0;
                        book.UserName = "";
                        book.isBorrowed = false;
                        book.BorrowedAt = new DateTime();

                        dataGridView_BookManager.DataSource = null;
                        dataGridView_BookManager.DataSource = DataManager.Books;
                        DataManager.Save();

                        TimeSpan timeDiff = DateTime.Now - oldDay;
                        int diffDays = timeDiff.Days;

                        if (diffDays > 7)
                        {
                            MessageBox.Show("\"" + book.Name + "\"이/가 연체 상태로 반납되었습니다.");

                            TextFile.ManageHistory($"{book.Name}'", "연체 반납");

                            //대출중인 도서의 수
                            label_allBorrowedBook.Text = DataManager.Books.Where((x) => x.isBorrowed).Count().ToString();
                            //연체중인 도서의 수
                            label_allDelayedBook.Text = DataManager.Books.Where((x) =>
                            {
                                return x.isBorrowed && x.BorrowedAt.AddDays(7) < DateTime.Now;
                            }).Count().ToString();
                        }
                        else
                        {
                            MessageBox.Show("\"" + book.Name + "\"이/가 반납되었습니다.");

                            TextFile.ManageHistory($"{book.Name}'", "반납");

                            //대출중인 도서의 수
                            label_allBorrowedBook.Text = DataManager.Books.Where((x) => x.isBorrowed).Count().ToString();
                        }
                    }
                    else
                    {
                        MessageBox.Show("대여상태가 아닙니다.");

                        TextFile.ManageHistory("대여상태 아닌 도서", "반납");
                    }
                }
                catch(Exception)
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
            DataManager.Load();
            dataGridView_BookManager.DataSource = null;
            dataGridView_BookManager.DataSource = DataManager.Books;

            //전체 도서 수
            label_allBookCount.Text = DataManager.Books.Count.ToString();
        }

        private void 사용자관리ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Form3().ShowDialog();
            DataManager.Load();
            dataGridView_UserManager.DataSource = null;
            dataGridView_UserManager.DataSource = DataManager.Users;
            dataGridView_BookManager.DataSource = null;
            dataGridView_BookManager.DataSource = DataManager.Books;

            //사용자 수
            label_allUserCount.Text = DataManager.Users.Count.ToString();
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
