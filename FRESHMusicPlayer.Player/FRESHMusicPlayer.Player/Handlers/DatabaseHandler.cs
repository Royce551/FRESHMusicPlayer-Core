using FRESHMusicPlayer.Utilities;
using Lite = LiteDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FRESHMusicPlayer.Handlers
{
    public static class DatabaseHandler
    {
        public static readonly int DatabaseVersion = 1;
        public static readonly string DatabasePath;
        static DatabaseHandler()
        {
            DatabasePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\FRESHMusicPlayer";
            DatabasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FRESHMusicPlayer");
        }
        #region v1
        /// <summary>
        /// Returns all of the tracks in the database.
        /// </summary>
        /// <returns>A list of file paths in the database.</returns>
        public static List<string> ReadSongs()
        {
            if (!File.Exists(DatabasePath + "\\database.json"))
            {
                Directory.CreateDirectory(DatabasePath);
                File.WriteAllText(DatabasePath + "\\database.json", $"{{\"Version\":{DatabaseVersion},\"Songs\":[]}}");
            }
            using (var file = File.OpenText(DatabasePath + "\\database.json")) // Read json file
            {
                var serializer = new JsonSerializer();
                var database = (DatabaseFormat)serializer.Deserialize(file, typeof(DatabaseFormat));
                return database.Songs;
            }
        }

        public static void ImportSong(string filepath)
        {
            var database = ReadSongs();

            database.Add(filepath); // Add the new song in
            var format = new DatabaseFormat
            {
                Version = 1,
                Songs = new List<string>()
            };
            format.Songs = database;

            using (var file = File.CreateText(DatabasePath + "\\database.json"))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, format);
            }

        }
        public static void ImportSong(string[] filepath)
        {
            var database = ReadSongs();

            database.AddRange(filepath);
            var format = new DatabaseFormat
            {
                Version = 1,
                Songs = new List<string>()
            };
            format.Songs = database;

            using (var file = File.CreateText(DatabasePath + "\\database.json"))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, format);
            }

        }
        public static void ImportSong(List<string> filepath)
        {
            var database = ReadSongs();

            database.AddRange(filepath);
            var format = new DatabaseFormat
            {
                Version = 1,
                Songs = new List<string>()
            };
            format.Songs = database;

            using (var file = File.CreateText(DatabasePath + "\\database.json"))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, format);
            }

        }
        public static void ImportSong(IList<string> filepath)
        {
            var database = ReadSongs();

            database.AddRange(filepath);
            var format = new DatabaseFormat
            {
                Version = 1,
                Songs = new List<string>()
            };
            format.Songs = database;

            using (var file = File.CreateText(DatabasePath + "\\database.json"))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, format);
            }

        }
        public static void DeleteSong(string filepath)
        {
            var database = ReadSongs();
            database.Remove(filepath);
            var format = new DatabaseFormat
            {
                Version = 1,
                Songs = database
            };


            using (var file = File.CreateText(DatabasePath + "\\database.json"))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, format);
            }
        }
        public static void ClearLibrary()
        {
            if (File.Exists(DatabasePath + "\\database.json"))
            {
                File.Delete(DatabasePath + "\\database.json");
                File.WriteAllText(DatabasePath + "\\database.json", @"{""Version"":1,""Songs"":[]}");
            }
        }
        #endregion

    }

}
