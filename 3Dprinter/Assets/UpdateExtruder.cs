using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateExtruder : MonoBehaviour {

    private Printer printer;

    void Awake() {
        printer = GameObject.FindObjectOfType<Printer>();
    }
	
	void Update () {
        transform.position = printer.CurrentPositionHead;
    }
}
