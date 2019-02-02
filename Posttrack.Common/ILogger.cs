using System;

namespace Posttrack.Common
{
    public interface ILogger
    {
        ILogger CreateScope(string name);

        void Debug(string message);

        void Info(string message);

        void Warning(string message);

#pragma warning disable CA1716 // Identifiers should not match keywords
        void Error(string message);
#pragma warning restore CA1716 // Identifiers should not match keywords

        void Fatal(string message);

        void Log(Exception ex);
    }
}