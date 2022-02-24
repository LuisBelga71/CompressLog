using NLog.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompressLogs
{
    public class Logger
    {
        private static NLog.Logger _LoggerInstance;
        public static NLog.Logger LoggerInstance
        {
            get
            {
                if (_LoggerInstance is null)
                    _LoggerInstance = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

                return _LoggerInstance;
            }
        }
    }
}
