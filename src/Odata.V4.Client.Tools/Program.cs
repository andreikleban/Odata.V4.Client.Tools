using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Odata.V4.Client.Tools.Abstractions;
using Odata.V4.Client.Tools.Generator;

namespace Odata.V4.Client.Tools
{
    /// <summary>
    /// Main class
    /// </summary>
    public class Program
    {
        private static CommandLineApplication _commandLine;
        private static CommandOption _metadataDocumentUriOption;
        private static CommandOption _noTrackingOption;
        private static CommandOption _namespaceOption;
        private static CommandOption _disableNamingAliasOption;
        private static CommandOption _makeTypesInternalOption;
        private static CommandOption _generateMultipleFilesOption;
        private static CommandOption _excludedOperationImportsOption;
        private static CommandOption _excludedBoundOperationsOption;
        private static CommandOption _excludedSchemaTypes;
        private static CommandOption _customContainerNameOption;
        private static CommandOption _outputDirOption;
        private static CommandOption _verboseOption;
        private static CommandOption _proxyOption;
        private static GeneratorParams _generatorParams;
        private static CommandOption _pluginsOption;

        /// <summary>
        /// Application entry point
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            _commandLine = new CommandLineApplication(false);
            _commandLine.Description = "Client Proxy generator for OData V3";
            _commandLine.Name = typeof(Program).Assembly.GetName().Name;

            _commandLine.HelpOption("-?|-h|--help");
            var version = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            _commandLine.VersionOption("--version", () => version, () => $"{_commandLine.Description}\n{version}");
            _metadataDocumentUriOption = _commandLine.Option("-m|--metadata", "The URI of the metadata document. The value must be set to a valid service document URI or a local file path", CommandOptionType.SingleValue);
            _noTrackingOption = _commandLine.Option("-n|--no-tracking", "The use of this disables entity and property tracking", CommandOptionType.NoValue);
            _namespaceOption = _commandLine.Option("-ns|--namespace", "The namespace of the client code generated.", CommandOptionType.SingleValue);
            _disableNamingAliasOption = _commandLine.Option("-na|--no-alias", "This flag indicates whether to disable naming alias", CommandOptionType.NoValue);
            _makeTypesInternalOption = _commandLine.Option("-i|--internal", "If set to true, generated types will have an \"internal\" class modifier instead of \"public\" thereby making them invisible outside the assembly", CommandOptionType.NoValue);
            _generateMultipleFilesOption = _commandLine.Option("-mf|--multiple", "This files indicates whether to generate the files into multiple files or single", CommandOptionType.NoValue);
            _excludedOperationImportsOption = _commandLine.Option("-eoi", "Comma-separated list of the names of operation imports to exclude from the generated code", CommandOptionType.SingleValue);
            _excludedBoundOperationsOption = _commandLine.Option("-ebo", "Comma-separated list of the names of entity types to exclude from the generated code", CommandOptionType.SingleValue);
            _excludedSchemaTypes = _commandLine.Option("-est", "Comma-separated list of the names of entity types to exclude from the generated code", CommandOptionType.SingleValue);
            _customContainerNameOption = _commandLine.Option("-c", "Custom container class name", CommandOptionType.SingleValue);
            _outputDirOption = _commandLine.Option("-o|--outputdir", "Full path to output directory", CommandOptionType.SingleValue);
            _verboseOption = _commandLine.Option("-v|--verbose", "Verbose", CommandOptionType.NoValue);
            _proxyOption = _commandLine.Option("-p|--proxy", "Proxy settings. Format: domain\\user:password@SERVER:PORT", CommandOptionType.SingleValue);
            _pluginsOption = _commandLine.Option("-pl|--plugins", "List of plugins. Format: Assembly.dll,Namespace.Class", CommandOptionType.MultipleValue);

            _generatorParams = new GeneratorParams();

            _commandLine.OnExecute(ParseArgs);


            var commandLineParseResult = (ExitCode)_commandLine.Execute(args);

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter(_commandLine.Name, _verboseOption.HasValue() ? LogLevel.Debug : LogLevel.None)
                    .AddConsole(options =>
                    {
                        options.Format = ConsoleLoggerFormat.Default;
                        options.TimestampFormat = "hh:mm:ss ";
                        options.IncludeScopes = false;
                    });
            });
            var logger = loggerFactory.CreateLogger(_commandLine.Name);

            try
            {
                // ExitCode.Default returns if options -h or --version is
                if (commandLineParseResult <= ExitCode.Default)
                    return;

                _generatorParams.Configuration = new ConfigurationBuilder().AddCommandLine(_commandLine.RemainingArguments.ToArray()).Build();

                var generator = new Odata4ClientGenerator(logger);
                generator.GenerateClientProxyClasses(_generatorParams);

            }
            catch (CommandParsingException e)
            {
                logger.LogError(e.Message);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw;
            }
        }

        private static int ParseArgs()
        {
            ExitCode result = ExitCode.Ok;

            if (!_commandLine.GetOptions().Any(o => o.HasValue()))
            {
                _commandLine.ShowHint();
                return (int) ExitCode.Error;
            }

            if (_metadataDocumentUriOption.HasValue())
                _generatorParams.MetadataDocumentUri = _metadataDocumentUriOption.Value();

            if (_noTrackingOption.HasValue())
                _generatorParams.UseDataServiceCollection = false;

            if (_namespaceOption.HasValue())
                _generatorParams.NamespacePrefix = _namespaceOption.Value();

            if (_disableNamingAliasOption.HasValue())
                _generatorParams.EnableNamingAlias = false;

            if (_makeTypesInternalOption.HasValue())
                _generatorParams.MakeTypesInternal = true;

            if (_generateMultipleFilesOption.HasValue())
                _generatorParams.GenerateMultipleFiles = true;

            if (_excludedOperationImportsOption.HasValue())
                _generatorParams.ExcludedOperationImports = _excludedOperationImportsOption.Value().Split(',');

            if (_excludedBoundOperationsOption.HasValue())
                _generatorParams.ExcludedBoundOperations = _excludedBoundOperationsOption.Value().Split(',');

            if (_excludedSchemaTypes.HasValue())
                _generatorParams.ExcludedSchemaTypes = _excludedSchemaTypes.Value().Split(',');

            if (_customContainerNameOption.HasValue())
                _generatorParams.CustomContainerName = _customContainerNameOption.Value();

            if (_outputDirOption.HasValue())
            {
                if (!Directory.Exists(_outputDirOption.Value()))
                    Directory.CreateDirectory(_outputDirOption.Value());

                _generatorParams.OutputDirectory = _outputDirOption.Value();
            }

            if (_verboseOption.HasValue())
                _generatorParams.Verbose = _verboseOption.HasValue();

            if (_proxyOption.HasValue())
            {
                var proxy = _proxyOption.Value();
                var proxyParts = proxy.Split('@');

                var server = string.Empty;
                var person = string.Empty;

                if (proxyParts.Length == 1)
                    server = proxyParts[0];
                else if (proxyParts.Length == 2)
                {
                    person = proxyParts[0];
                    server = proxyParts[1];
                }

                if (!string.IsNullOrWhiteSpace(server))
                {
                    _generatorParams.WebProxyHost = server;
                    if (!string.IsNullOrWhiteSpace(person))
                    {
                        var personParts = person.Split(':');
                        _generatorParams.WebProxyNetworkCredentialsPassword = personParts[1];

                        var userParts = personParts[0].Split('\\', '/');
                        _generatorParams.WebProxyNetworkCredentialsDomain = userParts[0];
                        _generatorParams.WebProxyNetworkCredentialsUsername = userParts[1];
                        _generatorParams.IncludeWebProxyNetworkCredentials = true;
                    }
                    _generatorParams.IncludeWebProxy = true;
                }
            }

            if (_pluginsOption.HasValue())
                _generatorParams.Plugins = _pluginsOption.Values;

            return (int)result;
        }
    }
}
