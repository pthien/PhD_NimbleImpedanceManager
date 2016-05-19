using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nimble.Sequences;
using System.IO;
using System.Collections.Generic;
using NLog;
using System.Linq;

namespace TelemParserTests
{
    [DeploymentItem("Data")]
    [DeploymentItem("NLog.config")]
    [TestClass]
    public class KYE_RealDataTests
    {
        private CompiledSequence seq;
        private SequenceFileManager fman;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        [TestInitialize]
        public void ParseSequence()
        {
            LogManager.ThrowExceptions = true;

            FilesForGenerationGUID files = new FilesForGenerationGUID();
            files.Sequence_h = @"SEQ_15_523_KYE_0e81180e-a80d-4168-9b9b-b36615b6e07a\Sequence.h";
            files.Sequence_c = @"SEQ_15_523_KYE_0e81180e-a80d-4168-9b9b-b36615b6e07a\Sequence.c";
            files.PulseData_h = @"SEQ_15_523_KYE_0e81180e-a80d-4168-9b9b-b36615b6e07a\PulseData.h";
            files.PulseData_c = @"SEQ_15_523_KYE_0e81180e-a80d-4168-9b9b-b36615b6e07a\PulseData.c";

            seq = new CompiledSequence(files);


            fman = new SequenceFileManager();

            fman.ScanDirectory(Directory.GetCurrentDirectory());

            logger.Info("this is a test message from a unit test");
        }

        [TestMethod]
        public void CanParseSequence_Old2DArrayFormat()
        {
            Assert.AreEqual(20, seq.ClockRate);
            Assert.AreEqual(13, seq.MeasurementSegments.Count);
            Assert.AreEqual(35, seq.Sequence.Length);
            Assert.AreEqual(6, seq.Sequence[0].Length);
        }

        [TestMethod]
        public void CanDetectMeasurements()
        {
            var mayberecord = NimbleMeasurementRecord.OpenMeasurementRecord(
                "MEASURE_15_523_KYE-84EB1877B26F-0e81180e-a80d-4168-9b9b-b36615b6e07a-2016-02-15_03-37-00-PM");
            Assert.IsTrue(mayberecord.HasValue);

            NimbleMeasurementRecord record = mayberecord.Value;

            Assert.AreEqual("MEASURE_15_523_KYE", record.SubjectName);
            Assert.AreEqual("0e81180e-a80d-4168-9b9b-b36615b6e07a", record.GenGuid.ToString());

            List<NimbleSegmentResponse> measurements = record.GetMeasurments();

            Assert.AreEqual(78, measurements.Count);

            var specificm = measurements.Find(x => x.SegmentName.Contains("COMPLIANCE_AONLY") && x.RepeatCount == 4);

            Assert.AreEqual(13, specificm.TelemetryResponses.Count);

            List<TelemetryResult> result = seq.ProcessMeasurementCall(specificm);


            Assert.AreEqual(20, seq.ClockRate);
            fman.ScanDirectory("MEASURE_15_523_KYE-84EB1877B26F-0e81180e-a80d-4168-9b9b-b36615b6e07a-2016-02-15_03-37-00-PM");
            Assert.AreEqual(13, seq.MeasurementSegments.Count);
        }

        [TestMethod]
        public void ImpedanceCalculationsReference()
        {
            var mayberecord = NimbleMeasurementRecord.OpenMeasurementRecord(
                "MEASURE_15_523_KYE-84EB1877B26F-0e81180e-a80d-4168-9b9b-b36615b6e07a-2016-02-15_03-37-00-PM");

            NimbleMeasurementRecord record = mayberecord.Value;

            List<NimbleSegmentResponse> measurements = record.GetMeasurments();

            NimbleSegmentResponse m =
                measurements.Where(x => x.RepeatCount == 5 && x.SegmentName == "IMPEDANCE_MP400_V10_AONLY").First();

            List<TelemetryResult> processed = seq.ProcessMeasurementCall(m);

            Assert.AreEqual(19, processed.Count());
            Assert.AreEqual(19, processed.OfType<ImpedanceResult>().Count());

            List<ImpedanceResult> imps = processed.Cast<ImpedanceResult>().ToList();

            Assert.AreEqual(19, imps.Count);
            Assert.AreEqual(9426, imps[0]._Impedance_ohms, 1);
            Assert.AreEqual(13196, imps[1]._Impedance_ohms, 1);
            Assert.AreEqual(121850, imps[2]._Impedance_ohms, 1);
            Assert.AreEqual(10968, imps[3]._Impedance_ohms, 1);
            Assert.AreEqual(11825, imps[4]._Impedance_ohms, 1);
            Assert.AreEqual(12339, imps[5]._Impedance_ohms, 1);
            Assert.AreEqual(120137, imps[6]._Impedance_ohms, 1);
            Assert.AreEqual(14224, imps[7]._Impedance_ohms, 1);
            Assert.AreEqual(13538, imps[8]._Impedance_ohms, 1);
            Assert.AreEqual(11996, imps[9]._Impedance_ohms, 1);
            Assert.AreEqual(120137, imps[10]._Impedance_ohms, 1);
        }

    }
}
