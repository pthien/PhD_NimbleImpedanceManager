using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PIC_Sequence;

namespace Nimble.Sequences
{
    public struct Measure
    {
        public double Current_uA { get; set; }
        public int PhaseWidth_us { get; set; }
        public PulseType ElectrodeConfigType { get; set; }
        public Type measurementType { get; set; }
        public string Name { get; set; }
    }

    public static class MeasurementSummary
    {
        static public void GenerateSummaryForSubject(string subjectName)
        {
            subjectName = "15_520_DINAH";
            //string folder = @"C:\Users\Patrick\Dropbox\Preitaly\NimbleBluetoothImpedanceManager\NimbleBluetoothImpedanceManager\bin\Debug\Output";
            //string folder = @"S:\Dropbox\Preitaly\NimbleBluetoothImpedanceManager\NimbleBluetoothImpedanceManager\bin\Debug\Output";
            //string folder = @"C:\Users\thienp\Desktop\Kraken processor test\Real DAta\Nimble Logs";
            //string folder = @"C:\Users\thienp\Desktop\chronics v2.2\20151001\20151001\Active Chronic Laptop Backups\20150825\Nimble Logs";
            string folder = @"C:\Users\thienp\Desktop\Active chronics backup 20151119\Active Chronics V2.2\Nimble Logs";
            List<NimbleMeasurementRecord> recordsall = SequenceFileManager.GetTelemetryRecords(folder);

            var myrecs = recordsall.Where(x => x.SubjectName == subjectName).OrderBy(x => x.Timestamp);

            //Dictionary<string, StreamWriter> knownMeasurementTypes = new Dictionary<string, StreamWriter>();

            SequenceFileManager fileMan = new SequenceFileManager();
            //fileMan.ScanDirectory()

            Measure mp580 = new Measure()
            {
                Current_uA = 75,
                ElectrodeConfigType = PulseType.MonoPolar,
                Name = "IMPEDANCE_MP580",
                measurementType = typeof(ImpedanceResult),
                PhaseWidth_us = 580
            };


            Measure mp400 = new Measure()
            {
                Current_uA = 75,
                ElectrodeConfigType = PulseType.MonoPolar,
                Name = "IMPEDANCE_MP400",
                measurementType = typeof(ImpedanceResult),
                PhaseWidth_us = 400
            };

            Measure mp290 = new Measure()
            {
                Current_uA = 75,
                ElectrodeConfigType = PulseType.MonoPolar,
                Name = "IMPEDANCE_MP290",
                measurementType = typeof(ImpedanceResult),
                PhaseWidth_us = 290
            };

            Measure mp145 = new Measure()
            {
                Current_uA = 75,
                ElectrodeConfigType = PulseType.MonoPolar,
                Name = "IMPEDANCE_MP145",
                measurementType = typeof(ImpedanceResult),
                PhaseWidth_us = 145
            };

            Measure mp25 = new Measure()
            {
                Current_uA = 75,
                ElectrodeConfigType = PulseType.MonoPolar,
                Name = "IMPEDANCE_MP25",
                measurementType = typeof(ImpedanceResult),
                PhaseWidth_us = 25
            };

            Measure cg25 = new Measure()
            {
                Current_uA = 75,
                ElectrodeConfigType = PulseType.CommonGround,
                Name = "IMPEDANCE_CG25",
                measurementType = typeof(ImpedanceResult),
                PhaseWidth_us = 25
            };

            Measure compliance = new Measure()
            {
                Name = "Compliance",
                measurementType = typeof(ComplianceResult),
            };

            //GeneratesummaryForMeasure(myrecs, mp580, fileMan);
            GeneratesummaryForMeasure(myrecs, mp400, fileMan);
            //GeneratesummaryForMeasure(myrecs, mp290, fileMan);
            //GeneratesummaryForMeasure(myrecs, mp145, fileMan);
            //GeneratesummaryForMeasure(myrecs, mp25, fileMan);
            //GeneratesummaryForMeasure(myrecs, cg25, fileMan);
            //GeneratesummaryForMeasure(myrecs, compliance, fileMan);

        }

        private static void GeneratesummaryForMeasure(IEnumerable<NimbleMeasurementRecord> records, Measure measure, SequenceFileManager fileMan)
        {
            if (File.Exists(measure.Name + ".csv"))
                File.Delete(measure.Name + ".csv");
            StreamWriter sw;
            sw = new StreamWriter(measure.Name + ".csv", true);

            sw.Write("Timestamp, ");
            for (int i = 1; i < 23; i++)
                sw.Write("A{0}, ", i);
            for (int i = 1; i < 23; i++)
                sw.Write("B{0}, ", i);
            sw.WriteLine();
        
            foreach (NimbleMeasurementRecord record in records.OrderBy(r => r.Timestamp))
            {
                NimbleImpedanceRecord impedanceRec = fileMan.ProcessSequenceResponse(record);

                var concatted = impedanceRec.SegmentImpedances.Where(x=>!x.SegmentName.StartsWith("IMPEDANCE_MP25")).Select(x => x.Impedances);
                var con = concatted.SelectMany(x => x).ToArray();

                if (measure.measurementType == typeof(ImpedanceResult))
                {
                    var measureResults = con.OfType<ImpedanceResult>().Where(x =>
                        //x.Implant == Implant.ImplantA &&
                        x.PhaseWidth_us == measure.PhaseWidth_us &&
                        x.Current_uA == measure.Current_uA &&
                        x.Type == measure.ElectrodeConfigType);


                    sw.Write("{0} {1}, ", record.Timestamp.ToShortDateString(), record.Timestamp.ToShortTimeString());
                    PrintImpedanceMeasureImplant(measureResults, sw, Implant.ImplantA);
                    PrintImpedanceMeasureImplant(measureResults, sw, Implant.ImplantB);
                    sw.WriteLine();
                }
                else if (measure.measurementType == typeof(ComplianceResult))
                {
                    var measureResults = con.OfType<ComplianceResult>().Where(x =>
                        //x.Implant == Implant.ImplantA &&
                        //x.PhaseWidth_us == measure.PhaseWidth_us &&
                        //x.Current_uA == measure.Current_uA &&
                      x.Type == measure.ElectrodeConfigType);

                    sw.Write("{0} {1}, ", record.Timestamp.ToShortDateString(), record.Timestamp.ToShortTimeString());
                    PrintComplianceMeasureImplant(measureResults, sw, Implant.ImplantA);
                    PrintComplianceMeasureImplant(measureResults, sw, Implant.ImplantB);
                    sw.WriteLine();
                }

            }
            sw.Close();
        }

        private static void PrintComplianceMeasureImplant(IEnumerable<ComplianceResult> measure1, StreamWriter sw, Implant implant)
        {
            for (int electrode = 1; electrode <= 22; electrode++)
            {
                bool gotVal = false;
                double compliance = -1;
                var c = measure1.Where(x => x.Electrode == electrode && x.Implant == implant)
                    .Select(x => x.InCompliance);
                if (c.Any())
                {
                    compliance = 100*(c.Where(v => v == true).Count() / c.Count());
                    gotVal = true;
                }

                if (gotVal)
                    sw.Write("{0}, ", compliance);
                else
                    sw.Write(",");
            }
        }

        private static void PrintImpedanceMeasureImplant(IEnumerable<ImpedanceResult> measure1, StreamWriter sw, Implant implant)
        {
            for (int electrode = 1; electrode <= 22; electrode++)
            {
                bool gotVal = false;
                double impedance = -1;
                var c = measure1.Where(x => x.Electrode == electrode && x.Implant == implant)
                    .Select(x => x._Impedance_ohms);
                var b = measure1.Where(x => x.Electrode == electrode && x.Implant == implant);
                if (c.Any())
                {
                    impedance = c.Median(); //c.Average();
                    gotVal = true;
                }

                if (gotVal)
                    sw.Write("{0}, {1}, ", impedance, c.Count());
                else
                    sw.Write(", ,");
            }
        }

        public static double Median<T>(this IEnumerable<T> source)
        {
            int count = source.Count();
            //if (count == 0)
            //    return null;

            source = source.OrderBy(n => n);

            int midpoint = count / 2;
            if (count % 2 == 0)
                return (Convert.ToDouble(source.ElementAt(midpoint - 1)) + Convert.ToDouble(source.ElementAt(midpoint))) / 2.0;
            else
                return Convert.ToDouble(source.ElementAt(midpoint));
        }
    }
}
