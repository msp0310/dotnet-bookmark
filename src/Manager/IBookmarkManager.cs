using DotNetBookmark.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetBookmark.Manager
{
    public interface IBookmarkManager
    {
        Task Load();
        Task Save();
        Bookmark Get(string name);
        void Add(string name, string path);
        void Remove(string name);
        void Clear();
    }
}
