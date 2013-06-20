using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TPL.Basics
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ContinueWhenAllTasksDone();
            Console.WriteLine("Done");
            Console.Read();
        }
        #region Create Tasks
        public static void CreatingTasks()
        {
            Task op = new Task(DownloadWebSite);
            op.Start();
        }

        public static void DownloadWebSite()
        {
            using (var webclient = new WebClient())
            {
                string website = "http://www.google.com";
                var siteContent = webclient.DownloadString(website);
                Console.WriteLine(siteContent);
                Console.WriteLine(website);
            }
        }
        #endregion

        #region Create Tasks with State
        private static void CreatingTasksWithState()
        {
            Task op = Task.Factory.StartNew(DownloadWebSiteState, "http://www.bing.com");
        }
        public static void DownloadWebSiteState(object website)
        {
            using (var webclient = new WebClient())
            {
                var siteContent = webclient.DownloadString(website.ToString());
                Console.WriteLine(siteContent);
                Console.WriteLine(website.ToString());
            }
        }
        #endregion

        #region Child Tasks
        private static void CreateWithChildTasks()
        {
            Task parent = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("I am the parent Task");

                Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("I am a task within the parent");
                });

                Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("I am a child task");
                }, TaskCreationOptions.AttachedToParent);
            });
        }
        #endregion

        #region Waiting on Tasks

        public static void WaitForTasks()
        {
            Task op = Task.Factory.StartNew(() => Thread.Sleep(3000));
            op.Wait();
        }

        public static void WaitForTasksWithResult()
        {
            Task<string> op = Task.Factory.StartNew<string>(() =>
                {
                    Thread.Sleep(2000);
                    return "All Done";
                });
            //Accessing the Result property with explicitly wait on a Task
            Console.WriteLine(op.Result);
        }

        public static void WaitForMultipleTasks()
        {
            Task op = Task.Factory.StartNew(() => Thread.Sleep(2000));
            Task op2 = Task.Factory.StartNew(() => Thread.Sleep(5000));
            Task.WaitAll(op, op2);

            Console.WriteLine(op.Status);
        }

        #endregion

        #region Continutations

        public static void CreateTasksWithContinuations()
        {
            Task op = Task.Factory.StartNew<string>(() =>
                {
                    using (var fileStream = File.Open("lorem.txt", FileMode.Open))
                    {
                        using (var reader = new StreamReader(fileStream))
                        {
                            return reader.ReadToEnd();
                        }
                    }

                }).ContinueWith((Task<string> prev) =>
                    {
                        var content = prev.Result;
                        Console.WriteLine(content);
                    });
        }

        public static void CreateTasksWithContinuationOptions()
        {
            Task<string> op = Task.Factory.StartNew<string>(() =>
            {
                using (var fileStream = File.Open("lorem.tx", FileMode.Open))
                {
                    using (var reader = new StreamReader(fileStream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            });

            op.ContinueWith((prev) =>
            {
                var content = prev.Result;
                Console.WriteLine(content);
            }, TaskContinuationOptions.NotOnFaulted);

            op.ContinueWith(prev =>
            {
                Console.WriteLine("I think we have a problem: \n" + prev.Status);
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        #endregion

        #region Composition

        public static void ContinueWhenAllTasksDone()
        {
            Task<string> op = Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(2000);
                    return "Tired of waiting";
                });

            Task<string> op2 = Task.Factory.StartNew<string>(() =>
                {
                    using (var fileStream = File.Open("lorem.txt", FileMode.Open))
                    {
                        using (var reader = new StreamReader(fileStream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                });
            Task.Factory.ContinueWhenAll(new Task<string>[] { op, op2 }, tasks =>
                {
                    foreach (Task<string> task in tasks)
                    {
                        if (!task.IsFaulted)
                        {
                            Console.WriteLine(task.Result.Substring(0, 5));
                        }
                    }
                });
        }

        public static void ContinueWhenAnyTasksDone()
        {
            Task<string> op = Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(2000);
                    return "Tired of waiting";
                });

            Task<string> op2 = Task.Factory.StartNew<string>(() =>
                {
                    using (var fileStream = File.Open("lorem.txt", FileMode.Open))
                    {
                        using (var reader = new StreamReader(fileStream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                });
            Task.Factory.ContinueWhenAny(new[] { op, op2 }, task =>
                {
                    if (!task.IsFaulted)
                    {
                        Console.WriteLine(task.Result);
                    }
                });
        }
        #endregion

    }
}
