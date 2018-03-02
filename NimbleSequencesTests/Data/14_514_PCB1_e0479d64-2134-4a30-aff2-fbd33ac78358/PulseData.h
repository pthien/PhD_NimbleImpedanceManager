/*********************************************
Contains computer generated code, do not edit
Generated on: 5/08/2015 3:43:24 PM
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


extern const int PulseData[241][12];
#endif /* PULSEDATA_H */
//{GenerationGUID: e0479d64-2134-4a30-aff2-fbd33ac78358}
