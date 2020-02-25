using MusicShop.DAL;
using MusicShop.Infrastructure;
using MusicShop.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MusicShop.Controllers
{
    public class CartController : Controller
    {
        private ShoppingCartManager shoppingCartManager;
        private ISessionManager sessionManager { get; set; }
        private StoreContext db;

        public CartController()
        {
            sessionManager = new SessionManager();
            db = new StoreContext();
            shoppingCartManager = new ShoppingCartManager(sessionManager, db);
        }

        // GET: Cart
        public ActionResult Index()
        {
            var cartItem = shoppingCartManager.GetCart();
            var cartTotalPrince = shoppingCartManager.GetCartTotalPrince();

            CartViewModel cartVM = new CartViewModel() { CartItems = cartItem, TotalPrice = cartTotalPrince };

            return View(cartVM);
        }

        public ActionResult AddToCart(int id)
        {
            shoppingCartManager.AddToCart(id);
            return RedirectToAction("Index");
        }

        public int GetCartItemsCount()
        {
            return shoppingCartManager.GetCartItemsCount();
        }

        public ActionResult RemoveFromCart(int albumID)
        {
            int itemCount = shoppingCartManager.RemoveFromCart(albumID);
            int cartItemsCount = shoppingCartManager.GetCartItemsCount();
            decimal cartTotal = shoppingCartManager.GetCartTotalPrince();

            var result = new CartRemoveViewModel
            {
                RemoveItemId = albumID,
                RemoveItemCount = itemCount,
                CartTotal = cartTotal,
                CartItemsCount = cartItemsCount
            };

            return Json(result);
        }
    }
}