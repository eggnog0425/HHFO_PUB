using System;
using System.Collections.Generic;
using System.Text;

namespace HHFO.Models
{
    public class Menu: IMenu
    {
        // todo:必要に応じてreactivePropertyにする
        // todo:最終的には文字列じゃなくてベクタアイコンにする
        string IMenu.Home => "Home";

        string IMenu.List => "List";
    }
}
