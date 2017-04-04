using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Printer : MonoBehaviour {

    private GcodeLoader gcodeLoader;

    public float DefaultHeadSpeed;
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
    public float TargetPositionExtruder;

    private float StartTime;
    private float distanceToMoveHead;

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
            y = ValidateParameter(y) ? y : CurrentPositionHead.y;
            TargetPositionHead = new Vector3(x, transform.position.y, y);
            TargetPositionTable = ValidateParameter(z) ? z : CurrentPositionTable;
            TargetPositionExtruder = ValidateParameter(extrusion) ? extrusion : CurrentPositionExtruder;
            DesiredSpeed = ValidateParameter(speed) ? speed : DefaultHeadSpeed;
            DesiredSpeed = DefaultHeadSpeed < DesiredSpeed ? DefaultHeadSpeed : DesiredSpeed;

            ArcRadius = 0;
            StartTime = Time.time;
            distanceToMoveHead = Vector3.Distance(StartPositionHead, TargetPositionHead);
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
        Move(0, 0, 0, CurrentPositionExtruder, DefaultHeadSpeed);
    }

    public bool IsBusy() {
        return Busy;
    }

    //public void SetCoordinatesRelative(bool relative = true) {
    //    UseAbsoluteCoordinates = relative;
    //}

    private void Step() {
        float distanceMoved = (Time.time - StartTime) * DesiredSpeed;
        float toStep = distanceMoved / distanceToMoveHead;
        CurrentPositionHead = Vector3.Lerp(StartPositionHead, TargetPositionHead, toStep);
        CurrentPositionExtruder = Mathf.Lerp(StartPositionExtruder, TargetPositionExtruder, toStep);
        CurrentPositionTable = Mathf.Lerp(StartPositionTable, TargetPositionTable, toStep);
    }

    private bool ValidateProgress() {
        float distanceHead = Vector2.Distance(CurrentPositionHead, TargetPositionHead);
        float distanceTable = CurrentPositionTable - TargetPositionTable;
        if (-Accuracy < distanceHead && distanceHead < Accuracy) {
            if (-Accuracy < distanceTable && distanceTable < Accuracy) {
                return true;
            }
        }
        return false;
    }

    private bool ValidateParameter(float param) {
        return (param != Gcodes.INVALID_NUMBER);
    }

    void Awake() {
        gcodeLoader = GameObject.FindObjectOfType<GcodeLoader>();
    }

	void Start () {
		
	}
	
	void FixedUpdate () {
        Busy = !ValidateProgress();
        if (!Busy) {
            NextGcodeCommand(this);
        }
        if (Busy) {
            Step();

            Debug.Log("position Head: " + CurrentPositionHead);
            Debug.Log("position Extruder: " + CurrentPositionExtruder);
            Debug.Log("position Table: " + CurrentPositionTable);
            Debug.Log("------------------");
        }
	}
}
