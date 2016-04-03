/*********************************************
Contains computer generated code, do not edit
Generated on: 8/03/2016 2:31:15 PM
*********************************************/
//{GenerationGUID: a4ea6e4c-9b2a-46ed-9db2-d8c59915738f}
#define GENERATION_GUID "a4ea6e4c-9b2a-46ed-9db2-d8c59915738f"
#define TOTAL_SEGMENTS 34
#define LAST_SEGMENT 33
#define NORMAL_SEGMENTS 19
#define STARTLOOP_SEGMENT 18
#define WARMUP_SEGMENT 20
#define WARMDOWN_SEGMENT 29
#define FIRSTIMPEDANCE_SEGMENT 21
#define LASTIMPEDANCE_SEGMENT 28
#define COMPLIANCEOFF_SEGMENT 32
#define COMPLIANCEWARMDOWN_SEGMENT 33
#define FIRSTCOMPLIANCEON_SEGMENT 30
#define LASTCOMPLIANCEON_SEGMENT 31
#define FIRSTSPECIAL_SEGMENT -1
#define LASTSPECIAL_SEGMENT -1
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
//{SegmentComments: DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, READMINMAX, IMPEDANCE_MP25_V10_AONLY, IMPEDANCE_MP25_V10_BONLY, IMPEDANCE_MP400_V10_AONLY, IMPEDANCE_MP400_V10_BONLY, IMPEDANCE_CG25_V10_AONLY, IMPEDANCE_CG25_V10_BONLY, IMPEDANCE_MP25_V10_AONLY, IMPEDANCE_MP25_V10_BONLY, WARMDOWN, COMPLIANCEON_A, COMPLIANCEON_B, COMPLIANCE_OFF, COMPLIANCE_WARMDOWN}
//{SegmentLevels: 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}
extern const int *Sequence[];
extern int SegmentLengths [34];
extern int SegmentRepeats [34];
