using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MusicShop.Infrastructure
{
    public static class UrlHelpers
    {
        public static string GenreIconPath(this UrlHelper helper, string genreIconFilename)
        {
            var genreIconFolder = AppConfig.GenreIconsFolderRelative;
            var path = Path.Combine(genreIconFolder, genreIconFilename);
            var absolutePath = helper.Content(path);
            return absolutePath;
        }

        public static string AlbumCoverPath(this UrlHelper helper, string albumFilename)
        {
            var albumCoverFolder = AppConfig.PhotosFolderRelative;
            var path = Path.Combine(albumCoverFolder, albumFilename);
            var absolutePath = helper.Content(path);
            return absolutePath;
        }

        public static string ImagesPath(this UrlHelper helper, string imageFilename)
        {
            var imageFolder = AppConfig.ImagesFolderRelative;
            var path = Path.Combine(imageFolder, imageFilename);
            var absolutePath = helper.Content(path);
            return absolutePath;
        }
    }
}