using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class updates the print bed location in the scene.
/// </summary>
public class PrintBedUpdater : MonoBehaviour {

    /// <param name="Printer">Object of the Printer that is used to request the printer bed location.</param>
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
