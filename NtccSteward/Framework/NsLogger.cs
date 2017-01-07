//using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using NtccSteward;
using Microsoft.AspNetCore.Mvc;
using apr = NtccSteward.Api.Repository;
using NtccSteward.Core.Framework;
using Microsoft.Extensions.Logging;

namespace NtccSteward.Framework
{
    public class NsLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string name)
        {
            return new NsLogger(Provider);
        }

        public IAppConfigProvider Provider { get; set; }

        public void Dispose()
        {
            return;
        }
    }

    public class NsLogger : ILogger
    {
        private apr.LoggerRepository logger = null;

        public NsLogger(IAppConfigProvider provider)
        {
            logger = new apr.LoggerRepository(provider);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public IDisposable BeginScopeImpl(object state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return !string.IsNullOrWhiteSpace(logger?.ConnectionString ?? string.Empty);
        }

        public void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var summary = exception.Message;
            var details = exception.StackTrace;

            LogException(logLevel, summary, details, 0);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            throw new NotImplementedException();
        }

        private void LogException(LogLevel logLevel, string summary, string details, int userId)
        {
            logger.LogInfo(logLevel, summary, details, userId);
        }
    }
}
