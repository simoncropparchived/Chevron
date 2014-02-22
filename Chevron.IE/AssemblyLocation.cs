using System;
using System.IO;

namespace Chevron
{
    static class AssemblyLocation
    {
        public static string CurrentDirectory
        {
            get
            {
                
                //Use codebase because location fails for unit tests.
                var assembly = typeof (AssemblyLocation).Assembly;
                var uri = new UriBuilder(assembly.CodeBase);
                var currentAssemblyPath = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(currentAssemblyPath);
            }
        }
    }
}