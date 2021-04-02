using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Odata.V4.Client.Tools.Abstractions;
using Odata.V4.Client.Tools.Properties;

namespace Odata.V4.Client.Tools
{
    internal static class PluginCreator
    {
        internal static Plugin Create(ILogger logger, GeneratorParams generatorParams, string command)
        {
            var args = command.Split(',');
            if (args.Length < 2)
                throw new ArgumentException(Resources.Incorrect_plugin_name);

            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), args[0])))
                throw new FileNotFoundException(Resources.Plugin_assembly_hasn_t_found, args[0]);

            try
            {
                var assembly =  Assembly.LoadFrom(args[0]);
                var pluginType = assembly.GetType(args[1], true, true);
                var plugin = Activator.CreateInstance(pluginType, logger, generatorParams) as Plugin;
                return plugin;
            }
            catch (Exception e)
            {
                logger.LogError(string.Format(Resources.Plugin_creation__0__error, args[0]), e);
                throw;
            }
        }
    }
}
