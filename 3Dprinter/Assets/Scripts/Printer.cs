using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Printer : MonoBehaviour {

    public bool Busy;

    public bool AbsoluteCoordinates;

    public Vector3 positionHead;
    public Vector3 positionTable;
    public float positionExtruder;

    public bool isBusy() {
        return Busy;
    }

    public void setCoordinatesRelative(bool relative = true) {

    }

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
