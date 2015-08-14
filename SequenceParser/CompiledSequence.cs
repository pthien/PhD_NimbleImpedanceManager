using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
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
                throw new ArgumentException("Segment files dont have matching generation guids");
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
                    if (commentsSplit[i].StartsWith("IMPEDANCE") || commentsSplit[i].StartsWith("COMPLIANCEON_"))
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
        public static Regex sequenceExtractor = new Regex(@"const int Sequence\[([0-9]+)\]\[([0-9]+)\] = {([0-9{},\s]*)};");
        private static int[][] ParseSequence(string alltext)
        {
            

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
                    p.LA_uA = int.Parse(metadatasplit[3], NumberStyles.Integer);
                    p.RE = int.Parse(metadatasplit[4], NumberStyles.Integer);
                    p.RM = int.Parse(metadatasplit[5], NumberStyles.Integer);
                    p.RA_uA = int.Parse(metadatasplit[6], NumberStyles.Integer);
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

        public List<TelemetryResult> ProcessMeasurementCall(NimbleSegmentMeasurment m1)
        {
            if (!m1.path.Contains(this.Guid.ToString()))
                throw new ArgumentException("Must supply a measurement from this sequence");

            CICState ImplantA = new CICState();
            CICState ImplantB = new CICState();

            int maxPWMA = -1, maxPWMB = -1, minPWMA = -1, minPWMB = -1;
            List<TelemetryResult> telemetryResults = new List<TelemetryResult>();
            var telemResponses = m1.NimbleResponses;

            for (int i = 0; i < m1.NimbleResponses.Count; i++)
            {
                NimbleResponse resp = m1.NimbleResponses[i];
                if (resp is SegmentChangeResponse)
                {
                    if (i == m1.NimbleResponses.Count - 1) //at last response
                    {
                        //nothing to extract
                        continue;
                    }
                    else
                    {
                        NimbleResponse nextresp = m1.NimbleResponses[i + 1];
                        if (nextresp is SegmentChangeResponse)
                        {
                            //extract all meaning, but there will be no telem pulses
                            int segnum = ((SegmentChangeResponse)resp).NewSegment;
                            for (int j = 0; j < Sequence[segnum].Length; j++)
                            {
                                int pulsenum = Sequence[segnum][j];
                                Pulse p = PulseData[pulsenum];
                                ImplantA.ApplyPulse(p.LE, p.LM, p.LA_uA);
                                ImplantB.ApplyPulse(p.RE, p.RM, p.RA_uA);
                            }
                        }
                        else if (((SegmentChangeResponse)resp).NewSegment != ((TelemetryResponse)nextresp).Segment) //next is telem response of different segment
                        {
                            //extract all meaning, but there will be no telem pulses
                        }
                        else //next is telem response of same segment
                        {
                            TelemetryResponse nextresp_t = (TelemetryResponse)nextresp;
                            int seg = nextresp_t.Segment;
                            int segIdxToStopAt = nextresp_t.SegmentPulseIndex;

                            //apply meaning up to, but not including next pulse
                            for (int j = 0; j < segIdxToStopAt; j++)
                            {
                                int pulsenum = Sequence[seg][j];
                                Pulse p = PulseData[pulsenum];
                                ImplantA.ApplyPulse(p.LE, p.LM, p.LA_uA);
                                ImplantB.ApplyPulse(p.RE, p.RM, p.RA_uA);
                            }


                        }
                    }
                }
                else
                {
                    TelemetryResponse resp_t = (TelemetryResponse)resp;

                    int pulsenum = Sequence[resp_t.Segment][resp_t.SegmentPulseIndex];
                    Pulse p = PulseData[pulsenum];

                    if (i == m1.NimbleResponses.Count - 1) //at last response
                    {
                        //parse data of current response
                        ExtractDataOfResponse(m1, resp_t, ImplantA, ref maxPWMA, ImplantB, telemetryResults, ref maxPWMB,
                            ref minPWMA, ref minPWMB, p);
                    }
                    else
                    {
                        
                        //apply current pulse
                        pulsenum = Sequence[resp_t.Segment][resp_t.SegmentPulseIndex];
                        p = PulseData[pulsenum];
                        ImplantA.ApplyPulse(p.LE, p.LM, p.LA_uA);
                        ImplantB.ApplyPulse(p.RE, p.RM, p.RA_uA);

                        //parse data of current response
                        ExtractDataOfResponse(m1, resp_t, ImplantA, ref maxPWMA, ImplantB, telemetryResults, ref maxPWMB,
                            ref minPWMA, ref minPWMB, p);
                        //apply meaning up to, but not including next pulse

                        NimbleResponse nextresp = m1.NimbleResponses[i + 1];
                        int segIdxToStopAt;
                        if (nextresp is SegmentChangeResponse)
                        {
                            segIdxToStopAt = Sequence[resp_t.Segment].Length;
                        }
                        else
                        {
                            TelemetryResponse nextresp_t = (TelemetryResponse)nextresp;
                            segIdxToStopAt = nextresp_t.SegmentPulseIndex;
                        }
                        int segToStartAt = resp_t.SegmentPulseIndex + 1;

                        if(segToStartAt< segIdxToStopAt)
                        for (int j = segToStartAt; j < segIdxToStopAt; j++)
                        {
                            pulsenum = Sequence[resp_t.Segment][j];
                            p = PulseData[pulsenum];
                            ImplantA.ApplyPulse(p.LE, p.LM, p.LA_uA);
                            ImplantB.ApplyPulse(p.RE, p.RM, p.RA_uA);
                        }
                    }

                }
            }

            //foreach (int n in m1.SegmentsRun)
            //{
            //    //if (n <= _HashDefines["NORMAL_SEGMENTS"])
            //    //    continue;

            //    for (int i = 0; i < Sequence[n].Length; i++)
            //    {
            //        int pulsenum = Sequence[n][i];
            //        Pulse p = PulseData[pulsenum];

            //        ImplantA.ApplyPulse(p.LE, p.LM, p.LA_uA);
            //        ImplantB.ApplyPulse(p.RE, p.RM, p.RA_uA);
            //        bool any =
            //            telemResponses.Any(x => x.Sequence == n && x.SequenceIndex == (i) && x.PulseIndex == pulsenum);
            //        if (p.TelemetryCount > 0 || any)
            //        {
            //            var resp = telemResponses.Where(x => x.Sequence == n && x.SequenceIndex == (i) && x.PulseIndex == pulsenum);
            //            var list = resp.ToList<TelemetryResponse>();

            //            if (list.Count == 1)
            //            {
            //                TelemetryResponse telemResp = list[0];
            //                if (telemResp.Captures_ticks.Count > 1)
            //                {
            //                    logger.Error("Too many captures. Telemresponse:{2}. Pulse:{0}, measurement:{1}", p, m1.path, telemResp);
            //                    continue;
            //                }
            //                if (telemResp.Captures_ticks.Count == 0)
            //                {
            //                    logger.Debug("none captured. Telemresponse:{2}. Pulse:{0}, measurement:{1}", p, m1.path, telemResp);
            //                    continue;
            //                }

            //                if (ImplantA.SetUpToReadMaxPWM)
            //                {
            //                    logger.Debug("Got Implant A Max PWM. Measurement:{0}, TelemResponse:{1}", m1, telemResp);
            //                    maxPWMA = telemResp.Captures_ticks[0];
            //                }
            //                else if (ImplantB.SetUpToReadMaxPWM)
            //                {
            //                    logger.Debug("Got Implant B Max PWM. Measurement:{0}, TelemResponse:{1}", m1, telemResp);
            //                    maxPWMB = telemResp.Captures_ticks[0];
            //                }
            //                else if (ImplantA.SetUpToReadMinPWM)
            //                {
            //                    logger.Debug("Got Implant A Min PWM. Measurement:{0}, TelemResponse:{1}", m1, telemResp);
            //                    minPWMA = telemResp.Captures_ticks[0];
            //                }
            //                else if (ImplantB.SetUpToReadMinPWM)
            //                {
            //                    logger.Debug("Got Implant B Min PWM. Measurement:{0}, TelemResponse:{1}", m1, telemResp);
            //                    minPWMB = telemResp.Captures_ticks[0];
            //                }
            //                else if (ImplantA.SetUpForImpedanceTelemetry)
            //                {
            //                    //double current_uA = CochlearImplantTokenEncoder.CITokenEncoder.AmplitudeToCurrent(p.LA_uA);
            //                    if (minPWMA > 0 && maxPWMA > 0 && telemResp.Captures_ticks.Count == 1)
            //                    {
            //                        var impRes = new ImpedanceResult(p.LE, p.LM, p.LA_uA, maxPWMA, minPWMA,
            //                             telemResp.Captures_ticks[0], ImplantA.vtel_gain, Implant.ImplantA, p.PW_us);
            //                        telemetryResults.Add(impRes);
            //                    }
            //                    else
            //                    {
            //                        logger.Error("Something missing when trying to calculate impedance. MaxPWM:{0}, MinPWM:{1}, telemResult:{2}, MeasurementRecord:{3}",
            //                            maxPWMA, minPWMA, telemResp, m1);
            //                    }
            //                }
            //                else if (ImplantB.SetUpForImpedanceTelemetry)
            //                {
            //                    //double current_uA = CochlearImplantTokenEncoder.CITokenEncoder.AmplitudeToCurrent(p.RA_uA);
            //                    if (minPWMB > 0 && maxPWMB > 0 && telemResp.Captures_ticks.Count == 1)
            //                    {
            //                        var impRes = new ImpedanceResult(p.RE, p.RM, p.RA_uA, maxPWMB, minPWMB,
            //                             telemResp.Captures_ticks[0], ImplantB.vtel_gain, Implant.ImplantB, p.PW_us);
            //                        telemetryResults.Add(impRes);
            //                    }
            //                    else
            //                    {
            //                        logger.Error("Something missing when trying to calculate impedance. MaxPWM:{0}, MinPWM:{1}, telemResult:{2}, MeasurementRecord:{3}",
            //                            maxPWMB, minPWMB, telemResp, m1);
            //                    }
            //                }
            //                else if (ImplantA.SetUpForComplianceTelemetry)
            //                {
            //                    ComplianceResult cr = new ComplianceResult(p, Implant.ImplantA, telemResp.Captures_ticks[0], ClockRate);
            //                    telemetryResults.Add(cr);
            //                }
            //                else if (ImplantB.SetUpForComplianceTelemetry)
            //                {
            //                    ComplianceResult cr = new ComplianceResult(p, Implant.ImplantB, telemResp.Captures_ticks[0], ClockRate);
            //                    telemetryResults.Add(cr);
            //                }
            //                else
            //                {
            //                    logger.Error("Unknown telemetry kind: {0}", m1.path);
            //                    throw new ArgumentOutOfRangeException("what kind of telemetry are you doing?!?!");
            //                }

            //            }
            //            else if (list.Count > 1)
            //            {
            //                logger.Error("More items than I'm expecting. Pulse:{0}, measurement:{1}", p, m1.path);
            //            }
            //            else
            //            {
            //                logger.Debug("Telem Response not found. Pulse:{0}, measurement:{1}", p, m1.path);
            //            }
            //        }
            //    }

            //}
            return telemetryResults;
        }

        private void ExtractDataOfResponse(NimbleSegmentMeasurment m1, TelemetryResponse resp_t, CICState ImplantA,
            ref int maxPWMA, CICState ImplantB, List<TelemetryResult> telemetryResults, ref int maxPWMB, ref int minPWMA, ref int minPWMB, Pulse p)
        {
            if (resp_t.Captures_ticks.Count > 0)
            {
                if (resp_t.Captures_ticks.Count > 1)
                {
                    logger.Error("Too many captures. Telemresponse:{2}. Pulse:{0}, measurement:{1}", p, m1.path, resp_t);
                }
                else if (resp_t.Captures_ticks.Count == 0)
                {
                    logger.Debug("none captured. Telemresponse:{2}. Pulse:{0}, measurement:{1}", p, m1.path, resp_t);
                }
                else
                {
                    if (ImplantA.SetUpToReadMaxPWM)
                    {
                        logger.Debug("Got Implant A Max PWM. Measurement:{0}, TelemResponse:{1}", m1, resp_t);
                        maxPWMA = resp_t.Captures_ticks[0];
                    }
                    else if (ImplantB.SetUpToReadMaxPWM)
                    {
                        logger.Debug("Got Implant B Max PWM. Measurement:{0}, TelemResponse:{1}", m1, resp_t);
                        maxPWMB = resp_t.Captures_ticks[0];
                    }
                    else if (ImplantA.SetUpToReadMinPWM)
                    {
                        logger.Debug("Got Implant A Min PWM. Measurement:{0}, TelemResponse:{1}", m1, resp_t);
                        minPWMA = resp_t.Captures_ticks[0];
                    }
                    else if (ImplantB.SetUpToReadMinPWM)
                    {
                        logger.Debug("Got Implant B Min PWM. Measurement:{0}, TelemResponse:{1}", m1, resp_t);
                        minPWMB = resp_t.Captures_ticks[0];
                    }
                    else if (ImplantA.SetUpForImpedanceTelemetry)
                    {
                        //double current_uA = CochlearImplantTokenEncoder.CITokenEncoder.AmplitudeToCurrent(p.LA_uA);
                        if (minPWMA > 0 && maxPWMA > 0 && resp_t.Captures_ticks.Count == 1)
                        {
                            var impRes = new ImpedanceResult(p.LE, p.LM, p.LA_uA, maxPWMA, minPWMA,
                                resp_t.Captures_ticks[0], ImplantA.vtel_gain, Implant.ImplantA, p.PW_us);
                            telemetryResults.Add(impRes);
                        }
                        else
                        {
                            logger.Error(
                                "Something missing when trying to calculate impedance. MaxPWM:{0}, MinPWM:{1}, telemResult:{2}, MeasurementRecord:{3}",
                                maxPWMA, minPWMA, resp_t, m1);
                        }
                    }
                    else if (ImplantB.SetUpForImpedanceTelemetry)
                    {
                        //double current_uA = CochlearImplantTokenEncoder.CITokenEncoder.AmplitudeToCurrent(p.RA_uA);
                        if (minPWMB > 0 && maxPWMB > 0 && resp_t.Captures_ticks.Count == 1)
                        {
                            var impRes = new ImpedanceResult(p.RE, p.RM, p.RA_uA, maxPWMB, minPWMB,
                                resp_t.Captures_ticks[0], ImplantB.vtel_gain, Implant.ImplantB, p.PW_us);
                            telemetryResults.Add(impRes);
                        }
                        else
                        {
                            logger.Error(
                                "Something missing when trying to calculate impedance. MaxPWM:{0}, MinPWM:{1}, telemResult:{2}, MeasurementRecord:{3}",
                                maxPWMB, minPWMB, resp_t, m1);
                        }
                    }
                    else if (ImplantA.SetUpForComplianceTelemetry)
                    {
                        ComplianceResult cr = new ComplianceResult(p, Implant.ImplantA, resp_t.Captures_ticks[0], ClockRate);
                        telemetryResults.Add(cr);
                    }
                    else if (ImplantB.SetUpForComplianceTelemetry)
                    {
                        ComplianceResult cr = new ComplianceResult(p, Implant.ImplantB, resp_t.Captures_ticks[0], ClockRate);
                        telemetryResults.Add(cr);
                    }
                    else
                    {
                        logger.Error("Unknown telemetry kind: {0}", m1.path);
                        throw new ArgumentOutOfRangeException("what kind of telemetry are you doing?!?!");
                    }
                }
            }
        }
    }

}
