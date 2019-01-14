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
            ILogger logger = new LoggerConfiguration()
                .WriteTo.Http(
                    requestUri: "http://localhost:31312",
                    batchFormatter: new ArrayBatchFormatter())
                .WriteTo.Console()
                .CreateLogger()
                .ForContext<Program>();

            logger.Information("Starting Serilog Publisher...");

            for(int i = 0; i < 30; i++)
            {
                if(i % 10 == 0)
                {
                    try
                    {
                        throw new Exception($"There was an error processing {i}");
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, ex.Message);
                    }
                }
                else
                {
                    logger.Information($"Processed cycle {i}");
                }             
                
                Thread.Sleep(50);
            }

            logger.Information("Finishing Serilog Publisher");

            Thread.Sleep(1000);
        }
    }
}
