using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace SimdTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write(".NET Version\tPlatform\tMethod\tVector Size\t");
            for (int i = 1; i <= Constants.TestRunCount; i++)
                Console.Write("Run {0}\t", i);
            Console.WriteLine("Avg.");

            RunTestProcess("Net_4.6\\SimdTest.Net_4_6_x64.exe");
            RunTestProcess("Net_4.6\\SimdTest.Net_4_6_x86.exe");
            RunTestProcess("Net_4.5.2\\SimdTest.Net_4_5_2_x64.exe");
            RunTestProcess("Net_4.5.2\\SimdTest.Net_4_5_2_x86.exe");

            Console.ReadLine();
        }

        private static void RunTestProcess(string processPath)
        {
            var processStartInfo = new ProcessStartInfo(processPath)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };
            var process = new Process()
            {
                StartInfo = processStartInfo,
            };
            process.OutputDataReceived += Process_OutputDataReceived;
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
            process.CancelOutputRead();
        }

        private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
                Console.WriteLine(e.Data);    
        }
    }
}
