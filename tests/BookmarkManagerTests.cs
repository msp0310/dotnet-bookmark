using DotNetBookmark.Manager;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace dotnet_bookmark.Tests
{
    public class BookmarkManagerTests : IDisposable
    {
        BookmarkManager BookmarkManager;

        public BookmarkManagerTests()
        {
            BookmarkManager = new BookmarkManager();
            BookmarkManager.Load().Wait();
        }

        public void Dispose()
        {
            BookmarkManager.Clear();
        }

        [Fact]
        public void Get_Exists()
        {
            BookmarkManager.Add("test", "c:\\");
            Assert.Equal("c:\\", BookmarkManager.Get("test").Path);
        }

        [Fact]
        public void Get_Not_Exists()
        {
            Assert.Throws<KeyNotFoundException>(() =>
            {
                BookmarkManager.Get("not_exist_name");
            });
        }

        [Fact]
        public void GetAll()
        {
            BookmarkManager.Add("test1", "c:\\");
            BookmarkManager.Add("test2", "c:\\");
            BookmarkManager.Add("test3", "c:\\");
            Assert.Equal(3, BookmarkManager.GetAll().Count);
        }

        [Fact]
        public void Clear()
        {
            BookmarkManager.Add("test", "c:\\");
            BookmarkManager.Clear();
            Assert.Equal(0, BookmarkManager.GetAll().Count);
        }
    }
}
