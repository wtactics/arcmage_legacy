using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ArtworkIndexer
{
    class Program
    {
        static void Main(string[] args)
        {
            var basePath = args[0];
            var mediaItems = new List<MediaItem>();
            var dirs = new HashSet<string>(GetDirectories(basePath)).ToList(); 
            var counter = 0;
            foreach (var dir in dirs)
            {
                var files = Directory.GetFiles(dir);
                foreach (var file in files)
                {
                    if (file.EndsWith(".png") && !file.EndsWith("_thumbnail.png"))
                    {
                       
                        var location = Path.GetDirectoryName(file);
                        if (location == null) continue;
                        ;
                        var filename = Path.GetFileNameWithoutExtension(file);

                        var psdFile = Path.Combine(location, $"{filename}.psd");
                        var thumbnail = Path.Combine(location, $"{filename}_thumbnail.png");
                        var oraFile = Path.Combine(location, $"{filename}.ora");
                        var xcfFile = Path.Combine(location, $"{filename}.xcf");
                        var tagsFile = Path.Combine(location, "tags.txt");

                        var mediaItem = new MediaItem();
                        mediaItem.FileName = filename;
                        mediaItem.Url = GetUrl(basePath, file);

                        // Creating Tags
                        var tags = filename;
                        if (File.Exists(tagsFile))
                        {
                            var line = File.ReadAllText(tagsFile);
                            tags += " " + line;
                        }
                        mediaItem.Tags = tags;

                        if (File.Exists(thumbnail))
                        {
                            mediaItem.ThumbNail = GetUrl(basePath, thumbnail);
                        }
                        else
                        {
                            mediaItem.ThumbNail = mediaItem.Url;
                        }
                        if (File.Exists(psdFile))
                        {
                            mediaItem.Psd = GetUrl(basePath, psdFile);
                        }
                        if (File.Exists(oraFile))
                        {
                            mediaItem.Psd = GetUrl(basePath, oraFile);
                        }
                        if (File.Exists(xcfFile))
                        {
                            mediaItem.Psd = GetUrl(basePath, xcfFile);
                        }

                        mediaItem.Id = "" + counter;
                        mediaItems.Add(mediaItem);
                        counter++;
                    }
                }
            }

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var json = $"var mediaitems ={JsonConvert.SerializeObject(mediaItems, settings)};";
            File.WriteAllText("mediaitems.js", json);

        }

        public static string GetUrl(string basePath, string file)
        {
            return System.Uri.EscapeUriString(file.Substring(basePath.Length).Replace("\\", "/"));
        }

        public static List<string> GetDirectories(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.AllDirectories)
        {
            if (searchOption == SearchOption.TopDirectoryOnly) return Directory.GetDirectories(path, searchPattern).ToList();

            var directories = new List<string>(GetDirectoriesSafe(path, searchPattern));

            for (var i = 0; i < directories.Count; i++)
                directories.AddRange(GetDirectories(directories[i], searchPattern));

            return directories;
        }

        private static List<string> GetDirectoriesSafe(string path, string searchPattern)
        {
            try
            {
                return Directory.GetDirectories(path, searchPattern).ToList();
            }
            catch (UnauthorizedAccessException)
            {
                return new List<string>();
            }
        }

    }
}
