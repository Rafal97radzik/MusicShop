using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MusicShop.Infrastructure
{
    public class AppConfig
    {
        private static string _genreIconsFolderRelative = ConfigurationManager.AppSettings["GenreIconsFolder"];
        private static string _photosFolderRelative = ConfigurationManager.AppSettings["PhotosFolder"];
        private static string _imagesFolderRelative = ConfigurationManager.AppSettings["ImagesFolder"];

        public static string GenreIconsFolderRelative { get { return _genreIconsFolderRelative; } }
        public static string PhotosFolderRelative { get { return _photosFolderRelative; } }
        public static string ImagesFolderRelative { get { return _imagesFolderRelative; } }
    }
}