/*********************************************
Contains computer generated code, do not edit
Generated on: 29/06/2015 4:17:42 PM
*********************************************/
//{GenerationGUID: 608e3568-59c1-408d-a53b-34203648716e}
#define GENERATION_GUID "608e3568-59c1-408d-a53b-34203648716e"
#define TOTAL_SEGMENTS 27
#define LAST_SEGMENT 26
#define NORMAL_SEGMENTS 19
#define STARTLOOP_SEGMENT 18
#define WARMUP_SEGMENT 20
#define WARMDOWN_SEGMENT 26
#define FIRSTIMPEDANCE_SEGMENT 21
#define LASTIMPEDANCE_SEGMENT 25
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
//{SegmentComments: DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, DUTY_ON, DUTY_OFF, READMINMAX, IMPEDANCE_MP145, IMPEDANCE_MP290, IMPEDANCE_MP580, IMPEDANCE_MP25, IMPEDANCE_CG145, WARMDOWN}
extern const int Sequence[27][38];
extern int SegmentLengths [27];
extern int SegmentRepeats [27];
