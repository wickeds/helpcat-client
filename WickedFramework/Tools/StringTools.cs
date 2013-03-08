using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace WickedFramework.Tools
{
    public static class StringTools
    {
        static readonly Regex halfmustache = new Regex(@"(\{+)([^\}]+)(\}+)");

        public static string NamedFormat(string format, params KeyValuePair<string, string>[] args)
        {
            return halfmustache.Replace(format, delegate(Match match)
            {
                string key = match.Groups["key"].Value;
                foreach (KeyValuePair<string, string> pair in args)
                    if (pair.Key == key)
                        return pair.Value;
                return match.Value;
            });
        }
    }
}
