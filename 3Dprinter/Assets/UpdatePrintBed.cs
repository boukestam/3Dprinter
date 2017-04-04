using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePrintBed : MonoBehaviour {

    private Printer printer;

    void Awake() {
        printer = GameObject.FindObjectOfType<Printer>();
    }

    void Update() {
        if(transform.localPosition.y != printer.CurrentPositionHead.y) {
            transform.localPosition = new Vector3(0, -printer.CurrentPositionHead.y, 0);
        }
    }
}
