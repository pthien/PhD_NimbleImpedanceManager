using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PIC_Sequence;
using NLog;

namespace Nimble.Sequences
{
    public class CompiledSequence
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public CompiledSequence(FilesForGenerationGUID SequenceFiles)
        {
            string alltext_seqh = File.ReadAllText(SequenceFiles.Sequence_h);
            SequenceComments = ExtractSequenceComments(alltext_seqh);
            Guid guid_sh = ExtractGuid(alltext_seqh);
            _HashDefines = ExtractHashDefines(alltext_seqh);

            string alltext_seqc = File.ReadAllText(SequenceFiles.Sequence_c);
            Sequence = ParseSequence(alltext_seqc);
            Guid guid_sc = ExtractGuid(alltext_seqc);


            string alltext_pulsec = File.ReadAllText(SequenceFiles.PulseData_c);
            ClockRate = ParsePulseDataClockRate(alltext_pulsec);
            PulseData = ParsePulseData(alltext_pulsec).ToArray();
            Guid guid_pc = ExtractGuid(alltext_pulsec);

            if (guid_sh == guid_sc && guid_sh == guid_pc)
            {
                Guid = guid_sh;
            }
            else
            {
                throw new ArgumentException("Sequence files dont have matching generation guids");
            }
        }

        public Guid Guid { get; private set; }

        public Dictionary<int, string> MeasurementSegments { get; private set; }

        public Pulse[] PulseData { get; private set; }

        public int ClockRate { get; private set; }

        public int[][] Sequence { get; private set; }

        private readonly Dictionary<string, int> _HashDefines;

        public readonly string[] SequenceComments;

        #region constructor functions
        /// <summary>
        /// Extracts the generation guid from a .c or .h sequence file
        /// </summary>
        /// <param name="alltext"></param>
        /// <returns></returns>
        public static Guid ExtractGuid(string alltext)
        {
            Regex regex_guid = new Regex("{GenerationGUID: ([A-Za-z0-9-]+)}");
            var m = regex_guid.Match(alltext);
            if (m.Success)
                return Guid.Parse(m.Groups[1].Value);
            return Guid.Empty;
        }

        public static int ParsePulseDataClockRate(string alltext)
        {
            Regex clockrate = new Regex(@"\(FCY != CLOCK_([0-4]{2})MHZ\)");
            var m = clockrate.Match(alltext);
            if (m.Success)
            {
                return int.Parse(m.Groups[1].Value);
            }
            throw new ArgumentException("Couldn't parse clock rate");
        }

        /// <summary>
        /// Gets the comments (aka name) for each segment in the sequence file
        /// </summary>
        /// <param name="alltext"></param>
        /// <returns></returns>
        private string[] ExtractSequenceComments(string alltext)
        {
            MeasurementSegments = new Dictionary<int, string>();
            Regex r = new Regex(@"{SegmentComments: ([ A-Z0-9a-z-_,|()]+)}");
            var match = r.Match(alltext);
            if (match.Success)
            {
                string allcomments = match.Groups[1].Value;
                string[] commentsSplit = allcomments.Split(',');

                for (int i = 0; i < commentsSplit.Length; i++)
                {
                    commentsSplit[i] = commentsSplit[i].Trim();
                }

                for (int i = 0; i < commentsSplit.Length; i++)
                {
                    if (commentsSplit[i].StartsWith("IMPEDANCE"))
                    {
                        MeasurementSegments.Add(i, commentsSplit[i].ToString());
                    }
                }
                return commentsSplit;
            }
            return null;
        }

        private static Dictionary<string, int> ExtractHashDefines(string alltext_seqh)
        {
            Regex defineExtractor = new Regex(@"#define ([A-Za-z_]+) ([0-9]+)");
            var matches = defineExtractor.Matches(alltext_seqh);

            Dictionary<string, int> result = new Dictionary<string, int>();

            foreach (Match m in matches)
            {
                if (m.Success)
                {
                    result.Add(m.Groups[1].Value, int.Parse(m.Groups[2].Value));
                }
            }
            return result;
        }

        private static int[][] ParseSequence(string alltext)
        {
            Regex sequenceExtractor = new Regex(@"const int Sequence\[([0-9]+)\]\[([0-9]+)\] = {([0-9{},\s]*)};");

            var m = sequenceExtractor.Match(alltext);
            if (m.Success)
            {
                int numSeqs = int.Parse(m.Groups[1].Value);
                int maxSeqLen = int.Parse(m.Groups[2].Value);

                string seqData = m.Groups[3].Value;

                var extractedSequence = Parse2DArray(numSeqs, seqData);

                return extractedSequence;


            }
            return null;
        }

        private static int[][] Parse2DArray(int numRows, string seqData)
        {
            int[][] extractedSequence = new int[numRows][];

            string[] segments = seqData.Trim().Split(new char[] { '}' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < segments.Length; i++)
            {
                string segment = segments[i].Trim('{', ',', '\r', '\n');
                string[] segnums = segment.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                List<int> lstsegnums = new List<int>();
                foreach (string segnum in segnums)
                {
                    lstsegnums.Add(int.Parse(segnum));
                }
                extractedSequence[i] = lstsegnums.ToArray();
            }
            return extractedSequence;
        }

        private static List<Pulse> ParsePulseData(string alltext)
        {
            //Regex pulseData = new Regex(@"{ (0x[0-9A-F]{4}),\s(0x[0-9A-F]{4}),\s(0x[0-9A-F]{4}),\s(0x[0-9A-F]{4}),\s(0x[0-9A-F]{4}),\s(0x[0-9A-F]{4}),\s(0x[0-9A-F]{4}),\s(0x[0-9A-F]{4}),\s([0-9]+),\s([0-9]+),\s([0-9]+),\s([0-9]+)\s?}\s?,?//\s?{ ([0-9]+):\t([0-9]+),\t([0-9]+),\t(-?[0-9]+),\t([0-9]+),\t([0-9]+),\t(-?[0-9]+),\t([0-9]+),\t([0-9]+)");

            Regex pulseData = new Regex(@"{([0-9A-Fx\s,]+)}.*// {([0-9:\s,-]+)}");
            var mc = pulseData.Matches(alltext);

            List<Pulse> result = new List<Pulse>();

            foreach (Match m in mc)
            {
                if (m.Success)
                {
                    string data = m.Groups[1].Value;
                    string metadata = m.Groups[2].Value;

                    Pulse p = new Pulse();
                    string[] datasplit = data.Trim(' ', '{', '}').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    string[] metadatasplit = metadata.Trim(' ', '{', '}').Split(new char[] { ',', ':' }, StringSplitOptions.RemoveEmptyEntries);

                    p.LE = int.Parse(metadatasplit[1], NumberStyles.Integer);
                    p.LM = int.Parse(metadatasplit[2], NumberStyles.Integer);
                    p.LA = int.Parse(metadatasplit[3], NumberStyles.Integer);
                    p.RE = int.Parse(metadatasplit[4], NumberStyles.Integer);
                    p.RM = int.Parse(metadatasplit[5], NumberStyles.Integer);
                    p.RA = int.Parse(metadatasplit[6], NumberStyles.Integer);
                    p.PW_us = int.Parse(metadatasplit[7], NumberStyles.Integer);
                    p.IPG_us = int.Parse(metadatasplit[8], NumberStyles.Integer);

                    p.TelemetryCount = int.Parse(datasplit[11], NumberStyles.Integer);
                    result.Add(p);
                }
            }
            return result;
        }
        #endregion

        //public NimbleImpedanceRecord ProcessSequenceResponse(NimbleMeasurementRecord measurementRecord)
        //{
        //    NimbleImpedanceRecord impedanceRecord = new NimbleImpedanceRecord(measurementRecord);
        //    var measurements = measurementRecord.GetMeasurments();

        //    foreach (NimbleSegmentMeasurment m in measurements)
        //    {
        //        List<ImpedanceResult> impedanceResults = ProcessMeasurementCall(m);
        //        impedanceRecord.AddSegmentImpedanceResult(impedanceResults);
        //    }
            
        //    return impedanceRecord;
        //}

        public List<ImpedanceResult> ProcessMeasurementCall(NimbleSegmentMeasurment m1)
        {
            CICState ImplantA = new CICState();
            CICState ImplantB = new CICState();
            
            int maxPWMA = -1, maxPWMB = -1, minPWMA = -1, minPWMB = -1;
            List<ImpedanceResult> impedanceResults = new List<ImpedanceResult>();
            var telemResponses = m1.TelemetryResponses;
            foreach (int n in m1.SegmentsRun)
            {
                if (n <= _HashDefines["NORMAL_SEGMENTS"])
                    continue;

                for (int i = 0; i < Sequence[n].Length; i++)
                {
                    int pulsenum = Sequence[n][i];
                    Pulse p = PulseData[pulsenum];

                    ImplantA.ApplyPulse(p.LE, p.LM, p.LA);
                    ImplantB.ApplyPulse(p.RE, p.RM, p.RA);

                    if (p.TelemetryCount > 0)
                    {
                        var resp = telemResponses.Where(x => x.Sequence == n && x.SequenceIndex == (i + 1) && x.PulseIndex == pulsenum);
                        var list = resp.ToList<TelemetryResponse>();

                        if (list.Count == 1)
                        {
                            TelemetryResponse telemResp = list[0];
                            if (telemResp.Captures_ticks.Count > 1)
                            {
                                logger.Error("Too many captures. Telemresponse:{2}. Pulse:{0}, measurement:{1}", p, m1.path, telemResp);
                                continue;
                            }
                            if (telemResp.Captures_ticks.Count == 0)
                            {
                                logger.Debug("none captured. Telemresponse:{2}. Pulse:{0}, measurement:{1}", p, m1.path, telemResp);
                                continue;
                            }

                            if (ImplantA.SetUpToReadMaxPWM)
                            {
                                logger.Debug("Got Implant A Max PWM. Measurement:{0}, TelemResponse:{1}", m1, telemResp);
                                maxPWMA = telemResp.Captures_ticks[0];
                            }
                            else if (ImplantB.SetUpToReadMaxPWM)
                            {
                                logger.Debug("Got Implant B Max PWM. Measurement:{0}, TelemResponse:{1}", m1, telemResp);
                                maxPWMB = telemResp.Captures_ticks[0];
                            }
                            else if (ImplantA.SetUpToReadMinPWM)
                            {
                                logger.Debug("Got Implant A Min PWM. Measurement:{0}, TelemResponse:{1}", m1, telemResp);
                                minPWMA = telemResp.Captures_ticks[0];
                            }
                            else if (ImplantB.SetUpToReadMinPWM)
                            {
                                logger.Debug("Got Implant B Min PWM. Measurement:{0}, TelemResponse:{1}", m1, telemResp);
                                minPWMB = telemResp.Captures_ticks[0];
                            }
                            else if (ImplantA.SetUpForImpedanceTelemetry)
                            {
                                double current_uA = CochlearImplantTokenEncoder.CITokenEncoder.AmplitudeToCurrent(p.LA);
                                if (minPWMA > 0 && maxPWMA > 0 && telemResp.Captures_ticks.Count == 1)
                                {
                                    var impRes = new ImpedanceResult(p.LE, p.LM, current_uA, maxPWMA, minPWMA,
                                         telemResp.Captures_ticks[0], ImplantA.vtel_gain, Implant.ImplantA, p.PW_us);
                                    impedanceResults.Add(impRes);
                                }
                                else
                                {
                                    logger.Error("Something missing when trying to calculate impedance. MaxPWM:{0}, MinPWM:{1}, telemResult:{2}, MeasurementRecord:{3}",
                                        maxPWMA, minPWMA, telemResp, m1);
                                }
                            }
                            else if (ImplantB.SetUpForImpedanceTelemetry)
                            {
                                double current_uA = CochlearImplantTokenEncoder.CITokenEncoder.AmplitudeToCurrent(p.RA);
                                if (minPWMB > 0 && maxPWMB > 0 && telemResp.Captures_ticks.Count == 1)
                                {
                                    var impRes = new ImpedanceResult(p.RE, p.RM, current_uA, maxPWMB, minPWMB,
                                         telemResp.Captures_ticks[0], ImplantB.vtel_gain, Implant.ImplantB, p.PW_us);
                                    impedanceResults.Add(impRes);
                                }
                                else
                                {
                                    logger.Error("Something missing when trying to calculate impedance. MaxPWM:{0}, MinPWM:{1}, telemResult:{2}, MeasurementRecord:{3}",
                                        maxPWMB, minPWMB, telemResp, m1);
                                }
                            }

                        }
                        else if (list.Count > 1)
                        {
                            logger.Error("More items than I'm expecting. Pulse:{0}, measurement:{1}", p, m1.path);
                        }
                        else
                        {
                            logger.Debug("Telem Response not found. Pulse:{0}, measurement:{1}", p, m1.path);
                        }
                    }
                }

            }
            return impedanceResults;
        }
    }

}
