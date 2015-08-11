#if !DNXCORE50
using Microsoft.Framework.Logging;

namespace SampleApp.Timings
{
    public static class TimingNLogLoggerFactoryExtensions
    {
        public static ILoggerFactory AddTimingNLog(this ILoggerFactory factory, global::NLog.LogFactory logFactory)
        {
            factory.AddProvider(new TimingNLogLoggerProvider(logFactory));
            return factory;
        }
    }
}
#endif
