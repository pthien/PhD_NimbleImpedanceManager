/*********************************************
Contains computer generated code, do not edit
Generated on: 29/06/2015 4:17:42 PM
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


extern const int PulseData[198][12];
#endif /* PULSEDATA_H */
//{GenerationGUID: 608e3568-59c1-408d-a53b-34203648716e}
