using Microsoft.AspNet.Identity.EntityFramework;
using MusicShop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MusicShop.DAL
{
    public class StoreContext:IdentityDbContext<ApplicationUser>
    {
        public StoreContext() : base("StoreContext")
        {
        }

        static StoreContext()
        {
            Database.SetInitializer(new StoreInitializer());
        }

        public static StoreContext Create()
        {
            return new StoreContext();
        }

        public virtual DbSet<Album> Albums { get; set; }

        public virtual DbSet<Genre> Genres { get; set; }

        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<OrderItem> OrderItems { get; set; }
    }
}