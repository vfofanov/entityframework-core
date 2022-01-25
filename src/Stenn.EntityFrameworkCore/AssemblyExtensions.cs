using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Stenn.EntityFrameworkCore
{
    public static class AssemblyExtensions
    {
        public static bool ResExists(this Assembly assembly, string embeddedResFileName)
        {
            var info = assembly.GetManifestResourceInfo(embeddedResFileName);
            return info != null;
        }
        
        public static Stream ResReadStream(this Assembly assembly, string embeddedResFileName)
        {
            var stream = assembly.GetManifestResourceStream(embeddedResFileName);
            if (stream == null)
            {
                throw new ArgumentException($"Can't find embedded resource with name '{embeddedResFileName}' in assembly '{assembly.FullName}'");
            }
            return stream;
        }

        public static string ResRead(this Assembly assembly, string embeddedResFileName, Encoding? encoding = null)
        {
            using var stream = assembly.ResReadStream(embeddedResFileName);
            using var reader = new StreamReader(stream, encoding, true, -1, false);
            return reader.ReadToEnd();
        }
    }
}