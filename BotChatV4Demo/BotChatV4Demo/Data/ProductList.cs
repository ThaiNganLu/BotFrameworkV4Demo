using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotChatV4Demo
{
    public class ProductList
    {
        public static List<Product> Foods = new List<Product>() {
            new Product("Lovely Rice", 2000, 0, "https://uphinh.org/images/2019/06/13/rice.jpg", "RICE"),
            new Product("Beauty Noodle", 3200, 0, "https://uphinh.org/images/2019/06/13/noodle.jpg", "NOODLE"),
            new Product("Hot Pizza", 1580, 0, "https://uphinh.org/images/2019/06/13/pizza.jpg", "PIZZA"),
        };

        public static List<Product> Drinks = new List<Product>() {
            new Product("Coca", 1200, 0, "https://uphinh.org/images/2019/06/13/coca.jpg", "COCA"),
            new Product("Milk Tea", 3600, 0, "https://uphinh.org/images/2019/06/13/milktea.jpg", "MILK TEA"),
            new Product("Coffee", 8500, 0, "https://uphinh.org/images/2019/06/13/coffee.jpg", "COFFEE"),
        };
    }
}
