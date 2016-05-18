using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nimble.Sequences;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PIC_Sequence;

namespace Nimble.Sequences.Tests
{
    [TestClass()]
    public class CompiledSequenceTests
    {
        #region declarations
        string test1 = @"/*********************************************
Contains computer generated code, do not edit
Generated on: 29/06/2015 4:53:55 PM
*********************************************/
//{GenerationGUID: ce5c2dc2-0049-419c-a83e-2489d4700bf3}
const int Sequence[27][38] = {
{1, 2, 1, 2},
{3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 15, 15, 15},
{16, 17, 16, 17},
{3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 15, 15, 15},
{18, 19, 18, 19},
{3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 15, 15, 15},
{20, 21, 20, 21},
{3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 15, 15, 15},
{22, 23, 22, 23},
{3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 15, 15, 15},
{24, 25, 24, 25},
{3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 15, 15, 15},
{26, 27, 26, 27},
{3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 15, 15, 15},
{28, 29, 28, 29},
{3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 15, 15, 15},
{30, 31, 30, 31},
{3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 15, 15, 15},
{32, 33, 32, 33},
{3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 15, 15, 15},
{34, 35, 36, 37, 38, 39, 34, 35, 36, 40, 38, 39, 34, 41, 42, 37, 38, 39, 34, 41, 42, 40, 38, 39, 43, 43},
{34, 41, 44, 45, 46, 47, 48, 49, 50, 51, 38, 39, 34, 41, 44, 52, 53, 54, 55, 56, 57, 58, 38, 39, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43},
{34, 41, 44, 45, 59, 60, 61, 62, 63, 64, 38, 39, 34, 41, 44, 52, 65, 66, 67, 68, 69, 70, 38, 39, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43},
{34, 41, 44, 45, 71, 72, 73, 74, 75, 76, 38, 39, 34, 41, 44, 52, 77, 78, 79, 80, 81, 82, 38, 39, 43, 43},
{34, 41, 44, 45, 83, 84, 85, 86, 87, 88, 38, 39, 34, 41, 44, 52, 89, 90, 91, 92, 93, 94, 38, 39, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43},
{34, 41, 44, 45, 95, 96, 97, 98, 99, 100, 38, 39, 34, 41, 44, 52, 101, 102, 103, 104, 105, 106, 38, 39, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43},
{107}
};


int SegmentLengths [] = {4,16,4,16,4,16,4,16,4,16,4,16,4,16,4,16,4,16,4,16,26,38,38,26,38,38,1};


int SegmentRepeats [] = {200,100,200,100,200,100,200,100,200,100,200,100,200,100,200,100,200,100,200,100,1,1,1,1,1,1,1536};
";

        int[][] output1 = new int[][] {
new int[] {1, 2, 1, 2},
new int[] {3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 15, 15, 15},
new int[] {16, 17, 16, 17},
new int[] {3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 15, 15, 15},
new int[] {18, 19, 18, 19},
new int[] {3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 15, 15, 15},
new int[] {20, 21, 20, 21},
new int[] {3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 15, 15, 15},
new int[] {22, 23, 22, 23},
new int[] {3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 15, 15, 15},
new int[] {24, 25, 24, 25},
new int[] {3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 15, 15, 15},
new int[] {26, 27, 26, 27},
new int[] {3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 15, 15, 15},
new int[] {28, 29, 28, 29},
new int[] {3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 15, 15, 15},
new int[] {30, 31, 30, 31},
new int[] {3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 15, 15, 15},
new int[] {32, 33, 32, 33},
new int[] {3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 15, 15, 15},
new int[] {34, 35, 36, 37, 38, 39, 34, 35, 36, 40, 38, 39, 34, 41, 42, 37, 38, 39, 34, 41, 42, 40, 38, 39, 43, 43},
new int[] {34, 41, 44, 45, 46, 47, 48, 49, 50, 51, 38, 39, 34, 41, 44, 52, 53, 54, 55, 56, 57, 58, 38, 39, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43},
new int[] {34, 41, 44, 45, 59, 60, 61, 62, 63, 64, 38, 39, 34, 41, 44, 52, 65, 66, 67, 68, 69, 70, 38, 39, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43},
new int[] {34, 41, 44, 45, 71, 72, 73, 74, 75, 76, 38, 39, 34, 41, 44, 52, 77, 78, 79, 80, 81, 82, 38, 39, 43, 43},
new int[] {34, 41, 44, 45, 83, 84, 85, 86, 87, 88, 38, 39, 34, 41, 44, 52, 89, 90, 91, 92, 93, 94, 38, 39, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43},
new int[] {34, 41, 44, 45, 95, 96, 97, 98, 99, 100, 38, 39, 34, 41, 44, 52, 101, 102, 103, 104, 105, 106, 38, 39, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43},
new int[] {107}
};

        string test2 = @"/*********************************************
Contains computer generated code, do not edit
Generated on: 29/06/2015 4:53:55 PM
*********************************************/
//{GenerationGUID: ce5c2dc2-0049-419c-a83e-2489d4700bf3}
const int Sequence[2][38] = {
{1, 2, 1, 2},
{107}
};

";


        private string basicPulseData = @"/*********************************************
Contains computer generated code, do not edit
Generated on: 29/06/2015 4:53:55 PM
Contains encoded pulse data intended for a PIC
operating at 20 MHz
*********************************************/

#include ""Clock.h""

#if (FCY != CLOCK_20MHZ)
 error_wrong_clock_freq
#endif

//{GenerationGUID: ce5c2dc2-0049-419c-a83e-2489d4700bf3}
const int PulseData[108][12] = {
{ 0xAB6F, 0x6DF4, 0xAB6F, 0x6DF4, 0xAFEB, 0xAEF4, 0xAFEB, 0xAEF4, 592, 16, 625000, 0 },// { 0:	23,	23,	0,	23,	23,	0,	30,	16  }

};
";

        #endregion

        //[TestMethod()]
        //public void ParseSequenceTest_Basic()
        //{
        //    var output = CompiledSequence_Accessor.ParseSequence(test2,0);
        //    int[,] t = new int[,] { { 12, 3 }, { 23, 4 } };
        //    int[][] expected = new[] { new[] { 1, 2, 1, 2 }, new[] { 107 } };

        //    for (int i = 0; i < output.Length; i++)
        //    {
        //        CollectionAssert.AreEqual(expected[i], output[i]);

        //    }
        //}

        [TestMethod()]
        public void ParseSequenceTest()
        {
            int[] MaxCurrentForEachSeg;
            var o1 = CompiledSequence_Accessor.ParseSequence(test1,0, out MaxCurrentForEachSeg);
            for (int i = 0; i < o1.Length; i++)
            {
                CollectionAssert.AreEqual(output1[i], output1[i]);
            }
            //Assert.Fail();
        }
           
        [TestMethod()]
        public void CompiledSequenceTest()
        {
            var m = CompiledSequence.sequenceExtractor.Match(test1,0);
            Assert.IsTrue(m.Success);
        }


        [TestMethod()]
        public void CompiledSequenceTest2()
        {
            string alltext = File.ReadAllText("14_514_PCB1_e0479d64-2134-4a30-aff2-fbd33ac78358\\Sequence.c");
            var m = CompiledSequence.sequenceExtractor.Match(alltext);
            Assert.IsTrue(m.Success);
        }

        [TestMethod()]
        public void ParseSequenceTest_BasicPulseData()
        {
            var output = CompiledSequence_Accessor.ParsePulseData(basicPulseData);
            Pulse p = new Pulse() { LE = 23, LM = 23, LA = 0, RE = 23, RM = 23, RA = 0, PW_us = 30, IPG_us = 16 };
            List<Pulse> expected = new List<Pulse>();
            expected.Add(p);
            CollectionAssert.AreEqual(expected, output);
        }

        [TestMethod()]
        public void ParseSequenceTest_BasicClockRate()
        {
            var output = CompiledSequence.ParsePulseDataClockRate(basicPulseData);
            Assert.AreEqual(20, output);
        }

      
        

        [TestMethod()]
        public void ExtractGuidTest()
        {

        }

        [TestMethod()]
        public void ParsePulseDataClockRateTest()
        {

        }

        [TestMethod()]
        public void ProcessMeasurementCallTest()
        {

        }
    }
}
