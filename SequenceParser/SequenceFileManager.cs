using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NLog;



namespace Nimble.Sequences
{
    public class SequenceFileManager
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private Dictionary<string, FilesForGenerationGUID> FilesByGenGUID;

        public Dictionary<string, CompiledSequence> CompiledSequences { get; private set; }

        Regex regex_guid = new Regex("{GenerationGUID: ([A-Za-z0-9-]+)}");
        public SequenceFileManager()
        {
            FilesByGenGUID = new Dictionary<string, FilesForGenerationGUID>();
            CompiledSequences = new Dictionary<string, CompiledSequence>();
        }

        /// <summary>
        /// Scans a folder and all subfolders for Segment or PulseData .c or .h files.
        /// </summary>
        /// <param name="path"></param>
        public void ScanDirectory(string path)
        {
            try
            {
                DoScanDirectory(path);

                foreach (KeyValuePair<string, FilesForGenerationGUID> kvp in FilesByGenGUID)
                {
                    FilesForGenerationGUID filesforguid = kvp.Value;
                    if (filesforguid.AllFilesReferenced && !CompiledSequences.ContainsKey(kvp.Key))
                    {
                        CompiledSequence cs = new CompiledSequence(kvp.Value);
                        CompiledSequences.Add(kvp.Key, cs);
                        logger.Info("Built sequence with ID {0}", kvp.Key);
                    }

                }
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
        }
        private void DoScanDirectory(string path)
        {
            try
            {
                string[] files = Directory.GetFiles(path);
                foreach (string f in files)
                {
                    TestIfValidFile(Path.Combine(path, f));
                }
                foreach (string d in Directory.GetDirectories(path))
                {
                    DoScanDirectory(d);
                }
            }
            catch (Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        public void TestIfValidFile(string filepath)
        {
            string filename = Path.GetFileName(filepath);

            //FileStream fs = new FileStream(filepath, FileMode.Open);
            try
            {
                if (filename == "PulseData.c" || filename == "PulseData.h" ||
                    filename == "Sequence.c" ||
                    filename == "Sequence.h")
                {

                    string alltext = File.ReadAllText(filepath);

                    var match = regex_guid.Match(alltext);
                    string guid = CompiledSequence.ExtractGuid(alltext).ToString();
                    if (guid != Guid.Empty.ToString())
                    {
                        FilesForGenerationGUID files;
                        if (FilesByGenGUID.ContainsKey(guid))
                        {
                            files = FilesByGenGUID[guid];
                        }
                        else
                        {
                            files = new FilesForGenerationGUID();
                            logger.Info("Found new sequence: {0}", guid);
                        }
                        switch (filename)
                        {
                            case "PulseData.c":
                                files.PulseData_c = filepath;
                                break;
                            case "PulseData.h":
                                files.PulseData_h = filepath;
                                break;
                            case "Sequence.c":
                                files.Sequence_c = filepath;
                                break;
                            case "Sequence.h":
                                files.Sequence_h = filepath;
                                break;
                        }
                        FilesByGenGUID[guid] = files;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

        }


        public static List<NimbleMeasurementRecord> GetTelemetryRecords(string directory)
        {
            string[] possibledirectories = Directory.GetDirectories(directory);
            var telemRecords = ParseTelemDataDirecortyNames(possibledirectories, directory);
            return telemRecords;
        }

        private static List<NimbleMeasurementRecord> ParseTelemDataDirecortyNames(string[] possibledirectories, string containingDirectory)
        {
            try
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

                        int hours = Int32.Parse(timeparts[3]) + (timeparts[6] == "AM" ? 0 : 12);
                        if (hours == 24)
                            hours = 12;
                        else if (hours == 12)
                            hours = 0;

                        var x = new DateTime(
                            Int32.Parse(timeparts[0]), Int32.Parse(timeparts[1]), Int32.Parse(timeparts[2]),
                            hours,
                            Int32.Parse(timeparts[4]), Int32.Parse(timeparts[5]), DateTimeKind.Local);

                        temp.Timestamp = x;
                        temp.RecordDirectory = Path.Combine(containingDirectory, s);
                        results.Add(temp);
                    }
                }

                return results;
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
            return new List<NimbleMeasurementRecord>();
        }

        public NimbleImpedanceRecord ProcessSequenceResponse(NimbleMeasurementRecord measurementRecord,
            bool LoadPreproccessIfExisting, bool Readonly)
        {
            try
            {
                //return ProcessSequenceResponseV2(measurementRecord);
                DateTime start = DateTime.Now;

                NimbleImpedanceRecord impedanceRecord = new NimbleImpedanceRecord(measurementRecord);

                //Attempt to load previously processed data
                if (LoadPreproccessIfExisting)
                {
                    var x = impedanceRecord.Load();
                    if (x.HasValue)
                        return x.Value;
                }

                var measurements = measurementRecord.GetMeasurments();

                if (CompiledSequences.ContainsKey(measurementRecord.GenGuid.ToString()))
                {
                    //logger.Info("Calculating impedances for {0} ({1})", measurementRecord, measurementRecord.RecordDirectory);
                    CompiledSequence cs = CompiledSequences[measurementRecord.GenGuid.ToString()];

                    List<AnnotatedTelemetryResponse> allMaxMinPWMresponses = cs.GetAllMaxMinPWMResponses(measurements);

                    //Process each segment response.
                    foreach (NimbleSegmentResponse m in measurements)
                    {
                        //var v1 = cs.ProcessMeasurementCall(m);
                        var v2a = cs.ProcessIndividualSegmentReponse(m);

                        if (v2a.Count > 0 && allMaxMinPWMresponses.Count > 0)
                        {
                            v2a.AddRange(allMaxMinPWMresponses);
                            var v2 = TelemetryResult.ConvertAnnotatedResponses(v2a, cs.ClockRate);

                            NimbleSegmentTelemetry segImp = new NimbleSegmentTelemetry
                            {
                                SegmentName = m.SegmentName,
                                RepeateCount = m.RepeatCount,
                                Impedances = v2//cs.ProcessMeasurementCall(m)
                            };
                            impedanceRecord.AddSegmentImpedanceResult(segImp);
                        }
                    }
                }
                else
                {
                    logger.Warn("Compiled sequence {0} not found. Probably need to do a scan");
                }
                logger.Info("Took {1:F4}s to calculate impedances for {0}. ({2})", measurementRecord, (DateTime.Now - start).TotalSeconds, measurementRecord.RecordDirectory);

                if (!Readonly)
                    impedanceRecord.Save();

                return impedanceRecord;
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
            return new NimbleImpedanceRecord();
        }

        public NimbleImpedanceRecord ProcessSequenceResponse(NimbleMeasurementRecord measurementRecord)
        {
            return ProcessSequenceResponse(measurementRecord, true, false);
        }

        //public NimbleImpedanceRecord ProcessSequenceResponseV2(NimbleMeasurementRecord measurementRecord)
        //{
        //    DateTime start = DateTime.Now;

        //    NimbleImpedanceRecord impedanceRecord = new NimbleImpedanceRecord(measurementRecord);

        //    var x = impedanceRecord.Load();
        //    if (x.HasValue)
        //        return x.Value;

        //    var measurements = measurementRecord.GetMeasurments();

        //    if (CompiledSequences.ContainsKey(measurementRecord.GenGuid.ToString()))
        //    {
        //        logger.Warn("Calculating impedances for {0} ({1})", measurementRecord, measurementRecord.RecordDirectory);
        //        CompiledSequence cs = CompiledSequences[measurementRecord.GenGuid.ToString()];

        //        var superSegments = CollateTelemetryResponses(measurements);

        //        foreach (NimbleSegmentMeasurment m in measurements)
        //        {
        //            NimbleSegmentTelemetry segImp = new NimbleSegmentTelemetry
        //            {
        //                SegmentName = m.SegmentName,
        //                RepeateCount = m.RepeatCount,
        //                Impedances = cs.ProcessMeasurementCall(m)
        //            };
        //            impedanceRecord.AddSegmentImpedanceResult(segImp);
        //        }
        //    }
        //    else
        //    {
        //        logger.Warn("Compiled sequence {0} not found. Probably need to do a scan");
        //    }
        //    logger.Info("Finished processing sequences response. Took {0}s", (DateTime.Now - start).TotalSeconds);

        //    impedanceRecord.Save();

        //    return impedanceRecord;
        //}

        ////collects the telemetry responses of multiple segment repeats into a single super segment
        //private static List<NimbleSegmentMeasurment> CollateTelemetryResponses(List<NimbleSegmentMeasurment> measurements)
        //{
        //    var groupedBySegments = measurements.GroupBy(a => a.SegmentName);

        //    List<NimbleSegmentMeasurment> superSegments = new List<NimbleSegmentMeasurment>();
        //    foreach (var segmentGroup in groupedBySegments)
        //    {
        //        NimbleSegmentMeasurment superSegment = new NimbleSegmentMeasurment();
        //        superSegment.path = "";
        //        superSegment.RepeatCount = -1;
        //        superSegment.NimbleResponses = new List<TelemetryResponse>();
        //        superSegment.SegmentsRun = new List<int>();
        //        superSegment.SegmentName = segmentGroup.Key;
        //        foreach (NimbleSegmentMeasurment measurement in segmentGroup)
        //        {
        //            superSegment.SegmentsRun.AddRange(measurement.SegmentsRun.ToArray());
        //            superSegment.NimbleResponses.AddRange(measurement.NimbleResponses.ToArray());
        //        }
        //        superSegments.Add(superSegment);
        //    }
        //    return superSegments;
        //}
    }

    public class FilesForGenerationGUID
    {
        public string PulseData_c = "", PulseData_h = "", Sequence_c = "", Sequence_h = "";

        public bool AllFilesReferenced
        {
            get { return PulseData_c != "" && PulseData_h != "" && Sequence_c != "" && Sequence_h != ""; }
        }

    }
}
