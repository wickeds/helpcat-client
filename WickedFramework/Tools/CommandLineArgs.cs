using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WickedFramework.Tools
{
    public static class CommandLineArgs
    {
        static readonly string[] args = Environment.GetCommandLineArgs();
        static readonly char[] keyTrimChars = new char[] { '/', '-' };

        public static bool GetKey(string key)
        {
            for (int i = 0; i < args.Length; i++)
                if (key.ToLowerInvariant() == args[i].ToLowerInvariant().Trim().TrimStart(keyTrimChars))
                    return true;
            return false;
        }

        public static T GetValue<T>(string key)
        {
            for (int i = 0; i < args.Length; i++)
                if (key.ToLowerInvariant() == args[i].ToLowerInvariant().Trim().TrimStart(keyTrimChars))
                    if (args.Length + 1 > i)
                        return (T)Convert.ChangeType(args[i + 1], typeof(T));
                    else
                        return default(T);
            return default(T);
        }
    }
}
