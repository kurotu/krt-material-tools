using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRT.MaterialTools.Common
{
    public static class InternalLogger
    {
        private static ILogger logger = new Logger(Debug.unityLogger.logHandler);

        internal static ILogger Logger => logger;

        public static void FilterLogType(LogType level)
        {
            logger.filterLogType = level;
        }
    }
}
