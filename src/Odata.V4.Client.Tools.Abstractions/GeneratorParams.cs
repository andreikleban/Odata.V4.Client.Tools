using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Odata.V4.Client.Tools.Abstractions
{
    public class GeneratorParams
    {
        public string MetadataDocumentUri { get; set; }
        public bool UseDataServiceCollection { get; set; } = true;
        public bool IgnoreUnexpectedElementsAndAttributes { get; set; } = true;
        public bool EnableNamingAlias { get; set; } = true;
        public string NamespacePrefix { get; set; } = string.Empty;
        public bool MakeTypesInternal { get; set; } = false;
        public bool GenerateMultipleFiles { get; set; } = false;
        public IEnumerable<string> ExcludedOperationImports { get; set; } = new List<string>();
        public IEnumerable<string> ExcludedBoundOperations { get; set; } = new List<string>();
        public IEnumerable<string> ExcludedSchemaTypes { get; set; } = new List<string>();
        public IList<string> CustomHttpHeaders { get; set; } = new List<string>();
        public bool IncludeWebProxy { get; set; }
        public string WebProxyHost { get; set; }
        public bool IncludeWebProxyNetworkCredentials { get; set; }
        public string WebProxyNetworkCredentialsUsername { get; set; }
        public string WebProxyNetworkCredentialsPassword { get; set; }
        public string WebProxyNetworkCredentialsDomain { get; set; }
        public string CustomContainerName { get; set; } = string.Empty;
        public string OutputDirectory { get; set; } = string.Empty;
        public string GeneratorVersion { get; set; } = "1.0.0";

        public IConfiguration Configuration { get; set; }

        public bool Verbose { get; set; }

        /// <summary>
        /// List of plugins names
        /// </summary>
        public IEnumerable<string> Plugins { get; set; } = new List<string>();

    }
}
