using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace helpcat.Connectivity
{
    [Serializable]
    class AuthentificationException : Exception
    {
        public AuthentificationException()
            : base()
        {
        }

        public AuthentificationException(string message)
            : base(message)
        {
        }
    }
}
