using Logging.Core.Helpers;
using Logging.Core.Models;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Http;
using Serilog.Sinks.Http.BatchFormatters;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Logging.Core
{
    public static class Logger
    {
        private static readonly ILogger _performanceLogger;
        private static readonly ILogger _usageLogger;
        private static readonly ILogger _errorLogger;
        private static readonly ILogger _diagnosticLogger;

        static Logger()
        {
            IHttpClient client = new LoggerClient();

            _performanceLogger = new LoggerConfiguration()
                .WriteTo.Http(
                    requestUri: "http://localhost:31311",
                    batchFormatter: new ArrayBatchFormatter(),
                    httpClient: client)
                .WriteTo.Console()
                .CreateLogger();

            _usageLogger = new LoggerConfiguration()
                .WriteTo.Http(
                    requestUri: "http://localhost:31311",
                    batchFormatter: new ArrayBatchFormatter(),
                    httpClient: client)
                .WriteTo.Console()
                .CreateLogger();

            _errorLogger = new LoggerConfiguration()
                .WriteTo.Http(
                    requestUri: "http://localhost:31311",
                    batchFormatter: new ArrayBatchFormatter(),
                    httpClient: client)
                .WriteTo.Console()
                .CreateLogger();

            _diagnosticLogger = new LoggerConfiguration()
                .WriteTo.Http(
                    requestUri: "http://localhost:31311",
                    batchFormatter: new ArrayBatchFormatter(),
                    httpClient: client)
                .WriteTo.Console()
                .CreateLogger();

            Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
        }

        public static void WritePerformance(LogDetail infoToLog)
        {
            //_perfLogger.Write(LogEventLevel.Information, "{@FlogDetail}", infoToLog);
            _performanceLogger.Write(LogEventLevel.Information,
                    "{Type}{Message}{Layer}{Location}{Product}" +
                    "{Environment}{ElapsedMilliseconds}{Hostname}" +
                    "{UserId}{UserName}{CorrelationId}{AdditionalInfo}",
                    "performance", infoToLog.Message,
                    infoToLog.Layer, infoToLog.Location,
                    infoToLog.Product,
                    infoToLog.Environment, infoToLog.ElapsedMilliseconds,
                    infoToLog.Hostname, infoToLog.UserId,
                    infoToLog.UserName, infoToLog.CorrelationId,
                    infoToLog.AdditionalInfo
                    );
        }   
        public static void WriteUsage(LogDetail infoToLog)
        {
            _usageLogger.Write(LogEventLevel.Information,
                    "{Type}{Message}{Layer}{Location}{Product}" +
                    "{Environment}{ElapsedMilliseconds}{Hostname}" +
                    "{UserId}{UserName}{CorrelationId}{AdditionalInfo}",
                    "usage", infoToLog.Message,
                    infoToLog.Layer, infoToLog.Location,
                    infoToLog.Product,
                    infoToLog.Environment, infoToLog.ElapsedMilliseconds,
                    infoToLog.Hostname, infoToLog.UserId,
                    infoToLog.UserName, infoToLog.CorrelationId,
                    infoToLog.AdditionalInfo
                    );
        }
        public static void WriteError(LogDetail infoToLog)
        {
            if (infoToLog.Exception != null)
            {
                var procName = FindProcName(infoToLog.Exception);
                infoToLog.Location = string.IsNullOrEmpty(procName) ? infoToLog.Location : procName;
                infoToLog.Message = GetMessageFromException(infoToLog.Exception);
            }
        
            _errorLogger.Write(LogEventLevel.Error,
                    "{Type}{Message}{Layer}{Location}{Product}" +
                    "{Environment}{ElapsedMilliseconds}{Exception}{Hostname}" +
                    "{UserId}{UserName}{CorrelationId}{AdditionalInfo}",
                    "error", infoToLog.Message,
                    infoToLog.Layer, infoToLog.Location,
                    infoToLog.Product,
                    infoToLog.Environment, infoToLog.ElapsedMilliseconds, infoToLog.Exception?.ToBetterString(),
                    infoToLog.Hostname, infoToLog.UserId,
                    infoToLog.UserName, infoToLog.CorrelationId,
                    infoToLog.AdditionalInfo
                    );
        }
        public static void WriteDiagnostic(LogDetail infoToLog)
        {
            // TO DO
            //var writeDiagnostics = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableDiagnostics"]);
            //if (!writeDiagnostics)
            //    return;

            _diagnosticLogger.Write(LogEventLevel.Information,
                    "{Type}{Message}{Layer}{Location}{Product}" +
                    "{Environment}{ElapsedMilliseconds}{Hostname}" +
                    "{UserId}{UserName}{CorrelationId}{AdditionalInfo}",
                    "diagnostic", infoToLog.Message,
                    infoToLog.Layer, infoToLog.Location,
                    infoToLog.Product,
                    infoToLog.Environment, infoToLog.ElapsedMilliseconds,
                    infoToLog.Hostname, infoToLog.UserId,
                    infoToLog.UserName, infoToLog.CorrelationId,
                    infoToLog.AdditionalInfo
                    );
        }

        private static string GetMessageFromException(Exception ex)
        {
            if (ex.InnerException != null)
                return GetMessageFromException(ex.InnerException);

            return ex.Message;
        }

        private static string FindProcName(Exception ex)
        {
            var sqlEx = ex as SqlException;
            if (sqlEx != null)
            {
                var procName = sqlEx.Procedure;
                if (!string.IsNullOrEmpty(procName))
                    return procName;
            }

            if (!string.IsNullOrEmpty((string)ex.Data["Procedure"]))
            {
                return (string)ex.Data["Procedure"];
            }

            if (ex.InnerException != null)
                return FindProcName(ex.InnerException);

            return null;
        }
    }

    internal class LoggerClient : IHttpClient
    {
        private readonly HttpClient _client;

        public LoggerClient()
        {
            _client = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes("testuser:password123");
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }      

        public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            return await _client.PostAsync(requestUri, content);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
