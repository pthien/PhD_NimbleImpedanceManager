/*********************************************
Contains computer generated code, do not edit
Generated on: 31/07/2015 3:27:34 PM
*********************************************/
//{GenerationGUID: bacd73be-60b4-49e3-ba31-e0903738cc93}
#define GENERATION_GUID "bacd73be-60b4-49e3-ba31-e0903738cc93"
#define TOTAL_SEGMENTS 31
#define LAST_SEGMENT 30
#define NORMAL_SEGMENTS 19
#define STARTLOOP_SEGMENT 18
#define WARMUP_SEGMENT 20
#define WARMDOWN_SEGMENT 26
#define FIRSTIMPEDANCE_SEGMENT 21
#define LASTIMPEDANCE_SEGMENT 25
#define COMPLIANCEOFF_SEGMENT 29
#define COMPLIANCEWARMDOWN_SEGMENT 30
#define FIRSTCOMPLIANCEON_SEGMENT 27
#define LASTCOMPLIANCEON_SEGMENT 28
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
//{SegmentComments: DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, READMINMAX, IMPEDANCE_MP145, IMPEDANCE_MP290, IMPEDANCE_MP580, IMPEDANCE_MP25, IMPEDANCE_CG145, WARMDOWN, COMPLIANCEON_A, COMPLIANCEON_B, COMPLIANCE_OFF, COMPLIANCE_WARMDOWN}
extern const int Sequence[31][38];
extern int SegmentLengths [31];
extern int SegmentRepeats [31];
