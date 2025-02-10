using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace myStore.Models
{
    public class Orders
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; } // Foreign Key for the user
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public string ShippingAddress { get; set; }

        // Navigation property for the list of ordered products
        public List<OrderItem> OrderItems { get; set; }
    }

    public class OrderItem
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        // Navigation properties
        public Product Product { get; set; }
        public Orders Order { get; set; }
    }
}
