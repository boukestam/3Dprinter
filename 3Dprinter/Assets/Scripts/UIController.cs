using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    private Printer Printer;

    private GameObject PrinterDetails;
    private Text ExtruderTemperature;
    private Text BedTemperature;
    private Text FeedRate;
    private Text ExtrusionPosition;
    private Text ExtruderPositionX;
    private Text ExtruderPositionY;
    private Text BedHeight;
    
    GameObject SimulationDetails;
    private Text TimeMultiplier;


	// Use this for initialization
	void Start () {
        Printer = (Printer)GameObject.Find("Printer").GetComponent("Printer");

        PrinterDetails = GameObject.Find("PrinterDetails");

        ExtruderTemperature = PrinterDetails.transform.FindChild("TempExtruderData").gameObject.GetComponent<Text>();
        BedTemperature = PrinterDetails.transform.FindChild("TempBedData").gameObject.GetComponent<Text>();
        FeedRate = PrinterDetails.transform.FindChild("FeedRateData").gameObject.GetComponent<Text>();
        ExtrusionPosition = PrinterDetails.transform.FindChild("ExtrusionPositionData").gameObject.GetComponent<Text>();
        ExtruderPositionX = PrinterDetails.transform.FindChild("ExtruderPositionXData").gameObject.GetComponent<Text>();
        ExtruderPositionY = PrinterDetails.transform.FindChild("ExtruderPositionYData").gameObject.GetComponent<Text>();
        BedHeight = PrinterDetails.transform.FindChild("BedPositionData").gameObject.GetComponent<Text>();

        SimulationDetails = GameObject.Find("SimulationDetails");

        TimeMultiplier = SimulationDetails.transform.FindChild("TimeMultiplierData").gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update () {
        string format = "0.0";
        ExtruderTemperature.text = Printer.CurrentExtruderTemperature.ToString(format) + " °C";
        BedTemperature.text = Printer.CurrentBedTemperature.ToString(format) + " °C";
        FeedRate.text = Printer.FeedRatePerMinute.ToString("0") + " mm/min";
        ExtrusionPosition.text = Printer.CurrentPositionExtruder.ToString(format) + " mm";
        ExtruderPositionX.text = Printer.CurrentPosition.x.ToString(format) + " mm";
        ExtruderPositionY.text = Printer.CurrentPosition.z.ToString(format) + " mm";
        BedHeight.text = Printer.CurrentPosition.y.ToString(format) + " mm";

        TimeMultiplier.text = Printer.TimeMultiplier.ToString(format) + " X";
    }
}
