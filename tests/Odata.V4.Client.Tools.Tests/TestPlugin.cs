using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Odata.V4.Client.Tools.Abstractions;

namespace Odata.V4.Client.Tools.Tests
{
    public class TestPlugin : Plugin
    {
        public TestPlugin(ILogger logger, GeneratorParams generatorParams) : base(logger, generatorParams)
        {
        }

        public override void Execute()
        {
            Logger.LogInformation($"Executing {nameof(TestPlugin)}");
            var pluginSetting = GeneratorParams.Configuration["testSetting"];
            if (string.IsNullOrWhiteSpace(pluginSetting))
                throw new ArgumentException("testSetting is empty");

            File.AppendAllText(Path.Combine(GeneratorParams.OutputDirectory, "ODataService.cs"), pluginSetting);
        }
    }
}
