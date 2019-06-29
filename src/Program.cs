using DotNetBookmark.Manager;
using DotNetBookmark.ValueObjects;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TextCopy;

namespace DotNetBookmark
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                return Wain(args);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }

            return 0;
        }

        private static int Wain(string[] args)
        {
            var app = new CommandLineApplication(throwOnUnexpectedArg: false);
            app.Description = "Simple Bookmark Console Application.";
            app.Command("add", c =>
            {
                c.Description = "add bookmark.";
                var name = c.Argument("name", "name");
                var path = c.Argument("path", "path");
                c.OnExecute(() => Add(name.Value, path.Value));
            });
            app.Command("get", c =>
            {
                c.Description = "get bookmark.";
                var name = c.Argument("name", "name");
                c.OnExecute(() => Get(name.Value));
            });
            app.Command("clip", c =>
            {
                c.Description = "clip bookmark.";
                var name = c.Argument("name", "name");
                c.OnExecute(() => Clip(name.Value));
            });
            app.Command("cd", c =>
            {
                c.Description = "change directory from bookmarking path.";
                var name = c.Argument("name", "name");
                c.OnExecute(() => ChangeDirectory(name.Value));
            });
            app.Command("list", c =>
            {
                c.Description = "show bookmarks.";
                c.OnExecute(() => ShowBookmarks());
            });
            app.Command("clear", c =>
            {
                c.Description = "clear bookmarks.";
                c.OnExecute(() => Clear());
            });

            app.OnExecute(() =>
            {
                app.ShowHelp();
            });

            return app.Execute(args);
        }
        public enum Operation
        {
            Add,
            Get,
            Clip,
            ChangeDirectory,
            Remove,
            Clear,
            List
        }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<int> Add(string name, string path)
            => await Execute(Operation.Add, name, path);

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static async Task<int> Get(string name)
            => await Execute(Operation.Get, name);

        /// <summary>
        /// Clip
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static async Task<int> Clip(string name)
            => await Execute(Operation.Clip, name);

        /// <summary>
        /// Change Directory
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static async Task<int> ChangeDirectory(string name)
            => await Execute(Operation.ChangeDirectory, name);

        /// <summary>
        /// Remove
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static async Task<int> Remove(string name)
            => await Execute(Operation.Remove, name);

        /// <summary>
        /// Clip
        /// </summary>
        /// <returns></returns>
        public static async Task<int> Clear()
            => await Execute(Operation.Clear);

        /// <summary>
        /// Show Bookmarks.
        /// </summary>
        /// <returns></returns>
        public static async Task<int> ShowBookmarks()
            => await Execute(Operation.List);

        /// <summary>
        /// Execute Command
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private static async Task<int> Execute(Operation command, string name = null, string path = null)
        {
            var bookmarkManager = new BookmarkManager();
            await bookmarkManager.Load();
            switch (command)
            {
                case Operation.Add:
                    bookmarkManager.Add(name, path);
                    break;

                case Operation.Remove:
                    bookmarkManager.Remove(name);
                    break;

                case Operation.Get:
                    try
                    {
                        Console.WriteLine(bookmarkManager.Get(name).Path);
                    }
                    catch
                    {
                        Console.Error.WriteLine("Not Exists Name.");
                    }
                    break;

                case Operation.Clip:
                    try
                    {
                        Clipboard.SetText(bookmarkManager.Get(name).Path);
                    }
                    catch
                    {
                        Console.Error.WriteLine("Not Exists Name.");
                    }
                    break;

                case Operation.ChangeDirectory:
                    try
                    {
                        Environment.CurrentDirectory = bookmarkManager.Get(name).Path;
                    }
                    catch
                    {
                        Console.Error.WriteLine("Not Exists Name.");
                    }
                    break;

                case Operation.List:
                    var bookmarks = bookmarkManager.GetAll();
                    if (bookmarks.Any())
                    {
                        OutputBookmarks(bookmarks);
                    }
                    else
                    {
                        Console.WriteLine("No Data.");
                    }
                    break;

                case Operation.Clear:
                    bookmarkManager.Clear();
                    break;
            }
            await bookmarkManager.Save();

            return 1;
        }

        /// <summary>
        /// Output Bookarmakrs.
        /// </summary>
        /// <param name="bookmarks"></param>
        private static void OutputBookmarks(IReadOnlyCollection<Bookmark> bookmarks)
        {
            var maxNameLength = bookmarks.Select(x => x.Name.Length).Max();
            var maxPathLength = bookmarks.Select(x => x.Path.Length).Max();
            var maxCreateDateTimeLength = bookmarks.Select(x => x.CreateDateTime.ToString().Length).Max();
            Console.WriteLine(string.Join(" | ", new[] { "Name".PadRight(maxNameLength), "Path".PadRight(maxPathLength), "CreateDateTime".PadRight(maxCreateDateTimeLength) }));

            var borderLength = maxNameLength + maxPathLength + maxCreateDateTimeLength + 10;
            Enumerable.Range(0, borderLength).ForEach(i => Console.Write("-"));
            Console.WriteLine();

            foreach (var bookmark in bookmarks)
            {
                Console.WriteLine(string.Join(" | ", new object[] { bookmark.Name.PadRight(maxNameLength), bookmark.Path.PadRight(maxPathLength), bookmark.CreateDateTime.ToString().PadRight(maxCreateDateTimeLength) }));
            }
        }
    }
}
