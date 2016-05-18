using Nimble.Sequences;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NimbleSequencesTests
{


    /// <summary>
    ///This is a test class for CICStateTest and is intended
    ///to contain all CICStateTest Unit Tests
    ///</summary>
    [DeploymentItem("NimbleSequences.dll")]
    [TestClass()]
    public class CICStateTest
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
        ///A test for ApplyPulse
        ///</summary>
        [TestMethod()]
        public void ApplyPulseTest_SetSenseElecs()
        {
            CICState target = new CICState(); // TODO: Initialize to an appropriate value
            int E = 0; // TODO: Initialize to an appropriate value
            int M = 0; // TODO: Initialize to an appropriate value
            int A = 205; // TODO: Initialize to an appropriate value
            target.ApplyPulse(E, M, A);
            Assert.AreEqual(CICState.SenseElectrodes.StimElecs, target.SenseElecs);
        }

        /// <summary>
        ///A test for ApplyPulse
        ///</summary>
        [TestMethod()]
        public void ApplyPulseTest_SetGain2()
        {
            CICState target = new CICState(); // TODO: Initialize to an appropriate value
            int E = 0; // TODO: Initialize to an appropriate value
            int M = 5; // TODO: Initialize to an appropriate value
            int A = 0; // TODO: Initialize to an appropriate value
            target.ApplyPulse(E, M, A);
            Assert.AreEqual(CICState.SenseElectrodes.StimElecs, target.SenseElecs);
            Assert.AreEqual(CICState.VTEL_AmplifierGain.Gain2, target.vtel_gain);


            //Assert.AreEqual(CICState.SenseElectrodes.StimElecs, target.vtel_ts_tokenclock);
            //Assert.AreEqual(CICState.SenseElectrodes.StimElecs, target.vtel_ts_tokenpos);
            //Assert.AreEqual(CICState.SenseElectrodes.StimElecs, target.vtel_vs);
        }

        /// <summary>
        ///A test for ApplyPulse
        ///</summary>
        [TestMethod()]
        public void ApplyPulseTest_SetGain1on5()
        {
            CICState target = new CICState(); // TODO: Initialize to an appropriate value
            int E = 0; // TODO: Initialize to an appropriate value
            int M = 5; // TODO: Initialize to an appropriate value
            int A = 3; // TODO: Initialize to an appropriate value

            target.ApplyPulse(E, M, A);
            Assert.AreEqual(CICState.SenseElectrodes.StimElecs, target.SenseElecs);
            Assert.AreEqual(CICState.VTEL_AmplifierGain.Gain1on5, target.vtel_gain);


            //Assert.AreEqual(CICState.SenseElectrodes.StimElecs, target.vtel_ts_tokenclock);
            //Assert.AreEqual(CICState.SenseElectrodes.StimElecs, target.vtel_ts_tokenpos);
            //Assert.AreEqual(CICState.SenseElectrodes.StimElecs, target.vtel_vs);
        }

        /// <summary>
        ///A test for ApplyPulse
        ///</summary>
        [TestMethod()]
        public void ApplyPulseTest_SetVtel_vsts()
        {
            CICState target = new CICState(); // TODO: Initialize to an appropriate value

            target.ApplyPulse(0, 6, 117);
            Assert.AreEqual(CICState.SenseElectrodes.StimElecs, target.SenseElecs);
            Assert.AreEqual(CICState.VTEL_AmplifierGain.Gain1, target.vtel_gain);
            Assert.AreEqual(CICState.VTEL_SampleTime_TokenClock.OnTokenClock, target.vtel_ts_tokenclock);
            Assert.AreEqual(CICState.VTEL_SampleTime_TokenPos.t6, target.vtel_ts_tokenpos);
            Assert.AreEqual(CICState.VTEL_VoltageSelect.vdd, target.vtel_vs);
        }


        /// <summary>
        /// Is the implant set up to return the minimum possible value. i.e. shortest possible time between the two spicks.
        /// </summary>
        [TestMethod()]
        public void ApplyPulseTest_SetupForMinPWM()
        {
            CICState target = new CICState(); // TODO: Initialize to an appropriate value
            target.ApplyPulse(0, 0, 205);
            target.ApplyPulse(0, 5, 3);
            target.ApplyPulse(0, 6, 112);
            target.ApplyPulse(0, 0, 87);
            Assert.AreEqual(true, target.SetUpToReadMinPWM);
            Assert.IsFalse(target.SetUpToReadMaxPWM);
            Assert.IsFalse(target.SetUpForVoltageTelemetry_EndPhase1);
        }

        /// <summary>
        /// Is the implant set up to return the maximum possible value. i.e. longest possible time between the two spicks.
        /// </summary>
        [TestMethod()]
        public void ApplyPulseTest_SetupForMaxPWM()
        {
            CICState target = new CICState(); // TODO: Initialize to an appropriate value
            target.ApplyPulse(0, 0, 205);
            target.ApplyPulse(0, 5, 0);
            target.ApplyPulse(0, 6, 117);
            target.ApplyPulse(0, 0, 87);
            Assert.IsTrue(target.SetUpToReadMaxPWM);
            Assert.IsFalse(target.SetUpForVoltageTelemetry_EndPhase1);
            Assert.IsFalse(target.SetUpToReadMinPWM);
        }


        [TestMethod()]
        public void ApplyPulseTest_SetupForImpedanceTelem()
        {
            CICState target = new CICState(); // TODO: Initialize to an appropriate value
            target.ApplyPulse(0, 0, 205);
            target.ApplyPulse(0, 5, 3);
            target.ApplyPulse(0, 6, 118);
            target.ApplyPulse(0, 0, 87);
            Assert.AreEqual(true, target.SetUpForVoltageTelemetry_EndPhase1);
            Assert.IsFalse(target.SetUpToReadMaxPWM);
            Assert.IsFalse(target.SetUpToReadMinPWM);
        }

        [TestMethod()]
        public void ApplyPulseTest_SetupForAccessVoltageMeasurement()
        {
            CICState target = new CICState(); // TODO: Initialize to an appropriate value
            target.ApplyPulse(0, 0, 205);
            target.ApplyPulse(0, 5, 3);
            target.ApplyPulse(0, 6, 46);
            target.ApplyPulse(0, 0, 87);
            Assert.IsTrue(target.SetUpForVoltageTelemetry_StartPhase1);
            Assert.IsFalse(target.SetUpForVoltageTelemetry_EndPhase1);
            Assert.IsFalse(target.SetUpToReadMaxPWM);
            Assert.IsFalse(target.SetUpToReadMinPWM);
        }

        /// <summary>
        ///A test for vtel_gain_To_VFullScale
        ///</summary>
        [TestMethod()]
        public void vtel_gain_To_VFullScaleTest_gain1()
        {
            CICState.VTEL_AmplifierGain Gain = CICState.VTEL_AmplifierGain.Gain1; // TODO: Initialize to an appropriate value
            double expected = 2;
            double actual;
            actual = CICState.vtel_gain_To_VFullScale(Gain);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for vtel_gain_To_VFullScale
        ///</summary>
        [TestMethod()]
        public void vtel_gain_To_VFullScaleTest_gain2()
        {
            CICState.VTEL_AmplifierGain Gain = CICState.VTEL_AmplifierGain.Gain2; // TODO: Initialize to an appropriate value
            double expected = 1;
            double actual;
            actual = CICState.vtel_gain_To_VFullScale(Gain);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for vtel_gain_To_VFullScale
        ///</summary>
        [TestMethod()]
        public void vtel_gain_To_VFullScaleTest_gain2on5()
        {
            CICState.VTEL_AmplifierGain Gain = CICState.VTEL_AmplifierGain.Gain2on5; // TODO: Initialize to an appropriate value
            double expected = 5;
            double actual;
            actual = CICState.vtel_gain_To_VFullScale(Gain);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for vtel_gain_To_VFullScale
        ///</summary>
        [TestMethod()]
        public void vtel_gain_To_VFullScaleTest_gain1on5()
        {
            CICState.VTEL_AmplifierGain Gain = CICState.VTEL_AmplifierGain.Gain1on5; // TODO: Initialize to an appropriate value
            double expected = 10;
            double actual;
            actual = CICState.vtel_gain_To_VFullScale(Gain);
            Assert.AreEqual(expected, actual);
        }
    }
}
