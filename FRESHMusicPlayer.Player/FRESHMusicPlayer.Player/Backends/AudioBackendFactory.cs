using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace FRESHMusicPlayer.Backends
{
    static class AudioBackendFactory
    {
        private static ContainerConfiguration config = new ContainerConfiguration();
        private static CompositionHost container;

        private static List<string> directories = new List<string>();

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
            //AppDomain.CurrentDomain.AppendPrivatePath(path);
            try
            {
                config.WithAssemblies(LoadAssemblies(Directory.GetFiles(path, "*.dll")));
            }
            catch (DirectoryNotFoundException)
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch
                {
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        static AudioBackendFactory()
        {
            AddDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backends"));
            AddDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FRESHMusicPlayer", "Backends"));
            config.WithAssembly(typeof(AudioBackendFactory).Assembly);
            container = config.CreateContainer();
        }

        public static IAudioBackend CreateBackend(string filename)
        {
            var exlist = new List<Exception>();
            foreach (var lazybackend in container.GetExports<Lazy<IAudioBackend>>())
            {
                IAudioBackend backend = null;
                try
                {
                    backend = lazybackend.Value;
                    backend.LoadSong(filename);
                    return backend;
                }
                catch (Exception ex)
                {
                    try
                    {
                        backend.Dispose();
                    }
                    catch
                    {
                    }
                    exlist.Add(ex);
                }
            }
            throw new Exception($"No backend could be found to play {filename}.\n\n{String.Join("\n\n", exlist)}\n");
        }
    }
}

