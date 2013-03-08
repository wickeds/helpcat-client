using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace helpcat.Misc
{
    static class WindowTools
    {
        public static T Search<T>()
        {
            Window instance = null;

            foreach (Window window in Application.Current.Windows)
            {
                if (window is T)
                {
                    instance = window;
                    break;
                }
            }

            return (T)Convert.ChangeType(instance, typeof(T));
        }
    }
}
