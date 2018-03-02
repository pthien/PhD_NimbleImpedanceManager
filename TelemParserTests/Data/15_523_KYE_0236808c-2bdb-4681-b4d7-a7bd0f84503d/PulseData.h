/*********************************************
Contains computer generated code, do not edit
Generated on: 3/02/2016 9:12:36 AM
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


extern const int PulseData[349][12];
#endif /* PULSEDATA_H */
//{GenerationGUID: 0236808c-2bdb-4681-b4d7-a7bd0f84503d}
