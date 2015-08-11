#if !DNXCORE50
using Microsoft.Framework.Logging;
using NLog;

namespace SampleApp.Timings
{
    public class TimingNLogLoggerProvider : ILoggerProvider
    {
        private readonly LogFactory _logFactory;

        public TimingNLogLoggerProvider(LogFactory logFactory)
        {
            this._logFactory = logFactory;
        }

        public ILogger CreateLogger(string name)
        {
            return new TimingNLogLogger(_logFactory.GetLogger(name));
        }
    }
}
#endif