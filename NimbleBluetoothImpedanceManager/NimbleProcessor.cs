using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NimbleBluetoothImpedanceManager
{
    class NimbleProcessor
    {
        public string Name { get; set; }
        public string BluetoothAddress { get; set; }
        public string GenGUID { get; set; }

        public override string ToString()
        {
            return string.Format("{0}({1})", Name, BluetoothAddress);
        }
    }
}
