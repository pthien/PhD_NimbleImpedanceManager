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

    

        public static void GetImpedanceForTelemetryRecord(NimbleMeasurementRecord record, SequenceFileManager fileMan)
        {
            string recordGuid = record.GenGuid.ToString();
        }
    }
}
