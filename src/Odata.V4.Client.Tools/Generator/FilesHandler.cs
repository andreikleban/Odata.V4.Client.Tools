using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Odata.V4.Client.Tools.Properties;

namespace Odata.V4.Client.Tools.Generator
{
    internal class FilesHandler
    {
        private readonly ILogger _logger;

        public FilesHandler(ILogger logger) : base()
        {
            _logger = logger;
            AddedFiles = new List<(string CreatedFile, string SourceFile)>();
            TokenReplacementValues = new Dictionary<string, string>();
        }

        public IList<(string CreatedFile, string SourceFile)> AddedFiles { get; private set; }
        
        public Task<string> AddFileAsync(string fileName, string targetPath)
        {
            AddedFiles.Add((targetPath, fileName));

            var content = File.ReadAllText(fileName);
            foreach (var token in TokenReplacementValues)
                content = content.Replace(token.Key, token.Value);

            File.WriteAllText(targetPath, content);
            _logger.LogInformation(string.Format(Resources.File__0__added_, targetPath));
            return Task.FromResult(string.Empty);
        }
        public IDictionary<string, string> TokenReplacementValues { get; }
    }
}
