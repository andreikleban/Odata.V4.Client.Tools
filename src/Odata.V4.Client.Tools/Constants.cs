using System;
using System.Collections.Generic;

namespace Odata.V4.Client.Tools
{
    internal class Constants
    {
        public const string DefaultServiceFilename = "ODataService";
        public const string CsdlFileNameSuffix = "Csdl.xml";
        public static Version EdmxVersion1 = new Version(1, 0, 0, 0);
        public static Version EdmxVersion2 = new Version(2, 0, 0, 0);
        public static Version EdmxVersion3 = new Version(3, 0, 0, 0);
        public static Version EdmxVersion4 = new Version(4, 0, 0, 0);

        public const string EdmxVersion1Namespace = "http://schemas.microsoft.com/ado/2007/06/edmx";
        public const string EdmxVersion2Namespace = "http://schemas.microsoft.com/ado/2008/10/edmx";
        public const string EdmxVersion3Namespace = "http://schemas.microsoft.com/ado/2009/11/edmx";
        public const string EdmxVersion4Namespace = "http://docs.oasis-open.org/odata/ns/edmx";

        private static Dictionary<string, Version> supportedEdmxNamespaces = new Dictionary<string, Version>
        {
            { EdmxVersion1Namespace, EdmxVersion1},
            { EdmxVersion2Namespace, EdmxVersion2},
            { EdmxVersion3Namespace, EdmxVersion3},
            { EdmxVersion4Namespace, EdmxVersion4}
        };

        public static Dictionary<string, Version> SupportedEdmxNamespaces => supportedEdmxNamespaces;
    }
}
