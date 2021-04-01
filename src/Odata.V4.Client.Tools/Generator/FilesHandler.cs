using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Odata.V4.Client.Tools.Generator
{
    internal class FilesHandler
    {
        public FilesHandler() : base()
        {
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
            return Task.FromResult(string.Empty);
        }
        public IDictionary<string, string> TokenReplacementValues { get; }
    }
}
