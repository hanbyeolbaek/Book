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
        static string xmlFileBooks = "./Books.xml";
        static string xmlFileUsers = "./Users.xml";

        static DataManager()
        {
            Load();
        }

        public static void Load()
        {
            try
            {
                
                
                FileInfo fileInfo = new FileInfo(xmlFileBooks);
                if (fileInfo.Exists)
                {
                    string booksOutput = File.ReadAllText(@"./Books.xml");
                    XElement booksXElement = XElement.Parse(booksOutput);
                    Books = (from item in booksXElement.Descendants("book")
                             select new Book()
                             {
                                 Isbn = item.Element("isbn").Value,
                                 Name = item.Element("name").Value,
                                 Publisher = item.Element("publisher").Value,
                                 Page = int.Parse(item.Element("page").Value),
                                 BorrowedAt = DateTime.Parse(item.Element("borrowedAt").Value),
                                 isBorrowed = item.Element("isBorrowed").Value != "0" ? true : false,
                                 UserId = int.Parse(item.Element("userId").Value),
                                 UserName = item.Element("userName").Value
                             }).ToList<Book>();
                }
                else
                {
                    BooksCreateFile();
                    Save();
                    Load();
                }

                fileInfo = new FileInfo(xmlFileUsers);
                if (fileInfo.Exists)
                {
                    string usersOutput = File.ReadAllText(@"./Users.xml");
                    XElement usersXElement = XElement.Parse(usersOutput);
                    Users = (from item in usersXElement.Descendants("user")
                             select new User()
                             {
                                 Id = int.Parse(item.Element("id").Value),
                                 Name = item.Element("name").Value
                             }).ToList<User>();
                }
                else
                {
                    UsersCreateFile();
                    Save();
                    Load();
                }
            }
            catch(FileLoadException e)
            {
                MessageBox.Show(e.ToString());
                Save();
            }
        }

        public static void BooksCreateFile()
        {
            string booksFileName = @xmlFileBooks;
            StreamWriter textWrite = File.CreateText(booksFileName); //파일이 없으면 자동으로 해당 파일을 생성한다.
            textWrite.Dispose(); //메모리 해제. using 키워드로도 할 수 있으나 파일생성만 할 것이므로 별 차이 없다. 로그 남길 때 using 키워드 이용함.
        }
        public static void UsersCreateFile()
        {
            string usersFileName = @xmlFileUsers;
            StreamWriter textWrite = File.CreateText(usersFileName); //파일이 없으면 자동으로 해당 파일을 생성한다.
            textWrite.Dispose(); //메모리 해제. using 키워드로도 할 수 있으나 파일생성만 할 것이므로 별 차이 없다. 로그 남길 때 using 키워드 이용함.
        }

        public static void Save()
        {
            string booksOutput = "";
            booksOutput += "<books>\n";
            foreach (var item in Books)
            {
                booksOutput += "<book>\n";
                booksOutput = booksOutput + " <isbn>" + item.Isbn + "</isbn>\n";
                booksOutput += " <name>" + item.Name + "</name>\n";
                booksOutput += " <publisher>" + item.Publisher + "</publisher>\n";
                booksOutput += " <page>" + item.Page + "</page>\n";
                booksOutput += " <borrowedAt>" + item.BorrowedAt + "</borrowedAt>\n";
                booksOutput += " <isBorrowed>" + (item.isBorrowed ? 1 : 0) + "</isBorrowed>\n";
                booksOutput += " <userId>" + item.UserId + "</userId>\n";
                booksOutput += " <userName>" + item.UserName + "</userName>\n";
                booksOutput += "</book>\n";
            }
            booksOutput += "</books>";

            string usersOutput = "";
            usersOutput += "<users>\n";
            foreach(var item in Users)
            {
                usersOutput += "<user>\n";
                usersOutput += " <id>" + item.Id + "</id>\n";
                usersOutput += " <name>" + item.Name + "</name>\n";
                usersOutput += "</user>\n";
            }
            usersOutput += "</users>";

            File.WriteAllText(@"./Books.xml", booksOutput);
            File.WriteAllText(@"./Users.xml", usersOutput);
        }


    }
}
