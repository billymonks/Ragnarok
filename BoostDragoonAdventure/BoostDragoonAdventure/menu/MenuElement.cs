using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wickedcrush.menu
{
    public class MenuElement
    {
        MenuElement left = null, right = null, up = null, down = null;
        String text;
        Object value;

        public MenuElement()
        {

        }
    }
}
