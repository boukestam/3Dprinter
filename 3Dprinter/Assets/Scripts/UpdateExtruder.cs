using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateExtruder : MonoBehaviour {

    private Printer printer;

    void Awake() {
        printer = GameObject.FindObjectOfType<Printer>();
    }
	
	void Update () {
        if (transform.localPosition.x != printer.CurrentPosition.x || transform.localPosition.z != printer.CurrentPosition.z) {
            transform.localPosition = new Vector3(printer.CurrentPosition.x, 0, printer.CurrentPosition.z);
        }
    }
}
