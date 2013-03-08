using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WickedFramework.Tools
{
    public class TempFolder : IDisposable
    {
        string created;
        bool permanent;

        public TempFolder(bool permanent = false)
        {
            this.permanent = permanent;
            Guid guid = Guid.NewGuid();
            string temp = Path.GetTempPath();
            created = Path.Combine(temp, guid.ToString() + Path.PathSeparator);
            Directory.CreateDirectory(created);
        }

        public override string ToString()
        {
            return created;
        }

        public void Dispose()
        {
            if (!permanent)
                Directory.Delete(created, true);
        }
    }
}
