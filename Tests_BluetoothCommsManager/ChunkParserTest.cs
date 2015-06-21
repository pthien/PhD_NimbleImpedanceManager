using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NimbleBluetoothImpedanceManager;

namespace Tests_BluetoothCommsManager
{
    
    
    /// <summary>
    ///This is a test class for ChunkParserTest and is intended
    ///to contain all ChunkParserTest Unit Tests
    ///</summary>
    [TestClass]
    public class ChunkParserTest
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
        ///A test for ParseChunk
        ///</summary>
        [TestMethod]
        public void ParseChunk_OK()
        {
            string chunk = "OK"; // TODO: Initialize to an appropriate value
            string[] expected = {"OK"};
            List<string> actual;
            actual = ChunkParser.ParseChunk(chunk);
            CollectionAssert.AreEqual(expected, actual.ToArray());
        }


        [TestMethod]
        public void ParseChunk_connattempt()
        {
            string chunk = "OK+CONNA"; // TODO: Initialize to an appropriate value
            string[] expected = { "OK+CONNA" };
            List<string> actual;
            actual = ChunkParser.ParseChunk(chunk);
            CollectionAssert.AreEqual(expected, actual.ToArray());
        }

        [TestMethod]
        public void ParseChunk_connattemptThenSuccess()
        {
            string chunk = "OK+CONNAOK+CONN"; // TODO: Initialize to an appropriate value
            string[] expected = { "OK+CONNA", "OK+CONN" };
            List<string> actual;
            actual = ChunkParser.ParseChunk(chunk);
            CollectionAssert.AreEqual(expected, actual.ToArray());
        }

        [TestMethod]
        public void ParseChunk_randomData()
        {
            string chunk = "skjnsd sld fjdf oisf"; // TODO: Initialize to an appropriate value
            string[] expected = { "skjnsd sld fjdf oisf" };
            List<string> actual;
            actual = ChunkParser.ParseChunk(chunk);
            CollectionAssert.AreEqual(expected, actual.ToArray());
        }

        [TestMethod]
        public void ParseChunk_randomDataThenOKPlus()
        {
            string chunk = "skjnsd sld fjdf oisf OK+CONN"; // TODO: Initialize to an appropriate value
            string[] expected = { "skjnsd sld fjdf oisf ", "OK+CONN" };
            List<string> actual;
            actual = ChunkParser.ParseChunk(chunk);
            CollectionAssert.AreEqual(expected, actual.ToArray());
        }

        [TestMethod]
        public void ParseChunk_randomDataNewlineThenOKPlus()
        {
            string chunk = "skjnsd sld fjdf oisf\nOK+CONN"; // TODO: Initialize to an appropriate value
            string[] expected = { "skjnsd sld fjdf oisf\n", "OK+CONN" };
            List<string> actual;
            actual = ChunkParser.ParseChunk(chunk);
            CollectionAssert.AreEqual(expected, actual.ToArray());
        }

        [TestMethod]
        public void ParseChunk_randomDataNewDatalineThenOKPlus()
        {
            string chunk = "skj.nsd sld fjdf oisf\nsdfsdOK+CONN"; // TODO: Initialize to an appropriate value
            string[] expected = { "skj.nsd sld fjdf oisf\nsdfsd", "OK+CONN" };
            List<string> actual;
            actual = ChunkParser.ParseChunk(chunk);
            CollectionAssert.AreEqual(expected, actual.ToArray());
        }

        [TestMethod]
        public void ParseChunk_OKOKPlus()
        {
            string chunk = "OKOK+CONN"; // TODO: Initialize to an appropriate value
            string[] expected = { "OK", "OK+CONN" };
            List<string> actual;
            actual = ChunkParser.ParseChunk(chunk);
            CollectionAssert.AreEqual(expected, actual.ToArray());
        }

        [TestMethod]
        public void ParseChunk_OKDataOKPlus()
        {
            string chunk = "OKsdfsdfOK+CONN"; // TODO: Initialize to an appropriate value
            string[] expected = { "OK", "sdfsdf", "OK+CONN" };
            List<string> actual;
            actual = ChunkParser.ParseChunk(chunk);
            CollectionAssert.AreEqual(expected, actual.ToArray());
        }

        [TestMethod]
        public void ParseChunk_NoneCaptured()
        {
            string chunk = "Start: CPI(2)|NPI(1) C{none captured}End"; // TODO: Initialize to an appropriate value
            string[] expected = { "Start: CPI(2)|NPI(1) C{none captured}End" };
            List<string> actual;
            actual = ChunkParser.ParseChunk(chunk);
            CollectionAssert.AreEqual(expected, actual.ToArray());
        }

        [TestMethod]
        public void ParseChunk_ErronousATPlusBeforeOKPlus()
        {
            string chunk = "AT+SLEEPOK+LOST"; // TODO: Initialize to an appropriate value
            string[] expected = { "AT+SLEEP","OK+LOST" };
            List<string> actual;
            actual = ChunkParser.ParseChunk(chunk);
            CollectionAssert.AreEqual(expected, actual.ToArray());
        }

        [TestMethod]
        public void ParseChunk_OKDisc()
        {
            string chunk = "OK+DISC:20C38FEAD43C"; // TODO: Initialize to an appropriate value
            string[] expected = { "OK+DISC:20C38FEAD43C" };
            List<string> actual;
            actual = ChunkParser.ParseChunk(chunk);
            CollectionAssert.AreEqual(expected, actual.ToArray());
        }
        
    }
}
