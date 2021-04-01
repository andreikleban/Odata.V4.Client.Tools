using Microsoft.Extensions.Logging;

namespace Odata.V4.Client.Tools.Abstractions
{
    /// <summary>
    /// Base plugin class
    /// </summary>
    public abstract class Plugin
    {
        private Plugin()
        {}

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="generatorParams"></param>
        protected Plugin(ILogger logger, GeneratorParams generatorParams)
        {
            Logger = logger;
            GeneratorParams = generatorParams;
        }

        /// <summary>
        /// <see cref="ILogger"/> instance
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Proxy classes generator's parameters
        /// </summary>
        protected GeneratorParams GeneratorParams { get;}

        /// <summary>
        /// Plugin operation implementation
        /// </summary>
        public abstract void Execute();
    }
}
