using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NimbleBluetoothImpedanceManager.RN4871Driver.Model
{
    class DeviceScanResult
    {
        public DeviceScanResult(string bluetoothAddress, string name)
        {
            Name = name;
            BluetoothAddress = bluetoothAddress;
        }

        public string BluetoothAddress { get; private set; }
        public string Name { get; private set; }
    }
}
