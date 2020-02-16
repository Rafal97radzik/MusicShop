using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MusicShop.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        [StringLength(150)]
        public string FirstName { get; set; }

        [StringLength(150)]
        public string LastName { get; set; }

        public string Adress { get; set; }

        [Required(ErrorMessage ="Wprowadź kod pocztowy i miasto")]
        [StringLength(50)]
        public string CodeAndCity { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string Coment { get; set; }

        public DateTime DateCreated { get; set; }

        public OrderState OrderState { get; set; }

        public decimal TotalPrince { get; set; }

        public List<OrderItem> OrderItems { get; set; }
    }

    public enum OrderState
    {
        New,
        Shipped
    }
}