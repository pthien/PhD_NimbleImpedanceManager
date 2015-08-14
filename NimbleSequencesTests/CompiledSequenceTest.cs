using Nimble.Sequences;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NimbleSequencesTests
{


    /// <summary>
    ///This is a test class for CompiledSequenceTest and is intended
    ///to contain all CompiledSequenceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CompiledSequenceTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for ExtractHashDefines
        ///</summary>
        [TestMethod()]
        [DeploymentItem("SequenceParser.dll")]
        public void ExtractHashDefinesTest()
        {
            string alltext_seqh = @"/*********************************************
Contains computer generated code, do not edit
Generated on: 29/06/2015 4:17:42 PM
*********************************************/
//{GenerationGUID: 608e3568-59c1-408d-a53b-34203648716e}
#define GENERATION_GUID ""608e3568-59c1-408d-a53b-34203648716e""
#define TOTAL_SEGMENTS 27
#define LAST_SEGMENT 26
#define NORMAL_SEGMENTS 19
#define STARTLOOP_SEGMENT 18
#define WARMUP_SEGMENT 20
#define WARMDOWN_SEGMENT 26
#define FIRSTIMPEDANCE_SEGMENT 21
#define LASTIMPEDANCE_SEGMENT 25
/* definitions: 
Warmup_segment: should run before an impedance measurement is made.
	measures min/max pwm times
Warmdown_segment: fills in time to make a full second of off duty cycle
FIRSTIMPEDANCE_SEGMENT: first segment index that measures impedances
LASTIMPEDANCE_SEGMENT: last segment index that measures impedances
	Note: all segments between last and first impedance segments should 
	take the same ammout of time to have an accurate duty cycle
STARTLOOP_SEGMENT: the segment at the start of the continuous loop.
	The continuous loop runs from STARTLOOP_SEGMENT to NORMAL_SEGMENTS
TOTAL_SEGMENTS: the total number of segments. not 0 based.
*/
//{SegmentComments: DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, READMINMAX, IMPEDANCE_MP145, IMPEDANCE_MP290, IMPEDANCE_MP580, IMPEDANCE_MP25, IMPEDANCE_CG145, WARMDOWN}
extern const int Segment[27][38];
extern int SegmentLengths [27];
extern int SegmentRepeats [27];
"; // TODO: Initialize to an appropriate value
            Dictionary<string, int> expected = new Dictionary<string, int>(); // TODO: Initialize to an appropriate value
            expected.Add("TOTAL_SEGMENTS", 27);
            expected.Add("LAST_SEGMENT", 26);
            expected.Add("NORMAL_SEGMENTS", 19);
            expected.Add("STARTLOOP_SEGMENT", 18);
            expected.Add("WARMUP_SEGMENT", 20);
            expected.Add("WARMDOWN_SEGMENT", 26);
            expected.Add("FIRSTIMPEDANCE_SEGMENT", 21);
            expected.Add("LASTIMPEDANCE_SEGMENT", 25);
            Dictionary<string, int> actual;
            actual = CompiledSequence_Accessor.ExtractHashDefines(alltext_seqh);
            Assert.IsTrue(expected.All(e => actual.Contains(e)));
            Assert.IsTrue(actual.All(e => expected.Contains(e)));
            Assert.AreEqual(expected.Count, actual.Count);
        }


        /// <summary>
        ///A test for ParseSequence
        ///</summary>
        [TestMethod()]
        [DeploymentItem("SequenceParser.dll")]
        public void ParseSequenceTest()
        {
            string alltext = string.Empty; // TODO: Initialize to an appropriate value
            int[][] expected = null; // TODO: Initialize to an appropriate value
            int[][] actual;
            actual = CompiledSequence_Accessor.ParseSequence(alltext);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
