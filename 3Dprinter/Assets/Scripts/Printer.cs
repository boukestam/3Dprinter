using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Printer : MonoBehaviour {

    private GcodeLoader GcodeLoader;
    private FilamentManager FilamentManager;

    public float MaxHeadSpeed;
    public float Accuracy;

    //public bool UseAbsoluteCoordinates;

    public bool Busy;

    public float DesiredSpeed;
    public float ArcRadius;

    public Vector3 StartPositionHead;
    public float StartPositionTable;
    public float StartPositionExtruder;

    public Vector3 CurrentPositionHead;
    public float CurrentPositionTable;
    public float CurrentPositionExtruder;
    
    public Vector3 TargetPositionHead;
    public float TargetPositionTable;
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
            StartPositionTable = CurrentPositionTable;
            StartPositionExtruder = CurrentPositionExtruder;
            x = ValidateParameter(x) ? x : CurrentPositionHead.x;
            y = ValidateParameter(y) ? y : CurrentPositionHead.z;
            TargetPositionHead = new Vector3(x, transform.localPosition.y, y);
            TargetPositionTable = ValidateParameter(z) ? z : CurrentPositionTable;
            float newTargetPositionExtruder = ValidateParameter(extrusion) ? extrusion : TargetPositionExtruder;
            float amountExtrudedForLine = newTargetPositionExtruder - TargetPositionExtruder;
            TargetPositionExtruder = newTargetPositionExtruder;
            DesiredSpeed = ValidateParameter(speed) ? speed : DesiredSpeed;
            DesiredSpeed = MaxHeadSpeed < DesiredSpeed ? MaxHeadSpeed : DesiredSpeed;

            ArcRadius = 0;
            StartTime = Time.time;
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
        float distanceMoved = (Time.time - StartTime) * DesiredSpeed;
        float toStep = distanceMoved / DistanceToMoveHead;
        if (toStep > 1) { toStep = 1; }
        Vector3 oldPositionHead = CurrentPositionHead;
        CurrentPositionHead = Vector3.Lerp(StartPositionHead, TargetPositionHead, toStep);
        CurrentPositionExtruder = Mathf.Lerp(StartPositionExtruder, TargetPositionExtruder, toStep);
        CurrentPositionTable = Mathf.Lerp(StartPositionTable, TargetPositionTable, toStep);
        if(Thickness > 0.0001f) {
            FilamentManager.AddFilament(oldPositionHead, CurrentPositionHead, Thickness);
        }
    }

    private bool ValidateProgress() {
        float distanceHead = Vector3.Distance(CurrentPositionHead, TargetPositionHead);
        float distanceTable = CurrentPositionTable - TargetPositionTable;
        if (-Accuracy < distanceHead && distanceHead < Accuracy) {
            if (-Accuracy < distanceTable && distanceTable < Accuracy) {
                return true;
            }
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
        Busy = !ValidateProgress();
        if (!Busy) {
            GcodeLoader.NextGcodeCommand(this);
        } else {
            Step();

            /*Debug.Log("position Head: " + CurrentPositionHead);
            Debug.Log("position Extruder: " + CurrentPositionExtruder);
            Debug.Log("position Table: " + CurrentPositionTable);
            Debug.Log("------------------");*/
        }
	}
}
