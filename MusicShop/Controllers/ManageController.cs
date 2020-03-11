using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MusicShop.App_Start;
using MusicShop.DAL;
using MusicShop.Models;
using MusicShop.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using MusicShop.Infrastructure;
using System.Web.Hosting;
using System.Net;

namespace MusicShop.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        StoreContext db = new StoreContext();

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            LinkSuccess,
            Error
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

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }


        // GET: Manage
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            if (TempData["ViewData"] != null)
            {
                ViewData = (ViewDataDictionary)TempData["ViewData"];
            }

            if (User.IsInRole("Admin"))
                ViewBag.UserIsAdmin = true;
            else
                ViewBag.UserIsAdmin = false;

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();

            var model = new ManageCredentialsViewModel
            {
                Message = message,
                HasPassword = this.HasPassword(),
                CurrentLogins = userLogins,
                OtherLogins = otherLogins,
                ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1,
                UserData = user.UserData
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeProfile([Bind(Prefix = "UserData")]UserData userData)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                user.UserData = userData;
                var result = await UserManager.UpdateAsync(user);

                AddErrors(result);
            }

            if (!ModelState.IsValid)
            {
                TempData["ViewData"] = ViewData;
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword([Bind(Prefix = "ChangePasswordViewModel")]ChangePasswordViewModel model)
        {
            // In case we have simple errors - return
            if (!ModelState.IsValid)
            {
                TempData["ViewData"] = ViewData;
                return RedirectToAction("Index");
            }

            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInAsync(user, isPersistent: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);

            // In case we have login errors
            if (!ModelState.IsValid)
            {
                TempData["ViewData"] = ViewData;
                return RedirectToAction("Index");
            }

            var message = ManageMessageId.ChangePasswordSuccess;
            return RedirectToAction("Index", new { Message = message });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword([Bind(Prefix = "SetPasswordViewModel")]SetPasswordViewModel model)
        {
            // In case we have simple errors - return
            if (!ModelState.IsValid)
            {
                TempData["ViewData"] = ViewData;
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInAsync(user, isPersistent: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);

                if (!ModelState.IsValid)
                {
                    TempData["ViewData"] = ViewData;
                    return RedirectToAction("Index");
                }
            }

            var message = ManageMessageId.SetPasswordSuccess;
            return RedirectToAction("Index", new { Message = message });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("Index", new { Message = ManageMessageId.LinkSuccess }) : RedirectToAction("Index", new { Message = ManageMessageId.Error });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInAsync(user, isPersistent: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Index", new { Message = message });
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("password-error", error);
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
        }

        public ActionResult HideProduct(int albumId)
        {
            var album = db.Albums.Find(albumId);
            album.IsHidden = true;
            db.SaveChanges();

            return RedirectToAction("AddProduct", new { confirmSuccess = true });
        }

        public ActionResult UnhideProduct(int albumId)
        {
            var album = db.Albums.Find(albumId);
            album.IsHidden = false;
            db.SaveChanges();

            return RedirectToAction("AddProduct", new { confirmSuccess = true });
        }

        public ActionResult OrdersList()
        {
            bool isAdmin = User.IsInRole("Admin");
            ViewBag.UserIsAdmin = isAdmin;
            IEnumerable<Order> userOrders;

            if (isAdmin)
            {
                userOrders = db.Orders.Include("OrderItems").
                    OrderByDescending(o => o.DateCreated).ToArray();
            }
            else
            {
                var userId = User.Identity.GetUserId();
                userOrders = db.Orders.Where(o => o.UserId == userId).Include("OrderItems").
                    OrderByDescending(o => o.DateCreated).ToArray();
            }

            return View(userOrders);
        }

        public OrderState ChangeOrderState(Order order)
        {
            Order orderToModify = db.Orders.Find(order.OrderId);
            orderToModify.OrderState = order.OrderState;
            db.SaveChanges();

            return order.OrderState;
        }

        public ActionResult AddProduct(int? albumId, bool? confirmSuccess)
        {
            var result = new EditProductViewModel();

            result.Genres = db.Genres.ToList();

            if (albumId.HasValue)
            {
                ViewBag.EditMode = true;
                result.Album = db.Albums.Find(albumId);
            }
            else
            {
                ViewBag.EditMode = false;
                result.Album = new Album();
            }

            result.ConfirmSuccess = confirmSuccess;

            return View(result);
        }

        [HttpPost]
        public ActionResult AddProduct(EditProductViewModel model, HttpPostedFileBase file)
        {
            if (model.Album.AlbumId > 0)
            {
                db.Entry(model.Album).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AddProduct", new { confirmSuccess = true });
            }
            else
            {
                if (file != null && file.ContentLength > 0)
                {
                    if (ModelState.IsValid)
                    {
                        var fileExt = Path.GetExtension(file.FileName);
                        var filename = Guid.NewGuid() + fileExt;

                        var path = Path.Combine(Server.MapPath(AppConfig.PhotosFolderRelative), filename);
                        file.SaveAs(path);

                        model.Album.CoverFileName = filename;
                        model.Album.DateAdded = DateTime.Now;

                        db.Entry(model.Album).State = EntityState.Added;
                        db.SaveChanges();

                        return RedirectToAction("AddProduct", new { confirmSuccess = true });
                    }
                    else
                    {
                        var genres = db.Genres.ToArray();
                        model.Genres = genres;
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Nie wskazano pliku.");
                    var genres = db.Genres.ToArray();
                    model.Genres = genres;
                    return View(model);
                }
            }
        }

        [AllowAnonymous]
        public ActionResult SendConfirmationEmail(int orderid, string lastname)
        {

            var order = db.Orders.Include("OrderItems").Include("OrderItems.Album").SingleOrDefault(o => o.OrderId == orderid && o.LastName==lastname);

            if (order == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            OrderConfirmationEmail email = new OrderConfirmationEmail();
            email.To = order.Email;
            email.Cost = order.TotalPrince;
            email.OrderNumber = order.OrderId;
            email.FullAddress = string.Format("{0} {1}, {2}, {3}", order.FirstName, order.LastName, order.Address, order.CodeAndCity);
            email.OrderItems = order.OrderItems;
            email.CoverPath = AppConfig.PhotosFolderRelative;
            email.Send();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}