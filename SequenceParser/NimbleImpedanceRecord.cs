using System;
using System.Collections.Generic;
using NLog;
using PIC_Sequence;

namespace Nimble.Sequences
{
    public struct ImpedanceResult
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public int _Electrode;
        public int _Return;
        public int _PhaseWidht_us;
        public Implant _Implant;
        public double _Impedance_ohms;
        public ImpedanceMeasurementTypes _Type;
        public ImpedanceCatagorization _Category;

        public ImpedanceResult(int E, int M, double current_uA, int maxPWM, int minPWM, int TelemResponse_ticks, CICState.VTEL_AmplifierGain Gain, Implant implant, int PhaseWidth_us)
        {

            _Electrode = E;
            _Return = M;
            _Implant = implant;
            _Type = E == M ? ImpedanceMeasurementTypes.CommonGround : ImpedanceMeasurementTypes.MonoPolar;
            _PhaseWidht_us = PhaseWidth_us;
            double vfs = CICState.vtel_gain_To_VFullScale(Gain);

            _Impedance_ohms = CalculateImpedance(current_uA, minPWM, maxPWM, TelemResponse_ticks, vfs);

            _Category = ImpedanceCatagorization.Error;
            
        }

        private static double CalculateImpedance(double current_uA, int minPWM_ticks, int maxPWM_ticks, int RawVal_ticks, double VoltageFullScale_V)
        {
            int RampTime_ticks = RawVal_ticks - minPWM_ticks;
            double RampTime_percentMax = (double)RampTime_ticks / (maxPWM_ticks - minPWM_ticks);

            double voltage_V = RampTime_percentMax * VoltageFullScale_V;
            double voltave_uV = voltage_V * 1000000;
            double impedance = impedance = voltave_uV / current_uA;

            return impedance;
        }

        public override string ToString()
        {
            string typestr = "";
            switch (_Type)
            {
                case ImpedanceMeasurementTypes.MonoPolar:
                    typestr = "MP";
                    break;
                case ImpedanceMeasurementTypes.CommonGround:
                    typestr = "CG";
                    break;
                default:
                    break;
            }
            return string.Format("{4}{0} vs {1} ({2}) {3:G4} @ {5}us", _Electrode, _Return, typestr, _Impedance_ohms, _Implant == Implant.ImplantA ? "A" : "B", _PhaseWidht_us);
            //return base.ToString();
        }
    }

    public struct NimbleImpedanceRecord
    {

        private DateTime _timestamp;
        private string _bluetoothAddress;
        private string _subjectName;
        private Guid _genGuid;
        private string _recordDirectory;
        private List<ImpedanceResult> _impedances;

        public DateTime Timestamp
        {
            get { return _timestamp; }
        }

        public string BluetoothAddress
        {
            get { return _bluetoothAddress; }
        }

        public string SubjectName
        {
            get { return _subjectName; }
        }

        public Guid GenGuid
        {
            get { return _genGuid; }
        }

        public string RecordDirectory
        {
            get { return _recordDirectory; }
        }

        public ImpedanceResult[] Impedances
        {
            get { return _impedances.ToArray(); }
        }

        public NimbleImpedanceRecord(NimbleMeasurementRecord record)
        {
            _timestamp = record.Timestamp;
            _bluetoothAddress = record.BluetoothAddress;
            _genGuid = record.GenGuid;
            _subjectName = record.SubjectName;
            _recordDirectory = record.RecordDirectory;
            _impedances = new List<ImpedanceResult>();
        }

        public void AddImpedanceResult(ImpedanceResult impres)
        {
            _impedances.Add(impres);
        }

        public void AddImpedanceResult(List<ImpedanceResult> impres)
        {
            foreach (ImpedanceResult r in impres)
                _impedances.Add(r);
        }
    }
    

    public enum ImpedanceMeasurementTypes
    {
        MonoPolar,
        CommonGround,
    }

    public enum ImpedanceCatagorization
    {
        OK,
        Open,
        HighZ,
        Short,
        Error
    }
}