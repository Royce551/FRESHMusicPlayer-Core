using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace FRESHMusicPlayer.Backends
{
    static class AudioBackendFactory
    {
        private readonly static ContainerConfiguration config = new ContainerConfiguration();
        private readonly static CompositionHost container;

        private static IEnumerable<Assembly> LoadAssemblies(IEnumerable<string> paths)
        {
            foreach (var file in paths)
            {
                Assembly assembly;
                try
                {
                    assembly = Assembly.LoadFrom(file);
                    
                    // Get types now, so that if it throws
                    // an exception, it will happen here and get caught,
                    // rather than in GetExports where it can't be.
                    assembly.GetTypes();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                    continue;
                }
                yield return assembly;
            }
        }

        private static void AddDirectory(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
 
            config.WithAssemblies(LoadAssemblies(Directory.GetFiles(path, "*.dll")));
        }

        static AudioBackendFactory()
        {
            AddDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backends"));
            AddDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FRESHMusicPlayer", "Backends"));
            config.WithAssembly(typeof(AudioBackendFactory).Assembly);
            container = config.CreateContainer();
        }

        public static async Task<IAudioBackend> CreateBackendAsync(string filename)
        {
            var problems = new List<(BackendLoadResult, Exception)>();
            foreach (var lazybackend in container.GetExports<Lazy<IAudioBackend>>())
            {
                IAudioBackend backend = lazybackend.Value;
                try
                {
                    var result = await backend.LoadSongAsync(filename);
                    if (result != BackendLoadResult.OK)
                    {
                        problems.Add((result, null));
                        backend.Dispose();
                    }
                    else return backend;
                }
                catch (Exception e)
                {
                    problems.Add((BackendLoadResult.UnknownError, e));
                    backend.Dispose();
                }
            }
            throw new Exception($"A backend couldn't be found to load this file\n{string.Join("\n", problems)}");
        }
    }
}

