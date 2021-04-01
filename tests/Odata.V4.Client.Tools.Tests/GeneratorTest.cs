using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Odata.V4.Client.Tools.Tests
{
    [TestClass]
    public class GeneratorTest
    {
        private const string OutputDir = "TestResults";

        [TestInitialize]
        public void Init()
        {
            if (Directory.Exists(OutputDir))
                Directory.Delete(OutputDir, true);
        }

        [TestMethod]
        [DataRow(new string[] {""})]
        public void EmptyArgs(string[] args)
        {
            Program.Main(args);
        }

        [TestMethod]
        [DataRow(new string[] { "-o", OutputDir, "-v"})]
        public void WithoutMetadata(string[] args)
        {
            Assert.ThrowsException<ArgumentNullException>(() => Program.Main(args));
        }

        [TestMethod]
        public void DefaultContainerName()
        {
            var args= new string[] { "-m", $"{Directory.GetCurrentDirectory()}\\Assets\\metadataV4.edmx", "-o", OutputDir, "-v" };

            Program.Main(args);
            Assert.IsTrue(File.Exists($"{OutputDir}\\OdataService.cs"), "OdataService.cs isn't exist");
            Assert.IsTrue(File.Exists($"{OutputDir}\\OdataServiceCsdl.xml"), "OdataServiceCsdl.xml isn't exist");
        }

        [TestMethod]
        public void CustomContainerName()
        {
            var args = new string[] { "-m", $"{Directory.GetCurrentDirectory()}\\Assets\\metadataV4.edmx", "-o", OutputDir, "-v", "-c", "TestContainer" };

            Program.Main(args);
            Assert.IsTrue(File.Exists($"{OutputDir}\\TestContainer.cs"), "TestContainer.cs isn't exist");
            Assert.IsTrue(File.Exists($"{OutputDir}\\TestContainerCsdl.xml"), "TestContainerCsdl.xml isn't exist");
        }


        [TestMethod]
        public void NamespaceTest()
        {
            var args = new string[] { "-m", $"{Directory.GetCurrentDirectory()}\\Assets\\metadataV4.edmx", "-o", OutputDir, "-v", "-ns", "TestNamespace" };

            Program.Main(args);
            var file = File.ReadAllText($"{OutputDir}\\OdataService.cs");
            Assert.IsTrue(file.Contains("TestNamespace"));
        }

        [TestMethod]
        public void WithMetadataV3()
        {
            var args = new string[] { "-m", $"{Directory.GetCurrentDirectory()}\\Assets\\metadataV3.edmx", "-o", OutputDir, "-v"};

            Assert.ThrowsException<ArgumentException>(() => Program.Main(args));
        }


        [TestMethod]
        public void PluginEmptySetting()
        {
            var args = new string[] { "-m", $"{Directory.GetCurrentDirectory()}\\Assets\\metadataV4.edmx", "-o", OutputDir, "-v", "-pl=Odata.V4.Client.Tools.Tests.dll,Odata.V4.Client.Tools.Tests.TestPlugin" };

            Assert.ThrowsException<ArgumentException>(() => Program.Main(args));
        }

        [TestMethod]
        public void WrongPluginName()
        {
            var args = new string[] { "-m", $"{Directory.GetCurrentDirectory()}\\Assets\\metadataV4.edmx", "-o", OutputDir, "-v", "-pl=Odata.V4.Client.Tools.Tests.TestPlugin" };
            Assert.ThrowsException<ArgumentException>(() => Program.Main(args));
        }

        [TestMethod]
        public void WrongPluginAssemblyName()
        {
            var args = new string[] { "-m", $"{Directory.GetCurrentDirectory()}\\Assets\\metadataV4.edmx", "-o", OutputDir, "-v", "-pl=Assembly.dll,Odata.V4.Client.Tools.Tests.TestPlugin" };
            Assert.ThrowsException<FileNotFoundException>(() => Program.Main(args));
        }

        [TestMethod]
        public void WrongPluginTypeName()
        {
            var args = new string[] { "-m", $"{Directory.GetCurrentDirectory()}\\Assets\\metadataV4.edmx", "-o", OutputDir, "-v", "-pl=Odata.V4.Client.Tools.Tests.dll,Odata.V4.Client.Tools.Tests.WrongTestPlugin" };
            Assert.ThrowsException<TypeLoadException>(() => Program.Main(args));
        }

        [TestMethod]
        public void PluginTest()
        {
            var args = new string[] { "-m", $"{Directory.GetCurrentDirectory()}\\Assets\\metadataV4.edmx", "-o", OutputDir, "-v", "-pl=Odata.V4.Client.Tools.Tests.dll,Odata.V4.Client.Tools.Tests.TestPlugin", "testSetting=testString" };
            Program.Main(args);
            var file = File.ReadAllText($"{OutputDir}\\OdataService.cs");
            Assert.IsTrue(file.Contains("testString"));
        }
    }
}
