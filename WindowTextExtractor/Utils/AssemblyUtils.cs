using System.IO;
using System.Linq;
using System.Reflection;

namespace WindowTextExtractor.Utils
{
    static class AssemblyUtils
    {
        public static string AssemblyTitle
        {
            get
            {
                var attribute = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false).FirstOrDefault() as AssemblyTitleAttribute;
                return attribute != null ? attribute.Title : "";
            }
        }

        public static string AssemblyProductName
        {
            get
            {
                var attribute = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false).FirstOrDefault() as AssemblyProductAttribute;
                return attribute != null ? attribute.Product : "";
            }
        }

        public static string AssemblyCopyright
        {
            get
            {
                var attribute = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false).FirstOrDefault() as AssemblyCopyrightAttribute;
                return attribute != null ? attribute.Copyright : "";
            }
        }

        public static string AssemblyCompany
        {
            get
            {
                var attribute = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false).FirstOrDefault() as AssemblyCompanyAttribute;
                return attribute != null ? attribute.Company : "";
            }
        }

        public static string AssemblyVersion
        {
            get
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                return version;
            }
        }

        public static string AssemblyProductVersion
        {
            get
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                return string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
            }
        }

        public static string AssemblyLocation
        {
            get
            {
                var location = Assembly.GetExecutingAssembly().Location;
                return location;
            }
        }

        public static string AssemblyDirectoryName
        {
            get
            {
                var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                return location;
            }
        }

        public static void ExtractFileFromAssembly(string resourceName, string path)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var outputFileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            var resouceStream = currentAssembly.GetManifestResourceStream(resourceName);
            resouceStream.CopyTo(outputFileStream);
            resouceStream.Close();
            outputFileStream.Close();
        }
    }
}
