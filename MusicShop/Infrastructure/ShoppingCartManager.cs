using MusicShop.DAL;
using MusicShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicShop.Infrastructure
{
    public class ShoppingCartManager
    {
        private StoreContext db;
        private ISessionManager session;

        public const string CartSessionKey = "CartData";

        public ShoppingCartManager(ISessionManager session, StoreContext db)
        {
            this.db = db;
            this.session = session;
        }

        public void AddToCart(int albumid)
        {
            var cart = GetCart();
            var cartItem = cart.Find(c => c.Album.AlbumId == albumid);

            if (cartItem != null)
                cartItem.Quantity++;
            else
            {
                var albumToAdd = db.Albums.Where(a => a.AlbumId == albumid).SingleOrDefault();

                if (albumToAdd != null)
                {
                    var newCartItem = new CartItem()
                    {
                        Album = albumToAdd,
                        Quantity = 1,
                        TotalPrice = albumToAdd.Price
                    };

                    cart.Add(newCartItem);
                }
            }

            session.Set(CartSessionKey, cart);
        }

        public List<CartItem> GetCart()
        {
            List<CartItem> cart;

            if (session.Get<List<CartItem>>(CartSessionKey) == null)
            {
                cart = new List<CartItem>();
            }
            else
            {
                cart = session.Get<List<CartItem>>(CartSessionKey);
            }

            return cart;
        }

        public int RemoveFromCart(int albumid)
        {
            var cart = GetCart();
            var cartItem = cart.Find(c => c.Album.AlbumId == albumid);

            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity--;
                    return cartItem.Quantity;
                }
                else
                    cart.Remove(cartItem);
            }

            return 0;
        }

        public decimal GetCartTotalPrince()
        {
            var cart = GetCart();
            return cart.Sum(c => (c.Quantity * c.Album.Price));
        }

        public int GetCartItemCount()
        {
            var cart = GetCart();
            return cart.Sum(c => c.Quantity);
        }

        public Order CreateOrder(Order newOrder, string userId)
        {
            var cart = GetCart();

            newOrder.DateCreated = DateTime.Now;
            //newOrder.UserId = userId;

            db.Orders.Add(newOrder);

            if (newOrder.OrderItems == null)
                newOrder.OrderItems = new List<OrderItem>();

            decimal cartTotal = 0;

            foreach(var cartItem in cart)
            {
                var newOrderItem = new OrderItem()
                {
                    AlbumId = cartItem.Album.AlbumId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Album.Price
                };

                cartTotal += (cartItem.Quantity * cartItem.Album.Price);

                newOrder.OrderItems.Add(newOrderItem);
            }

            newOrder.TotalPrince = cartTotal;

            db.SaveChanges();

            return newOrder;
        }

        public void EmptyCart()
        {
            session.Set<List<CartItem>>(CartSessionKey, null);
        }
    }
}