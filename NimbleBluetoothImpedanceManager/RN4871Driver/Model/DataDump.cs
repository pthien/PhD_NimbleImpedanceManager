using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NimbleBluetoothImpedanceManager.RN4871Driver.Model
{
    struct DataDump
    {
        public string BluetoothAddress { get; }
        public string Name { get; }
        public string Connected { get; }
        public string Authen { get; }
        public string Features { get; }
        public string Services { get; }

        public bool IsConnected => Connected != "no";

        public DataDump(string BluetoothAddress,
            string Name,
            string Connected,
            string Authen,
            string Features,
            string Services)
        {
            this.BluetoothAddress = BluetoothAddress;
            this.Name = Name;
            this.Connected = Connected;
            this.Authen = Authen;
            this.Features = Features;
            this.Services = Features;
        }

        public override string ToString()
        {
            return string.Format("Addr: {0}, Connected: {1}", BluetoothAddress, Connected);
        }
    }
}
