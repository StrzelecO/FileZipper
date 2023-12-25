using System;
using NLog;

namespace FileZipper
{
    public static class LoggerManager
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();
    }
}
