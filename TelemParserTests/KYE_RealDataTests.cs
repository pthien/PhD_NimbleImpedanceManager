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
            Assert.AreEqual(9426, imps[0].Impedance_ohms, 1);
            Assert.AreEqual(13196, imps[1].Impedance_ohms, 1);
            Assert.AreEqual(121850, imps[2].Impedance_ohms, 1);
            Assert.AreEqual(10968, imps[3].Impedance_ohms, 1);
            Assert.AreEqual(11825, imps[4].Impedance_ohms, 1);
            Assert.AreEqual(12339, imps[5].Impedance_ohms, 1);
            Assert.AreEqual(120137, imps[6].Impedance_ohms, 1);
            Assert.AreEqual(14224, imps[7].Impedance_ohms, 1);
            Assert.AreEqual(13538, imps[8].Impedance_ohms, 1);
            Assert.AreEqual(11996, imps[9].Impedance_ohms, 1);
            Assert.AreEqual(120137, imps[10].Impedance_ohms, 1);
        }

        private void GetSequence2()
        {
            FilesForGenerationGUID files = new FilesForGenerationGUID();
            files.Sequence_h = @"15_523_KYE_0236808c-2bdb-4681-b4d7-a7bd0f84503d\Sequence.h";
            files.Sequence_c = @"15_523_KYE_0236808c-2bdb-4681-b4d7-a7bd0f84503d\Sequence.c";
            files.PulseData_h = @"15_523_KYE_0236808c-2bdb-4681-b4d7-a7bd0f84503d\PulseData.h";
            files.PulseData_c = @"15_523_KYE_0236808c-2bdb-4681-b4d7-a7bd0f84503d\PulseData.c";

            seq = new CompiledSequence(files);

            fman = new SequenceFileManager();

            fman.ScanDirectory(Directory.GetCurrentDirectory());
        }


        [TestMethod]
        public void ImpedanceReference_MissingMaxMins_OnePassProcessing_vs_TwoPass()
        {
            GetSequence2();

            var mayberecord = NimbleMeasurementRecord.OpenMeasurementRecord(
                "15_523_KYE-84EB1877B26F-0236808c-2bdb-4681-b4d7-a7bd0f84503d-2016-02-03_09-17-36-AM");

            NimbleMeasurementRecord record = mayberecord.Value;

            List<NimbleSegmentResponse> measurements = record.GetMeasurments();

            var m_all =
                measurements.Where(x => x.SegmentName == "IMPEDANCE_MP400_V10_BONLY");

            List<TelemetryResult> processed;
            //old processing method
            processed = seq.ProcessMeasurementCall(m_all.ElementAt(0));
            Assert.AreEqual(18, processed.Count());
          

            processed = seq.ProcessMeasurementCall(m_all.ElementAt(1));
            Assert.AreEqual(21, processed.Count());

            List<ImpedanceResult> imps = processed.Where(x=>x is ImpedanceResult).Cast<ImpedanceResult>().ToList();
            Assert.AreEqual(9380.234505862647, imps[0].Impedance_ohms,  .0001);
            Assert.AreEqual(13902.847571189277, imps[1].Impedance_ohms, .0001);
            Assert.AreEqual(10552.763819095477, imps[2].Impedance_ohms, .0001);
            Assert.AreEqual(9547.7386934673359, imps[3].Impedance_ohms, .0001);
            Assert.AreEqual(10385.259631490788, imps[4].Impedance_ohms, .0001);
            Assert.AreEqual(12395.309882747068, imps[5].Impedance_ohms, .0001);
            Assert.AreEqual(10887.772194304858, imps[6].Impedance_ohms, .0001);
            Assert.AreEqual(8207.7051926298154, imps[7].Impedance_ohms, .0001);
            Assert.AreEqual(13400.335008375212, imps[8].Impedance_ohms,  .0001);
            Assert.AreEqual(10552.763819095477, imps[9].Impedance_ohms,   .0001);
            Assert.AreEqual(123618.0904522613, imps[10].Impedance_ohms, .0001);


            processed = seq.ProcessMeasurementCall(m_all.ElementAt(2));
            Assert.AreEqual(19, processed.Count());

            processed = seq.ProcessMeasurementCall(m_all.ElementAt(3));
            Assert.AreEqual(20, processed.Count());

            processed = seq.ProcessMeasurementCall(m_all.ElementAt(4));
            Assert.AreEqual(20, processed.Count());
            
            processed = seq.ProcessMeasurementCall(m_all.ElementAt(5));
            Assert.AreEqual(0, processed.Count());

            #region new method, same info
            //new processing method with no extra information
            List<AnnotatedTelemetryResponse> temp;
            temp = seq.ProcessIndividualSegmentReponse(m_all.ElementAt(1));
            processed = TelemetryResult.ConvertAnnotatedResponses(temp, seq.ClockRate);
            Assert.AreEqual(21, processed.Count());

            imps = processed.Where(x => x is ImpedanceResult).Cast<ImpedanceResult>().ToList();
            Assert.AreEqual(9380.234505862647, imps[0].Impedance_ohms, .0001);
            Assert.AreEqual(13902.847571189277, imps[1].Impedance_ohms, .0001);
            Assert.AreEqual(10552.763819095477, imps[2].Impedance_ohms, .0001);
            Assert.AreEqual(9547.7386934673359, imps[3].Impedance_ohms, .0001);
            Assert.AreEqual(10385.259631490788, imps[4].Impedance_ohms, .0001);
            Assert.AreEqual(12395.309882747068, imps[5].Impedance_ohms, .0001);
            Assert.AreEqual(10887.772194304858, imps[6].Impedance_ohms, .0001);
            Assert.AreEqual(8207.7051926298154, imps[7].Impedance_ohms, .0001);
            Assert.AreEqual(13400.335008375212, imps[8].Impedance_ohms, .0001);
            Assert.AreEqual(10552.763819095477, imps[9].Impedance_ohms, .0001);
            Assert.AreEqual(123618.0904522613, imps[10].Impedance_ohms, .0001);
            #endregion


            //new processing method, with extra information
            List<AnnotatedTelemetryResponse> allMaxMinPWMresponses = new List<AnnotatedTelemetryResponse>();
            foreach (NimbleSegmentResponse m in measurements)
            {
                var annotatedresponses = seq.ProcessIndividualSegmentReponse(m).Where(x => x is ReferenceVoltageResponse);
                allMaxMinPWMresponses.AddRange(annotatedresponses);
            }

            temp = seq.ProcessIndividualSegmentReponse(m_all.ElementAt(0)); temp.AddRange(allMaxMinPWMresponses);
            processed = TelemetryResult.ConvertAnnotatedResponses(temp, seq.ClockRate);
            Assert.AreEqual(18, processed.Count());
            

            temp = seq.ProcessIndividualSegmentReponse(m_all.ElementAt(1)); temp.AddRange(allMaxMinPWMresponses);
            processed = TelemetryResult.ConvertAnnotatedResponses(temp, seq.ClockRate);
            
            Assert.AreEqual(21, processed.Count());

            imps = processed.Where(x => x is ImpedanceResult).Cast<ImpedanceResult>().ToList();
            Assert.AreEqual(9380.234505862647, imps[0].Impedance_ohms,  10);
            Assert.AreEqual(13902.847571189277, imps[1].Impedance_ohms, 10);
            Assert.AreEqual(10552.763819095477, imps[2].Impedance_ohms, 10);
            Assert.AreEqual(9547.7386934673359, imps[3].Impedance_ohms, 10);
            Assert.AreEqual(10385.259631490788, imps[4].Impedance_ohms, 10);
            Assert.AreEqual(12395.309882747068, imps[5].Impedance_ohms, 10);
            Assert.AreEqual(10887.772194304858, imps[6].Impedance_ohms, 10);
            Assert.AreEqual(8207.7051926298154, imps[7].Impedance_ohms, 10);
            Assert.AreEqual(13400.335008375212, imps[8].Impedance_ohms, 10);
            Assert.AreEqual(10552.763819095477, imps[9].Impedance_ohms, 10);
            Assert.AreEqual(123618.0904522613, imps[10].Impedance_ohms, 10);

            temp = seq.ProcessIndividualSegmentReponse(m_all.ElementAt(2)); temp.AddRange(allMaxMinPWMresponses);
            processed = TelemetryResult.ConvertAnnotatedResponses(temp, seq.ClockRate);
            Assert.AreEqual(19, processed.Count());

            temp = seq.ProcessIndividualSegmentReponse(m_all.ElementAt(3)); temp.AddRange(allMaxMinPWMresponses);
            processed = TelemetryResult.ConvertAnnotatedResponses(temp, seq.ClockRate);
            Assert.AreEqual(20, processed.Count());

            temp = seq.ProcessIndividualSegmentReponse(m_all.ElementAt(4)); temp.AddRange(allMaxMinPWMresponses);
            processed = TelemetryResult.ConvertAnnotatedResponses(temp, seq.ClockRate);
            Assert.AreEqual(20, processed.Count());

            temp = seq.ProcessIndividualSegmentReponse(m_all.ElementAt(5)); temp.AddRange(allMaxMinPWMresponses);
            processed = TelemetryResult.ConvertAnnotatedResponses(temp, seq.ClockRate);
            Assert.AreEqual(20, processed.Count());

        }

    }
}
