using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nimble.Sequences;

namespace TelemParserTests
{
    [TestClass]
    public class RecordParserTest
    {
        [TestMethod]
        public void FolderParser_SingleRecordA()
        {
            string[] input = new string[] { "14_514_Kingston-84EB1877B26F-608e3568-59c1-408d-a53b-34203648716e-2015-06-29_04-21-17-PM" };
            var output = SequenceFileManager_Accessor.ParseTelemDataDirecortyNames(input, "");

            Assert.AreEqual(output.Count, 1);
            Assert.AreEqual(output[0].SubjectName, "14_514_Kingston");
            Assert.AreEqual(output[0].BluetoothAddress, "84EB1877B26F");
            Assert.AreEqual(output[0].GenGuid, Guid.Parse("608e3568-59c1-408d-a53b-34203648716e"));
            Assert.AreEqual(output[0].Timestamp, new DateTime(2015, 06, 29, 16, 21, 17));
        }

        [TestMethod]
        public void FolderParser_SingleRecordB()
        {
            string[] input = new string[] { "14_514_PCB1-F4B85EB48907-3b5bb552-704f-42f3-b627-08258d7b8c4e-2015-06-29_04-42-05-PM" };
            var output = SequenceFileManager_Accessor.ParseTelemDataDirecortyNames(input, "");

            Assert.AreEqual(output.Count, 1);
            Assert.AreEqual(output[0].SubjectName, "14_514_PCB1");
            Assert.AreEqual(output[0].BluetoothAddress, "F4B85EB48907");
            Assert.AreEqual(output[0].GenGuid, Guid.Parse("3b5bb552-704f-42f3-b627-08258d7b8c4e"));
            Assert.AreEqual(output[0].Timestamp, new DateTime(2015, 06, 29, 16, 42, 05));
        }

        [TestMethod]
        public void FolderParser_Null()
        {
            string[] input = new string[] { "14_514_Kingston-84EB1877B26F-608e3568-59c1-408d-a53b-34203648716e-2015-06-29_04-21-17-PM" };
            var output = SequenceFileManager_Accessor.ParseTelemDataDirecortyNames(null, "");
            Assert.IsNotNull(output);
            Assert.AreEqual(output.Count, 0);
        }

        [TestMethod]
        public void NinbleMeasurementRecord_CreateBasic()
        {
            var res = GetBasicMeasurements();

            Assert.AreEqual(15, res.Count);
        }

        private static List<NimbleSegmentResponse> GetBasicMeasurements()
        {
            NimbleMeasurementRecord rec = new NimbleMeasurementRecord();
            rec.RecordDirectory =
                @"14_514_PCB1-F4B85EB48907-608e3568-59c1-408d-a53b-34203648716e-2015-07-17_12-17-32-PM";
            var res = rec.GetMeasurments();
            return res;
        }

        [TestMethod]
        public void CompiledSequence_CreateBasic()
        {
            var seq = GetTestSequence_ImpedanceOnly();

            const int numsegments = 27;
            const int numpulses = 198;
            Assert.AreEqual(numpulses, seq.PulseData.Length);
            Assert.AreEqual(numsegments, seq.Sequence.Length);
            Assert.AreEqual(20, seq.ClockRate);
            Assert.IsNotNull(seq);
        }

        private static CompiledSequence GetTestSequence_ImpedanceOnly()
        {
            FilesForGenerationGUID files = new FilesForGenerationGUID();
            files.Sequence_h = @"14_514_Kingston_608e3568-59c1-408d-a53b-34203648716e\Sequence.h";
            files.Sequence_c = @"14_514_Kingston_608e3568-59c1-408d-a53b-34203648716e\Sequence.c";
            files.PulseData_h = @"14_514_Kingston_608e3568-59c1-408d-a53b-34203648716e\PulseData.h";
            files.PulseData_c = @"14_514_Kingston_608e3568-59c1-408d-a53b-34203648716e\PulseData.c";

            CompiledSequence seq = new CompiledSequence(files);
            return seq;
        }

        private static CompiledSequence GetTestSequence_ImpedanceAndCompliance()
        {
            FilesForGenerationGUID files = new FilesForGenerationGUID();
            files.Sequence_h = @"14_514_PCB1_bacd73be-60b4-49e3-ba31-e0903738cc93\Sequence.h";
            files.Sequence_c = @"14_514_PCB1_bacd73be-60b4-49e3-ba31-e0903738cc93\Sequence.c";
            files.PulseData_h = @"14_514_PCB1_bacd73be-60b4-49e3-ba31-e0903738cc93\PulseData.h";
            files.PulseData_c = @"14_514_PCB1_bacd73be-60b4-49e3-ba31-e0903738cc93\PulseData.c";

            CompiledSequence seq = new CompiledSequence(files);
            return seq;
        }

        [TestMethod]
        public void CompiledSequence_CanParseOld2DArraySequence()
        {
            FilesForGenerationGUID files = new FilesForGenerationGUID();
            files.Sequence_h = @"14_514_PCB1_bacd73be-60b4-49e3-ba31-e0903738cc93\Sequence.h";
            files.Sequence_c = @"14_514_PCB1_bacd73be-60b4-49e3-ba31-e0903738cc93\Sequence.c";
            files.PulseData_h = @"14_514_PCB1_bacd73be-60b4-49e3-ba31-e0903738cc93\PulseData.h";
            files.PulseData_c = @"14_514_PCB1_bacd73be-60b4-49e3-ba31-e0903738cc93\PulseData.c";

            CompiledSequence seq = new CompiledSequence(files);

            Assert.AreEqual(31, seq.Sequence.Length);
            Assert.AreEqual(4, seq.Sequence[0].Length);
            Assert.AreEqual(16, seq.Sequence[1].Length);
            Assert.AreEqual(38, seq.Sequence[25].Length);
        }

        [TestMethod]
        public void CompiledSequence_CanParseNewJaggedArraySequence()
        {
            FilesForGenerationGUID files = new FilesForGenerationGUID();
            files.Sequence_h = @"TEST_Dainty_dbb6c4e9-fb72-4d8d-a695-a5c058778693\Sequence.h";
            files.Sequence_c = @"TEST_Dainty_dbb6c4e9-fb72-4d8d-a695-a5c058778693\Sequence.c";
            files.PulseData_h = @"TEST_Dainty_dbb6c4e9-fb72-4d8d-a695-a5c058778693\PulseData.h";
            files.PulseData_c = @"TEST_Dainty_dbb6c4e9-fb72-4d8d-a695-a5c058778693\PulseData.c";

            CompiledSequence seq = new CompiledSequence(files);

            Assert.AreEqual(219, seq.Sequence.Length);
            Assert.AreEqual(9, seq.Sequence[0].Length);
            Assert.AreEqual(9, seq.Sequence[1].Length);
            Assert.AreEqual(38, seq.Sequence[213].Length);
        }


        [TestMethod]
        public void CompiledSequence_BasicImpedance()
        {
            var seq = GetTestSequence_ImpedanceAndCompliance();

            NimbleMeasurementRecord rec = new NimbleMeasurementRecord();
            rec.RecordDirectory =
                @"14_514_PCB1-F4B85EB48907-bacd73be-60b4-49e3-ba31-e0903738cc93-2015-08-03_01-04-50-PM";

            List<NimbleSegmentResponse> measurements = rec.GetMeasurments();

            List<TelemetryResult> impedances = seq.ProcessMeasurementCall(measurements[12]);

            Assert.IsTrue(impedances[0] is ImpedanceResult);
            if (impedances[0] is ImpedanceResult)
            {
                ImpedanceResult i0 = (ImpedanceResult)impedances[1];
                Assert.AreEqual(i0.ElectrodeName, "A2");
                Assert.AreEqual(i0._Category, ImpedanceCatagorization.Error);
                Assert.AreEqual(4071, Math.Round(i0.Impedance_ohms));
                Assert.AreEqual(2, i0.Electrode);
                Assert.AreEqual(i0.Implant, PIC_Sequence.Implant.ImplantA);
                Assert.AreEqual(25, i0.PhaseWidth_us);
                Assert.AreEqual(20, i0.Return);
                Assert.AreEqual(75, i0.Current_uA);
                Assert.AreEqual(i0.Type, PulseType.MonoPolar);
            }

            //var x = seq.ProcessSequenceResponse(rec);
        }

      
        [TestMethod]
        public void CompiledSequence_ImpedanceAndCompliance()
        {
            var seq = GetTestSequence_ImpedanceAndCompliance();

            NimbleMeasurementRecord rec = new NimbleMeasurementRecord();
            rec.RecordDirectory =
                @"14_514_PCB1-F4B85EB48907-bacd73be-60b4-49e3-ba31-e0903738cc93-2015-08-03_01-04-50-PM";

            List<NimbleSegmentResponse> measurements = rec.GetMeasurments();

            List<TelemetryResult> impedances = seq.ProcessMeasurementCall(measurements[0]);

            Assert.IsTrue(impedances[0] is ComplianceResult);
            Assert.AreEqual(15, impedances.Count);

            Assert.IsFalse(((ComplianceResult)impedances[0]).InCompliance);
            Assert.IsTrue(((ComplianceResult)impedances[1]).InCompliance);
            Assert.IsTrue(((ComplianceResult)impedances[2]).InCompliance);
            Assert.IsTrue(((ComplianceResult)impedances[3]).InCompliance);
            Assert.IsTrue(((ComplianceResult)impedances[4]).InCompliance);
            Assert.IsFalse(((ComplianceResult)impedances[5]).InCompliance);
            Assert.IsFalse(((ComplianceResult)impedances[6]).InCompliance);
            Assert.IsFalse(((ComplianceResult)impedances[7]).InCompliance);
            Assert.IsFalse(((ComplianceResult)impedances[8]).InCompliance);
            Assert.IsFalse(((ComplianceResult)impedances[9]).InCompliance);
            Assert.IsFalse(((ComplianceResult)impedances[10]).InCompliance);
            Assert.IsFalse(((ComplianceResult)impedances[11]).InCompliance);
            Assert.IsFalse(((ComplianceResult)impedances[12]).InCompliance);
            Assert.IsFalse(((ComplianceResult)impedances[13]).InCompliance);
            Assert.IsFalse(((ComplianceResult)impedances[14]).InCompliance);

            ComplianceResult i0 = (ComplianceResult)impedances[0];
            Assert.AreEqual("A1", impedances[0].ElectrodeName);
            Assert.AreEqual(50, impedances[0].PhaseWidth_us);
            Assert.AreEqual(90, impedances[0].Current_uA);

            //var x = seq.ProcessSequenceResponse(rec);
        }
    }
}
