using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Nimble.Sequences.AliveDevices
{
    static public class AliveDevices
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">May be a folder containing the log file, or the log file itself</param>
        /// <returns></returns>
        public static List<ReallyAliveDevicesMeasurement> GetAliveDevices(string path)
        {
            if (File.Exists(path) || Directory.Exists(path))
            {
                var attr = File.GetAttributes(path);
                if (attr.HasFlag(FileAttributes.Directory))
                {
                    string file = Path.Combine(path, "AliveDevices.txt");
                    return GetAliveDevicesFromFile(file);
                }
                return GetAliveDevicesFromFile(path);
            }
            return null;
        }

        private static List<ReallyAliveDevicesMeasurement> GetAliveDevicesFromFile(string file)
        {
            var lines = File.ReadAllLines(file);
            List<ReallyAliveDevicesMeasurement> alive = new List<ReallyAliveDevicesMeasurement>();

            foreach (string s in lines)

            {
                var parts = s.Split('|');
                var date = DateTime.Parse(parts[0]);

                string[] devices;
                if (parts[1].Contains(','))
                    devices = parts[1].Split(',');
                else
                    devices = new string[] { parts[1] };

                foreach (string id in devices)
                    alive.Add(new ReallyAliveDevicesMeasurement() { Timestamp = date, BluetoothAdress = id.Trim(), SubjectName = "???" });
            }
            return alive;
        }

        public static List<ReallyAliveDevicesMeasurement> GetReallyAliveDevices(string path)
        {
            if (File.Exists(path) || Directory.Exists(path))
            {
                var attr = File.GetAttributes(path);
                if (attr.HasFlag(FileAttributes.Directory))
                {
                    string file = Path.Combine(path, "ReallyAliveDevices.txt");
                    return GetReallyliveDevicesFromFile(file);
                }
                return GetReallyliveDevicesFromFile(path);
            }
            return null;
        }

        private static List<ReallyAliveDevicesMeasurement> GetReallyliveDevicesFromFile(string file)
        {
            var lines = File.ReadAllLines(file);
            List<ReallyAliveDevicesMeasurement> alive = new List<ReallyAliveDevicesMeasurement>();

            foreach (string s in lines)

            {
                var parts = s.Split('|');
                var date = DateTime.Parse(parts[0]);

                string[] devices;
                if (parts[1].Contains(','))
                    devices = parts[1].Split(',');
                else
                    devices = new string[] { parts[1] };

                Regex r = new Regex(@"([0-9A-Za-z_]+)\(([0-9A-Za-z]+)\)");
                foreach (string id in devices)
                {
                    Match m = r.Match(id);
                    if (m.Success)
                    {
                        string name = m.Groups[1].Value;
                        string btid = m.Groups[2].Value;
                        alive.Add(new ReallyAliveDevicesMeasurement() { Timestamp = date, BluetoothAdress = btid, SubjectName = name });
                    }
                    else
                    {

                    }

                }
            }
            return alive;
        }
    }


    //public struct AliveDevicesMeasurement
    //{
    //    public DateTime Timestamp { get; set; }
    //    public string DetectedBluetoothAdresses { get; set; }
    //}

    public struct ReallyAliveDevicesMeasurement
    {
        public DateTime Timestamp { get; set; }
        public string SubjectName { get; set; }
        public string BluetoothAdress { get; set; }

        public override string ToString()
        {
            return string.Format("{0:MM/dd/yy hh:mm:ss tt},{1},{2}", Timestamp, BluetoothAdress, SubjectName);
        }
    }

}
