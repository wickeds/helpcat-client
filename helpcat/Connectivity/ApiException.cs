using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace helpcat.Connectivity
{
    [Serializable]
    class ApiException : Exception
    {
        public ApiException()
            : base()
        {
        }

        public ApiException(string message)
            : base(message)
        {
        }
    }
}
