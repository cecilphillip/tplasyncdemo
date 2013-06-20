using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TPL.ExceptionHandling
{
    class Program
    {
        static void Main(string[] args)
        {
            UsingUnobservedTaskException();
            Console.WriteLine("Done");
            Console.Read();
            GC.Collect();
            Thread.Sleep(2000);
        }

        #region

        public static void TasksWithException()
        {
            int x = 0;
            Task<int> calc = Task.Factory.StartNew(() => 7 / x);

            try
            {
                Console.WriteLine(calc.Result);
            }
            catch (AggregateException aex)
            {
                Console.Write(aex.InnerException.Message);
            }
        }

        public static void TaskChildrenWithException()
        {
            try
            {
                var parent = Task.Factory.StartNew(() =>
                    {
                        Task.Factory.StartNew(() => // Child
                            {
                                Task.Factory.StartNew(() =>
                                    {
                                        throw null;
                                    }, TaskCreationOptions.AttachedToParent); // Grandchild

                            }, TaskCreationOptions.AttachedToParent);
                    });

                parent.Wait();
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static void TasksWithDifferentExceptions()
        {
            var parent = Task.Factory.StartNew(() =>
            {
                int[] numbers = { 0 };
                var childFactory = new TaskFactory(TaskCreationOptions.AttachedToParent, TaskContinuationOptions.None);
                childFactory.StartNew(() => 5 / numbers[0]); // Division by zero
                childFactory.StartNew(() => numbers[1]); // Index out of range
                childFactory.StartNew(() => { throw null; }); // Null reference
            });

            try
            {
                parent.Wait();
            }
            catch (AggregateException aex)
            {
                aex.Flatten().Handle(ex =>
                {
                    if (ex is DivideByZeroException)
                    {
                        Console.WriteLine("Divide by zero");
                        return true; // This exception is "handled"
                    }
                    if (ex is IndexOutOfRangeException)
                    {
                        Console.WriteLine("Index out of range");
                        return true; // This exception is "handled"
                    }

                    // All other exceptions will get rethrown
                    return false;
                });
            }
        }


        public static void UsingUnobservedTaskException()
        {
            //Run in release w/o debugger
            TaskScheduler.UnobservedTaskException += (sender, args) =>
                {
                    Console.WriteLine("Moving along. Nothing to see here");
                    // Prevents .NET from rethrowing the application
                    args.SetObserved();
                };

            var t = Task.Factory.StartNew(() =>
            {
                throw new Exception("ha!");
            });

        }
        #endregion
    }
}
