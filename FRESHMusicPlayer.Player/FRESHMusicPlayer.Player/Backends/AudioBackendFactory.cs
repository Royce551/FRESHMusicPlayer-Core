using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FRESHMusicPlayer.Backends
{
    /// <summary>
    /// A lower level class for directly getting audio backends.
    /// You should probably use the Player class unless there's a reason you need more control.
    /// </summary>
    public static class AudioBackendFactory
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

        /// <summary>
        /// Adds a directory where FMP will search for audio backends
        /// </summary>
        /// <param name="path">The file path of the directory</param>
        public static void AddDirectory(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
 
            config.WithAssemblies(LoadAssemblies(Directory.GetFiles(path, "*.dll")));
        }

        static AudioBackendFactory()
        {
            AddDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backends"));
            AddDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create), "FRESHMusicPlayer", "Backends"));
            config.WithAssembly(typeof(AudioBackendFactory).Assembly);
            container = config.CreateContainer();
        }

        /// <summary>
        /// Queries the audio backend system for the most appropiate backend for the supplied path
        /// </summary>
        /// <param name="filename">The file path to play</param>
        /// <returns>The appropiate backend</returns>
        /// <exception cref="Exception">Thrown if no backend could be found</exception>
        public static async Task<(IAudioBackend backend, PlaybackExceptionEventArgs problems)> CreateAndLoadBackendAsync(string filename)
        {
            var exceptions = new Dictionary<string, Exception>();
            var problems = new Dictionary<string, BackendLoadResult>();

            foreach (var lazybackend in container.GetExports<Lazy<IAudioBackend>>())
            {
                IAudioBackend backend = lazybackend.Value;
                try
                {
                    var result = await backend.LoadSongAsync(filename);
                    if (result != BackendLoadResult.OK)
                    {
                        problems.Add(backend.ToString(), result);
                        backend.Dispose();
                    }
                    else return (backend, null);
                }
                catch (Exception e)
                {
                    problems.Add(lazybackend.ToString(), BackendLoadResult.UnknownError);
                    exceptions.Add(lazybackend.ToString(), e);
                    backend.Dispose();
                }
            }
            return (null, new PlaybackExceptionEventArgs(exceptions, problems));
        }
    }
}

