using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MusicShop.App_Start;
using MusicShop.DAL;
using MusicShop.Infrastructure;
using MusicShop.Models;
using MusicShop.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public async Task<ActionResult> Checkout()
        {
            if (Request.IsAuthenticated)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

                var order = new Order
                {
                    FirstName=user.UserData.FirstName,
                    LastName=user.UserData.LastName,
                    Address=user.UserData.Address,
                    CodeAndCity=user.UserData.CodeAndCity,
                    Email=user.UserData.Email,
                    PhoneNumber=user.UserData.PhoneNumber
                };

                return View(order);
            }
            else
            {
                return RedirectToAction("Login", "Account", new { returnUrl=Url.Action("Checkout", "Cart")});
            }
        }

        [HttpPost]
        public async Task<ActionResult> Checkout(Order orderdetails)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();

                var newOrder = shoppingCartManager.CreateOrder(orderdetails, userId);

                var user = await UserManager.FindByIdAsync(userId);
                TryUpdateModel(user.UserData);
                await UserManager.UpdateAsync(user);

                shoppingCartManager.EmptyCart();

                return RedirectToAction("OrderConfirmation");
            }
            else
            {
                return View(orderdetails);
            }
        }

        public ActionResult OrderConfirmation()
        {
            return View();
        }
    }
}