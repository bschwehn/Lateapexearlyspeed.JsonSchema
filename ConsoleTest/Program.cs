using Cwe.StudyConfigurationConverter;
using Microsoft.Extensions.Logging;

namespace ConsoleTest;

internal class Program
{
    static void Main(string[] args)
    {
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        ILogger<Program> logger = loggerFactory.CreateLogger<Program>();
        JsonReader r = new();
        r.ValidateJson();
        ;
    }
}
