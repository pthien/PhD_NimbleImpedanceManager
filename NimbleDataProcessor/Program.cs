using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CommandLine;
using Nimble.Sequences;
using Nimble.Sequences.AliveDevices;
using System.IO;

namespace NimbleDataProcessor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var args = Environment.GetCommandLineArgs();
            var result = CommandLine.Parser.Default.ParseArguments<CmdLineOptions>(args);

            if (!result.Errors.Any())
            {
                if (!result.Value.VisualMode)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Form1());
                }
                else
                {
                    Console.WriteLine("hi");

                    List<NimbleMeasurementRecord> recordsall = SequenceFileManager.GetTelemetryRecords(result.Value.DataFolder);

                    ListSubjectSummary(recordsall);

                    SequenceFileManager sfm = new SequenceFileManager();
                    sfm.ScanDirectory(result.Value.SequenceFolder);

                    ListAllSubjectsSummary(recordsall);

                    var seqs = sfm.CompiledSequences.Values.ToList();


                    foreach (NimbleMeasurementRecord record in recordsall)
                    {
                        NimbleImpedanceRecord impRec = sfm.ProcessSequenceResponse(record, false, true);
                        
                    }

                    //MeasurementSummary.GenerateSummaryForSubject("15_523_KYE", result.Value.DataFolder, result.Value.SequenceFolder, result.Value.OutputFolder);

                    //ProcessAliveDevices(result);

                }
            }
        }

        private static void ProcessAliveDevices(ParserResult<CmdLineOptions> result)
        {
            var alive = AliveDevices.GetAliveDevices(result.Value.DataFolder);
            var reallyAlive = AliveDevices.GetReallyAliveDevices(result.Value.DataFolder);

            alive.AddRange(reallyAlive);

            var AllMeasures = new List<ReallyAliveDevicesMeasurement>(alive);
            AllMeasures.AddRange(reallyAlive);

            var names = AllMeasures.Where(x => x.SubjectName != "???").Select(x => x.SubjectName).Distinct();

            foreach (string name in names)
            {
                var subjectMeasures = AllMeasures.Where(x => x.SubjectName == "???" || x.SubjectName == name);

                var path = Path.Combine(result.Value.OutputFolder, "Is" + name + "Alive.csv");
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        var ordered = subjectMeasures.OrderBy(x => x.Timestamp).ToList();
                        foreach (var measure in ordered)
                        {
                            sw.WriteLine(measure.ToString());
                        }
                    }
                }


            }
        }

        private static void ListAllSubjectsSummary(List<NimbleMeasurementRecord> recordsall)
        {
            var subjects = recordsall.Select(x => x.SubjectName).Distinct();
            foreach (string s in subjects)
            {
                DisplaySubjectRecordingSummary(recordsall, s);
            }
        }

        private static void DisplaySubjectRecordingSummary(List<NimbleMeasurementRecord> recordsall, string s)
        {
            string fmt = "{0,-15}{1,-13}{2,-23:dd/MM/yy hh:mm:ss tt}{3,-23:dd/MM/yy hh:mm:ss tt}{4,-5:g2}";

            Console.WriteLine();
            Console.WriteLine("Subject: {0}", s);
            Console.WriteLine(fmt, "Guid", "Num Records", "Start Date", "End Date", "Days");
            var uniqueGenGuids = recordsall.Where(x => x.SubjectName == s).OrderBy(x => x.Timestamp)
                .Select(a => a.GenGuid.ToString()).Distinct().ToList();

            foreach (string guid in uniqueGenGuids)
            {
                var startDate = recordsall.Where(x => x.SubjectName == s && x.GenGuid.ToString() == guid)
                    .OrderBy(sub => sub.Timestamp)
                    .First().Timestamp;

                var endDate = recordsall.Where(x => x.SubjectName == s && x.GenGuid.ToString() == guid)
                    .OrderBy(sub => sub.Timestamp)
                    .Last().Timestamp;

                var numRecs = recordsall.Where(x => x.SubjectName == s && x.GenGuid.ToString() == guid).Count();

                Console.WriteLine(fmt, guid.Substring(0, 8) + "...", numRecs, startDate, endDate, (endDate - startDate).TotalDays);
            }
        }

        private static void ListSubjectSummary(List<NimbleMeasurementRecord> recordsall)
        {
            string fmt = "{0,-15}{1,-13}{2,-23:dd/MM/yy hh:mm:ss tt}{3,-23:dd/MM/yy hh:mm:ss tt}{4,-5:g2}";
            Console.WriteLine();
            Console.WriteLine(fmt, "Subject", "Num Records", "Start Date", "End Date", "Days");

            var subjects = recordsall.Select(x => x.SubjectName).Distinct();

            foreach (string s in subjects)
            {
                var startDate = recordsall.Where(x => x.SubjectName == s)
                    .OrderBy(sub => sub.Timestamp)
                    .First().Timestamp;
                var endDate = recordsall.Where(x => x.SubjectName == s)
                    .OrderBy(sub => sub.Timestamp)
                    .Last().Timestamp;

                var numRecs = recordsall.Where(x => x.SubjectName == s).Count();

                Console.WriteLine(fmt, s, numRecs, startDate, endDate, (endDate - startDate).TotalDays);
            }
        }
    }
}
