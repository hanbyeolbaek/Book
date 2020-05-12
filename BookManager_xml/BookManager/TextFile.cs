using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManager
{
    class TextFile
    {
        public static void ManageHistory(string str, string btn)
        {
            DirectoryInfo di = new DirectoryInfo(@"./");
            if (!di.Exists)
            {
                di.Create();
            }
            using (StreamWriter writer = new StreamWriter(@"./ManageHistroy.txt", true))
            {
                writer.WriteLine($"[{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss")}] '{str}' {btn} 클릭");
            }
        }

        public static void BooksHistory(string str, string btn)
        {
            DirectoryInfo di = new DirectoryInfo(@"./");
            if (!di.Exists)
            {
                di.Create();
            }
            using (StreamWriter writer = new StreamWriter(@"./BooksHistory.txt", true))
            {
                writer.WriteLine($"[{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss")}] '{str}' {btn} 클릭");
            }
        }

        public static void UsersHistory(string str, string btn)
        {
            DirectoryInfo di = new DirectoryInfo(@"./");
            if (!di.Exists)
            {
                di.Create();
            }
            using (StreamWriter writer = new StreamWriter(@"./UsersHistory.txt", true))
            {
                writer.WriteLine($"[{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss")}] '{str}' {btn} 클릭");
            }
        }
    }
}
