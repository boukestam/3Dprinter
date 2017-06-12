using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class contains all values of the G and M codes.
/// </summary>
public class GMcodes {

    public const float InvalidNumber = -1000000000;

    // G codes
    public const int Move0 = 0;
    public const int Move1 = 1;
    public const int HomeAxis = 28;
    public const int SetAbsolute = 90;
    public const int SetCurrentPosition = 92;

    // M codes
    public const int FanOn = 106;
    public const int FanOff = 107;
    public const int SetExtruderTemperature1 = 104;
    public const int SetExtruderTemperature2 = 109;
    public const int SetBedTemperature1 = 140;
    public const int SetBedTemperature2 = 190;


}
