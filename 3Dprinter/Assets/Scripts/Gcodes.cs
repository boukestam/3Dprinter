using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gcodes {

    public const float INVALID_NUMBER = -1000000000;

    // G codes
    public const int MOVE0 = 0;
    public const int MOVE1 = 1;
    public const int HOME_AXIS = 28;
    public const int SET_ABOSLUTE = 90;
    public const int SET_CURRENT_POS = 92;

    // M codes
    public const int FAN_ON = 106;
    public const int FAN_OFF = 107;
    public const int SET_EXTRUDER_TEMP = 104;
    public const int SET_BED_TEMP = 140;
}
