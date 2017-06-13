using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;
using UnityEditor;

/// <summary>
///     The GcodeLoader loads gcode from a file into memory. It is also used to loop through the gcode commands to let the Printer class execute the commands.
/// </summary>
public class GcodeLoader : MonoBehaviour {

    /// <param name="ModelLoaded">Boolean representing if a model has already been loaded into the gcode.</param>
    public bool ModelLoaded = false;

    /// <param name="Commands">A list containing all gcode commands.</param>
    private List<GcodeCommand> Commands = new List<GcodeCommand>();

    /// <param name="CommandsIndex">The current index of the last read gcode command. Used for the NextGcodeCommand function.</param>
    private int CommandsIndex = 0;

    void Awake() {
    }

    /// <summary>
    ///     Handles moving to the new Gcode command and then calls another function to execute the command for the Printer object.
    /// </summary>
    /// <param name="printer">The printer object that requested a new command.</param>
	public bool ExecuteNextCommand(Printer printer) {
        if (EndOfGcode() || ModelLoaded == false) {
            return false;
        }
        ExecuteCommand(Commands[CommandsIndex++], printer);
        return true;
    }

    /// <summary>
    ///     Returns true if the end of the gcode has been reached.
    /// </summary>
    private bool EndOfGcode() {
        return CommandsIndex >= Commands.Count;
    }

    /// <summary>
    ///     Determines the command to execute and then calls the matching function with parameters to the Printer object.
    /// </summary>
    /// <param name="command">The command object that will contain all information of the command.</param>
    /// // <param name="printer">The printer object that requested a new command.</param>
    private void ExecuteCommand(GcodeCommand command, Printer printer) {
        //string startCommand = commando[0].Key.ToString() + (int)commando[0].Value;
        switch (command.GetCommandType()) {
            case GMcodes.Move0:
            case GMcodes.Move1:
                printer.Move(command.X, command.Y, command.Z, command.E, command.F);
                break;
            case GMcodes.HomeAxis:
                printer.HomeAllAxis();
                break;
            case GMcodes.SetAbsolute:
                break;
            case GMcodes.SetCurrentPosition:
                break;
            case GMcodes.FanOn:
                break;
            case GMcodes.FanOff:
                break;
            case GMcodes.SetExtruderTemperature1:
            case GMcodes.SetExtruderTemperature2:
                printer.SetExtruderTemperature(command.S);
                break;
            case GMcodes.SetBedTemperature1:
            case GMcodes.SetBedTemperature2:
                printer.SetBedTemperature(command.S);
                break;
            default:
                break;
        }
    }

    /// <summary>
    ///     Loads all gcode from a file into memory and links all variables in the gcode into a structured class.
    /// </summary>
    /// <param name="filename">The name of the file where to load the gcode from.</param>
    public void Load(string filename) {
        Load(filename, Commands);
    }

    /// <summary>
    ///     Loads all gcode from a file into memory and links all variables in the gcode into a structured class.
    /// </summary>
    /// <param name="filename">The name of the file where to load the gcode from.</param>
    /// <param name="newCommands">The list where all gcode commands will be stored in.</param>
    private void Load(string filename, List<GcodeCommand> newCommands) {
        long beginTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        newCommands.Clear();
        ModelLoaded = false;

        var filestream = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
        var fileReader = new System.IO.StreamReader(filestream, System.Text.Encoding.UTF8, true, 128);
        string line;
        while ((line = fileReader.ReadLine()) != null) {
            var command = new GcodeCommand(line);
            if (command.IsValid()) {
                newCommands.Add(command);
            } else {
                command = null;
            }
        }
        ModelLoaded = true;

        long endTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        Debug.Log("GCODE_LOADER (LOAD_TIME:" + (endTime - beginTime) + "ms, LINES:" + Commands.Count + ")" );
    }
}