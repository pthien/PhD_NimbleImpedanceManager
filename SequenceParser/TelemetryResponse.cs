using System;
using System.Collections.Generic;

namespace Nimble.Sequences
{
    /// <summary>
    /// The telemetry response to a single pulse.
    /// </summary>
    public struct TelemetryResponse
    {
        public int _PulseIndex, _Sequence, _SequenceIndex;
        public List<int> Captures_ticks;

        public int PulseIndex
        {
            get { return _PulseIndex; }
        }

        public int Sequence
        {
            get { return _Sequence; }
        }

        public int SequenceIndex
        {
            get { return _SequenceIndex; }
            set { _SequenceIndex = value; }
        }

        public TelemetryResponse(int pulseIndex, int seqenceNum, int sequenceIndex, string caps)
        {

            //var m = r.Match(line);
            _PulseIndex = pulseIndex;
            _Sequence = seqenceNum;
            _SequenceIndex = sequenceIndex;

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
                string.Join("-", Captures_ticks) : "nc", _PulseIndex, _Sequence, _SequenceIndex);
        }
    }
}