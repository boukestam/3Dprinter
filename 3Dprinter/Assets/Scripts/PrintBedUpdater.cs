using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintBedUpdater : MonoBehaviour {

    private Printer Printer;

    void Awake() {
        Printer = GameObject.FindObjectOfType<Printer>();
    }

    void Update() {
        if(transform.localPosition.y != Printer.CurrentPosition.y) {
            transform.localPosition = new Vector3(0, -Printer.CurrentPosition.y, 0);
        }
    }
}
