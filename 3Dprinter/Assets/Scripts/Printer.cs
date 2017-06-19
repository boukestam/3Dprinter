using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
///     This class progresses the printprocess and is used as central control unit in executing G-code commands.
/// </summary>
public class Printer : MonoBehaviour {
    /// <param name="GcodeFile">The path of the current selected G-code file that will be printed.</param>
    public string GcodeFile;

    /// <param name="MaxHeadSpeed">The maximum allowed speed of the print head in feedrate/minute.</param>
    public float MaxHeadSpeed;
    /// <param name="Accuracy">This number is used to determine if two floating points are about the same value by using this number as maximum allowed difference.</param>
    public float Accuracy;

    /// <param name="TimeMultiplier">The current time speed of the printing process in contrast to real time printing speed.</param>
    public float TimeMultiplier;

    /// <param name="CurrentExtruderTemperature">The current temperature of the extruder. Used for comparison with the target temperature.</param>
    public float CurrentExtruderTemperature;
    /// <param name="CurrentBedTemperature">The current temperature of the bed. Used for comparison with the target temperature.</param>
    public float CurrentBedTemperature;

    /// <param name="CurrentPosition">The current position of the filament needle on the x and z axis. And the print bed position on the y axis.</param>
    public Vector3 CurrentPosition;
    /// <param name="CurrentPositionExtruder">The current position of the extruder. Used to determine how much extrusion needs to be added to the print model.</param>
    public float CurrentPositionExtruder;

    /// <param name="FeedRatePerMinute">The current feedrate/minute of the print needle moving around to print.</param>
    public float FeedRatePerMinute;
    /// <param name="FeedRatePerSecond">The current feedrate/second of the print needle moving around to print.</param>
    private float FeedRatePerSecond;

    /// <param name="TargetExtruderTemperature">The target temperature of the extruder.</param>
    private float TargetExtruderTemperature;
    /// <param name="TargetBedTemperature">The target temperature of the bed.</param>
    private float TargetBedTemperature;

    /// <param name="StartPosition">The start position of the current executing print command. Do not confuse this with CurrentPosition.</param>
    private Vector3 StartPosition;
    /// <param name="StartPositionExtruder">The start extruder position of the current executing print command. Do not confuse this with CurrentPositionExtruder.</param>
    private float StartPositionExtruder;

    /// <param name="TargetPosition">The target/goal position of the current executing print command.</param>
    private Vector3 TargetPosition;
    /// <param name="TargetPositionExtruder">The target/goal extruder position of the current executing print command.</param>
    private float TargetPositionExtruder=0;
    /// <param name="Thickness">The thickness of the filament in the current print command. This variable is used to tell the FilamentManager to print the filament with the given thickness.</param>
    private float Thickness;

    /// <param name="GcodeLoader">Object of the GcodeLoader that is used to execute all G-code commands.</param>
    private GcodeLoader GcodeLoader;
    /// <param name="FilamentManager">Object of the FilamentManager that is used to add new filament to the scene.</param>
    private FilamentManager FilamentManager;

    /// <param name="Busy">Boolean that is used to indicate if the printing process is currently active. true if active false if not.</param>
    private bool Busy;

    /// <param name="DistanceToMoveHead">The distance between the StartPosition and TargetPosition.</param>
    private float DistanceToMoveHead;

    /// <param name="PreviousToStep">The last time the local variable toStep was calculated. toStep is the amount moved from StartPosition to TargetPosition given in the range from 0 to 1.</param>
    private float PreviousToStep = 0;

    void Awake() {
        GcodeLoader = new GcodeLoader();
        FilamentManager = GameObject.FindObjectOfType<FilamentManager>();
    }

    void FixedUpdate() {
        float updateStartTime = Time.realtimeSinceStartup;
        float updateLength = updateStartTime + Time.fixedDeltaTime * 0.75f;

        float maxAllowedTime = TimeMultiplier * Time.fixedDeltaTime;
        while (updateLength > Time.realtimeSinceStartup) {
            Busy = !ValidateProgress();
            if (!Busy) {
                if (!GcodeLoader.ExecuteNextCommand(this)) {
                    break;
                }
            }
            maxAllowedTime -= Step(maxAllowedTime);
            if (maxAllowedTime <= 0) {
                break;
            }
        }
    }

    /// <summary>
    /// This function is used to trigger a select window to choose a file from the filesystem that has G-code inside it.
    /// </summary>
    public void SelectFile() {
        if (!GcodeLoader.ModelLoaded) {
            string path = EditorUtility.OpenFilePanel("Select G-code file", "", "gcode");
            if (path != "" && path != null) {
                GcodeFile = path;
                GcodeLoader.Load(GcodeFile);
            }
        } else {
            EditorUtility.DisplayDialog("Select G-code file", "The printer is still printing another model", "OK");
            return;
        }
    }

    /// <summary>
    /// This function sets the TimeMultiplier. This param influences the simulation speed. 
    /// </summary>
    /// <param name="multiplier">Multiply factor</param>
    public void SetTimeMultiplier(float multiplier) {
        TimeMultiplier = multiplier;
    }

    /// <summary>
    /// This function is used to give the printer a new move command, which in G-code is G0 or G1.
    /// </summary>
    /// <param name="x">Printer heads destination width (Unity X-axis)</param>
    /// <param name="y">Printer heads destination depth (Unity Z-axis)</param>
    /// <param name="z">Printer heads destination table height (Unity Y-axis)</param>
    /// <param name="extrusion">The new position of the printer extrusion step motor after moving. This new position is absoluut to the origin of the extrusion step motor.</param>
    /// <param name="feedrate">The speed of the printer head movement in mm/minute</param>
    public void Move(float x, float y, float z, float extrusion, float feedrate) {
        if (!Busy) {
            StartPosition = CurrentPosition;
            StartPositionExtruder = CurrentPositionExtruder;
            x = ValidateParameter(x) ? x : CurrentPosition.x;
            y = ValidateParameter(y) ? y : CurrentPosition.z;
            z = ValidateParameter(z) ? z : CurrentPosition.y;
            TargetPosition = new Vector3(x, z, y);
            float newTargetPositionExtruder = ValidateParameter(extrusion) ? extrusion : TargetPositionExtruder;
            float amountExtrudedForLine = newTargetPositionExtruder - TargetPositionExtruder;
            TargetPositionExtruder = newTargetPositionExtruder;
            FeedRatePerMinute = ValidateParameter(feedrate) ? feedrate : FeedRatePerMinute;
            FeedRatePerMinute = MaxHeadSpeed < FeedRatePerMinute ? MaxHeadSpeed : FeedRatePerMinute;
            FeedRatePerSecond = FeedRatePerMinute / 60;
            
            DistanceToMoveHead = Vector3.Distance(StartPosition, TargetPosition);
            if (DistanceToMoveHead > 0.000001 && amountExtrudedForLine > 0.0000001) {
                Thickness = 10 * amountExtrudedForLine / DistanceToMoveHead;
            } else {
                Thickness = 0.0f;
            }
            if(Thickness < 0) {
                Thickness = 0;
            }
        }
    }

    /// <summary>
    /// Changes the TargetExtruderTemperature of the extruder.
    /// </summary>
    /// <param name="temperature">The new target temperature.</param>
    public void SetExtruderTemperature(float temperature) {
        TargetExtruderTemperature = temperature;
        if (TargetExtruderTemperature > 0) { } // Make the unused warning go away
        //hack;
        CurrentExtruderTemperature = temperature;
    }

    /// <summary>
    /// Changes the TargetBedTemperature of the bed.
    /// </summary>
    /// <param name="temperature">The new target temperature.</param>
    public void SetBedTemperature(float temperature) {
        TargetBedTemperature = temperature;
        if (TargetBedTemperature > 0) { } // Make the unused warning go away
        //hack;
        CurrentBedTemperature = temperature;
    }

    //TODO add axis params.
    /// <summary>
    /// This function returns all axis to the origin of the printerhead.
    /// </summary>
    public void HomeAllAxis() {
        Move(0, 0, 0, CurrentPositionExtruder, MaxHeadSpeed);
    }

    /// <summary>
    /// Returns true if the printer is busy executing a command.
    /// </summary>
    public bool IsBusy() {
        return Busy;
    }

    /// <summary>
    /// This function is the simulation step. This function will change the position of the printerhead and printerbed based on the time passed since the last commando.
    /// </summary>
    /// <param name="maxAllowedTime">This parameter is used to make a smaller print step. To maintain the FeedRate speed.</param>
    /// <returns>This function returns the time used by this function.</returns>
    private float Step(float maxAllowedTime) {
        if (DistanceToMoveHead == 0) {
            return 0;
        }
        float timeToFinish = DistanceToMoveHead * (1 - PreviousToStep) / FeedRatePerSecond;
        float timeToUse = Mathf.Min(timeToFinish, maxAllowedTime);
        float toStep = timeToUse / timeToFinish;
        toStep += PreviousToStep;
        if (toStep >= 1) {
            toStep = 1;
            PreviousToStep = 0;
        } else {
            PreviousToStep = toStep;
        }

        CurrentPosition = Vector3.Lerp(StartPosition, TargetPosition, toStep);
        CurrentPositionExtruder = Mathf.Lerp(StartPositionExtruder, TargetPositionExtruder, toStep);
        if (Thickness > 0.0001f) {
            FilamentManager.AddFilament(StartPosition, CurrentPosition, Thickness);
        }
        return timeToUse;
    }

    /// <summary>
    /// This function returns true if the currentPosition is equal to the TargetPosition. Those to values may divert from each other with the Accuracy value
    /// </summary>
    private bool ValidateProgress() {
        float distanceHead = Vector3.Distance(CurrentPosition, TargetPosition);
        if (-Accuracy < distanceHead && distanceHead < Accuracy) {
            return true;
        }
        return false;
    }

    /// <summary>
    /// This function returns if the parameter given is valid.
    /// </summary>
    /// <param name="param">parameter to validate</param>
    private bool ValidateParameter(float param) {
        bool valid = ((int)param != (int)GMcodes.InvalidNumber);
        return valid;
    }
}
