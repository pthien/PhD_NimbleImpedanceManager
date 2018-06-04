using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CommandLine;
using Nimble.Sequences;
using Nimble.Sequences.AliveDevices;
using System.IO;
using OfficeOpenXml;
using PIC_Sequence;

namespace NimbleDataProcessor
{
    public struct ReportedMeasure
    {
        public double Current_uA { get; set; }
        public int PhaseWidth_us { get; set; }
        public PulseType ElectrodeConfigType { get; set; }
        public Type measurementType { get; set; }
        public string Name { get; set; }
    }

    static class Program
    {
        static ReportedMeasure mp580 = new ReportedMeasure()
        {
            Current_uA = 75,
            ElectrodeConfigType = PulseType.MonoPolar,
            Name = "IMPEDANCE_MP580",
            measurementType = typeof(ImpedanceResult),
            PhaseWidth_us = 580
        };

        static ReportedMeasure mp400 = new ReportedMeasure()
        {
            Current_uA = 75,
            ElectrodeConfigType = PulseType.MonoPolar,
            Name = "IMPEDANCE_MP400",
            measurementType = typeof(ImpedanceResult),
            PhaseWidth_us = 400
        };

        static ReportedMeasure mp280 = new ReportedMeasure()
        {
            Current_uA = 75,
            ElectrodeConfigType = PulseType.MonoPolar,
            Name = "IMPEDANCE_MP280",
            measurementType = typeof(ImpedanceResult),
            PhaseWidth_us = 280
        };

        static ReportedMeasure mp145 = new ReportedMeasure()
        {
            Current_uA = 75,
            ElectrodeConfigType = PulseType.MonoPolar,
            Name = "IMPEDANCE_MP145",
            measurementType = typeof(ImpedanceResult),
            PhaseWidth_us = 145
        };

        static ReportedMeasure mp25 = new ReportedMeasure()
        {
            Current_uA = 75,
            ElectrodeConfigType = PulseType.MonoPolar,
            Name = "IMPEDANCE_MP25",
            measurementType = typeof(ImpedanceResult),
            PhaseWidth_us = 25
        };

        static ReportedMeasure cg25 = new ReportedMeasure()
        {
            Current_uA = 75,
            ElectrodeConfigType = PulseType.CommonGround,
            Name = "IMPEDANCE_CG25",
            measurementType = typeof(ImpedanceResult),
            PhaseWidth_us = 25
        };

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var args = Environment.GetCommandLineArgs();
            var result = Parser.Default.ParseArguments<CmdLineOptions>(args);



            if (!result.Errors.Any())
            {
                if (string.IsNullOrEmpty(result.Value.DataFolder))
                {
                    Console.WriteLine("Select Data folder");
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    var res = fbd.ShowDialog();
                    if (res == DialogResult.OK)
                        result.Value.DataFolder = fbd.SelectedPath;
                    else
                        return;
                }

                if (string.IsNullOrEmpty(result.Value.SequenceFolder))
                {
                    Console.WriteLine("Select sequence folder");
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    var res = fbd.ShowDialog();
                    if (res == DialogResult.OK)
                        result.Value.SequenceFolder = fbd.SelectedPath;
                    else
                        return;
                }

                Console.WriteLine("hi");

                List<NimbleMeasurementRecord> recordsall = SequenceFileManager.GetTelemetryRecords(result.Value.DataFolder);

                List<string> subjects = ListSubjectSummary(recordsall);

                //subjects.Clear();
                //subjects.Add("15_520_DINAH");

                SequenceFileManager sfm = new SequenceFileManager();
                sfm.ScanDirectory(result.Value.SequenceFolder);

                ListAllSubjectsSummary(recordsall, sfm.CompiledSequences.Keys.ToList());

                Console.WriteLine("\nPress any key to continue and process data...");
                Console.ReadKey();

                Console.WriteLine("\nProcessing records...");


                var seqs = sfm.CompiledSequences.Values.ToList();

                int testCount = 0;
                int recordsCount = recordsall.Count();
                List<NimbleImpedanceRecord> allImpedanceRecords = new List<NimbleImpedanceRecord>();
                foreach (NimbleMeasurementRecord record in recordsall)
                {
                    //using (ProgressBar progresss = new ProgressBar())
                    {
                        NimbleImpedanceRecord impRec = sfm.ProcessSequenceResponse(record, false, true);
                        allImpedanceRecords.Add(impRec);
                        testCount++;
                        Console.Write("{0}/{1}\r", testCount, recordsCount);
                    }
                    //if (testCount > 100)
                    //    break;
                }

                Console.WriteLine("\nProcessing Complete...");

                //Console.WriteLine("\nPress any key to generate summaries...");
                //Console.ReadKey();

                //allImpedanceRecords = allImpedanceRecords.OrderBy(x => x.Timestamp).ToList();

                //MeasurementSummary.GenerateSummaryForSubject("15_523_KYE", result.Value.DataFolder, result.Value.SequenceFolder, result.Value.OutputFolder);

                //ProcessAliveDevices(result);

                //measure: 580, monopolar
                //var h = allImpedanceRecords.Where(x=>x.SegmentImpedances. .Select(x => x.SegmentImpedances);

                subjects.Sort();

                //Waveforms
                //foreach (string subject in subjects)
                //{
                //    var subjectrecords = allImpedanceRecords.Where(x => x.SubjectName == subject).OrderBy(x => x.Timestamp).ToList();

                //    var electrodes = GetDistictElectrodes(subjectrecords);

                //    ExcelPackage pkg = new ExcelPackage();
                //    var sheet = pkg.Workbook.Worksheets.Add("data V10");
                //    var sheet_avg = pkg.Workbook.Worksheets.Add("averages V10");

                //    var sheet_2 = pkg.Workbook.Worksheets.Add("data v2");
                //    var sheet_avg_2 = pkg.Workbook.Worksheets.Add("averages v2");

                //    AddVoltageDataToSheets(subjectrecords, electrodes, sheet, sheet_avg, "_V10");
                //    AddVoltageDataToSheets(subjectrecords, electrodes, sheet_2, sheet_avg_2, "_V2");

                //    pkg.SaveAs(new FileInfo(subject + "_waveforms.xlsx"));
                //}

                //return;
                foreach (string subject in subjects)
                {
                    ExcelPackage pkg = new ExcelPackage();

                    var subjectrecords = allImpedanceRecords.Where(x => x.SubjectName == subject).OrderBy(x => x.Timestamp).ToList();

                    AddMeasureToWorkSheet(subjectrecords, pkg, mp25);
                    AddMeasureToWorkSheet(subjectrecords, pkg, cg25);
                    AddMeasureToWorkSheet(subjectrecords, pkg, mp145);
                    AddMeasureToWorkSheet(subjectrecords, pkg, mp280);
                    AddMeasureToWorkSheet(subjectrecords, pkg, mp400);
                    AddMeasureToWorkSheet(subjectrecords, pkg, mp580);

                    pkg.SaveAs(new FileInfo(subject + ".xlsx"));
                }

                foreach (string subject in subjects)
                {
                    ExcelPackage pkg = new ExcelPackage();

                    var subjectrecords = allImpedanceRecords.Where(x => x.SubjectName == subject).OrderBy(x => x.Timestamp).ToList();

                    AddMeasureToWorkSheet_averages(subjectrecords, pkg, mp25);
                    AddMeasureToWorkSheet_averages(subjectrecords, pkg, cg25);
                    AddMeasureToWorkSheet_averages(subjectrecords, pkg, mp145);
                    AddMeasureToWorkSheet_averages(subjectrecords, pkg, mp280);
                    AddMeasureToWorkSheet_averages(subjectrecords, pkg, mp400);
                    AddMeasureToWorkSheet_averages(subjectrecords, pkg, mp580);

                    pkg.SaveAs(new FileInfo(subject + "_averages.xlsx"));
                }

                CreateSummaryWorksheetsForSubjects(subjects, allImpedanceRecords);

                //foreach (string measureName in g)
                //{
                //    var sheet = pkg.Workbook.Worksheets.Add(measureName);

                //    int count = 1;
                //    foreach (var electrodeName in f)
                //    {
                //        sheet.Cells[1, count + 1].Value = electrodeName;
                //    }

                //}

            }
        }

        private static void AddVoltageDataToSheets(List<NimbleImpedanceRecord> subjectrecords, List<string> electrodes, ExcelWorksheet sheet, ExcelWorksheet sheet_avg, string voltageSuffix = "_V10")
        {
            int recordCount = 0;
            int rowNum = 2;
            foreach (var record in subjectrecords)
            {
                sheet_avg.Cells[1, recordCount * 7 + 1].Value = record.Timestamp.ToString("yyyy/MM/dd HH:mm");
                sheet_avg.Cells[1, recordCount * 7 + 1 + 1].Value = 25;
                sheet_avg.Cells[1, recordCount * 7 + 1 + 2].Value = 145;
                sheet_avg.Cells[1, recordCount * 7 + 1 + 3].Value = 280;
                sheet_avg.Cells[1, recordCount * 7 + 1 + 4].Value = 400;
                sheet_avg.Cells[1, recordCount * 7 + 1 + 5].Value = 580;


                var maxRowCountThisRecord = 0;
                int elecCount = -1;
                foreach (var electrode in electrodes)
                {
                    elecCount++;

                    var recs_mp25 = record.SegmentImpedances.Where(x => x.SegmentName.Contains("MP25" + voltageSuffix)).SelectMany(x => x.Impedances)
                        .Where(x => x.Electrode == int.Parse(electrode.Remove(0, 1)))
                        .Where(x => x.Implant == (electrode[0] == 'A' ? Implant.ImplantA : Implant.ImplantB))
                        .Where(x => x.Current_uA == 75 && x.PhaseWidth_us == 25 && x.Type == PulseType.MonoPolar)
                        .Where(x => x is ImpedanceResult).Cast<ImpedanceResult>().Select(x => x.Voltage_V);

                    var recs_mp145 = record.SegmentImpedances.Where(x => x.SegmentName.Contains("MP145" + voltageSuffix)).SelectMany(x => x.Impedances)
                      .Where(x => x.Electrode == int.Parse(electrode.Remove(0, 1)))
                      .Where(x => x.Implant == (electrode[0] == 'A' ? Implant.ImplantA : Implant.ImplantB))
                      .Where(x => x.Current_uA == 75 && x.PhaseWidth_us == 145 && x.Type == PulseType.MonoPolar)
                        .Where(x => x is ImpedanceResult).Cast<ImpedanceResult>().Select(x => x.Voltage_V);

                    var recs_mp280 = record.SegmentImpedances.Where(x => x.SegmentName.Contains("MP280" + voltageSuffix)).SelectMany(x => x.Impedances)
                        .Where(x => x.Electrode == int.Parse(electrode.Remove(0, 1)))
                        .Where(x => x.Implant == (electrode[0] == 'A' ? Implant.ImplantA : Implant.ImplantB))
                        .Where(x => x.Current_uA == 75 && x.PhaseWidth_us == 280 && x.Type == PulseType.MonoPolar)
                        .Where(x => x is ImpedanceResult).Cast<ImpedanceResult>().Select(x => x.Voltage_V);

                    var recs_mp290 = record.SegmentImpedances.Where(x => x.SegmentName.Contains("MP290" + voltageSuffix)).SelectMany(x => x.Impedances)
                      .Where(x => x.Electrode == int.Parse(electrode.Remove(0, 1)))
                      .Where(x => x.Implant == (electrode[0] == 'A' ? Implant.ImplantA : Implant.ImplantB))
                      .Where(x => x.Current_uA == 75 && x.PhaseWidth_us == 290 && x.Type == PulseType.MonoPolar)
                      .Where(x => x is ImpedanceResult).Cast<ImpedanceResult>().Select(x => x.Voltage_V);

                    var recs_mp400 = record.SegmentImpedances.Where(x => x.SegmentName.Contains("MP400" + voltageSuffix)).SelectMany(x => x.Impedances)
                      .Where(x => x.Electrode == int.Parse(electrode.Remove(0, 1)))
                      .Where(x => x.Implant == (electrode[0] == 'A' ? Implant.ImplantA : Implant.ImplantB))
                      .Where(x => x.Current_uA == 75 && x.PhaseWidth_us == 400 && x.Type == PulseType.MonoPolar)
                        .Where(x => x is ImpedanceResult).Cast<ImpedanceResult>().Select(x => x.Voltage_V);

                    var recs_mp580 = record.SegmentImpedances.Where(x => x.SegmentName.Contains("MP580" + voltageSuffix)).SelectMany(x => x.Impedances)
                     .Where(x => x.Electrode == int.Parse(electrode.Remove(0, 1)))
                     .Where(x => x.Implant == (electrode[0] == 'A' ? Implant.ImplantA : Implant.ImplantB))
                     .Where(x => x.Current_uA == 75 && x.PhaseWidth_us == 580 && x.Type == PulseType.MonoPolar)
                        .Where(x => x is ImpedanceResult).Cast<ImpedanceResult>().Select(x => x.Voltage_V);

                    if (recs_mp280.Count() > recs_mp290.Count())
                        recs_mp290 = recs_mp280;

                    List<int> counts = new List<int>();
                    counts.Add(recs_mp25.Count());
                    counts.Add(recs_mp145.Count());
                    counts.Add(recs_mp280.Count());
                    counts.Add(recs_mp290.Count());
                    counts.Add(recs_mp400.Count());
                    counts.Add(recs_mp580.Count());

                    int maxCount = counts.Max();
                    int baseCol = elecCount * 7 + 1;
                    maxRowCountThisRecord = Math.Max(maxCount, maxRowCountThisRecord);
                    sheet.Cells[rowNum - 1, baseCol].Value = electrode;
                    sheet.Cells[rowNum - 1, baseCol + 1].Value = 25;
                    sheet.Cells[rowNum - 1, baseCol + 2].Value = 145;
                    sheet.Cells[rowNum - 1, baseCol + 3].Value = 280;
                    sheet.Cells[rowNum - 1, baseCol + 4].Value = 400;
                    sheet.Cells[rowNum - 1, baseCol + 5].Value = 580;
                    for (int i = 0; i < maxCount; i++)
                    {
                        sheet.Cells[rowNum + i, baseCol].Value = record.Timestamp.ToString("yyyy/MM/dd HH:mm");

                        if (recs_mp25.Count() > i) sheet.Cells[rowNum + i, baseCol + 1].Value = recs_mp25.ElementAt(i);
                        if (recs_mp145.Count() > i) sheet.Cells[rowNum + i, baseCol + 2].Value = recs_mp145.ElementAt(i);
                        if (recs_mp290.Count() > i) sheet.Cells[rowNum + i, baseCol + 3].Value = recs_mp290.ElementAt(i);
                        if (recs_mp400.Count() > i) sheet.Cells[rowNum + i, baseCol + 4].Value = recs_mp400.ElementAt(i);
                        if (recs_mp580.Count() > i) sheet.Cells[rowNum + i, baseCol + 5].Value = recs_mp580.ElementAt(i);
                    }

                    sheet_avg.Cells[elecCount + 2, recordCount * 7 + 1].Value = electrode;
                    if (recs_mp25.Count() > 0) sheet_avg.Cells[elecCount + 2, recordCount * 7 + 1 + 1].Value = recs_mp25.Average();
                    if (recs_mp145.Count() > 0) sheet_avg.Cells[elecCount + 2, recordCount * 7 + 1 + 2].Value = recs_mp145.Average();
                    if (recs_mp290.Count() > 0) sheet_avg.Cells[elecCount + 2, recordCount * 7 + 1 + 3].Value = recs_mp290.Average();
                    if (recs_mp400.Count() > 0) sheet_avg.Cells[elecCount + 2, recordCount * 7 + 1 + 4].Value = recs_mp400.Average();
                    //sheet.Cells[elecCount+2, recordCount * 7 + 1 + 5].Value = recs_mp580.Average();


                }//for each electrode
                recordCount++;
                rowNum += maxRowCountThisRecord + 2;
            }//foreach record
        }

        private static void CreateSummaryWorksheetsForSubjects(List<string> subjects, List<NimbleImpedanceRecord> allImpedanceRecords)
        {

        }

        private static void AddMeasureToWorkSheet_averages(List<NimbleImpedanceRecord> allRecs, ExcelPackage pkg, ReportedMeasure repMeas)
        {
            //create the sheets
            ExcelWorksheet sheet1 = null;
            ExcelWorksheet sheet2 = null;
            if (repMeas.measurementType == typeof(ImpedanceResult))
            {
                sheet1 = pkg.Workbook.Worksheets.Add(repMeas.Name + "(Impedance)");
                sheet2 = pkg.Workbook.Worksheets.Add(repMeas.Name + "(Voltage)");
            }
            else if (repMeas.measurementType == typeof(ComplianceResult))
            {
                sheet1 = pkg.Workbook.Worksheets.Add(repMeas.Name + "(Compliance)");
            }

            //get the unique electrode names
            List<string> distinctElectrodesNames = GetDistictElectrodesForMeasure(allRecs, repMeas);

            //add headers to the columns of the sheets
            int colCount = 1;
            foreach (var electrodeName in distinctElectrodesNames)
            {
                sheet1.Cells[1, colCount + 1].Value = electrodeName;
                if (repMeas.measurementType == typeof(ImpedanceResult))
                    sheet2.Cells[1, colCount + 1].Value = electrodeName;
                colCount++;
            }
            /*for each timepoint,
             *  iterate over each electrode
             *      and for each electrode, print all measurements for that timepoint
             * */

            int rowCount = 2;
            foreach (var impRec in allRecs)
            {
                var recs = impRec.SegmentImpedances.Where(x => x.SegmentName.StartsWith(repMeas.Name));
                var check = recs.SelectMany(x => x.Impedances).Where(x => x.Current_uA == repMeas.Current_uA && x.Type == repMeas.ElectrodeConfigType && x.PhaseWidth_us == repMeas.PhaseWidth_us);

                if (!check.Any())
                    continue;

                colCount = 2;
                foreach (var electrodeName in distinctElectrodesNames)
                {

                    var electrodeMeasures = check.
                        Where(x => x.Implant == (electrodeName[0] == 'A' ? Implant.ImplantA : Implant.ImplantB)).
                        Where(x => x.Electrode == int.Parse(electrodeName.Remove(0, 1)));

                    if (!electrodeMeasures.Any())
                        continue;

                    double values = 0;
                    double values2 = 0;
                    if (repMeas.measurementType == typeof(ImpedanceResult))
                    {
                        var emes = electrodeMeasures.Where(x => x is ImpedanceResult).Cast<ImpedanceResult>();
                        values = emes.Select(x => x.Impedance_ohms).Average();
                        values2 = emes.Select(x => x.Voltage_V).Average();
                    }

                    sheet1.Cells[rowCount, colCount].Value = values;
                    if (repMeas.measurementType == typeof(ImpedanceResult))
                        sheet2.Cells[rowCount, colCount].Value = values2;


                    colCount++;
                }

                sheet1.Cells[rowCount, 1].Value = impRec.Timestamp.ToString("yyyy/MM/dd HH:mm");
                if (repMeas.measurementType == typeof(ImpedanceResult))
                    sheet2.Cells[rowCount, 1].Value = impRec.Timestamp.ToString("yyyy/MM/dd HH:mm");

                rowCount += 1;
                //sheet.Cells[2, count + 1].Value = imp;
            }
        }

        private static void AddMeasureToWorkSheet(List<NimbleImpedanceRecord> allRecs, ExcelPackage pkg, ReportedMeasure repMeas)
        {
            //create the sheets
            ExcelWorksheet sheet1 = null;
            ExcelWorksheet sheet2 = null;
            if (repMeas.measurementType == typeof(ImpedanceResult))
            {
                sheet1 = pkg.Workbook.Worksheets.Add(repMeas.Name + "(Impedance)");
                sheet2 = pkg.Workbook.Worksheets.Add(repMeas.Name + "(Voltage)");
            }
            else if (repMeas.measurementType == typeof(ComplianceResult))
            {
                sheet1 = pkg.Workbook.Worksheets.Add(repMeas.Name + "(Compliance)");
            }

            //get the unique electrode names
            List<string> distinceElectrodesNames = GetDistictElectrodesForMeasure(allRecs, repMeas);

            //add headers to the columns of the sheets
            int colCount = 1;
            foreach (var electrodeName in distinceElectrodesNames)
            {
                sheet1.Cells[1, colCount + 1].Value = electrodeName;
                if (repMeas.measurementType == typeof(ImpedanceResult))
                    sheet2.Cells[1, colCount + 1].Value = electrodeName;
                colCount++;
            }
            /*for each timepoint,
             *  iterate over each electrode
             *      and for each electrode, print all measurements for that timepoint
             * */

            int rowCount = 1;
            foreach (var impRec in allRecs)
            {
                var recs = impRec.SegmentImpedances.Where(x => x.SegmentName.StartsWith(repMeas.Name));
                var check = recs.SelectMany(x => x.Impedances).Where(x => x.Current_uA == repMeas.Current_uA && x.Type == repMeas.ElectrodeConfigType && x.PhaseWidth_us == repMeas.PhaseWidth_us);

                int maxRowsThisTimepoint = 0;
                colCount = 2;
                foreach (var electrodeName in distinceElectrodesNames)
                {
                    int rowsThisElec = 1;

                    var electrodeMeasures = check.
                        Where(x => x.Implant == (electrodeName[0] == 'A' ? Implant.ImplantA : Implant.ImplantB)).
                        Where(x => x.Electrode == int.Parse(electrodeName.Remove(0, 1)));


                    double[] values = new double[0];
                    double[] values2 = new double[0];
                    if (repMeas.measurementType == typeof(ImpedanceResult))
                    {
                        var emes = electrodeMeasures.Where(x => x is ImpedanceResult).Cast<ImpedanceResult>();
                        values = emes.Select(x => x.Impedance_ohms).ToArray();
                        values2 = emes.Select(x => x.Voltage_V).ToArray();
                    }

                    for (int i = 0; i < values.Length; i++)
                    {
                        sheet1.Cells[rowCount + rowsThisElec, colCount].Value = values[i];
                        if (repMeas.measurementType == typeof(ImpedanceResult))
                            sheet2.Cells[rowCount + rowsThisElec, colCount].Value = values2[i];
                        rowsThisElec++;
                        maxRowsThisTimepoint = Math.Max(rowsThisElec, maxRowsThisTimepoint);
                    }

                    colCount++;
                }

                for (int i = 1; i < maxRowsThisTimepoint; i++)
                {
                    sheet1.Cells[rowCount + i, 1].Value = impRec.Timestamp.ToString("yyyy/MM/dd HH:mm");
                    if (repMeas.measurementType == typeof(ImpedanceResult))
                        sheet2.Cells[rowCount + i, 1].Value = impRec.Timestamp.ToString("yyyy/MM/dd HH:mm");
                }
                rowCount += maxRowsThisTimepoint;
                maxRowsThisTimepoint = 0;
                //sheet.Cells[2, count + 1].Value = imp;
            }
        }

        private static List<string> GetDistictElectrodes(IEnumerable<NimbleImpedanceRecord> allRecs)
        {
            var e = allRecs.SelectMany(x => x.SegmentImpedances).SelectMany(x => x.Impedances);
            List<string> distinceElectrodesNames = e.
                Select(x => x.ElectrodeName).
                Distinct().
                OrderBy(x => int.Parse(x.Remove(0, 1))).
                OrderBy(x => x[0]).ToList();
            return distinceElectrodesNames;
        }

        private static List<string> GetDistictElectrodesForMeasure(List<NimbleImpedanceRecord> allRecs, ReportedMeasure repMeas)
        {
            var e = allRecs.SelectMany(x => x.SegmentImpedances).SelectMany(x => x.Impedances);
            List<string> distinceElectrodesNames = e.
                Where(x => x.PhaseWidth_us == repMeas.PhaseWidth_us && x.Current_uA == repMeas.Current_uA && x.Type == repMeas.ElectrodeConfigType).
                Select(x => x.ElectrodeName).
                Distinct().
                OrderBy(x => int.Parse(x.Remove(0, 1))).
                OrderBy(x => x[0]).ToList();
            return distinceElectrodesNames;
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

        private static void ListAllSubjectsSummary(List<NimbleMeasurementRecord> recordsall, List<string> sequenceIDs)
        {
            var subjects = recordsall.Select(x => x.SubjectName).Distinct();
            foreach (string s in subjects)
            {
                DisplaySubjectRecordingSummary(recordsall, s, sequenceIDs);
            }
        }

        private static void DisplaySubjectRecordingSummary(List<NimbleMeasurementRecord> recordsall, string s, List<string> sequenceIDs)
        {
            string fmt = "{0,-15}{1,-13}{2,-23:dd/MM/yy hh:mm:ss tt}{3,-23:dd/MM/yy hh:mm:ss tt}{4,-5:g2}{5,-4}";

            Console.WriteLine();
            Console.WriteLine("Subject: {0}", s);
            Console.WriteLine(fmt, "Guid", "Num Records", "Start Date", "End Date", "Days", "Seq");
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

                bool seqFound = sequenceIDs.Contains(guid);


                Console.WriteLine(fmt, guid.Substring(0, 8) + "...", numRecs, startDate, endDate, (endDate - startDate).TotalDays, seqFound ? "y" : "n");
            }
        }

        private static List<string> ListSubjectSummary(List<NimbleMeasurementRecord> recordsall)
        {
            string fmt = "{0,-15}{1,-13}{2,-23:dd/MM/yy hh:mm: tt}{3,-23:dd/MM/yy hh:mm:ss tt}{4,-5:g2}";
            Console.WriteLine();
            Console.WriteLine(fmt, "Subject", "Num Records", "Start Date", "End Date", "Days");

            var subjects = recordsall.Select(x => x.SubjectName).Distinct().ToList();

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
            return subjects;
        }
    }
}
