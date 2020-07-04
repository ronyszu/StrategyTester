using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Input;

namespace StrategyTester
{
    public static class Program
    {

        #region Nested classes to support running as service
        public const string ServiceName = "StrategyTesterService";

        public class Service : ServiceBase
        {
            public Service()
            {
                ServiceName = Program.ServiceName;
            }

            protected override void OnStart(string[] args)
            {
                Program.Start(args);
            }

            protected override void OnStop()
            {
                Program.Stop();
            }
        }
        #endregion


        static void Main(string[] args)
        {

            Console.WriteLine("Starting Stategy Tester");
            Console.WriteLine("Developed by Rony Szuster");


            if (!Environment.UserInteractive)
                // running as service
                using (var service = new Service())
                    ServiceBase.Run(service);
            else
            {
                // running as console app
                Start(args);

                Console.WriteLine("Press any key to stop...");
                Console.ReadKey(true);

                Stop();
            }
        }

        private static void Start(string[] args)
        {

            Console.WriteLine("Trying to open strategy file");

            //Perform desired changes in the file
            string text = File.ReadAllText(@"C:\Users\ronys\AppData\Roaming\MetaQuotes\Terminal\D0E8209F77C8CF37AD8BF550E51FF075\MQL5\Experts\DojiStarRígido.mq5");
            text = text.Replace("0.03", "0.05");
            File.WriteAllText(@"C:\Users\ronys\AppData\Roaming\MetaQuotes\Terminal\D0E8209F77C8CF37AD8BF550E51FF075\MQL5\Experts\DojiStarRígido.mq5", text);

            //Run MQL5 editor
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(@"C:\Users\ronys\AppData\Roaming\MetaQuotes\Terminal\D0E8209F77C8CF37AD8BF550E51FF075\MQL5\Experts\DojiStarRígido.mq5")
            {
               UseShellExecute = true
            };
            p.Start();

            //wait for EA to open up  TODO: improve this
            System.Threading.Thread.Sleep(12000);

            if (p.Responding){
                //Run Expert Advisor (manually) -> simulate keys CTRL + F5

                KeyboardInput.SendCTRLF5();

            }


            Console.WriteLine("Ran strategy file");
        }

        private static void Stop()
        {
            // onstop code here
        }




    }
}
