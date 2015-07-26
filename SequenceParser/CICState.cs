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

        public SenseElectrodes SenseElecs;
        public VTEL_AmplifierGain vtel_gain;
        public VTEL_VoltageSelect vtel_vs;
        public VTEL_SampleTime_TokenPos vtel_ts_tokenpos;
        public VTEL_SampleTime_TokenClock vtel_ts_tokenclock;
        public bool TelemetryEnabled = false;

        public const double VREF = 1.105;

        public bool SetUpToReadMaxPWM
        {
            get
            {
                bool x = SenseElecs == SenseElectrodes.StimElecs &&
                    vtel_gain == VTEL_AmplifierGain.Gain2 &&
                    vtel_vs == VTEL_VoltageSelect.vdd &&
                    vtel_ts_tokenclock == VTEL_SampleTime_TokenClock.OnTokenClock &&
                    TelemetryEnabled == true;
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
                    TelemetryEnabled == true;
                return x;
            }
        }

        public bool SetUpForImpedanceTelemetry
        {
            get
            {
                bool x = SenseElecs == SenseElectrodes.StimElecs &&
                    vtel_vs == VTEL_VoltageSelect.senseelectrodes &&
                    vtel_ts_tokenclock == VTEL_SampleTime_TokenClock.OnTokenClock &&
                    vtel_ts_tokenpos == VTEL_SampleTime_TokenPos.t6 &&
                    TelemetryEnabled == true;
                return x;
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
            TelemetryEnabled = false;
            SenseElecs = SenseElectrodes.StimElecs;
        }

        public void ApplyPulse(int E, int M, int A)
        {
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
                        TelemetryEnabled = true;
                        break;
                    case 30:
                        TelemetryEnabled = false;
                        break;
                    case 13:
                        ResetRegs();
                        break;
                    default:
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
                    default:
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
            t0 = 1,
            t1 = 2,
            t2 = 3,
            t3 = 4,
            t4 = 5,
            t5 = 6,
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

        public static double vtel_gain_To_VFullScale(CICState.VTEL_AmplifierGain Gain)
        {
            double gain;
            switch (Gain)
            {
                case CICState.VTEL_AmplifierGain.Gain2:
                    gain = CICState.VREF / 2;
                    break;
                case CICState.VTEL_AmplifierGain.Gain1:
                    gain = CICState.VREF / 1;
                    break;
                case CICState.VTEL_AmplifierGain.Gain2on5:
                    gain = CICState.VREF * 5 / 2;
                    break;
                case CICState.VTEL_AmplifierGain.Gain1on5:
                    gain = CICState.VREF * 5;
                    break;
                default:
                    gain = -1;
                    break;
            }
            return gain;
        }
    }
}
