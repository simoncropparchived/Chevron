using System;
using System.IO;

namespace Chevron
{
    static class AssemblyLocation
    {
        static AssemblyLocation()
        {
            //Use codebase because location fails for unit tests.
            var assembly = typeof(AssemblyLocation).Assembly;
            var uri = new UriBuilder(assembly.CodeBase);
            var currentAssemblyPath = Uri.UnescapeDataString(uri.Path);
            CurrentDirectory = Path.GetDirectoryName(currentAssemblyPath);
        }

        public static string CurrentDirectory;
    }
}