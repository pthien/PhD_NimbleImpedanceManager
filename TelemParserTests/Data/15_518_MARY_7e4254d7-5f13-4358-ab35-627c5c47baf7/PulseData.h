/*********************************************
Contains computer generated code, do not edit
Generated on: 14/08/2015 10:13:34 AM
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


extern const int PulseData[163][12];
#endif /* PULSEDATA_H */
//{GenerationGUID: 7e4254d7-5f13-4358-ab35-627c5c47baf7}
