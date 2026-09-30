using System;
using System.Threading;

// Task 3: Fix the race with a critical section (lock).
class Program
{
    const int NumThreads = 4;
    const int IncrementsPerThread = 1_000_000;

    static long counter = 0;
    static readonly object counterLock = new object(); // ONE lock object shared by all threads

    static void Worker()
    {
        for (int i = 0; i < IncrementsPerThread; i++)
        {
            lock (counterLock) // only the update is inside the critical section
            {
                counter++;
            }
        }
    }

    static void Main()
    {
        Thread[] threads = new Thread[NumThreads];

        for (int i = 0; i < NumThreads; i++)
        {
            threads[i] = new Thread(Worker);
            threads[i].Start();
        }

        for (int i = 0; i < NumThreads; i++)
        {
            threads[i].Join();
        }

        long expected = (long)NumThreads * IncrementsPerThread;
        Console.WriteLine($"Expected: {expected}");
        Console.WriteLine($"Actual: {counter}");
        Console.WriteLine($"Correct: {(counter == expected ? "Y" : "N")}");
    }
}
