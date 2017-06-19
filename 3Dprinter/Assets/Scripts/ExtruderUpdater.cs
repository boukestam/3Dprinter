using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class updates the extruder needle location in the scene.
/// </summary>
public class ExtruderUpdater : MonoBehaviour {
    /// <param name="Printer">Object of the Printer that is used to request the printer needle location.</param>
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
