using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NLog;

namespace Nimble.Sequences
{
    public abstract class NimbleResponse
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public string RawResponse { get; protected set; }

        static Regex regex_telemResponse = new Regex(@"{T:C([0-9]+)S([0-9]+)I([0-9]+)\[([0-9nc-]+)\]}");
       static Regex regex_segNum = new Regex(@"{seg:([0-9]+)");

        protected NimbleResponse(string RawResponse)
        {
            this.RawResponse = RawResponse;
        }

        public static NimbleResponse GetResponse(string RawResponse)
        {
            var m2 = regex_telemResponse.Match(RawResponse);
            var m3 = regex_segNum.Match(RawResponse);
            if (m2.Success)
            {
                var a = int.Parse(m2.Groups[1].Value);
                var b = int.Parse(m2.Groups[2].Value);
                var c = int.Parse(m2.Groups[3].Value);
                var d = m2.Groups[4].Value;

                TelemetryResponse t = new TelemetryResponse(a, b, c, d, RawResponse);
                return t;
            }
            if (m3.Success)
            {
                return new SegmentChangeResponse(int.Parse(m3.Groups[1].Value), RawResponse);
            }
            logger.Debug("Ignoring response: {0}", RawResponse);
            return null;
            //throw new ArgumentOutOfRangeException();
            //return new NimbleResponse(RawResponse);
        }
    }

    public class SegmentChangeResponse : NimbleResponse
    {
        public int NewSegment { get; protected set; }
        public SegmentChangeResponse(int NewSegment, string RawResponse) : base(RawResponse)
        {
            this.NewSegment = NewSegment;
        }
    }

    /// <summary>
    /// The telemetry response to a single pulse.
    /// </summary>
    public class TelemetryResponse : NimbleResponse
    {
        public List<int> Captures_ticks;

        public int PulseIndex { get; private set; }

        public int Segment { get; private set; }

        public int SegmentPulseIndex { get; private set; }

        public TelemetryResponse(int pulseIndex, int seqenceNum, int segmentPulseIndex, string caps, string RawResponse) : base(RawResponse)
        {

            //var m = r.Match(line);
            PulseIndex = pulseIndex;
            Segment = seqenceNum;
            SegmentPulseIndex = segmentPulseIndex;

            Captures_ticks = new List<int>();
            if (caps == "nc")
                return;
            else
            {
                var capparts = caps.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in capparts)
                {
                    Captures_ticks.Add(int.Parse(s));
                }
            }
        }

        public override string ToString()
        {
            return String.Format("Caps: {0}. PI:{1} Seq:{2} SeqIdx:{3}", Captures_ticks.Count > 0 ?
                string.Join("-", Captures_ticks) : "nc", PulseIndex, Segment, SegmentPulseIndex);
        }
    }
}