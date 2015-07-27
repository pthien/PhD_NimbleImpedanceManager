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
        public Dictionary<string, CompiledSequence> CompiledSequences;

        Regex regex_guid = new Regex("{GenerationGUID: ([A-Za-z0-9-]+)}");
        public SequenceFileManager()
        {
            FilesByGenGUID = new Dictionary<string, FilesForGenerationGUID>();
            CompiledSequences = new Dictionary<string, CompiledSequence>();
        }

        /// <summary>
        /// Scans a folder and all subfolders for Sequence or PulseData .c or .h files.
        /// </summary>
        /// <param name="path"></param>
        public void ScanDirectory(string path)
        {
            //if (path == "")
            //    path = @"\\prometheus\user$\thienp\VisionProcessingHardware\DualImplants";

            DoScanDirectory(path);

            foreach (KeyValuePair<string, FilesForGenerationGUID> kvp in FilesByGenGUID)
            {
                FilesForGenerationGUID filesforguid = kvp.Value;
                if (filesforguid.AllFilesReferenced && !CompiledSequences.ContainsKey(kvp.Key))
                {
                    CompiledSequence cs = new CompiledSequence(kvp.Value);
                    CompiledSequences.Add(kvp.Key, cs);
                    logger.Info("Built sequence for: {0}", kvp.Key);
                }

            }
        }
        private void DoScanDirectory(string path)
        {
            string[] files = Directory.GetFiles(path);
            foreach (string f in files)
            {
                TestIfValidFile(Path.Combine(path, f));
            }
            try
            {
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
                    var x = new DateTime(
                        Int32.Parse(timeparts[0]), Int32.Parse(timeparts[1]), Int32.Parse(timeparts[2]),
                        hours,
                        Int32.Parse(timeparts[4]), Int32.Parse(timeparts[5]));

                    temp.Timestamp = x;
                    temp.RecordDirectory = Path.Combine(containingDirectory, s);
                    results.Add(temp);
                }
            }

            return results;

        }

        public NimbleImpedanceRecord ProcessSequenceResponse(NimbleMeasurementRecord measurementRecord)
        {
            NimbleImpedanceRecord impedanceRecord = new NimbleImpedanceRecord(measurementRecord);

            var x = impedanceRecord.Load();
            if (x.HasValue)
                return x.Value;

            var measurements = measurementRecord.GetMeasurments();

            if (CompiledSequences.ContainsKey(measurementRecord.GenGuid.ToString()))
            {
                CompiledSequence cs = CompiledSequences[measurementRecord.GenGuid.ToString()];
                foreach (NimbleSegmentMeasurment m in measurements)
                {
                    NimbleSegmentImpedance segImp = new NimbleSegmentImpedance
                    {
                        SegmentName = m.SegmentName,
                        RepeateCount = m.RepeateCount,
                        Impedances = cs.ProcessMeasurementCall(m)
                    };
                    impedanceRecord.AddSegmentImpedanceResult(segImp);
                }
            }
            else
            {
                logger.Warn("Compiled sequence {0} not found. Probably need to do a scan");
            }

            impedanceRecord.Save();
            return impedanceRecord;
        }

    }

    public class FilesForGenerationGUID
    {
        public string PulseData_c, PulseData_h, Sequence_c, Sequence_h;

        public bool AllFilesReferenced
        {
            get { return PulseData_c != "" && PulseData_h != "" && Sequence_c != "" && Sequence_h != ""; }
        }

    }
}
