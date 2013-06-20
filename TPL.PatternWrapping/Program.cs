using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TPL.PatternWrapping
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WrapEAP();
            Console.WriteLine("Done");
            Console.Read();
        }

        public static void WrapAPM()
        {
            string path = "lorem.txt";
            FileInfo fi = new FileInfo(path);
            byte[] data = new byte[fi.Length];


            var fileStream = File.Open(path, FileMode.Open);

            Task<int> op = Task<int>.Factory.FromAsync(fileStream.BeginRead, fileStream.EndRead, data, 0, data.Length, null);
            op.ContinueWith((Task<int> prev) =>
            {
                fileStream.Close();
                if (prev.Result > 0)
                {
                    var encoding = new UTF8Encoding();
                    Console.WriteLine(encoding.GetString(data));
                }
            });
        }

        public static void WrapEAP()
        {
            using (var client = new WebClient())
            {
                client.DownloadStringTaskAsync("http://dotnetmiami.com")
                .ContinueWith(t =>
                {
                    if (!t.IsFaulted)
                    {
                        Console.WriteLine(t.Result);
                    }
                });
            }

        }
    }

    public static class WebClientEx
    {
        public static Task<string> DownloadStringTaskAsync(this WebClient client, string url)
        {
            var tcs = new TaskCompletionSource<string>();
            DownloadStringCompletedEventHandler h = null;
            h = (source, args) =>
                {
                    client.DownloadStringCompleted -= h;
                    if (args.Cancelled)
                    {
                        tcs.SetCanceled();
                    }
                    else if (args.Error != null)
                    {
                        tcs.SetException(args.Error);
                    }
                    else
                    {
                        tcs.SetResult(args.Result);
                    }
                };

            client.DownloadStringCompleted += h;
            client.DownloadStringAsync(new Uri(url));
            return tcs.Task;
        }
    }
}
