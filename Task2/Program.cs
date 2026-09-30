using System;
using System.Diagnostics;
using System.Threading;

class Program
{
    const int Iterations = 50;

    static void Main()
    {
        string processLabPath =
            @"D:\232517_Lab3_pdc\ProcessLab\bin\Release\net9.0\ProcessLab.exe";

        Stopwatch processStopwatch = Stopwatch.StartNew();

        for (int i = 0; i < Iterations; i++)
        {
            ProcessStartInfo info = new ProcessStartInfo();

            info.FileName = processLabPath;
            info.ArgumentList.Add("--empty");
            info.UseShellExecute = false;

            Process? process = Process.Start(info);

            if (process != null)
            {
                process.WaitForExit();
            }
        }

        processStopwatch.Stop();


        Stopwatch threadStopwatch = Stopwatch.StartNew();

        for (int i = 0; i < Iterations; i++)
        {
            Thread t = new Thread(() =>
            {
            });

            t.Start();

            t.Join();
        }

        threadStopwatch.Stop();


        double avgProcessMs =
            processStopwatch.Elapsed.TotalMilliseconds / Iterations;

        double avgThreadMs =
            threadStopwatch.Elapsed.TotalMilliseconds / Iterations;

        double ratio = avgProcessMs / avgThreadMs;


        Console.WriteLine(
            $"Average process creation time: {avgProcessMs:F3} ms"
        );

        Console.WriteLine(
            $"Average thread creation time: {avgThreadMs:F3} ms"
        );

        Console.WriteLine(
            $"Process creation was {ratio:F1}x more expensive than thread creation."
        );
    }
}