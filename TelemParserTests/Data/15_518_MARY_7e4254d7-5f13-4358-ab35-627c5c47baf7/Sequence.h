/*********************************************
Contains computer generated code, do not edit
Generated on: 14/08/2015 10:13:33 AM
*********************************************/
//{GenerationGUID: 7e4254d7-5f13-4358-ab35-627c5c47baf7}
#define GENERATION_GUID "7e4254d7-5f13-4358-ab35-627c5c47baf7"
#define TOTAL_SEGMENTS 33
#define LAST_SEGMENT 32
#define NORMAL_SEGMENTS 19
#define STARTLOOP_SEGMENT 18
#define WARMUP_SEGMENT 20
#define WARMDOWN_SEGMENT 28
#define FIRSTIMPEDANCE_SEGMENT 21
#define LASTIMPEDANCE_SEGMENT 27
#define COMPLIANCEOFF_SEGMENT 31
#define COMPLIANCEWARMDOWN_SEGMENT 32
#define FIRSTCOMPLIANCEON_SEGMENT 29
#define LASTCOMPLIANCEON_SEGMENT 30
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
//{SegmentComments: DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, READMINMAX, IMPEDANCE_MP400_V10, IMPEDANCE_MP580_V10, IMPEDANCE_MP290_V10, IMPEDANCE_MP25_V10, IMPEDANCE_CG25_V10, IMPEDANCE_MP145_V10, IMPEDANCE_MP25_V2, WARMDOWN, COMPLIANCEON_A, COMPLIANCEON_B, COMPLIANCE_OFF, COMPLIANCE_WARMDOWN}
extern const int Sequence[33][38];
extern int SegmentLengths [33];
extern int SegmentRepeats [33];
