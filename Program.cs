using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sample05
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().Run();
        }
        public void Run()
        {
            Task task1 = null, task2 = null, task3 = null;
            try
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                CancellationToken ct = cts.Token;
                Console.WriteLine("Start Run\n");
                task1 = Task.Factory.StartNew(Do01, ct, ct);
                task2 = Task.Factory.StartNew(Do02, ct, ct);
                task3 = Task.Factory.StartNew(Do03, ct, ct);
                Thread.Sleep(2000);
                Console.WriteLine("\nInvoke Cancel.");
                cts.Cancel();
                Console.WriteLine("WaitAll.");
                Task.WaitAll(task1, task2, task3);
            }
            catch (AggregateException aex)
            {
                Console.WriteLine("\nAggregateException in Run: " + aex.Message);
                aex.Flatten();
                foreach (Exception ex in aex.InnerExceptions)
                    Console.WriteLine("  Exception: " + ex.Message);
            }
            finally
            {
                Console.WriteLine("\nStatus task1: " + task1.Status);
                Console.WriteLine("Status task2: " + task2.Status);
                Console.WriteLine("Status task3: " + task3.Status);
                Console.WriteLine("\nEnd Run");
                Console.ReadLine();
            }
        }
        private void Do01(Object ct)
        {
            Console.WriteLine("Start Do01");
            try
            {
                Thread.Sleep(1000);  // doing some work
                int a = 1;
                a = a / --a; // create an exception.
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nException Do01:\n" + ex);
                throw;
            }
            finally
            {
                Console.WriteLine("\nEnd Do01");
            }
        }
        private void Do02(Object ct)
        {
            Console.WriteLine("Start Do02");
            try
            {
                for (int i = 1; i < 30; i++)
                {
                    ((CancellationToken)ct).ThrowIfCancellationRequested();
                    Thread.Sleep(100); // doing some work
                }
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine("\nOperationCanceledException Do02:\n" + ex);
                throw;
                // or: throw new OperationCanceledException("Message", ex, (CancellationToken)ct);
            }
            finally
            {
                Console.WriteLine("\nEnd Do02");
            }
        }
        private void Do03(Object ct)
        {
            Console.WriteLine("Start Do03");
            try
            {
                Thread.Sleep(100);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nException Do03:\n" + ex);
                throw;
            }
            finally
            {
                Console.WriteLine("\nEnd Do03");
            }
        }
    }
}
