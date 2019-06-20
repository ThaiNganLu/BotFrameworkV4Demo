using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotChatV4Demo
{
    public class Menu
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Action { get; set; }
        public string Code { get; set; }

        public Menu() { }

        public Menu(string name = "",string description = "", string image = "", string action = "", string code = "")
        {
            this.Name = name;
            this.Description = description;
            this.Image = image;
            this.Action = action;
            this.Code = code;
        }
    }
}
