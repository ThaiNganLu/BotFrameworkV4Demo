using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotChatV4Demo
{
    public class Order
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public List<Product> Cart { get; set; }

        public Order(string name = "", string phone = "", List<Product> cart = null)
        {
            this.Name = name;
            this.Phone = phone;
            this.Cart = cart;
        }

        public Order() { }
    }
}
