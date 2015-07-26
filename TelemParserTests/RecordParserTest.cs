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
            string[] input = new string[]
            {"14_514_Kingston-84EB1877B26F-608e3568-59c1-408d-a53b-34203648716e-2015-06-29_04-21-17-PM"};
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
            string[] input = new string[]
            {"14_514_PCB1-F4B85EB48907-3b5bb552-704f-42f3-b627-08258d7b8c4e-2015-06-29_04-42-05-PM"};
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
            string[] input = new string[]
            {"14_514_Kingston-84EB1877B26F-608e3568-59c1-408d-a53b-34203648716e-2015-06-29_04-21-17-PM"};
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

        private static List<NimbleSegmentMeasurment> GetBasicMeasurements()
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
            var seq = GetTestSequence();

            const int numsegments = 27;
            const int numpulses = 198;
            Assert.AreEqual(numpulses, seq.PulseData.Length);
            Assert.AreEqual(numsegments, seq.Sequence.Length);
            Assert.AreEqual(20, seq.ClockRate);
            Assert.IsNotNull(seq);
        }

        private static CompiledSequence GetTestSequence()
        {
            FilesForGenerationGUID files = new FilesForGenerationGUID();
            files.Sequence_h = @"14_514_Kingston_608e3568-59c1-408d-a53b-34203648716e\Sequence.h";
            files.Sequence_c = @"14_514_Kingston_608e3568-59c1-408d-a53b-34203648716e\Sequence.c";
            files.PulseData_h = @"14_514_Kingston_608e3568-59c1-408d-a53b-34203648716e\PulseData.h";
            files.PulseData_c = @"14_514_Kingston_608e3568-59c1-408d-a53b-34203648716e\PulseData.c";

            CompiledSequence seq =new CompiledSequence(files);
            return seq;
        }

        [TestMethod]
        public void CompiledSequence_BasicImpedance()
        {
            var seq = GetTestSequence();

            NimbleMeasurementRecord rec = new NimbleMeasurementRecord();
            rec.RecordDirectory =
                @"14_514_PCB1-F4B85EB48907-608e3568-59c1-408d-a53b-34203648716e-2015-07-17_12-17-32-PM";

            //var x = seq.ProcessSequenceResponse(rec);
        }
    }
}
