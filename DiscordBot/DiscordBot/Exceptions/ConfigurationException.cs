using System;

namespace DiscordBot.Exceptions;

public class ConfigurationException : Exception
{
    public ConfigurationException(string message)
        : base(message)
    { }
}
