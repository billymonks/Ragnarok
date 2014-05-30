using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wickedcrush.menu
{
    public class Menu
    {
        MenuElement current;
        List<MenuElement> elements;

        public Menu()
        {
            elements = new List<MenuElement>();
        }
    }
}
