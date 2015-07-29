// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Xunit;

namespace Microsoft.Framework.Logging
{
    public class LoggerTest
    {
        [Fact]
        public void Log_IgnoresExceptionInIntermediateLoggers()
        {
            // Arrange
            var store = new List<string>();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new CustomLoggerProvider("provider1", ThrowExceptionAt.Log, store));
            loggerFactory.AddProvider(new CustomLoggerProvider("provider2", ThrowExceptionAt.None, store));
            var logger = loggerFactory.CreateLogger("Test");

            // Act
            logger.LogInformation("Hello!");

            // Assert
            Assert.Equal(new[] { "provider2.Test-Hello!" }, store);
        }

        [Fact]
        public void BeginScope_IgnoresExceptionInIntermediateLoggers()
        {
            // Arrange
            var store = new List<string>();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new CustomLoggerProvider("provider1", ThrowExceptionAt.BeginScope, store));
            loggerFactory.AddProvider(new CustomLoggerProvider("provider2", ThrowExceptionAt.None, store));
            var logger = loggerFactory.CreateLogger("Test");

            // Act
            logger.BeginScope("Scope1");

            // Assert
            Assert.Equal(new[] { "provider2.Test-Scope1" }, store);
        }

        [Fact]
        public void IsEnabled_IgnoresExceptionInIntermediateLoggers()
        {
            // Arrange
            var store = new List<string>();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new CustomLoggerProvider("provider1", ThrowExceptionAt.IsEnabled, store));
            loggerFactory.AddProvider(new CustomLoggerProvider("provider2", ThrowExceptionAt.None, store));
            var logger = loggerFactory.CreateLogger("Test");

            // Act
            if(logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Hello!");
            }

            // Assert
            Assert.Equal(new[] { "provider1.Test-Hello!", "provider2.Test-Hello!" }, store);
        }

        private class CustomLoggerProvider : ILoggerProvider
        {
            private readonly string _providerName;
            private readonly ThrowExceptionAt _throwExceptionAt;
            private readonly List<string> _store;

            public CustomLoggerProvider(string providerName, ThrowExceptionAt throwExceptionAt, List<string> store)
            {
                _providerName = providerName;
                _throwExceptionAt = throwExceptionAt;
                _store = store;
            }

            public ILogger CreateLogger(string name)
            {
                return new CustomLogger($"{_providerName}.{name}", _throwExceptionAt, _store);
            }
        }

        private class CustomLogger : ILogger
        {
            private readonly string _name;
            private readonly ThrowExceptionAt _throwExceptionAt;
            private readonly List<string> _store;

            public CustomLogger(string name, ThrowExceptionAt throwExceptionAt, List<string> store)
            {
                _name = name;
                _throwExceptionAt = throwExceptionAt;
                _store = store;
            }

            public IDisposable BeginScopeImpl(object state)
            {
                if(_throwExceptionAt == ThrowExceptionAt.BeginScope)
                {
                    throw new InvalidOperationException("Error occurred while creating scope.");
                }
                _store.Add($"{_name}-{state}");

                return null;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                if (_throwExceptionAt == ThrowExceptionAt.IsEnabled)
                {
                    throw new InvalidOperationException("Error occurred while checking if logger is enabled.");
                }

                return true;
            }

            public void Log(
                LogLevel logLevel,
                int eventId,
                object state,
                Exception exception,
                Func<object, Exception, string> formatter)
            {
                if (_throwExceptionAt == ThrowExceptionAt.Log)
                {
                    throw new InvalidOperationException("Error occurred while logging data.");
                }
                _store.Add($"{_name}-{state}");
            }
        }

        private enum ThrowExceptionAt
        {
            None,
            BeginScope,
            Log,
            IsEnabled
        }
    }
}
