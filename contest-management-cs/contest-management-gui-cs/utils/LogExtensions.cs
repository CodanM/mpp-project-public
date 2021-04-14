using log4net;

namespace contest_management_gui_cs.utils
{
    public static class LogExtensions
    {
        public static void TraceEntry(this ILog log, string args = "")
        {
            log.Logger.Log(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, log4net.Core.Level.Trace,
                    "Enter " + args, null);
        }

        public static void TraceExit(this ILog log)
        {
            log.Logger.Log(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, log4net.Core.Level.Trace,
                "Exit", null);
        }
    }
}