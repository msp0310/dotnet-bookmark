using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetBookmark.ValueObjects
{
    public class Bookmark
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Path
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Create Date Time
        /// </summary>
        public DateTime CreateDateTime { get; set; }
    }
}
