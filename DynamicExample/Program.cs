using System;
using System.Threading.Tasks;
using SimpleExec;

namespace DynamicExample
{
    class Program
    {
        static void Main(string[] args) {
            dynamic sh = new Shell();
            sh.ping("cabal");
            Console.WriteLine(sh.ssh("kane@cabal","--","df -h"));
        }
        /*
        static async Task Main(string[] args)
        {
            SyncExample();
            dynamic sh = new Shell(async: true);
            Console.WriteLine("Hello World!");
            var pingTask = sh.ping("example.com");
            var ipconfTask = sh.ipconfig("/all");
            string[] outputs = await Task.WhenAll(pingTask,ipconfTask);
            foreach(var stdout in outputs) {
                Console.WriteLine("== Task ==");
                Console.WriteLine(stdout);
                Console.WriteLine("==========");
            }
            await sh.powershell("-Command","Start-Process https://example.com");
        }*/
    }
}
