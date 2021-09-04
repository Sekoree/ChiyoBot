using System;
using System.Threading.Tasks;

namespace ChiyoBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var b = new Bot();
            await b.RunAsync();
            Console.ReadLine();
            await b.StopAsync();
        }
    }
}
