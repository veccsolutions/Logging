#if !DNXCORE50
using System;
using System.Threading;
using Microsoft.Framework.Logging;
using Microsoft.Framework.Logging.NLog;

namespace SampleApp.Timings
{
    public class TimingNLogLogger : NLogLogger
    {
        public TimingNLogLogger(NLog.Logger logger)
            : base(logger)
        {
        }

        public override void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            //dont send thread abort exceptions to NLog.
            if (exception is ThreadAbortException)
            {
                return;
            }

            base.Log(logLevel, eventId, state, exception, formatter);
        }

        public override IDisposable BeginScopeImpl(object state)
        {
            var innerScope = base.BeginScopeImpl(state);

            return new TimingNLogScope(this, innerScope);
        }
    }
}
#endif
