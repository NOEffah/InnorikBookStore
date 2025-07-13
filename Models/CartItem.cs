using System;

namespace BookStore.Models
{
    public class CartItem
    {
        public Book Book { get; set; } = null!;
        public int Quantity { get; set; }
    }

}
