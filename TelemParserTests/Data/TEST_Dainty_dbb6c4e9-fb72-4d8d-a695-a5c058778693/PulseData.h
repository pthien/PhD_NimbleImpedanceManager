/*********************************************
Contains computer generated code, do not edit
Generated on: 12/05/2016 3:10:53 PM
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


extern const int PulseData[743][12];
#endif /* PULSEDATA_H */
//{GenerationGUID: dbb6c4e9-fb72-4d8d-a695-a5c058778693}
