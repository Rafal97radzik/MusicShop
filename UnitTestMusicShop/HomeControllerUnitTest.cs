using System;
using System.Data.Entity;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MusicShop.Controllers;
using MusicShop.DAL;
using MusicShop.Infrastructure;
using MusicShop.Models;
using System.Linq;
using AutoMoq;
using System.Collections.Generic;
using MusicShop.ViewModel;

namespace UnitTestMusicShop
{
    [TestClass]
    public class HomeControllerUnitTest
    {

        [TestMethod]
        public void StaticContent_ReturnsView()
        {
            var auto = new AutoMoqer();

            var controller = auto.Create<HomeController>();

            var result = controller.StaticContent("Onas");

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Index_Returns3BestsellersAnd3NewItems()
        {
            var db = new Mock<StoreContext>();
            var cache = new Mock<ICacheProvider>();
            var genres = new Mock<DbSet<Genre>>();

            var genresData = new List<Genre>
            {
               new Genre()
            }.AsQueryable();

            genres.As<IQueryable<Genre>>().Setup(g => g.Provider).Returns(genresData.Provider);
            genres.As<IQueryable<Genre>>().Setup(g => g.Expression).Returns(genresData.Expression);
            genres.As<IQueryable<Genre>>().Setup(g => g.ElementType).Returns(genresData.ElementType);
            genres.As<IQueryable<Genre>>().Setup(g => g.GetEnumerator()).Returns(genresData.GetEnumerator());

            db.Setup(d => d.Genres).Returns(genres.Object);

            var albumsData = new List<Album>
            {
                new Album{IsBestseller=true, IsHidden=false},
                new Album{IsBestseller=true, IsHidden=false},
                new Album{IsBestseller=true, IsHidden=false},
                new Album{IsBestseller=false, IsHidden=false},
                new Album{IsBestseller=true, IsHidden=false},
                new Album{IsBestseller=false, IsHidden=true},
            }.AsQueryable();

            var albums = new Mock<DbSet<Album>>();
            albums.As<IQueryable<Album>>().Setup(a=>a.Provider).Returns(albumsData.Provider);
            albums.As<IQueryable<Album>>().Setup(a=>a.Expression).Returns(albumsData.Expression);
            albums.As<IQueryable<Album>>().Setup(a=>a.ElementType).Returns(albumsData.ElementType);
            albums.As<IQueryable<Album>>().Setup(a=>a.GetEnumerator()).Returns(albumsData.GetEnumerator());

            db.Setup(d => d.Albums).Returns(albums.Object);

            var controller = new HomeController(db.Object, cache.Object);

            var result = controller.Index() as ViewResult;

            var viewModel = result.ViewData.Model as HomeViewModel;

            Assert.IsTrue(viewModel.Bestsellers.Count() == 3);
            Assert.IsTrue(viewModel.NewArrivals.Count() == 3);
        }
    }
}
