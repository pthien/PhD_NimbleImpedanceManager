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
            SegmentToLevelMap = ExtractSegment2LevelMap(alltext_seqh);
            Guid guid_sh = ExtractGuid(alltext_seqh);
            _HashDefines = ExtractHashDefines(alltext_seqh);

            string alltext_seqc = File.ReadAllText(SequenceFiles.Sequence_c);
            Sequence = ParseSequence(alltext_seqc, SequenceComments.Length, out MaxCurrentuAForEachSegment);
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
        private int[] MaxCurrentuAForEachSegment;
        private Dictionary<int, int> SegmentToLevelMap;
        private readonly Dictionary<string, int> _HashDefines;
        public readonly string[] SequenceComments;

        public int GetMaxStimLevel()
        {
            return SegmentToLevelMap.Values.Max();
        }


        public void ConvertStimLevel2SegNumbers(int StimLevel, out int StartLoopSeg, out int EndLoopSeg)
        {
            StartLoopSeg = int.MaxValue;
            EndLoopSeg = -1;

            if (!SegmentToLevelMap.ContainsValue(StimLevel))
                throw new ArgumentOutOfRangeException("Stim level not found");

            foreach (KeyValuePair<int, int> kvp in SegmentToLevelMap)
            {
                if (kvp.Value == StimLevel)
                {
                    StartLoopSeg = Math.Min(kvp.Key, StartLoopSeg);
                    EndLoopSeg = Math.Max(kvp.Key, EndLoopSeg);
                }
            }


        }
        public int ConvertSegNumber2StimLevel(int segNumber)
        {
            if (SegmentToLevelMap.ContainsKey(segNumber))
                return SegmentToLevelMap[segNumber];
            return 0;
        }

        public int GetMaxCurrentInRampLevel(int RampLevel)
        {
            int start, end;
            ConvertStimLevel2SegNumbers(RampLevel, out start, out end);
            return MaxCurrentuAForEachSegment[end];
        }

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
                    if (commentsSplit[i].StartsWith("IMPEDANCE") || commentsSplit[i].StartsWith("SPECIAL_") || commentsSplit[i].StartsWith("COMPLIANCEON_"))
                    {
                        MeasurementSegments.Add(i, commentsSplit[i].ToString());
                    }
                }
                return commentsSplit;
            }
            return null;
        }

        private Dictionary<int, int> ExtractSegment2LevelMap(string alltext)
        {
            var map = new Dictionary<int, int>();
            Regex r = new Regex(@"{SegmentLevels: ([ 0-9-_,|()]+)}");

            var match = r.Match(alltext);
            if (match.Success)
            {
                string alllevels = match.Groups[1].Value;
                string[] levelsSplit = alllevels.Split(',');

                int[] myInts = Array.ConvertAll(levelsSplit, s => int.Parse(s));

                for (int i = 0; i < myInts.Count(); i++)
                {
                    map.Add(i, myInts[i]);
                }
            }
            return map;
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
        public static Regex sequenceRowExtractor = new Regex(@"const int Sequence_row([0-9]+)\[\] = {(.+)}; //MaxCurrent ([0-9]+)");

        private static int[][] ParseSequence(string alltext, int numSegments, out int[] MaxCurrentuAForEachSegment)
        {
            var match = sequenceExtractor.Match(alltext);
            if (match.Success) //old format
            {
                MaxCurrentuAForEachSegment = new int[numSegments];
                int numSeqs = int.Parse(match.Groups[1].Value);
                int maxSeqLen = int.Parse(match.Groups[2].Value);

                string seqData = match.Groups[3].Value;

                var extractedSequence = Parse2DArray(numSeqs, seqData);

                return extractedSequence;


            }
            else //new format
            {
                var mc = sequenceRowExtractor.Matches(alltext);

                int[][] extractedSequence = new int[numSegments][];
                MaxCurrentuAForEachSegment = new int[numSegments];

                int count = 0;
                foreach (Match m in mc)
                {
                    int rownum = int.Parse(m.Groups[1].Value);
                    int[] rowData = Array.ConvertAll<string, int>(m.Groups[2].Value.Split(','), int.Parse);
                    extractedSequence[count] = rowData;
                    MaxCurrentuAForEachSegment[count] = int.Parse(m.Groups[3].Value);
                    count++;
                }
                return extractedSequence;
            }
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

            Regex pulseData = new Regex(@"{([0-9A-Fx\s,]+)}.*// {([0-9:\s\.,-]+)}");
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
                    p.LA = double.Parse(metadatasplit[3], NumberStyles.Number);
                    p.RE = int.Parse(metadatasplit[4], NumberStyles.Integer);
                    p.RM = int.Parse(metadatasplit[5], NumberStyles.Integer);
                    p.RA = double.Parse(metadatasplit[6], NumberStyles.Number);
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

        public void DetermineMaxMinPWMFromSegmentResponses(IEnumerable<NimbleSegmentResponse> segResponses, out int MaxPWM_ticks, out int MinPWM_ticks)
        {
            MaxPWM_ticks = -1;
            MinPWM_ticks = -1;

            //foreach (NimbleSegmentResponse segResponse in segResponses)
            //{
            //    var telemResponses = segResponse.;
            //}
        }

        private void ProcessSegmentResponses(IEnumerable<NimbleSegmentResponse> segResponses)
        {

            foreach (NimbleSegmentResponse segResponse in segResponses)
            {
                ProcessIndividualSegmentReponse(segResponse);
            }
        }

        private void ProcessIndividualSegmentReponse(NimbleSegmentResponse segResponse)
        {
            CICState ImplantA = new CICState();
            CICState ImplantB = new CICState();

            var telemResponses = segResponse.TelemetryResponses;

            int numTelemetryLines = segResponse.TelemetryResponses.Count;

            for (int telemLineNumber = 0; telemLineNumber < numTelemetryLines; telemLineNumber++)
            {
                NimbleResponse responseLine = segResponse.TelemetryResponses[telemLineNumber];

                if (responseLine is SegmentChangeResponse) //
                {
                    if (telemLineNumber == numTelemetryLines - 1) //at last response
                    {
                        //nothing to extract
                        break;
                    }
                    else
                    {
                        NimbleResponse nextresp = segResponse.TelemetryResponses[telemLineNumber + 1];
                        if (nextresp is SegmentChangeResponse)  //going from segment change response to another segment change response, so no telem in between
                        {
                            //extract all meaning, but there will be no telem pulses
                            int segnum = ((SegmentChangeResponse)responseLine).NewSegment;
                            ApplyAllPulsesOfSegment(ImplantA, ImplantB, segnum);
                        }
                        else if (((SegmentChangeResponse)responseLine).NewSegment != ((TelemetryResponse)nextresp).Segment) //next is telem response of different segment
                        {
                            logger.Warn("This is unlikely to happen! sdfsferawvhn");

                            //Apply current segment
                            int currentSegNum = ((SegmentChangeResponse)responseLine).NewSegment;
                            ApplyAllPulsesOfSegment(ImplantA, ImplantB, currentSegNum);

                            //Apply start of next segment
                            TelemetryResponse nextresp_t = (TelemetryResponse)nextresp;
                            int nextSeg = nextresp_t.Segment;
                            int segIdxToStopAt = nextresp_t.SegmentPulseIndex;
                            ApplySegmentRange(ImplantA, ImplantB, nextSeg, 0, segIdxToStopAt);
                        }
                        else //next is telem response of same segment
                        {
                            TelemetryResponse nextresp_t = (TelemetryResponse)nextresp;
                            int seg = nextresp_t.Segment;
                            int segIdxToStopAt = nextresp_t.SegmentPulseIndex;

                            //apply meaning up to, but not including next pulse
                            ApplySegmentRange(ImplantA, ImplantB, seg, 0, segIdxToStopAt);
                        }
                    }
                }
                else //response line is not a SegmentChangeResponse
                {
                    TelemetryResponse resp_t = (TelemetryResponse)responseLine;
                    //if (resp_t.Segment <= 18)
                    //{
                    //    logger.Warn("Impossible response ({0}), must be corrupted in file {1}", resp_t, m1.path);
                    //    continue;
                    //}
                    int pulsenum = Sequence[resp_t.Segment][resp_t.SegmentPulseIndex];
                    Pulse p = PulseData[pulsenum];

                    if (telemLineNumber == segResponse.TelemetryResponses.Count - 1) //at last response
                    {
                        //parse data of current response
                        /*ExtractDataOfResponse(segResponse, resp_t, ImplantA, ref maxPWMA, ImplantB, telemetryResults, ref maxPWMB,
                           ref minPWMA, ref minPWMB, p);*/
                    }
                    else
                    {
                        if (telemLineNumber == 0) //at start, so need to apply beginning of this segment
                        {
                            ApplySegmentRange(ImplantA, ImplantB,
                                resp_t.Segment, 0,
                                resp_t.SegmentPulseIndex);
                        }

                        //apply current pulse
                        ApplyPulse(ImplantA, ImplantB, resp_t.SegmentPulseIndex, resp_t.Segment);
                        pulsenum = Sequence[resp_t.Segment][resp_t.SegmentPulseIndex];
                        p = PulseData[pulsenum];
                        ImplantA.ApplyPulse(p.LE, p.LM, p.LA);
                        ImplantB.ApplyPulse(p.RE, p.RM, p.RA);

                        //parse data of current response
                        if (resp_t.Segment <= 18)
                        {
                            logger.Warn("impossible response ({0}), must be corrupted in file {1}", resp_t, segResponse.path);
                            //continue;
                        }
                        else
                        {
                            //ExtractDataOfResponse(segResponse, resp_t, ImplantA, ref maxPWMA, ImplantB, telemetryResults, ref maxPWMB,
                            //    ref minPWMA, ref minPWMB, p);
                        }
                        //apply meaning up to, but not including next pulse

                        NimbleResponse nextresp = segResponse.TelemetryResponses[telemLineNumber + 1];
                        int segIdxToStopAt;
                        if (nextresp is SegmentChangeResponse || ((TelemetryResponse)nextresp).Segment != resp_t.Segment)
                        {
                            //apply rest of segment
                            segIdxToStopAt = Sequence[resp_t.Segment].Length;
                            if (segIdxToStopAt > 1)
                                ApplySegmentRange(ImplantA, ImplantB,
                                    resp_t.Segment, resp_t.SegmentPulseIndex + 1,
                                    segIdxToStopAt);
                        }
                        else
                        {
                            //apply up to next response
                            TelemetryResponse nextresp_t = (TelemetryResponse)nextresp;
                            segIdxToStopAt = nextresp_t.SegmentPulseIndex;
                            ApplySegmentRange(ImplantA, ImplantB,
                                resp_t.Segment, resp_t.SegmentPulseIndex + 1,
                                segIdxToStopAt);

                        }
                        //int segToStartAt = resp_t.SegmentPulseIndex + 1;

                        //int segmentToApply = resp_t.Segment;
                        //if (segToStartAt < segIdxToStopAt)
                        //    ApplySegmentRange(ImplantA, ImplantB,  segIdxToStopAt, segToStartAt, segmentToApply);

                        if (nextresp is TelemetryResponse)
                        {
                            if (((TelemetryResponse)nextresp).Segment != resp_t.Segment)
                            {
                                //apply start of next segment
                                int segToStartAt = 0;
                                segIdxToStopAt = ((TelemetryResponse)nextresp).SegmentPulseIndex;

                                for (int j = segToStartAt; j < segIdxToStopAt; j++)
                                {
                                    pulsenum = Sequence[((TelemetryResponse)nextresp).Segment][j];
                                    p = PulseData[pulsenum];
                                    ImplantA.ApplyPulse(p.LE, p.LM, p.LA);
                                    ImplantB.ApplyPulse(p.RE, p.RM, p.RA);
                                }
                            }
                        }

                    }

                }
            }
        }

        private void ApplyAllPulsesOfSegment(CICState ImplantA, CICState ImplantB, int segnum)
        {
            int start = 0;
            int end = Sequence[segnum].Length;
            ApplySegmentRange(ImplantA, ImplantB, segnum, start, end);
        }

        //private void ApplyPulseRangeOfSegment(CICState ImplantA, CICState ImplantB, int segnum, int StartPulse, int EndPulse)
        //{
        //    for (int j = StartPulse; j < EndPulse; j++)
        //    {
        //        int pulsenum = Sequence[segnum][j];
        //        Pulse p = PulseData[pulsenum];
        //        ImplantA.ApplyPulse(p.LE, p.LM, p.LA);
        //        ImplantB.ApplyPulse(p.RE, p.RM, p.RA);
        //    }
        //}

        public List<TelemetryResult> ProcessMeasurementCall(NimbleSegmentResponse m1)
        {
            try
            {
                if (!m1.path.Contains(this.Guid.ToString()))
                    throw new ArgumentException("Must supply a measurement from this sequence");

                CICState ImplantA = new CICState();
                CICState ImplantB = new CICState();

                int maxPWMA = -1, maxPWMB = -1, minPWMA = -1, minPWMB = -1;
                List<TelemetryResult> telemetryResults = new List<TelemetryResult>();
                var telemResponses = m1.TelemetryResponses;

                for (int i = 0; i < m1.TelemetryResponses.Count; i++)
                {
                    NimbleResponse resp = m1.TelemetryResponses[i];

                    if (resp is SegmentChangeResponse)
                    {
                        if (i == m1.TelemetryResponses.Count - 1) //at last response
                        {
                            //nothing to extract
                            continue;
                        }
                        else
                        {
                            NimbleResponse nextresp = m1.TelemetryResponses[i + 1];
                            if (nextresp is SegmentChangeResponse)
                            {
                                //extract all meaning, but there will be no telem pulses
                                int segnum = ((SegmentChangeResponse)resp).NewSegment;
                                for (int j = 0; j < Sequence[segnum].Length; j++)
                                {
                                    int pulsenum = Sequence[segnum][j];
                                    Pulse p = PulseData[pulsenum];
                                    ImplantA.ApplyPulse(p.LE, p.LM, p.LA);
                                    ImplantB.ApplyPulse(p.RE, p.RM, p.RA);
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
                                    ImplantA.ApplyPulse(p.LE, p.LM, p.LA);
                                    ImplantB.ApplyPulse(p.RE, p.RM, p.RA);
                                }


                            }
                        }
                    }
                    else
                    {
                        TelemetryResponse resp_t = (TelemetryResponse)resp;
                        //if (resp_t.Segment <= 18)
                        //{
                        //    logger.Warn("Impossible response ({0}), must be corrupted in file {1}", resp_t, m1.path);
                        //    continue;
                        //}
                        int pulsenum = Sequence[resp_t.Segment][resp_t.SegmentPulseIndex];
                        Pulse p = PulseData[pulsenum];

                        if (i == m1.TelemetryResponses.Count - 1) //at last response
                        {
                            //parse data of current response
                            ExtractDataOfResponse(m1, resp_t, ImplantA, ref maxPWMA, ImplantB, telemetryResults, ref maxPWMB,
                                ref minPWMA, ref minPWMB, p);
                        }
                        else
                        {
                            if (i == 0) //at start, so need to apply beginning of this segment
                            {
                                ApplySegmentRange(ImplantA, ImplantB,
                                    resp_t.Segment, 0,
                                    resp_t.SegmentPulseIndex);
                            }

                            //apply current pulse
                            ApplyPulse(ImplantA, ImplantB, resp_t.SegmentPulseIndex, resp_t.Segment);
                      

                            //parse data of current response
                            if (resp_t.Segment <= 18)
                            {
                                logger.Warn("impossible response ({0}), must be corrupted in file {1}", resp_t, m1.path);
                                //continue;
                            }
                            else
                            {
                                ExtractDataOfResponse(m1, resp_t, ImplantA, ref maxPWMA, ImplantB, telemetryResults, ref maxPWMB,
                                    ref minPWMA, ref minPWMB, p);
                            }
                            //apply meaning up to, but not including next pulse

                            NimbleResponse nextresp = m1.TelemetryResponses[i + 1];
                            int segIdxToStopAt;
                            if (nextresp is SegmentChangeResponse || ((TelemetryResponse)nextresp).Segment != resp_t.Segment)
                            {
                                //apply rest of segment
                                segIdxToStopAt = Sequence[resp_t.Segment].Length;
                                if (segIdxToStopAt > 1)
                                    ApplySegmentRange(ImplantA, ImplantB,
                                        resp_t.Segment, resp_t.SegmentPulseIndex + 1,
                                        segIdxToStopAt);
                            }
                            else
                            {
                                //apply up to next response
                                TelemetryResponse nextresp_t = (TelemetryResponse)nextresp;
                                segIdxToStopAt = nextresp_t.SegmentPulseIndex;
                                ApplySegmentRange(ImplantA, ImplantB,
                                    resp_t.Segment, resp_t.SegmentPulseIndex + 1,
                                    segIdxToStopAt);

                            }
                            //int segToStartAt = resp_t.SegmentPulseIndex + 1;

                            //int segmentToApply = resp_t.Segment;
                            //if (segToStartAt < segIdxToStopAt)
                            //    ApplySegmentRange(ImplantA, ImplantB,  segIdxToStopAt, segToStartAt, segmentToApply);

                            if (nextresp is TelemetryResponse)
                            {
                                if (((TelemetryResponse)nextresp).Segment != resp_t.Segment)
                                {
                                    //apply start of next segment
                                    int segToStartAt = 0;
                                    segIdxToStopAt = ((TelemetryResponse)nextresp).SegmentPulseIndex;

                                    for (int j = segToStartAt; j < segIdxToStopAt; j++)
                                    {
                                        pulsenum = Sequence[((TelemetryResponse)nextresp).Segment][j];
                                        p = PulseData[pulsenum];
                                        ImplantA.ApplyPulse(p.LE, p.LM, p.LA);
                                        ImplantB.ApplyPulse(p.RE, p.RM, p.RA);
                                    }
                                }
                            }

                        }

                    }
                }

                return telemetryResults;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return new List<TelemetryResult>();
        }

        private void ApplySegmentRange(CICState ImplantA, CICState ImplantB, int segmentToApply, int segIdxToStartAt, int segIdxToStopAt)
        {
            Pulse p;
            int pulsenum;
            if (segIdxToStopAt < segIdxToStartAt)
            {
                logger.Warn("Trying to apply a segment backwards! indicies {0} to {1} in seg {2}",
                    segIdxToStartAt, segIdxToStopAt, segmentToApply);
                return;
            }

            else
                for (int j = segIdxToStartAt; j < segIdxToStopAt; j++)
                {
                    pulsenum = Sequence[segmentToApply][j];
                    p = PulseData[pulsenum];
                    ImplantA.ApplyPulse(p.LE, p.LM, p.LA);
                    ImplantB.ApplyPulse(p.RE, p.RM, p.RA);
                }
        }

        private void ApplyPulse(CICState ImplantA, CICState ImplantB, int segIdx, int segmentToApply)
        {

            int pulsenum = Sequence[segmentToApply][segIdx];
            Pulse p = PulseData[pulsenum];
            ImplantA.ApplyPulse(p.LE, p.LM, p.LA);
            ImplantB.ApplyPulse(p.RE, p.RM, p.RA);

        }


        private void ExtractDataOfResponse(NimbleSegmentResponse m1, TelemetryResponse resp_t, CICState ImplantA,
            ref int maxPWMA, CICState ImplantB, List<TelemetryResult> telemetryResults, ref int maxPWMB, ref int minPWMA, ref int minPWMB, Pulse p)
        {
            Logger hiddenLogger = LogManager.GetLogger("Nimble.Sequences.CompiledSequence.Suppressed");
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
                        if (maxPWMA > 950 || maxPWMA < 800)
                            maxPWMA = -1;
                    }
                    else if (ImplantB.SetUpToReadMaxPWM)
                    {
                        logger.Debug("Got Implant B Max PWM. Measurement:{0}, TelemResponse:{1}", m1, resp_t);
                        maxPWMB = resp_t.Captures_ticks[0];
                        if (maxPWMB > 950 || maxPWMB < 800)
                            maxPWMB = -1;
                    }
                    else if (ImplantA.SetUpToReadMinPWM)
                    {
                        logger.Debug("Got Implant A Min PWM. Measurement:{0}, TelemResponse:{1}", m1, resp_t);
                        minPWMA = resp_t.Captures_ticks[0];
                        if (minPWMA < 50 || minPWMA > 150)
                            minPWMA = -1;
                    }
                    else if (ImplantB.SetUpToReadMinPWM)
                    {
                        logger.Debug("Got Implant B Min PWM. Measurement:{0}, TelemResponse:{1}", m1, resp_t);
                        minPWMB = resp_t.Captures_ticks[0];
                        if (minPWMB < 50 || minPWMB > 150)
                            minPWMB = -1;
                        hiddenLogger.Info("Set minPWMB: {0}  raw:{1}", minPWMB, resp_t.Captures_ticks[0]);
                    }
                    else if (ImplantA.SetUpForVoltageTelemetry_EndPhase1)
                    {
                        //double current_uA = CochlearImplantTokenEncoder.CITokenEncoder.AmplitudeToCurrent(p.LA_uA);
                        if (minPWMA > 0 && maxPWMA > 0 && resp_t.Captures_ticks.Count == 1)
                        {
                            var impRes = new ImpedanceResult(p.LE, p.LM, p.LA, maxPWMA, minPWMA,
                                resp_t.Captures_ticks[0], ImplantA.vtel_gain, Implant.ImplantA, p.PW_us);
                            telemetryResults.Add(impRes);
                        }
                        else
                        {
                            hiddenLogger.Warn(
                                "Something missing when trying to calculate impedance. A. MaxPWM:{0}, MinPWM:{1}, telemResult:{2}, MeasurementRecord:{3}",
                                maxPWMA, minPWMA, resp_t, m1);
                        }
                    }
                    else if (ImplantB.SetUpForVoltageTelemetry_EndPhase1)
                    {
                        //double current_uA = CochlearImplantTokenEncoder.CITokenEncoder.AmplitudeToCurrent(p.RA_uA);
                        if (minPWMB > 0 && maxPWMB > 0 && resp_t.Captures_ticks.Count == 1)
                        {
                            var impRes = new ImpedanceResult(p.RE, p.RM, p.RA, maxPWMB, minPWMB,
                                resp_t.Captures_ticks[0], ImplantB.vtel_gain, Implant.ImplantB, p.PW_us);
                            telemetryResults.Add(impRes);
                        }
                        else
                        {
                            hiddenLogger.Warn(
                                "Something missing when trying to calculate impedance. B. MaxPWM:{0}, MinPWM:{1}, telemResult:{2}, MeasurementRecord:{3}",
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
