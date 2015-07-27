using System;
using System.Collections.Generic;
using NLog;
using PIC_Sequence;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Nimble.Sequences
{
    [Serializable]
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

        //public ImpedanceResult()
        //{
        //    _Electrode = -1;
        //    _Return = -1;
        //    _PhaseWidht_us = -1;
        //    _Implant = Implant.ImplantA;
        //    _Impedance_ohms = -1;
        //    _Type = ImpedanceMeasurementTypes.MonoPolar;
        //    _Category = ImpedanceCatagorization.OK;
        //}

        public string ElectrodeName
        {
            get
            {
                switch (_Implant)
                {
                    case Implant.ImplantA:
                        return "A" + _Electrode;
                    case Implant.ImplantB:
                        return "B" + _Electrode;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

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

    [Serializable]
    public struct NimbleSegmentImpedance
    {
        public string SegmentName;
        public int RepeateCount;
        public List<ImpedanceResult> Impedances;

        public override string ToString()
        {
            return String.Format("{0} #{1}", SegmentName, RepeateCount);
        }
    }

    [Serializable]
    public struct NimbleImpedanceRecord
    {

        private DateTime _timestamp;
        private string _bluetoothAddress;
        private string _subjectName;
        private Guid _genGuid;
        private string _recordDirectory;
        private List<NimbleSegmentImpedance> _segmentImpedances;

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

        public NimbleSegmentImpedance[] SegmentImpedances
        {
            get { return _segmentImpedances.ToArray(); }
        }

        public NimbleImpedanceRecord(NimbleMeasurementRecord record)
        {
            _timestamp = record.Timestamp;
            _bluetoothAddress = record.BluetoothAddress;
            _genGuid = record.GenGuid;
            _subjectName = record.SubjectName;
            _recordDirectory = record.RecordDirectory;
            _segmentImpedances = new List<NimbleSegmentImpedance>();
        }

        public void AddSegmentImpedanceResult(NimbleSegmentImpedance impres)
        {
            _segmentImpedances.Add(impres);
        }

        public void AddSegmentImpedanceResult(List<NimbleSegmentImpedance> impres)
        {
            foreach (NimbleSegmentImpedance r in impres)
                _segmentImpedances.Add(r);
        }

        public void SaveSummary()
        {
            string path = Path.Combine(RecordDirectory, "summary.csv");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                WriteToStream(fs);
                fs.Flush();
            }
        }

        public NimbleImpedanceRecord? Load()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Path.Combine(RecordDirectory, "preprocessed.bin");
            if (File.Exists(path))
            {
                Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                var o = (NimbleImpedanceRecord)formatter.Deserialize(stream);
                stream.Close();
                return o;
            }
            return null;
        }

        public void Save()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Path.Combine(RecordDirectory, "preprocessed.bin");
            Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, this);
            stream.Close();

        }

        private void WriteToStream(Stream s)
        {
            StreamWriter sw = new StreamWriter(s);

            sw.WriteLine("Subject, {0}", SubjectName);
            sw.WriteLine("Bluetooth Addr, {0}", BluetoothAddress);
            sw.WriteLine("Timestamp, {0}", Timestamp);
            sw.WriteLine("Sequence GUID, {0}", GenGuid);
            sw.WriteLine("Record Directory, {0}", RecordDirectory);

            foreach (NimbleSegmentImpedance m in SegmentImpedances)
            {
                sw.Write(m + ",");

                for (int i = 1; i < 7; i++)
                {
                    var one = m.Impedances.Where(x => x._Implant == Implant.ImplantA && x._Electrode == i);
                    if (one.Any())
                    {
                        var first = one.First();
                        sw.Write(first._Impedance_ohms + ",");
                    }
                    else
                    {
                        sw.Write(" ,");
                    }

                }
                for (int i = 1; i < 7; i++)
                {
                    var one = m.Impedances.Where(x => x._Implant == Implant.ImplantB && x._Electrode == i);
                    if (one.Any())
                    {
                        var first = one.First();
                        sw.Write(first._Impedance_ohms + ",");
                    }
                    else
                    {
                        sw.Write(" ,");
                    }
                }
                sw.WriteLine();
            }
            s.Flush();
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