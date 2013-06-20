using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AsyncAwait.Fundamentals
{
   public class Program
    {
        static void Main(string[] args)
        {
            Task task = AsyncRead();
            task.Wait();
            Console.Read();
        }

        public static async Task AsyncDownload()
        {
            try
            {
                string url = "http://www.google.com";
                var content = await new WebClient().DownloadStringTaskAsync(url);
                Console.WriteLine(content);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static async Task AsyncRead()
        {
            using (StreamReader reader = File.OpenText("lorem.txt"))
            {
                Console.WriteLine("File opened");
                string result = await reader.ReadLineAsync();
                Console.WriteLine("First line contains: " + result);
            }
        }
    }
}
