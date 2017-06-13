using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    private Printer Printer;

    GameObject PrinterStats;
    private Text ExtruderTemperature;
    private Text BedTemperature;
    private Text FeedRate;
    private Text ExtrusionPosition;
    private Text ExtruderXPosition;
    private Text ExtruderYPosition;
    private Text BedHeigth;
    
    GameObject SimulationDetails;
    private Text TimeMultiplier;
    private Text File;

    // Use this for initialization
    void Start () {
        Printer = (Printer)GameObject.Find("Printer").GetComponent("Printer");

        PrinterStats = GameObject.Find("PrinterStats");

        ExtruderTemperature = PrinterStats.transform.FindChild("TempExtruderData").gameObject.GetComponent<Text>();
        BedTemperature = PrinterStats.transform.FindChild("TempBedData").gameObject.GetComponent<Text>();
        FeedRate = PrinterStats.transform.FindChild("FeedRateData").gameObject.GetComponent<Text>();
        ExtrusionPosition = PrinterStats.transform.FindChild("ExtrusionPositionData").gameObject.GetComponent<Text>();
        ExtruderXPosition = PrinterStats.transform.FindChild("ExtruderPositionXData").gameObject.GetComponent<Text>();
        ExtruderYPosition = PrinterStats.transform.FindChild("ExtruderPositionYData").gameObject.GetComponent<Text>();
        BedHeigth = PrinterStats.transform.FindChild("BedPositionData").gameObject.GetComponent<Text>();

        SimulationDetails = GameObject.Find("SimulationDetails");

        TimeMultiplier = SimulationDetails.transform.FindChild("TimeMultiplierData").gameObject.GetComponent<Text>();
        File = SimulationDetails.transform.FindChild("FileData").gameObject.GetComponent<Text>();        
    }

    // Update is called once per frame
    void Update () {
        string format = "0.0";
        ExtruderTemperature.text = Printer.CurrentExtruderTemperature.ToString(format) + " °C";
        BedTemperature.text = Printer.CurrentBedTemperature.ToString(format) + " °C";
        FeedRate.text = Printer.FeedRatePerMinute.ToString("0") + " mm/min";
        ExtrusionPosition.text = Printer.CurrentPositionExtruder.ToString(format) + " mm";
        ExtruderXPosition.text = Printer.CurrentPosition.x.ToString(format) + " mm";
        ExtruderYPosition.text = Printer.CurrentPosition.z.ToString(format) + " mm";
        BedHeigth.text = Printer.CurrentPosition.y.ToString(format) + " mm";

        TimeMultiplier.text = Printer.TimeMultiplier.ToString(format) + " X";
        string file = Printer.GcodeFile;
        File.text = file.Substring(file.LastIndexOf('/') + 1);
    }
}
