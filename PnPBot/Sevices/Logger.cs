using Discord;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PnPBot.Sevices;

public class Logger : ILogger
{
    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        Console.WriteLine(exception?.Message); if (!IsEnabled(logLevel))
            return;

        Console.WriteLine($"[{eventId.Id,2}: {logLevel,-12}]");
        Console.Write($"{formatter(state, exception)}");
        Console.WriteLine();
    }
}
