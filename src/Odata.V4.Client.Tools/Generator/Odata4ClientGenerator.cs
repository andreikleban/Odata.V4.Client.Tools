using System;
using System.IO;
using System.Net;
using System.Xml;
using Microsoft.Extensions.Logging;
using Odata.V4.Client.Tools.Abstractions;
using Odata.V4.Client.Tools.Properties;
using Odata.V4.Client.Tools.Templates;

namespace Odata.V4.Client.Tools.Generator
{
    public class Odata4ClientGenerator
    {
        private readonly ILogger _logger;

        public Odata4ClientGenerator(ILogger logger)
        {
            _logger = logger;
        }

        private static string GetMetadata(GeneratorParams generatorParams, out Version edmxVersion)
        {
            if (string.IsNullOrEmpty(generatorParams.MetadataDocumentUri))
                throw new ArgumentNullException("OData Service Endpoint", Resources.Please_input_the_metadata_document_address);

            if (File.Exists(generatorParams.MetadataDocumentUri))
                generatorParams.MetadataDocumentUri = new FileInfo(generatorParams.MetadataDocumentUri).FullName;

            if (generatorParams.MetadataDocumentUri.StartsWith("https:", StringComparison.Ordinal)
                || generatorParams.MetadataDocumentUri.StartsWith("http", StringComparison.Ordinal))
            {
                if (!generatorParams.MetadataDocumentUri.EndsWith("$metadata", StringComparison.Ordinal))
                {
                    generatorParams.MetadataDocumentUri = generatorParams.MetadataDocumentUri.TrimEnd('/') + "/$metadata";
                }
            }

            Stream metadataStream;
            var metadataUri = new Uri(generatorParams.MetadataDocumentUri);
            if (!metadataUri.IsFile)
            {
                var webRequest = (HttpWebRequest)WebRequest.Create(metadataUri);

                if (generatorParams.IncludeWebProxy)
                {
                    var proxy = new WebProxy(generatorParams.WebProxyHost);
                    if (generatorParams.IncludeWebProxyNetworkCredentials)
                    {
                        proxy.Credentials = new NetworkCredential(
                            generatorParams.WebProxyNetworkCredentialsUsername,
                            generatorParams.WebProxyNetworkCredentialsPassword,
                            generatorParams.WebProxyNetworkCredentialsDomain);
                    }

                    webRequest.Proxy = proxy;
                }

                WebResponse webResponse = webRequest.GetResponse();
                metadataStream = webResponse.GetResponseStream();
            }
            else
            {
                // Set up XML secure resolver
                var xmlUrlResolver = new XmlUrlResolver
                {
                    Credentials = CredentialCache.DefaultNetworkCredentials
                };

                metadataStream = (Stream)xmlUrlResolver.GetEntity(metadataUri, null, typeof(Stream));
            }

            var workFile = Path.GetTempFileName();

            try
            {
                using (XmlReader reader = XmlReader.Create(metadataStream))
                {
                    using (var writer = XmlWriter.Create(workFile))
                    {
                        while (reader.NodeType != XmlNodeType.Element)
                        {
                            reader.Read();
                        }

                        if (reader.EOF)
                        {
                            throw new InvalidOperationException(Resources.The_metadata_is_an_empty_file);
                        }

                        Constants.SupportedEdmxNamespaces.TryGetValue(reader.NamespaceURI, out edmxVersion);
                        writer.WriteNode(reader, false);
                    }
                }
                return workFile;
            }
            catch (WebException e)
            {
                throw new InvalidOperationException(string.Format(Resources.Cannot_access_metadata, generatorParams.MetadataDocumentUri), e);
            }
        }

        public void GenerateClientProxyClasses(GeneratorParams configuration)
        {
            if (string.IsNullOrWhiteSpace(configuration.MetadataDocumentUri))
                throw new ArgumentNullException(nameof(configuration.MetadataDocumentUri));

            GetMetadata(configuration, out var version);
            if (version != Constants.EdmxVersion4)
                throw new ArgumentException(string.Format(Resources.Wrong_edx_version, version));

            ODataT4CodeGenerator t4CodeGenerator = new ODataT4CodeGenerator();
            t4CodeGenerator.MetadataDocumentUri = configuration.MetadataDocumentUri;
            t4CodeGenerator.UseDataServiceCollection = configuration.UseDataServiceCollection;
            t4CodeGenerator.TargetLanguage = ODataT4CodeGenerator.LanguageOption.CSharp;
            t4CodeGenerator.IgnoreUnexpectedElementsAndAttributes = configuration.IgnoreUnexpectedElementsAndAttributes;
            t4CodeGenerator.EnableNamingAlias = configuration.EnableNamingAlias;
            t4CodeGenerator.NamespacePrefix = configuration.NamespacePrefix;
            t4CodeGenerator.MakeTypesInternal = configuration.MakeTypesInternal;
            t4CodeGenerator.GenerateMultipleFiles = configuration.GenerateMultipleFiles;
            t4CodeGenerator.ExcludedOperationImports = configuration.ExcludedOperationImports;
            t4CodeGenerator.ExcludedBoundOperations = configuration.ExcludedBoundOperations;
            t4CodeGenerator.ExcludedSchemaTypes = configuration.ExcludedSchemaTypes;
            t4CodeGenerator.CustomHttpHeaders = configuration.CustomHttpHeaders;
            t4CodeGenerator.IncludeWebProxy = configuration.IncludeWebProxy;
            t4CodeGenerator.WebProxyHost = configuration.WebProxyHost;
            t4CodeGenerator.IncludeWebProxyNetworkCredentials = configuration.IncludeWebProxyNetworkCredentials;
            t4CodeGenerator.WebProxyNetworkCredentialsUsername = configuration.WebProxyNetworkCredentialsUsername;
            t4CodeGenerator.WebProxyNetworkCredentialsPassword = configuration.WebProxyNetworkCredentialsPassword;
            t4CodeGenerator.WebProxyNetworkCredentialsDomain = configuration.WebProxyNetworkCredentialsDomain;
            t4CodeGenerator.CustomContainerName = configuration.CustomContainerName;

            var tempFile = Path.GetTempFileName();
            var referenceFolder = configuration.OutputDirectory;

            var fileHandler = new FilesHandler();
            fileHandler.TokenReplacementValues.Add("#VersionNumber#", configuration.GeneratorVersion);

            var serviceFilename = Constants.DefaultServiceFilename;
            if (!string.IsNullOrWhiteSpace(configuration.CustomContainerName))
                serviceFilename = configuration.CustomContainerName;

            var csdlFileName = string.Concat(serviceFilename, Constants.CsdlFileNameSuffix);
            var metadataFile = Path.Combine(referenceFolder, csdlFileName);
            fileHandler.AddFileAsync(tempFile, metadataFile).ConfigureAwait(true);

            t4CodeGenerator.EmitContainerPropertyAttribute = false;
            t4CodeGenerator.MetadataFilePath = metadataFile;
            t4CodeGenerator.MetadataFileRelativePath = csdlFileName;

            try
            {
                using (StreamWriter writer = File.CreateText(tempFile))
                {
                    writer.Write(t4CodeGenerator.TransformText());
                    writer.Flush();
                    if (t4CodeGenerator.Errors != null && t4CodeGenerator.Errors.Count > 0)
                    {
                        foreach (var err in t4CodeGenerator.Errors)
                            _logger?.LogWarning(err.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Generators error", e);
                throw;
            }

            var outputFile = Path.Combine(referenceFolder, string.IsNullOrWhiteSpace(configuration.CustomContainerName) ? $"{Constants.DefaultServiceFilename}.cs" : $"{configuration.CustomContainerName}.cs");
            fileHandler.AddFileAsync(tempFile, outputFile).ConfigureAwait(true);
            t4CodeGenerator.MultipleFilesManager?.GenerateFiles(true, fileHandler, referenceFolder, true, false);

            foreach (var pluginCommand in configuration.Plugins)
            {
                var plugin = PluginCreator.Create(_logger, configuration, pluginCommand);
                plugin.Execute();
            }

            _logger?.LogInformation("Client Proxy for OData V4 was generated.");
        }
    }
}
