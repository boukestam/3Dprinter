using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Printer : MonoBehaviour {

    public bool fastMode;
    public float MaxHeadSpeed;
    public float Accuracy;

    public float TimeMultiplier;

    private GcodeLoader GcodeLoader;
    private FilamentManager FilamentManager;
    
    private bool Busy;

    public float FeedRatePerMinute;
    private float FeedRatePerSecond;
    public float ArcRadius;

    public Vector3 StartPosition;
    public float StartPositionExtruder;

    public Vector3 CurrentPosition;
    public float CurrentPositionExtruder;
    
    public Vector3 TargetPosition;
    public float TargetPositionExtruder=0;
    public float Thickness;

    private float StartTime;
    private float DistanceToMoveHead;

    private float previousToStep = 0;

    /// <summary>
    /// This function sets the TimeMultiplier. This param influences the simulation speed. 
    /// </summary>
    /// <param name="multiplier">multiply factor</param>
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

            ArcRadius = 0;
            StartTime = Time.realtimeSinceStartup;
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
        float timeToFinish = DistanceToMoveHead * (1 - previousToStep) / FeedRatePerSecond;
        float timeToUse = Mathf.Min(timeToFinish, maxAllowedTime);
        float toStep = timeToUse / timeToFinish;
        toStep += previousToStep;
        if (toStep >= 1 || fastMode) {
            toStep = 1;
            previousToStep = 0;
        } else {
            previousToStep = toStep;
        }

        CurrentPosition = Vector3.Lerp(StartPosition, TargetPosition, toStep);
        CurrentPositionExtruder = Mathf.Lerp(StartPositionExtruder, TargetPositionExtruder, toStep);
        if(Thickness > 0.0001f) {
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
        bool valid = ((int)param != (int)Gcodes.INVALID_NUMBER);
        return valid;
    }

    void Awake() {
        GcodeLoader = GameObject.FindObjectOfType<GcodeLoader>();
        FilamentManager = GameObject.FindObjectOfType<FilamentManager>();
    }

	void Start () {
		
	}
	
	void FixedUpdate () {
        /*float updateStartTime = Time.realtimeSinceStartup;
        float updateLength = updateStartTime + Time.fixedDeltaTime * 0.75f;

        float maxAllowedTime = TimeMultiplier * Time.fixedDeltaTime;
        while (updateLength > Time.realtimeSinceStartup) {
            Busy = !ValidateProgress();
            if (!Busy) {
                if (!GcodeLoader.NextGcodeCommand(this)) {
                    break;
                }
            }
            maxAllowedTime -= Step(maxAllowedTime);
            if (maxAllowedTime <= 0) {
                break;
            }
        }*/
    }
}
