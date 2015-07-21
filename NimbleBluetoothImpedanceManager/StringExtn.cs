using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NimbleBluetoothImpedanceManager
{
    static class StringExtn
    {
        public static string EscapeWhiteSpace(this string str)
        {
            return str.Replace("\n", "\\n").Replace("\r", "\\r").Replace(" ", "\\_").Replace("\0", @"\0").Replace("\t", @"\t");
        }
    }
}
