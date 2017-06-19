using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class makes the camera draggable, rotatable and zoomable in the scene.
/// </summary>
public class CameraMovement : MonoBehaviour {
    /// <param name="ScrollSpeed">The speed at which scrolling/zooming in/out is executed when scrolling with the left mouse wheel.</param>
    public float ScrollSpeed = 10f;
    /// <param name="MoveSpeed">The speed of the camera moving left/right and up/down when dragging with the move mouse button.</param>
    public float MoveSpeed = 0.1f;
    /// <param name="RotateSpeed">The speed of the camera rotating when dragging with the rotate mouse button.</param>
    public float RotateSpeed = 0.1f;

    /// <param name="PreviousMousePosition">Remembers the last mouse position since the last Update.</param>
    private Vector3 PreviousMousePosition;

    /// <param name="Yaw">The yaw orientation of the camera.</param>
    private float Yaw = 0f;
    /// <param name="Pitch">The pitch orientation of the camera.</param>
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
