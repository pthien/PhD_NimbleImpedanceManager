/*********************************************
Contains computer generated code, do not edit
Generated on: 8/03/2016 2:31:15 PM
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


extern const int PulseData[161][12];
#endif /* PULSEDATA_H */
//{GenerationGUID: a4ea6e4c-9b2a-46ed-9db2-d8c59915738f}
