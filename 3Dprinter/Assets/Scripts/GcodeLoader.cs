using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;

/// <summary>
///     The GcodeLoader loads gcode from a file into memory. It is also used to loop through the gcode commands to let the Printer class execute the commands.
/// </summary>
public class GcodeLoader : MonoBehaviour {

    /// <param name="ModelLoaded">Boolean representing if a model has already been loaded into the gcode.</param>
    private bool ModelLoaded = false;

    /// <param name="Commands">A list containing all gcode commands.</param>
    private List<GcodeCommand> Commands = new List<GcodeCommand>();

    /// <param name="CommandsIndex">The current index of the last read gcode command. Used for the NextGcodeCommand function.</param>
    int CommandsIndex = 0;

    void Awake() {
        LoadGcode("Assets/Bunny.gcode", Commands);
    }

    /// <summary>
    ///     Returns true if the end of the gcode has been reached.
    /// </summary>
    private bool EndOfGcode() {
        return CommandsIndex >= Commands.Count;
    }

    /// <summary>
    ///     Handles moving to the new Gcode command and then calls another function to execute the command for the Printer object.
    /// </summary>
    /// <param name="printer">The printer object that requested a new command.</param>
	public bool NextGcodeCommand(Printer printer) {
        if (EndOfGcode() || ModelLoaded == false) {
            return false;
        }
        ExecuteCommand(Commands[CommandsIndex++], printer);
        return true;
    }

    /// <summary>
    ///     Determines the command to execute and then calls the matching function with parameters to the Printer object.
    /// </summary>
    /// <param name="command">The command object that will contain all information of the command.</param>
    /// // <param name="printer">The printer object that requested a new command.</param>
    private void ExecuteCommand(GcodeCommand command, Printer printer) {
        //string startCommand = commando[0].Key.ToString() + (int)commando[0].Value;
        switch (command.GetCommandType()) {
            case Gcodes.MOVE0:
            case Gcodes.MOVE1:
                printer.Move(command.Get('X'), command.Get('Y'), command.Get('Z'), command.Get('E'), command.Get('F'));
                break;
            case Gcodes.HOME_AXIS:
                printer.HomeAllAxis();
                break;
            case Gcodes.SET_ABOSLUTE:
                break;
            case Gcodes.SET_CURRENT_POS:
                break;
            case Gcodes.FAN_ON:
                break;
            case Gcodes.FAN_OFF:
                break;
            case Gcodes.SET_EXTRUDER_TEMP:
                break;
            case Gcodes.SET_BED_TEMP:
                break;
            default:
                break;
        }
    }

    /// <summary>
    ///     Loads all gcode from a file into memory and links all variables in the gcode into a structured class.
    /// </summary>
    /// <param name="filename">The name of the file where to load the gcode from.</param>
    /// // <param name="newCommands">The list where all gcode commands will be stored in.</param>
    private void LoadGcode(string filename, List<GcodeCommand> newCommands) {
        long beginTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        newCommands.Clear();
        ModelLoaded = false;
        try {
            StreamReader fileReader = new StreamReader(filename, Encoding.Default);
            using (fileReader) {
                string line;

                while ((line = fileReader.ReadLine()) != null) {
                    GcodeCommand command = new GcodeCommand(line);
                    if (command.IsValid()) {
                        newCommands.Add(command);
                    } else {
                        command = null;
                    }
                }
                fileReader.Close();
            }
        } catch (Exception e) {
            Debug.Log(e.Message);
        }
        ModelLoaded = true;

        long endTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        Debug.Log("GCODE_LOADER (LOAD_TIME:" + (endTime - beginTime) + "ms, LINES:" + Commands.Count + ")" );
    }
}