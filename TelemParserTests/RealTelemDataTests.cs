using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nimble.Sequences;

namespace TelemParserTests
{
    [TestClass]
    public class RealTelemDataTests
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        private static CompiledSequence GetTestSequence_RealDataMary()
        {
            FilesForGenerationGUID files = new FilesForGenerationGUID();
            files.Sequence_h = @"15_518_MARY_7e4254d7-5f13-4358-ab35-627c5c47baf7\Sequence.h";
            files.Sequence_c = @"15_518_MARY_7e4254d7-5f13-4358-ab35-627c5c47baf7\Sequence.c";
            files.PulseData_h = @"15_518_MARY_7e4254d7-5f13-4358-ab35-627c5c47baf7\PulseData.h";
            files.PulseData_c = @"15_518_MARY_7e4254d7-5f13-4358-ab35-627c5c47baf7\PulseData.c";

            CompiledSequence seq = new CompiledSequence(files);
            return seq;
        }

        [TestMethod]
        public void CompiledSequence_Mary_2015_08_14_2pm_MissingInitialSegmentChangeMessage()
        {
            var seq = GetTestSequence_RealDataMary();

            NimbleMeasurementRecord rec = new NimbleMeasurementRecord();
            rec.RecordDirectory =
                @"15_518_MARY-84EB1877AF2A-7e4254d7-5f13-4358-ab35-627c5c47baf7-2015-08-14_02-00-30-PM";

            List<NimbleSegmentMeasurment> measurements = rec.GetMeasurments();
            NimbleSegmentMeasurment m =
                measurements.Where(x => x.RepeatCount == 5 && x.SegmentName == "IMPEDANCE_MP25_V2").First();

            List<TelemetryResult> processed = seq.ProcessMeasurementCall(m);

            Assert.AreEqual(11, processed.Count());
            Assert.AreEqual(11, processed.OfType<ImpedanceResult>().Count());

            List<ImpedanceResult> imps = processed.Cast<ImpedanceResult>().ToList();
           
            Assert.AreEqual(11, imps.Count);
            Assert.AreEqual(5095.541401, imps[0]._Impedance_ohms, 100);
            Assert.AreEqual(5170.068027, imps[1]._Impedance_ohms, 100);
            Assert.AreEqual(4749.787956, imps[2]._Impedance_ohms, 100);
            Assert.AreEqual(4365.180467, imps[3]._Impedance_ohms, 100);
            Assert.AreEqual(4891.719745, imps[4]._Impedance_ohms, 100);
            Assert.AreEqual(13384.28875, imps[5]._Impedance_ohms, 100);
            Assert.AreEqual(13316.3482, imps[6]._Impedance_ohms,  100);
            Assert.AreEqual(4263.269639, imps[7]._Impedance_ohms, 100);
            Assert.AreEqual(4127.388535, imps[8]._Impedance_ohms, 100);
            Assert.AreEqual(3380.042463, imps[9]._Impedance_ohms, 100);
            Assert.AreEqual(4789.808917, imps[10]._Impedance_ohms,100);

            
        }

        [TestMethod]
        public void CompiledSequence_Mary_2015_08_14_2pm()
        {
            var seq = GetTestSequence_RealDataMary();

            NimbleMeasurementRecord rec = new NimbleMeasurementRecord();
            rec.RecordDirectory =
                @"15_518_MARY-84EB1877AF2A-7e4254d7-5f13-4358-ab35-627c5c47baf7-2015-08-14_02-00-30-PM";

            List<NimbleSegmentMeasurment> measurements = rec.GetMeasurments();

            Assert.AreEqual(54, measurements.Count);

            foreach (NimbleSegmentMeasurment meas in measurements)
            {
                List<TelemetryResult> imps = seq.ProcessMeasurementCall(meas);



                // Assert.IsTrue(impedances[0] is ImpedanceResult);
            }
            List<TelemetryResult> impedances;

            impedances = seq.ProcessMeasurementCall(measurements[0]);
            Assert.IsInstanceOfType(impedances[0], typeof(ComplianceResult));
            Assert.AreEqual(5, impedances.Where(x => x.PhaseWidth_us == 400).Count());
            Assert.AreEqual(5, impedances.OfType<ComplianceResult>().Where(x => x.InCompliance == true).Count());




            impedances = seq.ProcessMeasurementCall(measurements[26]);
            Assert.IsInstanceOfType(impedances[0], typeof(ImpedanceResult));
            Assert.AreEqual(5, impedances.Where(x => x.PhaseWidth_us == 145).Count());
            Assert.AreEqual(5, impedances.Where(x => x is ComplianceResult).Count());

            //Assert.IsTrue(impedances[0] is ImpedanceResult);
            //if (impedances[0] is ImpedanceResult)
            //{
            //    ImpedanceResult i0 = (ImpedanceResult)impedances[1];
            //    Assert.AreEqual(i0.ElectrodeName, "A2");
            //    Assert.AreEqual(i0._Category, ImpedanceCatagorization.Error);
            //    Assert.AreEqual(4071, Math.Round(i0._Impedance_ohms));
            //    Assert.AreEqual(2, i0.Electrode);
            //    Assert.AreEqual(i0.Implant, PIC_Sequence.Implant.ImplantA);
            //    Assert.AreEqual(25, i0.PhaseWidth_us);
            //    Assert.AreEqual(20, i0.Return);
            //    Assert.AreEqual(75, i0.Current_uA);
            //    Assert.AreEqual(i0.Type, PulseType.MonoPolar);
            //}

            //var x = seq.ProcessSequenceResponse(rec);
        }

        [TestMethod]
        public void CompiledSequence_Mary_2015_08_14_1pm_MissingCharInSegnumber()
        {
            var seq = GetTestSequence_RealDataMary();

            NimbleMeasurementRecord rec = new NimbleMeasurementRecord();
            rec.RecordDirectory =
                @"15_518_MARY-84EB1877AF2A-7e4254d7-5f13-4358-ab35-627c5c47baf7-2015-08-14_01-00-28-PM";

            List<NimbleSegmentMeasurment> measurements = rec.GetMeasurments();

            foreach (NimbleSegmentMeasurment meas in measurements)
            {
                List<TelemetryResult> impedances = seq.ProcessMeasurementCall(meas);

                // Assert.IsTrue(impedances[0] is ImpedanceResult);
            }
            //if (impedances[0] is ImpedanceResult)
            //{
            //    ImpedanceResult i0 = (ImpedanceResult)impedances[1];
            //    Assert.AreEqual(i0.ElectrodeName, "A2");
            //    Assert.AreEqual(i0._Category, ImpedanceCatagorization.Error);
            //    Assert.AreEqual(4071, Math.Round(i0._Impedance_ohms));
            //    Assert.AreEqual(2, i0.Electrode);
            //    Assert.AreEqual(i0.Implant, PIC_Sequence.Implant.ImplantA);
            //    Assert.AreEqual(25, i0.PhaseWidth_us);
            //    Assert.AreEqual(20, i0.Return);
            //    Assert.AreEqual(75, i0.Current_uA);
            //    Assert.AreEqual(i0.Type, PulseType.MonoPolar);
            //}

            //var x = seq.ProcessSequenceResponse(rec);
        }

    }
}
