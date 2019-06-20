using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotChatV4Demo
{
    public class MenuList
    {
        public static List<Menu> Menu = new List<Menu>() {
            new Menu("Order Food", "Welcome to Order Bot, click here to get overview of FOOD menu!", "https://uphinh.org/images/2019/06/05/food.jpg","Go to FOOD menu", "FOOD"),
            new Menu("Order Drink", "Welcome to Order Bot, click here to get overview of DRINK menu!", "https://uphinh.org/images/2019/06/05/drink.jpg","Go to DRINK menu", "DRINK"),
            new Menu("Review Order", "Welcome to Order Bot, click here to get your ordered!", "https://uphinh.org/images/2019/06/13/order.png","Review Order", "VIEW ORDER"),
            new Menu("Switch KB", "Welcome to Order Bot, click here to switch your Knowlegde Base!", "https://uphinh.vn/images/2019/06/20/kb.png","Switch KB", "SWITCH KB"),
            new Menu("LUIS Demo", "Welcome to Order Bot, click here to get LUIS Demo!", "https://uphinh.vn/images/2019/06/20/luis.png","Go to LUIS Demo", "LUIS DEMO"),
            new Menu("Card Demo", "Welcome to Order Bot, click here to get Card Demo!", "https://uphinh.vn/images/2019/06/20/card.jpg","Go to Card Demo", "CARD DEMO")
        };
    }
}
