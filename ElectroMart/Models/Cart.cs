using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using myStore.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace myStore.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        // Внешний ключ на таблицу пользователей
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual IdentityUser User { get; set; }

        // Внешний ключ на таблицу продуктов
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        // Дополнительные поля для корзины, например, количество товаров
        public int Quantity { get; set; }
    }
}
