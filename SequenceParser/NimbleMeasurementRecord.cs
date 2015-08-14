using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Nimble.Sequences
{
    /// <summary>
    /// The result of a single XmitTelem Command.
    /// </summary>
    public struct NimbleSegmentMeasurment
    {
        public List<NimbleResponse> NimbleResponses;
        /// <summary>
        /// The segments used in the producetion of this measurement.
        /// </summary>
        //public List<int> SegmentsRun;
        public string SegmentName;
        public int RepeatCount;
        public string path;

        public NimbleSegmentMeasurment(string filepath)
        {
            NimbleResponses = new List<NimbleResponse>();
            //SegmentsRun = new List<int>();
            SegmentName = "";
            RepeatCount = -1;
            path = "";

            string fname = Path.GetFileNameWithoutExtension(filepath);
            Regex regex_measurementfile = new Regex(@"(.*)_([0-9]+)\Z");
            var m = regex_measurementfile.Match(fname);
            if (m.Success)
            {
                SegmentName = m.Groups[1].Value;
                RepeatCount = int.Parse(m.Groups[2].Value);
                path = filepath;

                string alltext = File.ReadAllText(filepath);
                string[] lines = alltext.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                Regex regex_telemResponse = new Regex(@"{T:C([0-9]+)S([0-9]+)I([0-9]+)\[([0-9nc-]+)\]}");
                Regex regex_segNum = new Regex(@"{seg:([0-9]+)");

                foreach (string s in lines)
                {
                    NimbleResponse resp = NimbleResponse.GetResponse(s);
                    if (resp != null)
                        NimbleResponses.Add(resp);
                    //var m2 = regex_telemResponse.Match(s);
                    //var m3 = regex_segNum.Match(s);
                    //if (m2.Success)
                    //{
                    //    var a = int.Parse(m2.Groups[1].Value);
                    //    var b = int.Parse(m2.Groups[2].Value);
                    //    var c = int.Parse(m2.Groups[3].Value);
                    //    var d = m2.Groups[4].Value;

                    //    TelemetryResponse t = new TelemetryResponse(a, b, c, d);
                    //    NimbleResponses.Add(t);
                    //}
                    //else if (m3.Success)
                    //{
                    //    SegmentsRun.Add(int.Parse(m3.Groups[1].Value));
                    //}
                }
            }

        }

        public override string ToString()
        {
            return string.Format("{0} #{1}", SegmentName, RepeatCount);//return base.ToString();
        }
    }

    /// <summary>
    /// A measurement record. Contains all measurements made at a single timepoint for a single subject. 
    /// </summary>
    public struct NimbleMeasurementRecord
    {
        public DateTime Timestamp { get; set; }
        public string BluetoothAddress { get; set; }
        public string SubjectName { get; set; }
        public Guid GenGuid { get; set; }
        public string RecordDirectory { get; set; }

        public List<NimbleSegmentMeasurment> GetMeasurments()
        {
            string[] potentialMeasuremetns = Directory.GetFiles(RecordDirectory);
            List<NimbleSegmentMeasurment> output = new List<NimbleSegmentMeasurment>();
            foreach (string file in potentialMeasuremetns)
            {
                NimbleSegmentMeasurment s = new NimbleSegmentMeasurment(file);
                if (s.SegmentName.Length >= 1)
                    output.Add(new NimbleSegmentMeasurment(file));
            }
            return output;
        }

        public override string ToString()
        {
            return String.Format("Record: {0}, {1}", SubjectName, Timestamp.ToShortDateString() + " " + Timestamp.ToShortTimeString());
        }
    }
}