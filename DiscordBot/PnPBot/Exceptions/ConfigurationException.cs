using System;

namespace PnPBot.Exceptions;

public class ConfigurationException : Exception
{
    public ConfigurationException(string message)
        : base(message)
    { }
}
