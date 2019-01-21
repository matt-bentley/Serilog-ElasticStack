using Logging.Core;
using Logging.Core.Models;
using Serilog;
using Serilog.Sinks.Http.BatchFormatters;
using System;
using System.Threading;

namespace SerilogPublisher
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.WriteDiagnostic(GetLogDetail("Starting application"));

            for(int i = 0; i < 100; i++)
            {               
                ProcessAppCycle(i);                
                Thread.Sleep(50);
            }

            Logger.WriteDiagnostic(GetLogDetail("Stopping application"));

            Thread.Sleep(1000);
        }

        private static void ProcessAppCycle(int i)
        {
            var tracker = GetPerformanceTracker("ProcessAppCycle");           

            if (i % 10 == 0)
            {
                try
                {
                    throw new Exception($"There was an error processing {i}");
                }
                catch (Exception ex)
                {
                    Logger.WriteError(GetLogDetail($"Error processing cycle {i}", ex));
                }
            }
            else
            {
                Logger.WriteUsage(GetLogDetail($"Processed cycle {i}"));
            }

            tracker.Stop();
        }

        private static PerformanceTracker GetPerformanceTracker(string message)
        {
            return new PerformanceTracker(message, "", Environment.UserName, "Console","SerilogPublisher","Job", "Development");
        }

        private static LogDetail GetLogDetail(string message, Exception ex = null)
        {
            return new LogDetail
            {
                Product = "SerilogPublisher",
                Location = "Console",    // this application
                Layer = "Job",                  // unattended executable invoked somehow
                Environment = "Developement",
                UserName = Environment.UserName,
                Hostname = Environment.MachineName,
                Message = message,
                Exception = ex
            };
        }
    }
}
