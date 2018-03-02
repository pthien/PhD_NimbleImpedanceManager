using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nimble.Sequences;

namespace TelemParserTests
{
    [TestClass]
    [DeploymentItem("Data")]
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

            List<NimbleSegmentResponse> measurements = rec.GetMeasurments();
            NimbleSegmentResponse m =
                measurements.Where(x => x.RepeatCount == 5 && x.SegmentName == "IMPEDANCE_MP25_V2").First();

            List<TelemetryResult> processed = seq.ProcessMeasurementCall(m);

            Assert.AreEqual(11, processed.Count());
            Assert.AreEqual(11, processed.OfType<ImpedanceResult>().Count());

            List<ImpedanceResult> imps = processed.Cast<ImpedanceResult>().ToList();
           
            Assert.AreEqual(11, imps.Count);
            Assert.AreEqual(5102.04081632653, imps[0].Impedance_ohms, 50);
            Assert.AreEqual(5170.06802721088, imps[1].Impedance_ohms, 50);
            Assert.AreEqual(4761.90476190476, imps[2].Impedance_ohms, 50);
            Assert.AreEqual(4370.74829931972, imps[3].Impedance_ohms, 50);
            Assert.AreEqual(4897.95918367347, imps[4].Impedance_ohms, 50);
            Assert.AreEqual(13367.3469387755, imps[5].Impedance_ohms, 50);
            Assert.AreEqual(13333.3333333333, imps[6].Impedance_ohms, 50);
            Assert.AreEqual(4268.70748299319, imps[7].Impedance_ohms, 50);
            Assert.AreEqual(4132.65306122449, imps[8].Impedance_ohms, 50);
            Assert.AreEqual(3384.35374149659, imps[9].Impedance_ohms, 50);
            Assert.AreEqual(4795.91836734693, imps[10].Impedance_ohms,50);

            Assert.AreEqual(5102.04081632653, imps[0].Impedance_ohms, 10);
            Assert.AreEqual(5170.06802721088, imps[1].Impedance_ohms, 10);
            Assert.AreEqual(4761.90476190476, imps[2].Impedance_ohms, 10);
            Assert.AreEqual(4370.74829931972, imps[3].Impedance_ohms, 10);
            Assert.AreEqual(4897.95918367347, imps[4].Impedance_ohms, 10);
            Assert.AreEqual(13367.3469387755, imps[5].Impedance_ohms, 10);
            Assert.AreEqual(13333.3333333333, imps[6].Impedance_ohms, 10);
            Assert.AreEqual(4268.70748299319, imps[7].Impedance_ohms, 10);
            Assert.AreEqual(4132.65306122449, imps[8].Impedance_ohms, 10);
            Assert.AreEqual(3384.35374149659, imps[9].Impedance_ohms, 10);
            Assert.AreEqual(4795.91836734693, imps[10].Impedance_ohms,10);

            Assert.AreEqual(5102.04081632653, imps[0].Impedance_ohms, 1);
            Assert.AreEqual(5170.06802721088, imps[1].Impedance_ohms, 1);
            Assert.AreEqual(4761.90476190476, imps[2].Impedance_ohms, 1);
            Assert.AreEqual(4370.74829931972, imps[3].Impedance_ohms, 1);
            Assert.AreEqual(4897.95918367347, imps[4].Impedance_ohms, 1);
            Assert.AreEqual(13367.3469387755, imps[5].Impedance_ohms, 1);
            Assert.AreEqual(13333.3333333333, imps[6].Impedance_ohms, 1);
            Assert.AreEqual(4268.70748299319, imps[7].Impedance_ohms, 1);
            Assert.AreEqual(4132.65306122449, imps[8].Impedance_ohms, 1);
            Assert.AreEqual(3384.35374149659, imps[9].Impedance_ohms, 1);
            Assert.AreEqual(4795.91836734693, imps[10].Impedance_ohms,1);

            Assert.AreEqual(5102.04081632653, imps[0].Impedance_ohms, .000001);
            Assert.AreEqual(5170.06802721088, imps[1].Impedance_ohms, .000001);
            Assert.AreEqual(4761.90476190476, imps[2].Impedance_ohms, .000001);
            Assert.AreEqual(4370.74829931972, imps[3].Impedance_ohms, .000001);
            Assert.AreEqual(4897.95918367347, imps[4].Impedance_ohms, .000001);
            Assert.AreEqual(13367.3469387755, imps[5].Impedance_ohms, .000001);
            Assert.AreEqual(13333.3333333333, imps[6].Impedance_ohms, .000001);
            Assert.AreEqual(4268.70748299319, imps[7].Impedance_ohms, .000001);
            Assert.AreEqual(4132.65306122449, imps[8].Impedance_ohms, .000001);
            Assert.AreEqual(3384.35374149659, imps[9].Impedance_ohms, .000001);
            Assert.AreEqual(4795.91836734693, imps[10].Impedance_ohms,.000001);
        }

        [TestMethod]
        public void CompiledSequence_Mary_2015_08_14_2pm()
        {
            var seq = GetTestSequence_RealDataMary();

            NimbleMeasurementRecord rec = new NimbleMeasurementRecord();
            rec.RecordDirectory =
                @"15_518_MARY-84EB1877AF2A-7e4254d7-5f13-4358-ab35-627c5c47baf7-2015-08-14_02-00-30-PM";

            List<NimbleSegmentResponse> measurements = rec.GetMeasurments();

            Assert.AreEqual(54, measurements.Count);

            foreach (NimbleSegmentResponse meas in measurements)
            {
                List<TelemetryResult> imps = seq.ProcessMeasurementCall(meas);



                // Assert.IsTrue(impedances[0] is ImpedanceResult);
            }
            List<TelemetryResult> impedances;

            impedances = seq.ProcessMeasurementCall(measurements[0]);
            Assert.IsInstanceOfType(impedances[0], typeof(ComplianceResult));
            Assert.AreEqual(5, impedances.Where(x => x.PhaseWidth_us == 400).Count());
            Assert.AreEqual(5, impedances.OfType<ComplianceResult>().Where(x => x.InCompliance == true).Count());




            impedances = seq.ProcessMeasurementCall(measurements[21]);
            Assert.IsInstanceOfType(impedances[0], typeof(ImpedanceResult));
            Assert.AreEqual(11, impedances.Where(x => x.PhaseWidth_us == 145).Count());
            Assert.AreEqual(0, impedances.Where(x => x is ComplianceResult).Count());

            ImpedanceResult imp0 = (ImpedanceResult)impedances[0];

            Assert.AreEqual("A1", imp0.ElectrodeName);
            Assert.AreEqual(PulseType.MonoPolar, imp0.Type);
            Assert.AreEqual(5952, imp0.Impedance_ohms,1);
            Assert.AreEqual(5952, imp0.Impedance_ohms,10);
            Assert.AreEqual(5952, imp0.Impedance_ohms,1000);

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

            List<NimbleSegmentResponse> measurements = rec.GetMeasurments();

            foreach (NimbleSegmentResponse meas in measurements)
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
