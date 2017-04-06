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
    
    //public bool UseAbsoluteCoordinates;

    public bool Busy;

    public float FeedRatePerMinute;
    float FeedRatePerSecond;
    public float ArcRadius;

    public Vector3 StartPositionHead;
    public float StartPositionExtruder;

    public Vector3 CurrentPositionHead;
    public float CurrentPositionExtruder;
    
    public Vector3 TargetPositionHead;
    public float TargetPositionExtruder=0;
    public float Thickness;

    private float StartTime;
    private float DistanceToMoveHead;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x">Printer heads destination width (Unity X-axis)</param>
    /// <param name="y">Printer heads destination depth (Unity Z-axis)</param>
    /// <param name="z">Printer heads destination table height (Unity Y-axis)</param>
    /// <param name="extrusion"></param>
    /// <param name="speed"></param>
    public void Move(float x, float y, float z, float extrusion, float speed) {
        if (!Busy) {
            StartPositionHead = CurrentPositionHead;
            StartPositionExtruder = CurrentPositionExtruder;
            x = ValidateParameter(x) ? x : CurrentPositionHead.x;
            y = ValidateParameter(y) ? y : CurrentPositionHead.z;
            z = ValidateParameter(z) ? z : CurrentPositionHead.y;
            TargetPositionHead = new Vector3(x, z, y);
            float newTargetPositionExtruder = ValidateParameter(extrusion) ? extrusion : TargetPositionExtruder;
            float amountExtrudedForLine = newTargetPositionExtruder - TargetPositionExtruder;
            TargetPositionExtruder = newTargetPositionExtruder;
            FeedRatePerMinute = ValidateParameter(speed) ? speed : FeedRatePerMinute;
            FeedRatePerMinute = MaxHeadSpeed < FeedRatePerMinute ? MaxHeadSpeed : FeedRatePerMinute;
            FeedRatePerSecond = FeedRatePerMinute / 60;

            ArcRadius = 0;
            StartTime = Time.realtimeSinceStartup;
            DistanceToMoveHead = Vector3.Distance(StartPositionHead, TargetPositionHead);
            if (DistanceToMoveHead > 0.000001 && amountExtrudedForLine > 0.0000001) {
                Thickness = 10 * amountExtrudedForLine / DistanceToMoveHead;
            } else {
                Thickness = 0.05f;
            }
            if(Thickness < 0) {
                Thickness = 0;
            }
        }
    }

    public void ArcClockWise(float x, float y, float z, float radius) {
        
    }

    public void ArcCounterClockWise(float x, float y, float z, float radius) {

    }

    public void Dwell(float seconds) {
        if (0 < seconds) {
            new WaitForSeconds(seconds);
        }
    }

    public void HomeAllAxis() {
        Move(0, 0, 0, CurrentPositionExtruder, MaxHeadSpeed);
    }

    public bool IsBusy() {
        return Busy;
    }

    //public void SetCoordinatesRelative(bool relative = true) {
    //    UseAbsoluteCoordinates = relative;
    //}

    private void Step() {
        float distanceMoved = (Time.realtimeSinceStartup - StartTime) * FeedRatePerSecond * TimeMultiplier;
        float toStep = distanceMoved / DistanceToMoveHead;
        if (toStep > 1 || fastMode) {
            toStep = 1;
        }
        Vector3 oldPositionHead = CurrentPositionHead;
        CurrentPositionHead = Vector3.Lerp(StartPositionHead, TargetPositionHead, toStep);
        CurrentPositionExtruder = Mathf.Lerp(StartPositionExtruder, TargetPositionExtruder, toStep);
        if(Thickness > 0.0001f) {
            FilamentManager.AddFilament(oldPositionHead, CurrentPositionHead, Thickness);
        }
    }

    private bool ValidateProgress() {
        float distanceHead = Vector3.Distance(CurrentPositionHead, TargetPositionHead);
        if (-Accuracy < distanceHead && distanceHead < Accuracy) {
            return true;
        }
        return false;
    }

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
        float updateStartTime = Time.realtimeSinceStartup;
        float updateLength = updateStartTime + Time.fixedDeltaTime * 0.70f;
        while (updateLength > Time.realtimeSinceStartup) {
            Busy = !ValidateProgress();
            if (!Busy) {
                if (!GcodeLoader.NextGcodeCommand(this)) {
                    break;
                }
            } else {
                Step();
            }
        }
    }
}
