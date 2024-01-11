// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Security.Cryptography;
using Thread = System.Threading.Thread;


var sw = Stopwatch.StartNew();

// Wait for the host process to apply the JobObject before starting new threads.
await Task.Delay(1000);

Console.WriteLine("ConsoleApp2 started");
Console.WriteLine($"Environment.ProcessorCount: {Environment.ProcessorCount}");

// await RunOnThreadPoolAsync(200, 10000);
RunOnThreads(200, 10000);

Console.WriteLine("Terminated");

void Work(CancellationToken cancellationToken)
{
    Console.WriteLine($"{sw.Elapsed} #: {Thread.GetCurrentProcessorId()} ThreadPool: {Thread.CurrentThread.IsThreadPoolThread}");
    
    while (!cancellationToken.IsCancellationRequested)
    {
        var i = RandomNumberGenerator.GetHexString(256);

        if (i == "")
        {
            throw new Exception();
        }
    }
}

async Task RunOnThreadPoolAsync(int count, int milliseconds)
{
    var tasks = new List<Task>();

    for (var i = 0; i < count; i++)
    {
        var t = Task.Factory.StartNew(() =>
        {
            var cancellationToken = new CancellationTokenSource(milliseconds).Token;
            Work(cancellationToken);
        });

        await Task.Delay(500);
        tasks.Add(t);
    }

    Task.WaitAll(tasks.ToArray());
}

void RunOnThreads(int count, int milliseconds)
{
    var threads = new List<Thread>();

    for (var i = 0; i < count; i++)
    {
        var t = new Thread(() =>
        {
            var cancellationToken = new CancellationTokenSource(milliseconds).Token;
            Work(cancellationToken);
        });

        t.Start();
        Thread.Sleep(500);
        threads.Add(t);
    }

    foreach (var t in threads)
    {
        t.Join();
    }
}
