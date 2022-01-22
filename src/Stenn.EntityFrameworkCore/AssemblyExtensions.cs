using System;
using System.IO;
using System.Reflection;

namespace Stenn.EntityFrameworkCore
{
    public static class AssemblyExtensions
    {
        public static Stream ReadResStream(this Assembly assembly, string embeddedResFileName)
        {
            var stream = assembly.GetManifestResourceStream(embeddedResFileName);
            if (stream == null)
            {
                throw new ArgumentException($"Can't find embedded resource with name '{embeddedResFileName}' in assembly '{assembly.FullName}'");
            }
            return stream;
        }

        public static string ReadRes(this Assembly assembly, string embeddedResFileName)
        {
            using var stream = assembly.ReadResStream(embeddedResFileName);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}