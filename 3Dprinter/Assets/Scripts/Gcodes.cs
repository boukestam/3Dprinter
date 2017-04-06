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
    /*public static readonly Tuple<char, int> MOVE1 = new Tuple<char, int>('G', 1);
    public static readonly Tuple<char, int> HOME_AXIS = new Tuple<char, int>('G', 28);
    public static readonly Tuple<char, int> SET_ABOSLUTE = new Tuple<char, int>('G', 90);
    public static readonly Tuple<char, int> SET_CURRENT_POS = new Tuple<char, int>('G', 92);

    public static readonly Tuple<char, int> FAN_ON = new Tuple<char, int>('M', 106);
    public static readonly Tuple<char, int> FAN_OFF = new Tuple<char, int>('M', 107);
    public static readonly Tuple<char, int> SET_EXTRUDER_TEMP = new Tuple<char, int>('M', 104);
    public static readonly Tuple<char, int> SET_BED_TEMP = new Tuple<char, int>('M', 140);*/
    /*
    public static WriteTuple<char, float> X = new WriteTuple<char, float>('X', Gcodes.INVALID_NUMBER);
    public static WriteTuple<char, float> Y = new WriteTuple<char, float>('Y', Gcodes.INVALID_NUMBER);
    public static WriteTuple<char, float> Z = new WriteTuple<char, float>('Z', Gcodes.INVALID_NUMBER);
    public static WriteTuple<char, float> E = new WriteTuple<char, float>('E', Gcodes.INVALID_NUMBER);
    public static WriteTuple<char, float> F = new WriteTuple<char, float>('F', Gcodes.INVALID_NUMBER);
    public static WriteTuple<char, float> S = new WriteTuple<char, float>('S', Gcodes.INVALID_NUMBER);
    */
}
