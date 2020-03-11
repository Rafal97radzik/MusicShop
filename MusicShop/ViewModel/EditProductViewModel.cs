using MusicShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicShop.ViewModel
{
    public class EditProductViewModel
    {
        public Album Album { get; set; }
        public IEnumerable<Genre> Genres { get; set; }
        public bool? ConfirmSuccess { get; set; }
    }
}