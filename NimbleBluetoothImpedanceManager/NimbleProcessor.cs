using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NimbleBluetoothImpedanceManager
{
    struct NimbleProcessor
    {
        public string Name { get; set; }
        public string BluetoothAddress { get; set; }

        public override string ToString()
        {
            return string.Format("{0}({1})", Name, BluetoothAddress);
        }
    }
}
