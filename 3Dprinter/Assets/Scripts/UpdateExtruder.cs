using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateExtruder : MonoBehaviour {

    private Printer printer;

    void Awake() {
        printer = GameObject.FindObjectOfType<Printer>();
    }
	
	void Update () {
        if (transform.localPosition.x != printer.CurrentPositionHead.x || transform.localPosition.z != printer.CurrentPositionHead.z) {
            transform.localPosition = new Vector3(printer.CurrentPositionHead.x, 0, printer.CurrentPositionHead.z);
        }
    }
}
