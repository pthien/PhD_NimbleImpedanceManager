using PIC_Sequence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Nimble.Sequences.CICState;

namespace Nimble.Sequences
{
    public class AnnotatedTelemetryResponse
    {
        public TelemetryResponse BaseResponse { get; }

        public Implant ImplantGivingResponse { get; }

        public readonly List<int> Captures_ticks;

        public Pulse Pulse { get; }

        public AnnotatedTelemetryResponse(TelemetryResponse baseResponse, Implant whichImplant, Pulse p)
        {
            BaseResponse = baseResponse;
            ImplantGivingResponse = whichImplant;
            Captures_ticks = BaseResponse.Captures_ticks;
            Pulse = p;
        }

        public enum TelemetryResponseType
        {
            ElectrodeVoltageMeasurement,
            ComplianceMeasurement,
            ReferenceVoltageMeasurement,
            RegisterRead,
        }
    }

    public class ReferenceVoltageResponse : AnnotatedTelemetryResponse
    {
        public VoltageReferences VoltageReference { get; }
        public ReferenceVoltageResponse(TelemetryResponse baseResponse, Implant whichImplant, Pulse p, VoltageReferences reference) : base(baseResponse, whichImplant, p)
        {
            VoltageReference = reference;
        }

        public enum VoltageReferences
        {
            MaxPWMValue,
            MinPWMValue
        }
    }

    public class ElectrodeVoltageResponse : AnnotatedTelemetryResponse
    {
        public VTEL_AmplifierGain vtel_gain { get; private set; }
        public VTEL_VoltageSelect vtel_vs { get; private set; }
        public VTEL_SampleTime_TokenPos vtel_ts_tokenpos { get; private set; }
        public VTEL_SampleTime_TokenClock vtel_ts_tokenclock { get; private set; }

        public ElectrodeVoltageResponse(TelemetryResponse baseResponse, Implant whichImplant, Pulse p, CICState state) : base(baseResponse, whichImplant, p)
        {
            vtel_gain = state.vtel_gain;
            vtel_vs = state.vtel_vs;
            vtel_ts_tokenpos = state.vtel_ts_tokenpos;
            vtel_ts_tokenclock = state.vtel_ts_tokenclock;
        }
    }

    public class ComplianceVoltageResponse : AnnotatedTelemetryResponse
    {
        public ComplianceVoltageResponse(TelemetryResponse baseResponse, Implant whichImplant, Pulse p) : base(baseResponse, whichImplant, p)
        {

        }
    }

    public class RegisterReadResponse : AnnotatedTelemetryResponse
    {
        public CIRegisters Register { get; }

        public RegisterReadResponse(TelemetryResponse baseResponse, Implant whichImplant, Pulse p, CIRegisters register) : base(baseResponse, whichImplant, p)
        {
            Register = register;
        }


    }


}
