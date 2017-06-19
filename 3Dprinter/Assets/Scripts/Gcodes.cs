using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class contains all values of the matching G and M codes.
/// </summary>
public class GMcodes {

    public const float InvalidNumber = -1000000000;

    // G codes
    /// <param name="Move0">The command for moving the printer with filament.</param>
    public const int Move0 = 0;
    /// <param name="Move1">The command for moving the printer without filament.</param>
    public const int Move1 = 1;
    /// <param name="HomeAxis">A Move1 command at maximum speed to the home coordinates (0,0,0).</param>
    public const int HomeAxis = 28;

    //public const int SetCurrentPosition = 92;
    //public const int SetAbsolute = 90;

    // M codes
    /// <param name="SetExtruderTemperature1">Sets the extruder temperature for RepRap G-code.</param>
    public const int SetExtruderTemperature1 = 104;
    /// <param name="SetExtruderTemperature2">Sets the extruder temperature for UltiGCode G-code.</param>
    public const int SetExtruderTemperature2 = 109;
    /// <param name="SetBedTemperature1">Sets the bed temperature for RepRap G-code.</param>
    public const int SetBedTemperature1 = 140;
    /// <param name="SetBedTemperature2">Sets the bed temperature for UltiGCode G-code.</param>
    public const int SetBedTemperature2 = 190;

    //public const int FanOn = 106;
    //public const int FanOff = 107;


}
