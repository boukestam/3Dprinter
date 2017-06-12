using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    public float ScrollSpeed = 10f;
    public float MoveSpeed = 0.1f;
    public float RotateSpeed = 0.1f;

    private Vector3 PreviousMousePosition;

    private float Yaw = 0f;
    private float Pitch = 0f;
    
	void Start () {
        PreviousMousePosition = Input.mousePosition;
        Yaw = transform.eulerAngles.y;
        Pitch = transform.eulerAngles.x;
	}
	
	void Update () {
        var mouseDelta = Input.mousePosition - PreviousMousePosition;
        PreviousMousePosition = Input.mousePosition;

        var translate = new Vector3();

        translate.z = Input.mouseScrollDelta.y * ScrollSpeed;

        if (Input.GetButton("CameraMove")) {
            translate.x -= mouseDelta.x * MoveSpeed;
            translate.y -= mouseDelta.y * MoveSpeed;
        }

        if (Input.GetButton("CameraRotate")) {
            Yaw += mouseDelta.x * RotateSpeed;
            Pitch -= mouseDelta.y * RotateSpeed;
        }

        transform.Translate(translate);
        transform.eulerAngles = new Vector3(Pitch, Yaw, 0f);
	}
}
