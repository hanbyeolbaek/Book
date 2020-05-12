using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace BookManager
{
    class DataManager
    {
        public static List<Book> Books = new List<Book>();
        public static List<User> Users = new List<User>();
        static string jsonFileBooks = "./Books.json";
        static string jsonFileUsers = "./Users.json";

        static DataManager()
        {
            JsonLoad();
        }

        public static void JsonLoad()
        {
            FileInfo fileInfo = new FileInfo(jsonFileBooks);
            if (fileInfo.Exists)
            {
                string stBookValueJson = File.ReadAllText(@jsonFileBooks);
                JObject jsonObjectBook = JObject.Parse(stBookValueJson);
                Books = (from item in jsonObjectBook["books"]["book"]
                         select new Book()
                         {
                             Isbn = item["isbn"].ToString(),
                             Name = item["name"].ToString(),
                             Publisher = item["publisher"].ToString(),
                             Page = int.Parse(item["page"].ToString()),
                             BorrowedAt = DateTime.Parse(item["borrowedAt"].ToString()),
                             isBorrowed = item["isBorrowed"].ToString() == "1" ? true : false,
                             UserId = int.Parse(item["userId"].ToString()),
                             UserName = item["userName"].ToString()
                         }).ToList<Book>();
            }
            else
            {
                BooksCreateFile();
                SaveJson();
                JsonLoad();
            }

            fileInfo = new FileInfo(jsonFileUsers);
            if (fileInfo.Exists)
            {
                string stUserValueJson = File.ReadAllText(@jsonFileUsers);
                JObject jsonObjectUser = JObject.Parse(stUserValueJson);
                Users = (from item in jsonObjectUser["users"]["user"]
                         select new User()
                         {
                             Id = int.Parse(item["id"].ToString()),
                             Name = item["name"].ToString()
                         }).ToList<User>();
            }
            else
            {
                UsersCreateFile();
                SaveJson();
                JsonLoad();
            }
        }

        public static void BooksCreateFile()
        {
            string booksFileName = @jsonFileBooks;
            StreamWriter textWrite = File.CreateText(booksFileName);
            textWrite.Dispose();
        }

        public static void UsersCreateFile()
        {
            string usersFileName = jsonFileUsers;
            StreamWriter textWrite = File.CreateText(usersFileName);
            textWrite.Dispose();
        }

        public static void SaveJson()
        {
            var jBoookArray = new JArray();
            foreach(var item in Books)
            {
                var jBookobject = new JObject();
                jBookobject.Add("isbn", item.Isbn);
                jBookobject.Add("name", item.Name);
                jBookobject.Add("publisher", item.Publisher);
                jBookobject.Add("page", item.Page);
                jBookobject.Add("borrowedAt", item.BorrowedAt.ToLongDateString());
                jBookobject.Add("isBorrowed", item.isBorrowed);
                jBookobject.Add("userId", item.UserId);
                jBookobject.Add("userName", item.UserName);
                jBoookArray.Add(jBookobject);
            }

            var jBookArrayObject = new JObject();
            jBookArrayObject.Add("book", jBoookArray);

            var jBooksObject = new JObject();
            jBooksObject.Add("books", jBookArrayObject);

            var jUserArray = new JArray();
            foreach(var item in Users)
            {
                var jUserObject = new JObject();
                jUserObject.Add("id", item.Id);
                jUserObject.Add("name", item.Name);
                jUserArray.Add(jUserObject);
            }

            var jUserArrayObject = new JObject();
            jUserArrayObject.Add("user", jUserArray);

            var jUsersObject = new JObject();
            jUsersObject.Add("users", jUserArrayObject);

            //저장
            File.WriteAllText(@"./Books.json", jBooksObject.ToString());
            File.WriteAllText(@"./Users.json", jUsersObject.ToString());
        }
    }
}
