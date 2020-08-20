using System;
using System.Collections.Generic;
using System.Composition;

namespace FRESHMusicPlayer.Backends
{
    class AudioBackendFactory
    {
        [ImportMany]
        private IEnumerable<Lazy<IAudioBackend>> backends { get; set; }
        
        ///<summary>
        /// The exceptions that were raised by rejected backends.
        ///</summary>
        public IEnumerable<Exception> Exceptions { get; private set; } = null;
        
        public IAudioBackend CreateBackend(string filename)
        {
            var exlist = new List<Exception>();
            Exceptions = exlist;
            foreach (var lazybackend in backends)
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
                        backend?.Dispose();
                    }
                    catch
                    {
                    }
                    exlist.Add(ex);
                }
            }
            throw new Exception($"No backend could be found to play {filename}.\n\n{String.Join("\n\n", Exceptions)}\n");
        }
    }
}

