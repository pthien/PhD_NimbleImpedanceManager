using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nimble.Sequences;

namespace NimbleSequencesTests
{
    [TestClass]
    public class WirelessRampSequenceTests
    {
        [TestMethod]
        [DeploymentItem("Data")]
        public void TestParseingOfSegment2LevelMap()
        {
            FilesForGenerationGUID files = new FilesForGenerationGUID();
            files.Sequence_h = @"TEST44CH_Kye_a4ea6e4c-9b2a-46ed-9db2-d8c59915738f\Sequence.h";
            files.Sequence_c = @"TEST44CH_Kye_a4ea6e4c-9b2a-46ed-9db2-d8c59915738f\Sequence.c";
            files.PulseData_h = @"TEST44CH_Kye_a4ea6e4c-9b2a-46ed-9db2-d8c59915738f\PulseData.h";
            files.PulseData_c = @"TEST44CH_Kye_a4ea6e4c-9b2a-46ed-9db2-d8c59915738f\PulseData.c";

            CompiledSequence c = new CompiledSequence(files);

            Assert.IsNotNull(c.Sequence);
            Assert.AreEqual(34, c.Sequence.Length);
            Assert.IsNotNull(c.PulseData);
            Assert.AreEqual(161, c.PulseData.Length);
            Assert.AreEqual(20, c.ClockRate);

            Assert.AreEqual(10, c.GetMaxStimLevel());

            Assert.AreEqual(3, c.ConvertSegNumber2StimLevel(5));
            int start, end;
            c.ConvertStimLevel2SegNumbers(6, out start, out end);
            Assert.AreEqual(10, start);
            Assert.AreEqual(11, end);
        }


    }
}
