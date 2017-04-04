using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePrintBed : MonoBehaviour {

    private Printer printer;

    void Awake() {
        printer = GameObject.FindObjectOfType<Printer>();
    }

    void Update() {
        if(transform.position.y != -printer.CurrentPositionTable) {
            transform.position = new Vector3(0, -printer.CurrentPositionTable, 0);
        }
    }
}
