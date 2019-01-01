using System;
using System.Collections.Generic;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace LiquidVictor.Output.RevealJs.Layout.Service
{
    public static class ContainerConfigurationExtensions
    {
        public static ContainerConfiguration WithAssembliesInRoot(this ContainerConfiguration configuration, AttributedModelProvider conventions)
        {
            string rootPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            return configuration.WithAssembliesInPath(rootPath, conventions, SearchOption.TopDirectoryOnly);
        }

        //public static ContainerConfiguration WithAssembliesInPath(this ContainerConfiguration configuration, 
        //    string path, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        //{
        //    return configuration.WithAssembliesInPath(path, null, searchOption);
        //}

        public static ContainerConfiguration WithAssembliesInPath(this ContainerConfiguration configuration, 
            string path, AttributedModelProvider conventions, 
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var assemblies = Directory
                .GetFiles(path, "*.dll", searchOption)
                .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath);

            return configuration.WithAssemblies(assemblies, conventions);
        }
    }
}
