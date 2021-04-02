using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Odata.V4.Client.Tools.Abstractions
{
    /// <summary>
    /// Proxy classes generator params
    /// </summary>
    public class GeneratorParams
    {
        /// <summary>
        /// The URI of the metadata document.The value must be set to a valid service document URI or a local file path
        /// eg : "http://services.odata.org/V4/OData/OData.svc/", "File:///C:/Odata.edmx", or @"C:\Odata.edmx"
        /// ### Notice ### If the OData service requires authentication for accessing the metadata document, the value of
        /// MetadataDocumentUri has to be set to a local file path, or the client code generation process will fail.
        /// </summary>
        public string MetadataDocumentUri { get; set; }

        /// <summary>
        /// The use of DataServiceCollection enables entity and property tracking. The value must be set to true or false.
        /// </summary>
        public bool UseDataServiceCollection { get; set; } = true;

        /// <summary>
        /// This flag indicates whether to ignore unexpected elements and attributes in the metadata document and generate
        /// the client code if any. The value must be set to true or false.
        /// </summary>
        public bool IgnoreUnexpectedElementsAndAttributes { get; set; } = true;

        /// <summary>
        /// This flag indicates whether to enable naming alias. The value must be set to true or false./// 
        /// </summary>
        public bool EnableNamingAlias { get; set; } = true;

        /// <summary>
        /// The namespace of the client code generated. It replaces the original namespace in the metadata document,
        /// unless the model has several namespaces.
        /// </summary>
        public string NamespacePrefix { get; set; } = string.Empty;

        /// <summary>
        /// If set to true, generated types will have an "internal" class modifier ("Friend" in VB) instead of "public"
        /// thereby making them invisible outside the assembly
        /// </summary>
        public bool MakeTypesInternal { get; set; } = false;

        /// <summary>
        /// This files indicates whether to generate the files into multiple files or single.
        /// If set to true then multiple files will be generated. Otherwise only a single file is generated.
        /// </summary>
        public bool GenerateMultipleFiles { get; set; } = false;

        /// <summary>
        /// List of the names of operation imports to exclude from the generated code
        /// </summary>
        public IEnumerable<string> ExcludedOperationImports { get; set; } = new List<string>();

        /// <summary>
        /// List of the names of bound operations to exclude from the generated code
        /// </summary>
        public IEnumerable<string> ExcludedBoundOperations { get; set; } = new List<string>();

        /// <summary>
        /// List of the names of entity types to exclude from the generated code
        /// </summary>
        public IEnumerable<string> ExcludedSchemaTypes { get; set; } = new List<string>();
        
        /// <summary>
        /// (Optional) Custom http headers
        /// </summary>
        public IList<string> CustomHttpHeaders { get; set; } = new List<string>();


        /// <summary>
        /// Enables proxy server settings
        /// </summary>
        public bool IncludeWebProxy { get; set; }

        /// <summary>
        /// Proxy server 
        /// </summary>
        public string WebProxyHost { get; set; }

        /// <summary>
        /// Enables proxy credentials settings
        /// </summary>
        public bool IncludeWebProxyNetworkCredentials { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string WebProxyNetworkCredentialsUsername { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string WebProxyNetworkCredentialsPassword { get; set; }

        /// <summary>
        /// Proxy server user's domain
        /// </summary>
        public string WebProxyNetworkCredentialsDomain { get; set; }

        /// <summary>
        /// Custom container class name
        /// </summary>
        public string CustomContainerName { get; set; } = string.Empty;

        /// <summary>
        /// Proxy classes output directory
        /// </summary>
        public string OutputDirectory { get; set; } = string.Empty;

        /// <summary>
        /// <see cref="IConfiguration"/> instance
        /// </summary>
        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// Enables verbose logging
        /// </summary>
        public bool Verbose { get; set; }

        /// <summary>
        /// List of plugins names
        /// </summary>
        public IEnumerable<string> Plugins { get; set; } = new List<string>();

    }
}
