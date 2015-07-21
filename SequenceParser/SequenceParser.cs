using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nimble.Sequences
{
    public static class SequenceParser
    {
        public static void ParseRecord()
        {

        }

        public static void ParseTelemetryData()
        {

        }

        public static List<string> GetTelemetryRecords(string directory)
        {
            string[] possibledirectories = Directory.GetDirectories(directory);
            var telemRecords = ParseTelemDataDirecortyNames(possibledirectories, directory);
            return null;
        }

        public static List<NimbleMeasurementRecord> ParseTelemDataDirecortyNames(string[] possibledirectories, string containingDirectory)
        {
            var results = new List<NimbleMeasurementRecord>();
            if (possibledirectories == null)
                return results;

            List<string> TelemetryRecordDirectories = new List<string>();
            Regex folder = new Regex("([A-Za-z0-9_]+)-([A-Z0-9]{12})-([A-Za-z0-9-]{36})-([0-9APM_-]{22})");
            foreach (string s in possibledirectories)
            {
                var m = folder.Match(s);
                if (m.Success)
                {
                    NimbleMeasurementRecord temp = new NimbleMeasurementRecord();
                    temp.SubjectName = m.Groups[1].Value;
                    temp.BluetoothAddress = m.Groups[2].Value;
                    temp.GenGuid = Guid.Parse(m.Groups[3].Value);
                    string[] timeparts = m.Groups[4].Value.Split(new char[] { '-', '_' });

                    int hours = int.Parse(timeparts[3]) + (timeparts[6] == "AM" ? 0 : 12);
                    var x = new DateTime(
                        int.Parse(timeparts[0]), int.Parse(timeparts[1]), int.Parse(timeparts[2]),
                        hours,
                    int.Parse(timeparts[4]), int.Parse(timeparts[5]));

                    temp.Timestamp = x;
                    temp.RecordDirectory = Path.Combine(containingDirectory, s);
                    results.Add(temp);
                }
            }

            return results;

        }

        public static void GetImpedanceForTelemetryRecord(NimbleMeasurementRecord record, SequenceFileManager fileMan)
        {
            string recordGuid = record.GenGuid.ToString();
        }
    }
}
