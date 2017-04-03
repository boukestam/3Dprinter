using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gcodes {

    public const float INVALID_NUMBER = -10000000000;

    public const string MOVE0 = "G0";
    public const string MOVE1 = "G1";
    public const string HOME_AXIS = "G28";
    public const string SET_ABOSLUTE = "G90";
    public const string SET_CURRENT_POS = "G92";

    public const string FAN_ON = "M106";
    public const string FAN_OFF = "M107";
    public const string SET_EXTRUDER_TEMP = "M104";
    public const string SET_BED_TEMP = "M140";

    public const string SET = "S";
    public static Tuple<string, float> X = new Tuple<string, float>("X", Gcodes.INVALID_NUMBER);
    public static Tuple<string, float> Y = new Tuple<string, float>("Y", Gcodes.INVALID_NUMBER);
    public static Tuple<string, float> Z = new Tuple<string, float>("Z", Gcodes.INVALID_NUMBER);
    public static Tuple<string, float> E = new Tuple<string, float>("E", Gcodes.INVALID_NUMBER);
    public static Tuple<string, float> F = new Tuple<string, float>("F", Gcodes.INVALID_NUMBER);
    public static Tuple<string, float> S = new Tuple<string, float>("S", Gcodes.INVALID_NUMBER);

}
