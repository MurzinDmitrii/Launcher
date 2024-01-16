using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Launcher.Classes
{
    /// <summary>
    /// Класс для работы с треем
    /// </summary>
    class WrapClass
    {
        public static void Wrap(Window window)
        {
            window.Hide();
        }

        public static void Unwrap(Window window)
        {
            window.Show();
        }
    }
}
