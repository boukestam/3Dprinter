using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtruderUpdater : MonoBehaviour {

    private Printer Printer;

    void Awake() {
        Printer = GameObject.FindObjectOfType<Printer>();
    }
	
	void Update () {
        if (transform.localPosition.x != Printer.CurrentPosition.x || transform.localPosition.z != Printer.CurrentPosition.z) {
            transform.localPosition = new Vector3(Printer.CurrentPosition.x, 0, Printer.CurrentPosition.z);
        }
    }
}
