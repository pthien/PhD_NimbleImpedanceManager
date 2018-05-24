using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NimbleBluetoothImpedanceManager.RN4871Driver.Model
{
    public struct Status
    {
        public DeviceState DeviceState;
        public string RemoteDeviceAddress;
        public List<string> KnownDevices;
    }


    public enum DeviceState
    {
        Disconnected,
        Connecting,
        Connected,
        Unknown
    }
}
