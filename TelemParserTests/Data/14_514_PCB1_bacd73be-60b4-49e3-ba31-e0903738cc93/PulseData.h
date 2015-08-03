/*********************************************
Contains computer generated code, do not edit
Generated on: 31/07/2015 3:27:34 PM
Contains encoded pulse data intended for a PIC
operating at 20 MHz
*********************************************/

#ifndef PULSEDATA_H
#define	PULSEDATA_H

#include "Clock.h"
#include <stdint.h>

#if (FCY != CLOCK_20MHZ)
 error_wrong_clock_freq
#endif

#define POWER_UP_PULSE_INDEX 0


extern const int PulseData[242][12];
#endif /* PULSEDATA_H */
//{GenerationGUID: bacd73be-60b4-49e3-ba31-e0903738cc93}
