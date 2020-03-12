using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicShop.Controllers;
using MusicShop.ViewModel;
using MusicShop.Infrastructure;
using System.Collections.Generic;
using MusicShop.Models;
using Moq;
using System.Data.Entity;
using MusicShop.DAL;

namespace MusicShop.Tests
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void IndexAction_Returns3BestsellersAnd3NewItems()
        {
            //Arrange
            var data = new List<Album>
            {
                new Album{IsBestseller=true, IsHidden=false},
                new Album{IsBestseller=true, IsHidden=false},
                new Album{IsBestseller=true, IsHidden=false},
                new Album{IsBestseller=false, IsHidden=false},
                new Album{IsBestseller=false, IsHidden=false},
                new Album{IsBestseller=true, IsHidden=true},
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Album>>();
            mockSet.As<IQueryable<Album>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Album>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Album>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Album>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<StoreContext>();
            mockContext.Setup(c => c.Albums).Returns(mockSet.Object);

            var mockCache = new Mock<ICacheProvider>();

            var controller = new HomeController(mockContext.Object, mockCache.Object);

            //Act

            var result = controller.Index() as ViewResult;

            //Assert

            var viewModel = result.ViewData.Model as HomeViewModel;
            Assert.IsTrue(viewModel.Bestsellers.Count() == 3);
            Assert.IsTrue(viewModel.NewArrivals.Count() == 3);
        }

        [TestMethod]
        public void StaticContentAction_WithViewNamePassed_ReturnsViewWithTheSameName()
        {
            var mockCache = new Mock<ICacheProvider>();
            var mockContext = new Mock<StoreContext>();

            var controller = new HomeController(mockContext.Object, mockCache.Object);

            var result = controller.StaticContent("TestView") as ViewResult;

            Assert.AreEqual("TestView", result.ViewName);
        }
    }
}
