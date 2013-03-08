using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace helpcat.Misc
{
    static class DynamicTools
    {
        public static bool HasProperty(dynamic o, string p)
        {
            return o.GetType().GetProperty(p) != null;
        }
    }
}
