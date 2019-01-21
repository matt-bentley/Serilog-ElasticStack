using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Logging.Core.Models
{
    public class PerformanceTracker
    {
        private readonly Stopwatch _sw;
        private readonly LogDetail _infoToLog;

        public PerformanceTracker(string message, string userId, string userName,
                   string location, string product, string layer, string environment)
        {
            _sw = Stopwatch.StartNew();
            _infoToLog = new LogDetail()
            {
                Message = message,
                UserId = userId,
                UserName = userName,
                Product = product,
                Layer = layer,
                Location = location,
                Hostname = Environment.MachineName,
                Environment = environment
            };

            var beginTime = DateTime.Now;
            _infoToLog.AdditionalInfo = new Dictionary<string, object>()
            {
                { "Started", beginTime.ToString(CultureInfo.InvariantCulture)   }
            };
        }

        public PerformanceTracker(string name, string userId, string userName,
                   string location, string product, string layer, string environment,
                   Dictionary<string, object> perfParams)
            : this(name, userId, userName, location, product, layer, environment)
        {
            foreach (var item in perfParams)
                _infoToLog.AdditionalInfo.Add("input-" + item.Key, item.Value);
        }

        public void Stop()
        {
            _sw.Stop();
            _infoToLog.ElapsedMilliseconds = _sw.ElapsedMilliseconds;
            Logger.WritePerformance(_infoToLog);
        }
    }
}
