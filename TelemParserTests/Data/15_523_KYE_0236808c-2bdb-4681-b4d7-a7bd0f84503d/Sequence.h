/*********************************************
Contains computer generated code, do not edit
Generated on: 3/02/2016 9:12:36 AM
*********************************************/
//{GenerationGUID: 0236808c-2bdb-4681-b4d7-a7bd0f84503d}
#define GENERATION_GUID "0236808c-2bdb-4681-b4d7-a7bd0f84503d"
#define TOTAL_SEGMENTS 35
#define LAST_SEGMENT 34
#define NORMAL_SEGMENTS 19
#define STARTLOOP_SEGMENT 18
#define WARMUP_SEGMENT 20
#define WARMDOWN_SEGMENT 34
#define FIRSTIMPEDANCE_SEGMENT 22
#define LASTIMPEDANCE_SEGMENT 33
#define COMPLIANCEOFF_SEGMENT -1
#define COMPLIANCEWARMDOWN_SEGMENT -1
#define FIRSTCOMPLIANCEON_SEGMENT -1
#define LASTCOMPLIANCEON_SEGMENT -1
#define FIRSTSPECIAL_SEGMENT 21
#define LASTSPECIAL_SEGMENT 21
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
//{SegmentComments: DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, READMINMAX, SPECIAL_SENTRY, IMPEDANCE_MP25_V10_AONLY, IMPEDANCE_MP25_V10_BONLY, IMPEDANCE_MP145_V10_AONLY, IMPEDANCE_MP145_V10_BONLY, IMPEDANCE_MP280_V10_AONLY, IMPEDANCE_MP280_V10_BONLY, IMPEDANCE_MP400_V10_AONLY, IMPEDANCE_MP400_V10_BONLY, IMPEDANCE_CG25_V10_AONLY, IMPEDANCE_CG25_V10_BONLY, IMPEDANCE_COMPLIANCE_AONLY, IMPEDANCE_COMPLIANCE_BONLY, WARMDOWN}
extern const int Sequence[35][39];
extern int SegmentLengths [35];
extern int SegmentRepeats [35];
