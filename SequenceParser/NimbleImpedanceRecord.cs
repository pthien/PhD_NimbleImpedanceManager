using System;
using System.Collections.Generic;
using NLog;
using PIC_Sequence;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Runtime.Serialization.Formatters.Binary;

namespace Nimble.Sequences
{
    [Serializable]
    public abstract class TelemetryResult
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public int Electrode { get; protected set; }
        public int Return { get; protected set; }
        public int PhaseWidth_us { get; protected set; }
        public Implant Implant { get; protected set; }
        public PulseType Type
        {
            get
            {
                return Electrode == Return ? PulseType.CommonGround : PulseType.MonoPolar;
            }
        }
        public double Current_uA { get; protected set; }

        public string ElectrodeName
        {
            get
            {
                switch (Implant)
                {
                    case Implant.ImplantA:
                        return "A" + Electrode;
                    case Implant.ImplantB:
                        return "B" + Electrode;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public override string ToString()
        {
            string typestr = "";
            switch (Type)
            {
                case PulseType.MonoPolar:
                    typestr = "MP";
                    break;
                case PulseType.CommonGround:
                    typestr = "CG";
                    break;
                default:
                    break;
            }
            //return string.Format("{4}{0} vs {1} ({2}) {3:G4} @ {5}us", Electrode, Return, typestr, _Impedance_ohms, Implant == Implant.ImplantA ? "A" : "B", PhaseWidth_us);
            return base.ToString();
        }

        public static List<TelemetryResult> ConvertAnnotatedResponses(List<AnnotatedTelemetryResponse> annotatedResponses, int clockRate_MHz)
        {
            List<TelemetryResult> telemResults = new List<TelemetryResult>();
            try
            {
                var references = annotatedResponses.Where(x => x is ReferenceVoltageResponse).Cast<ReferenceVoltageResponse>();
                var maxrefs = references
                    .Where(x => x.VoltageReference == ReferenceVoltageResponse.VoltageReferences.MaxPWMValue && x.Captures_ticks.Count == 1);
                var minrefs = references
                    .Where(x => x.VoltageReference == ReferenceVoltageResponse.VoltageReferences.MinPWMValue && x.Captures_ticks.Count == 1);

                var maxAList = maxrefs.Where(x => x.ImplantGivingResponse == Implant.ImplantA).Select(x => x.Captures_ticks[0]);
                var maxBList = maxrefs.Where(x => x.ImplantGivingResponse == Implant.ImplantB).Select(x => x.Captures_ticks[0]);

                var minAList = minrefs.Where(x => x.ImplantGivingResponse == Implant.ImplantA).Select(x => x.Captures_ticks[0]);
                var minBList = minrefs.Where(x => x.ImplantGivingResponse == Implant.ImplantB).Select(x => x.Captures_ticks[0]);

                //TODO: !!! Fix this. Previous code checked A and B channels. Needs to be able to adapt if A or B channels are not available
                //if (maxAList.Count() == 0 || maxBList.Count() == 0 || minAList.Count() == 0 || minBList.Count() == 0)
                if (maxAList.Count() == 0 || minAList.Count() == 0 )
                    return telemResults;

                int maxPWM_A = maxAList.Max();
                int maxPWM_B = 10;

                int minPWM_A = minAList.Max();
                int minPWM_B = 5;

                var impedanceResponses = annotatedResponses.Where(x => x is ElectrodeVoltageResponse).Cast<ElectrodeVoltageResponse>();
                var complianceResponses = annotatedResponses.Where(x => x is ComplianceVoltageResponse).Cast<ComplianceVoltageResponse>();



                foreach (ElectrodeVoltageResponse e in impedanceResponses)
                {
                    int maxPWM = maxPWM_B, minPWM = minPWM_B;
                    if (e.ImplantGivingResponse == Implant.ImplantA)
                    {
                        maxPWM = maxPWM_A;
                        minPWM = minPWM_A;
                    }
                    if (e.Captures_ticks.Count > 0)
                    {
                        var result = new ImpedanceResult(e.Pulse, maxPWM, minPWM, e.Captures_ticks[0], e.vtel_gain, e.ImplantGivingResponse);
                        telemResults.Add(result);
                    }
                }

                foreach (ComplianceVoltageResponse e in complianceResponses)
                {
                    if (e.Captures_ticks.Count > 0)
                    {
                        var result = new ComplianceResult(e.Pulse, e.ImplantGivingResponse, e.Captures_ticks[0], clockRate_MHz);
                        telemResults.Add(result);
                    }
                }

                return telemResults;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }

    [Serializable]
    public class ImpedanceResult : TelemetryResult
    {
        public ImpedanceCatagorization _Category;
        public double Impedance_ohms { get; private set; }
        public double Voltage_V { get; private set; }

        public ImpedanceResult(int E, int M, double current_uA, int maxPWM, int minPWM, int TelemResponse_ticks, CICState.VTEL_AmplifierGain Gain, Implant implant, int PhaseWidth_us)
        {
            Electrode = E;
            Return = M;
            Implant = implant;
            base.PhaseWidth_us = PhaseWidth_us;
            Current_uA = current_uA;
            double vfs = CICState.vtel_gain_To_VFullScale(Gain);

            Impedance_ohms = CalculateImpedance(current_uA, minPWM, maxPWM, TelemResponse_ticks, vfs);

            _Category = ImpedanceCatagorization.Error;
        }

        public ImpedanceResult(Pulse p, int maxPWM, int minPWM, int TelemResponse_ticks, CICState.VTEL_AmplifierGain Gain, Implant implant)
            : this(implant == Implant.ImplantA ? p.LE : p.RE,
                  implant == Implant.ImplantA ? p.LM : p.RM,
                  implant == Implant.ImplantA ? p.LA : p.RA,
                  maxPWM, minPWM, TelemResponse_ticks, Gain, implant, p.PW_us)
        {

        }

        private double CalculateImpedance(double current_uA, int minPWM_ticks, int maxPWM_ticks, int RawVal_ticks, double VoltageFullScale_V)
        {
            int RampTime_ticks = RawVal_ticks - minPWM_ticks;
            double RampTime_percentMax = (double)RampTime_ticks / (maxPWM_ticks - minPWM_ticks);

            Voltage_V = RampTime_percentMax * VoltageFullScale_V;
            double voltave_uV = Voltage_V * 1000000;
            double impedance = impedance = voltave_uV / current_uA;

            return impedance;
        }

        public override string ToString()
        {
            string typestr = "";
            switch (Type)
            {
                case PulseType.MonoPolar:
                    typestr = "MP";
                    break;
                case PulseType.CommonGround:
                    typestr = "CG";
                    break;
                default:
                    break;
            }
            return string.Format("{4}{0} vs {1} ({2}) {3:G4} @ {5}us", Electrode, Return, typestr, Impedance_ohms, Implant == Implant.ImplantA ? "A" : "B", PhaseWidth_us);
            //return base.ToString();
        }


    }

    [Serializable]
    public class ComplianceResult : TelemetryResult
    {
        public bool InCompliance
        {
            get
            {
                if (clockrate_MHz == 20)
                    return TelemResponse_ticks < 150;
                throw new NotImplementedException();
                //return false;
            }
        }

        private int TelemResponse_ticks;
        private int clockrate_MHz;

        public ComplianceResult(Pulse p, Implant implant, int TelemResponse_ticks, int clockrate_MHz = 20)
        {
            Implant = implant;
            PhaseWidth_us = p.PW_us;
            this.clockrate_MHz = clockrate_MHz;
            this.TelemResponse_ticks = TelemResponse_ticks;
            switch (implant)
            {
                case Implant.ImplantA:
                    Electrode = p.LE;
                    Return = p.LM;
                    Current_uA = p.LA;
                    break;
                case Implant.ImplantB:
                    Electrode = p.RE;
                    Return = p.RM;
                    Current_uA = p.RA;
                    break;
            }

        }

        public override string ToString()
        {
            return string.Format("{0} - {1}uA, {2}us {3} compliance", ElectrodeName, Current_uA, PhaseWidth_us,
                InCompliance ? "in" : "out of");
        }
    }

    [Serializable]
    public struct NimbleSegmentTelemetry
    {
        public string SegmentName;
        public int RepeateCount;
        public List<TelemetryResult> Impedances;

        public override string ToString()
        {
            return String.Format("{0} #{1}", SegmentName, RepeateCount);
        }
    }

    [Serializable]
    public struct NimbleImpedanceRecord
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private DateTime _timestamp;
        private string _bluetoothAddress;
        private string _subjectName;
        private Guid _genGuid;
        private string _recordDirectory;
        private List<NimbleSegmentTelemetry> _segmentImpedances;

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

        public NimbleSegmentTelemetry[] SegmentImpedances
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
            _segmentImpedances = new List<NimbleSegmentTelemetry>();
        }

        public void AddSegmentImpedanceResult(NimbleSegmentTelemetry impres)
        {
            _segmentImpedances.Add(impres);
        }

        public void AddSegmentImpedanceResult(List<NimbleSegmentTelemetry> impres)
        {
            foreach (NimbleSegmentTelemetry r in impres)
                _segmentImpedances.Add(r);
        }

        public void SaveSummary()
        {
            string path = Path.Combine(RecordDirectory, "summary.csv");
            logger.Info("Saved summary: {0}", path);
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
                logger.Info("Loaded preprocessed file: {0}", path);
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
            logger.Info("Saved processed impedances: {0}", path);

            SaveSummary();
        }

        private void WriteToStream(Stream s)
        {
            StreamWriter sw = new StreamWriter(s);

            sw.WriteLine("Subject, {0}", SubjectName);
            sw.WriteLine("Bluetooth Addr, {0}", BluetoothAddress);
            sw.WriteLine("Timestamp, {0}", Timestamp);
            sw.WriteLine("Sequence GUID, {0}", GenGuid);
            sw.WriteLine("Record Directory, {0}", RecordDirectory);

            foreach (NimbleSegmentTelemetry m in SegmentImpedances)
            {

                sw.Write(m + ",");

                for (int i = 1; i < 23; i++)
                {
                    int copy = i;
                    var one = m.Impedances.Where(x => x.Implant == Implant.ImplantA && x.Electrode == copy).OfType<ImpedanceResult>();
                    if (one.Any())
                    {
                        var first = one.First();
                        sw.Write(first.Impedance_ohms + ",");
                    }
                    else
                    {
                        sw.Write(" ,");
                    }

                }
                for (int i = 1; i < 23; i++)
                {
                    var one = m.Impedances.Where(x => x.Implant == Implant.ImplantB && x.Electrode == i).OfType<ImpedanceResult>();
                    if (one.Any())
                    {
                        var first = one.First();
                        sw.Write(first.Impedance_ohms + ",");
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


    public enum PulseType
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