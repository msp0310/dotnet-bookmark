using DotNetBookmark.ValueObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DotNetBookmark.Manager
{
    public sealed class BookmarkManager
        : IBookmarkManager
    {
        /// <summary>
        /// Resource File Name.
        /// </summary>
        private static readonly string ResourceFileName = "path.json";

        /// <summary>
        /// Bookmarks
        /// </summary>
        private Dictionary<string, Bookmark> Bookmarks;

        public void Add(string name, string path)
        {
            path = Path.GetFullPath(path);
            var bookmark = new Bookmark
            {
                Name = name,
                Path = path,
                CreateDateTime = DateTime.Now
            };
            if (Bookmarks.ContainsKey(name))
            {
                Bookmarks[name] = bookmark;
            }
            else
            {
                Bookmarks.Add(name, bookmark);
            }
        }

        public IReadOnlyCollection<Bookmark> GetAll()
            => new ReadOnlyCollection<Bookmark>(Bookmarks.Select(x => x.Value).OrderByDescending(x => x.CreateDateTime).ToList());

        public Bookmark Get(string name)
        {
            if (!string.IsNullOrEmpty(name) &&
                Bookmarks.TryGetValue(name, out var bookmark))
            {
                return bookmark;
            }

            throw new KeyNotFoundException();
        }

        public async Task Load()
        {
            var resourcePath = GetResourcePath();
            var contents = Utf8Json.JsonSerializer.ToJsonString(Enumerable.Empty<string>());
            if (File.Exists(resourcePath))
            {
                contents = await File.ReadAllTextAsync(resourcePath);
            }
            var bookmarks = Utf8Json.JsonSerializer.Deserialize<List<Bookmark>>(contents);
            this.Bookmarks = bookmarks.ToDictionary(x => x.Name, x => x);
        }

        public void Remove(string name)
        {
            Bookmarks.Remove(name);
        }

        public void Clear()
        {
            Bookmarks.Clear();
        }

        public async Task Save()
        {
            await File.WriteAllTextAsync(GetResourcePath(), Utf8Json.JsonSerializer.PrettyPrint(Utf8Json.JsonSerializer.Serialize(this.Bookmarks.Select(x => x.Value).ToList())));
        }

        /// <summary>
        /// Get Resource Path.
        /// </summary>
        /// <returns></returns>
        private string GetResourcePath()
        {
            var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Join(directoryName, ResourceFileName);
        }
    }
}
