using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotChatV4Demo
{
    public class Product
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public string Image { get; set; }
        public string Code { get; set; }

        public Product(){}

        public Product(string name = "", int price = 0, int quantity = 0, string image = "", string code = "")
        {
            this.Name = name;
            this.Price = price;
            this.Quantity = quantity;
            this.Image = image;
            this.Code = code;
        }
    }
}
