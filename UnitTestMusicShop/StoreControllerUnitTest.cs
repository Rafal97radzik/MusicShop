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
using Newtonsoft.Json;

namespace UnitTestMusicShop
{
    [TestClass]
    public class StoreControllerUnitTest
    {
        [TestMethod]
        public void Index_ReturnsView()
        {
            var auto = new AutoMoqer();

            var controller = auto.Create<StoreController>();

            var result = controller.Index();

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Details_ReturnsAlbum()
        {
            var db = new Mock<StoreContext>();
            var expected = new Album { AlbumId = 2, AlbumTitle = "TestAlbum2", ArtistName = "TestArtist2", GenreId=2 };

            var albumsData = new List<Album>
            {
                new Album{AlbumId=1, AlbumTitle="TestAlbum1", ArtistName="TestArtist1", GenreId=1},
                expected,
                new Album{AlbumId=3, AlbumTitle="TestAlbum3", ArtistName="TestArtist3", GenreId=3}
            }.AsQueryable();

            var albums = new Mock<DbSet<Album>>();
            albums.As<IQueryable<Album>>().Setup(a => a.Provider).Returns(albumsData.Provider);
            albums.As<IQueryable<Album>>().Setup(a => a.Expression).Returns(albumsData.Expression);
            albums.As<IQueryable<Album>>().Setup(a => a.ElementType).Returns(albumsData.ElementType);
            albums.As<IQueryable<Album>>().Setup(a => a.GetEnumerator()).Returns(albumsData.GetEnumerator());

            db.Setup(d => d.Albums).Returns(albums.Object);

            var controller = new StoreController(db.Object);

            var result = controller.Details(2) as ViewResult;

            var viewModel = result.ViewData.Model as Album;

            Assert.AreEqual(expected, viewModel);
        }

        [TestMethod]
        public void AlbumsSuggestions_ReturnsJson()
        {
            var db = new Mock<StoreContext>();

            var albumsData = new List<Album>
            {
                new Album{IsHidden=false, AlbumTitle="TestTitle", ArtistName="TestArtist"}
            }.AsQueryable();

            var albums = new Mock<DbSet<Album>>();
            albums.As<IQueryable<Album>>().Setup(a => a.Provider).Returns(albumsData.Provider);
            albums.As<IQueryable<Album>>().Setup(a => a.Expression).Returns(albumsData.Expression);
            albums.As<IQueryable<Album>>().Setup(a => a.ElementType).Returns(albumsData.ElementType);
            albums.As<IQueryable<Album>>().Setup(a => a.GetEnumerator()).Returns(albumsData.GetEnumerator());

            db.Setup(d => d.Albums).Returns(albums.Object);

            var controller = new StoreController(db.Object);

            var result = controller.AlbumsSuggestions("Title");

            Assert.IsInstanceOfType(result, typeof(JsonResult));
        }

        [TestMethod]
        public void AlbumsSuggestions_Returns5Albums()
        {
            var db = new Mock<StoreContext>();

            var albumsData = new List<Album>
            {
                new Album{IsHidden=true, AlbumTitle="TestTitle1", ArtistName="TestArtist1"},
                new Album{IsHidden=false, AlbumTitle="TestTitle2", ArtistName="TestArtist2"},
                new Album{IsHidden=false, AlbumTitle="TestTitle3", ArtistName="TestArtist3"},
                new Album{IsHidden=true, AlbumTitle="TestTitle4", ArtistName="TestArtist4"},
                new Album{IsHidden=false, AlbumTitle="TestTitle5", ArtistName="TestArtist5"},
                new Album{IsHidden=false, AlbumTitle="TestTitle6", ArtistName="TestArtist6"},
                new Album{IsHidden=false, AlbumTitle="TestTitle7", ArtistName="TestArtist7"},
                new Album{IsHidden=false, AlbumTitle="TestTitle8", ArtistName="TestArtist8"},
                new Album{IsHidden=true, AlbumTitle="TestTitle9", ArtistName="TestArtist9"},
                new Album{IsHidden=false, AlbumTitle="TestTitle10", ArtistName="TestArtist10"}
            }.AsQueryable();

            var albums = new Mock<DbSet<Album>>();
            albums.As<IQueryable<Album>>().Setup(a => a.Provider).Returns(albumsData.Provider);
            albums.As<IQueryable<Album>>().Setup(a => a.Expression).Returns(albumsData.Expression);
            albums.As<IQueryable<Album>>().Setup(a => a.ElementType).Returns(albumsData.ElementType);
            albums.As<IQueryable<Album>>().Setup(a => a.GetEnumerator()).Returns(albumsData.GetEnumerator());

            db.Setup(d => d.Albums).Returns(albums.Object);

            var controller = new StoreController(db.Object);

            var result = controller.AlbumsSuggestions("Title") as JsonResult;

            var list =result.Data as IEnumerable<dynamic>;

            Assert.AreEqual(5 ,list.Count());
        }
    }
}
