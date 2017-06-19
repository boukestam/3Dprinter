using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     This class updates the values that are displayed in an printer process. 
///     Things like details about the printer (e.g. print needle location, temperature) and simulation details (e.g. print time speed) are updated here.
/// </summary>
public class UIController : MonoBehaviour {

    /// <param name="Printer">Object of the Printer that is used to request alot of settings of the printer (e.g. location, temperature).</param>
    private Printer Printer;

    /// <param name="PrinterDetails">GameObject containing the textfields on the screen displaying the details of the printer (e.g. printer location, feedrate, bed height).</param>
    private GameObject PrinterDetails;
    /// <param name="ExtruderTemperature">The text field for displaying the current extruder temperature.</param>
    private Text ExtruderTemperature;
    /// <param name="BedTemperature">The text field for displaying the current bed temperature.</param>
    private Text BedTemperature;
    /// <param name="FeedRate">The text field for displaying the current feedrate.</param>
    private Text FeedRate;
    /// <param name="ExtrusionPosition">The text field for displaying the current position of the extruder.</param>
    private Text ExtrusionPosition;
    /// <param name="ExtruderPositionX">The text field for displaying the current x location of the print needle.</param>
    private Text ExtruderPositionX;
    /// <param name="ExtruderPositionY">The text field for displaying the current y location of the print needle.</param>
    private Text ExtruderPositionY;
    /// <param name="BedHeight">The text field for displaying the current bed height.</param>
    private Text BedHeight;

    /// <param name="SimulationDetails">GameObject containing the textfields on the screen displaying the details of the simulation (e.g. filename, time speed of simulation).</param>
    GameObject SimulationDetails;
    /// <param name="TimeMultiplier">The text field for displaying the current time speed of the simulation.</param>
    private Text TimeMultiplier;
    /// <param name="File">The text field for displaying the current selected file to print from.</param>
    private Text File;

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
        File = SimulationDetails.transform.FindChild("FileData").gameObject.GetComponent<Text>();        
    }

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
        string temp = Printer.GcodeFile;
        File.text = temp.Substring(temp.LastIndexOf('/') + 1);
    }
}
