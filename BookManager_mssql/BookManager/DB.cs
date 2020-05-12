using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManager
{
    class DB
    {
        public static List<Book> Books = new List<Book>();
        public static List<User> Users = new List<User>();
        public static SqlConnection conn = new SqlConnection();

        public static void ConnectDB()
        {
            conn.ConnectionString = string.Format("Data Source=({0});" +
                "Initial Catalog = {1};" +
                "Integrated Security = {2};" +
                "Timeout = 3",
                "local", "MYDB1", "SSPI");
            conn = new SqlConnection(conn.ConnectionString);
            conn.Open();
        }

        public static void SelectDB()
        {
            ConnectDB();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = DB.conn;
            cmd.CommandText = "select * from Book_Manager order by Isbn";

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds, "Book_Manager");

            Books.Clear();
            foreach (DataRow item in ds.Tables[0].Rows)
            {
                Book book = new Book();
                book.Isbn = item["Isbn"].ToString();
                book.Name = item["Name"].ToString();
                book.Publisher = item["Publisher"].ToString();
                book.Page = int.Parse(item["Page"].ToString());
                //book.UserId = int.Parse(item["UserId"].ToString() == "" ? null : item["UserId"].ToString());
                if (item["UserId"].ToString() == "")
                    book.UserId = null;
                else
                    book.UserId = int.Parse(item["UserId"].ToString());
                book.UserName = item["UserName"].ToString();
                book.isBorrowed = bool.Parse(item["isBorrowed"].ToString());
                //book.BorrowedAt = DateTime.Parse(item["BorrowedAt"].ToString());
                if (item["BorrowedAt"].ToString() == "")
                    book.BorrowedAt = DateTime.MinValue;
                else
                    book.BorrowedAt = DateTime.Parse(item["BorrowedAt"].ToString());
                Books.Add(book);
            }

            cmd.CommandText = "";

            cmd.CommandText = "select * from User_Manager order by Id";

            ds = new DataSet();
            da.Fill(ds, "User_Manager");

            Users.Clear();
            foreach (DataRow item in ds.Tables[0].Rows)
            {
                User user = new User();
                user.Id = int.Parse(item["Id"].ToString());
                user.Name = item["Name"].ToString();
                Users.Add(user);
            }

            conn.Close();
        }
    }
}
