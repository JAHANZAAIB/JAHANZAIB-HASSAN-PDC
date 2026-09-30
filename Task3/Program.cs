using System;
using System.Threading;

class Program
{
    static void Worker()
    {
        Thread.Sleep(200);
    }

    static void Main()
    {
        Thread t = new Thread(Worker);

        Console.WriteLine($"After creation: {t.ThreadState}");
        // Conceptually: New

        t.Start();

        Console.WriteLine($"Immediately after Start(): {t.ThreadState}");
        // Conceptually: Runnable / Ready or Running

        Thread.Sleep(50);

        Console.WriteLine($"While worker is sleeping: {t.ThreadState}");
        // Conceptually: Blocked / Waiting

        t.Join();

        Console.WriteLine($"After Join() completes: {t.ThreadState}");
        // Conceptually: Terminated
    }
}