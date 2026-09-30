using System;
using System.Threading;

// Task 2: Trace every increment and analyse the log for collisions.
// Run without arguments for the racy version; "dotnet run -- --lock" for the fixed one (Task 3).
class Program
{
    const int NumThreads = 4;
    const int IncrementsPerThread = 250_000;

    static int counter = 0;                                   // shared state
    static readonly object gate = new object();               // used only with --lock
    static bool useLock = false;
    static int[][] seenLog = new int[NumThreads][];           // private log per thread

    static void Worker(int id)
    {
        int[] log = seenLog[id];                              // this thread's own log
        for (int i = 0; i < IncrementsPerThread; i++)
        {
            if (useLock)
            {
                lock (gate)
                {
                    int seen = counter;                       // (1) load
                    log[i] = seen;                            // private record of the load
                    counter = seen + 1;                       // (2) add and (3) store
                }
            }
            else
            {
                int seen = counter;                           // (1) load
                log[i] = seen;
                counter = seen + 1;                           // (2)+(3)
            }
        }
    }

    static void Main(string[] args)
    {
        useLock = args.Length > 0 && args[0] == "--lock";
        int total = NumThreads * IncrementsPerThread;

        Thread[] threads = new Thread[NumThreads];
        for (int t = 0; t < NumThreads; t++)
        {
            seenLog[t] = new int[IncrementsPerThread];
            int id = t;                                       // copy: each lambda needs its own id
            threads[t] = new Thread(() => Worker(id));
            threads[t].Start();
        }
        foreach (Thread th in threads) th.Join();

        // Analysis: how many increments loaded each value?
        int[] readCount = new int[total + 1];
        for (int t = 0; t < NumThreads; t++)
            for (int i = 0; i < IncrementsPerThread; i++)
                readCount[seenLog[t][i]]++;

        int collisions = 0;                                   // loads of a value already loaded
        for (int v = 0; v <= total; v++)
            if (readCount[v] > 1) collisions += readCount[v] - 1;

        string mode = useLock ? "with lock" : "no synchronization";
        Console.WriteLine($"Mode: {mode}");
        Console.WriteLine($"Total increments: {total}");
        Console.WriteLine($"Final counter: {counter}");
        Console.WriteLine($"Lost updates: {total - counter}");
        Console.WriteLine($"Collisions: {collisions}");

        // ---- Your task (completed): first 5 colliding values and who loaded them ----
        int shown = 0;
        for (int v = 0; v <= total && shown < 5; v++)
        {
            if (readCount[v] <= 1) continue;
            shown++;
            Console.WriteLine($"Value {v} was loaded {readCount[v]} times:");
            for (int t = 0; t < NumThreads; t++)
                for (int i = 0; i < IncrementsPerThread; i++)
                    if (seenLog[t][i] == v)
                        Console.WriteLine($"    thread {t}, loop index {i}");
        }
        if (shown == 0) Console.WriteLine("No colliding values found in this run.");
    }
}
