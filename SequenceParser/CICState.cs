using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace Nimble.Sequences
{
    public class CICState
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public SenseElectrodes SenseElecs { get; private set; }
        public VTEL_AmplifierGain vtel_gain { get; private set; }
        public VTEL_VoltageSelect vtel_vs { get; private set; }
        public VTEL_SampleTime_TokenPos vtel_ts_tokenpos { get; private set; }
        public VTEL_SampleTime_TokenClock vtel_ts_tokenclock { get; private set; }
        public bool VoltageTelemetryEnabled { get; private set; }
        public bool ComplianceTelemetryEnabled { get; private set; }

        public const double VREF = 1.105;

        public bool SetUpToReadMaxPWM
        {
            get
            {
                bool x = SenseElecs == SenseElectrodes.StimElecs &&
                    vtel_gain == VTEL_AmplifierGain.Gain2 &&
                    vtel_vs == VTEL_VoltageSelect.vdd &&
                    vtel_ts_tokenclock == VTEL_SampleTime_TokenClock.OnTokenClock &&
                    VoltageTelemetryEnabled && !ComplianceTelemetryEnabled;
                return x;
            }
        }

        public bool SetUpToReadMinPWM
        {
            get
            {
                bool x = SenseElecs == SenseElectrodes.StimElecs &&
                    vtel_gain == VTEL_AmplifierGain.Gain1on5 &&
                    vtel_vs == VTEL_VoltageSelect.vss &&
                    vtel_ts_tokenclock == VTEL_SampleTime_TokenClock.OnTokenClock &&
                    VoltageTelemetryEnabled && !ComplianceTelemetryEnabled;
                return x;
            }
        }

        public bool SetUpForVoltageTelemetry_EndPhase1
        {
            get
            {
                bool x = SenseElecs == SenseElectrodes.StimElecs &&
                    vtel_vs == VTEL_VoltageSelect.senseelectrodes &&
                    vtel_ts_tokenclock == VTEL_SampleTime_TokenClock.OnTokenClock &&
                    vtel_ts_tokenpos == VTEL_SampleTime_TokenPos.t6 &&
                    VoltageTelemetryEnabled && !ComplianceTelemetryEnabled;
                return x;
            }
        }

        public bool SetUpForVoltageTelemetry_StartPhase1
        {
            get
            {
                bool x = SenseElecs == SenseElectrodes.StimElecs &&
                    vtel_vs == VTEL_VoltageSelect.senseelectrodes &&
                    vtel_ts_tokenclock == VTEL_SampleTime_TokenClock.TwoCellsAfterTokenClock &&
                    vtel_ts_tokenpos == VTEL_SampleTime_TokenPos.t1 &&
                    VoltageTelemetryEnabled && !ComplianceTelemetryEnabled;
                return x;
            }
        }

        public bool SetUpForComplianceTelemetry
        {
            get
            {

                bool x = !VoltageTelemetryEnabled && ComplianceTelemetryEnabled;
                return x;
            }
        }

        public bool SetUpToReturnAnyTelemetry
        {
            get
            {
                return SetUpForComplianceTelemetry || SetUpForVoltageTelemetry_EndPhase1 || SetUpForVoltageTelemetry_StartPhase1 || SetUpToReadMaxPWM || SetUpToReadMinPWM;
            }
        }

        public CICState()
        {
            ResetRegs();
        }

        public void ResetRegs()
        {
            vtel_ts_tokenpos = VTEL_SampleTime_TokenPos.t10;
            vtel_ts_tokenclock = VTEL_SampleTime_TokenClock.OnTokenClock;
            vtel_gain = VTEL_AmplifierGain.Gain1;
            vtel_vs = VTEL_VoltageSelect.vref;
            VoltageTelemetryEnabled = false;
            SenseElecs = SenseElectrodes.StimElecs;
        }

        public void ApplyPulse(int E, int M, double A_dbl)
        {
            int A = (int)A_dbl;
            A = A < 0 ? -A : A;
            E = E < 0 ? -E : E;
            M = M < 0 ? -M : M;
            logger.Debug("Applying pulse E:{0} M:{1} A:{2}", E, M, A);
            if (E != 0)
            {
                logger.Debug("Not a control pulse, ingoring");
                return;
            }

            if (E == 0 && M == 0)
            {
                switch (A)
                {
                    case 205:
                        SenseElecs = SenseElectrodes.StimElecs;
                        break;
                    case 87:
                        VoltageTelemetryEnabled = true;
                        break;
                    case 30:
                        VoltageTelemetryEnabled = false;
                        ComplianceTelemetryEnabled = false;

                        break;
                    case 92:
                        ComplianceTelemetryEnabled = true;
                        break;
                    case 13:
                        ResetRegs();
                        break;
                    default:
                        logger.Error("Trying to apply an unknown M: E:{0} M:{1} A{2}", E, M, A);
                        throw new NotImplementedException();
                }
            }
            else if (E == 0)
            {
                switch (M)
                {
                    case 13:
                        ResetRegs();
                        break;
                    case 5:
                        vtel_gain = (VTEL_AmplifierGain)A;
                        break;
                    case 6:
                        int vs = A & 0x07;
                        vtel_vs = (VTEL_VoltageSelect)vs;

                        bool clock = (A & 0x8) > 0;
                        vtel_ts_tokenclock = clock ? VTEL_SampleTime_TokenClock.TwoCellsAfterTokenClock : VTEL_SampleTime_TokenClock.OnTokenClock;

                        int pos = (A >> 4);
                        vtel_ts_tokenpos = (VTEL_SampleTime_TokenPos)pos;
                        break;
                    case 31:
                        break;
                    default:
                        logger.Error("Trying to apply an unknown M: E:{0} M:{1} A{2}", E, M, A);
                        throw new NotImplementedException();
                }
            }
        }

        public enum SenseElectrodes
        {
            StimElecs,
            SE1andSE2_reg
        }

        public enum VTEL_AmplifierGain
        {
            Gain2 = 0,
            Gain1 = 1,
            Gain2on5 = 2,
            Gain1on5 = 3,
        }

        public enum VTEL_VoltageSelect
        {
            vss = 0,
            vdda = 1,
            vddd = 2,
            vref = 3,
            io = 4,
            vdd = 5,
            senseelectrodes = 6,
            notused = 7,
        }

        public enum VTEL_SampleTime_TokenPos
        {
            /// <summary>
            /// Onset of Sync1
            /// </summary>
            t0 = 1,

            /// <summary>
            /// Onset of DT1
            /// </summary>
            t1 = 2,

            /// <summary>
            /// Onset of DT2
            /// </summary>
            t2 = 3,
            t3 = 4,
            t4 = 5,
            t5 = 6,
            /// <summary>
            /// End f PX1
            /// </summary>
            t6 = 7,
            t7 = 8,
            t8 = 9,
            t9 = 10,
            t10 = 11, //default
            t11 = 12,
            t12 = 13,
            t13 = 0,
        }

        public enum VTEL_SampleTime_TokenClock
        {
            OnTokenClock = 0, //default
            TwoCellsAfterTokenClock = 1,
        }

        public enum CIRegisters
        {
            ChipID,
        }

        public static double vtel_gain_To_VFullScale(CICState.VTEL_AmplifierGain Gain)
        {
            double gain;
            switch (Gain)
            {
                case CICState.VTEL_AmplifierGain.Gain2:
                    gain = 1;
                    break;
                case CICState.VTEL_AmplifierGain.Gain1:
                    gain = 2;
                    break;
                case CICState.VTEL_AmplifierGain.Gain2on5:
                    gain = 5;
                    break;
                case CICState.VTEL_AmplifierGain.Gain1on5:
                    gain = 10;
                    break;
                default:
                    gain = -1;
                    break;
            }
            return gain;
        }
    }
}
