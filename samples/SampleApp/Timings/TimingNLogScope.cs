#if !DNXCORE50
using System;
using System.Diagnostics;
using Microsoft.Framework.Logging;

namespace SampleApp.Timings
{
    public class TimingNLogScope : IDisposable
    {
        private bool _disposed = false; // To detect redundant calls
        private IDisposable _innerScope;
        private ILogger _logger;
        private Stopwatch _stopwatch;

        public TimingNLogScope(ILogger logger, IDisposable innerScope)
        {
            this._logger = logger;
            this._innerScope = innerScope;
            this._logger.LogDebug("Enter");
            this._stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    this._logger.LogDebug("Exit");
                    this._logger.LogDebug($"Elapsed: {this._stopwatch.Elapsed}");
                    this._stopwatch.Stop();
                    this._stopwatch = null;
                    this._innerScope.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
#endif