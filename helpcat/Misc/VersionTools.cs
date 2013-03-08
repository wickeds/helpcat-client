using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WickedDeployment
{
    static class VersionTools
    {
        /// <summary>
        /// Compares two .NET-style version strings
        /// </summary>
        /// <param name="v1">First version string</param>
        /// <param name="v2">Second version string</param>
        /// <returns>-1 if first version string is higher, 0 if both are equal and 1 if second version string is higher</returns>
        public static int Compare(string v1, string v2)
        {
            string[] v1s = v1.Trim().Split('.');
            string[] v2s = v2.Trim().Split('.');

            return PartialCompare(v1s, v2s, 0);
        }

        /// <summary>
        /// Compares two .NET-style version strings
        /// </summary>
        /// <param name="v1s">First string array with one version string part per entry</param>
        /// <param name="v2s">First string array with one version string part per entry</param>
        /// <param name="pos">Offset on which string array entry to start</param>
        /// <returns>-1 if first version string is higher, 0 if both are equal and 1 if second version string is higher</returns>
        static int PartialCompare(string[] v1s, string[] v2s, int pos)
        {
            if (v1s[pos] == "*" || v2s[pos] == "*")
                return 0;
            else if (v1s[pos] == v2s[pos])
            {
                if (pos < v1s.Length && pos < v2s.Length)
                    return PartialCompare(v1s, v2s, pos + 1);
                return 0;
            }
            else
                return int.Parse(v1s[pos]) < int.Parse(v2s[pos]) ? 1 : 0;
        }
    }
}
