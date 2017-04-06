using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;

public class GcodeLoader : MonoBehaviour {

    private bool loadModelOnAwake = true;
    private bool forwardPassGcode = true;

    private bool modelLoaded = false;
    private List<Dictionary<char, float>> commands = new List<Dictionary<char, float>>();
    int commandoIndex = 0;

    void Awake() {
        if (loadModelOnAwake) {
            LoadGcode("Assets/Bunny.gcode");
        }
        commandoIndex = 0;
    }
	
	public bool NextGcodeCommand(Printer printer) {
        if (modelLoaded && forwardPassGcode) {
            if (commandoIndex == 0) { Debug.Log("GCODE_PASS##############################"); Debug.Log("START_PASS_GCODE!!!"); }
            if (commandoIndex >= commands.Count) {
                return false;
            }

            SendCommando(commands[commandoIndex], printer);
            commandoIndex++;
            if (commandoIndex >= commands.Count) { Debug.Log("DONE_PASS_CGODE!!!"); Debug.Log("GCODE_PASS##############################"); }
        }
        return true;
    }
    
    private void SendCommando(Dictionary<char, float> commando, Printer printer) {
        //string startCommand = commando[0].Key.ToString() + (int)commando[0].Value;
        float commandNumber = 0;
        if (!commando.TryGetValue('G', out commandNumber)) {
            commando.TryGetValue('M', out commandNumber);
        }
        switch ((int)commandNumber) {
            case Gcodes.MOVE0:
            case Gcodes.MOVE1:
                printer.Move(commando['X'], commando['Y'], commando['Z'], commando['E'], commando['F']);
                break;
            case Gcodes.HOME_AXIS:
                printer.HomeAllAxis();
                break;
            case Gcodes.SET_ABOSLUTE:
                //printer.HomeAllAxis();
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

    void LoadGcode(string filename) {
        long begin = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        LoadCommands(filename, commands);
        Debug.Log("LOAD_MODEL##############################");
        Debug.Log("LINE_COUNT:" + commands.Count);
        long end = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        Debug.Log("LOAD_TIME:" + (end - begin) + " ms");
        Debug.Log("LOAD_MODEL##############################");
    }

    private void LoadCommands(string filename, List<Dictionary<char, float>> newCommands) {
        newCommands.Clear();
        try {
            File.ReadAllLines(filename);
            StreamReader fileReader = new StreamReader(filename, Encoding.Default);
            using (fileReader) {
                string line;
                Dictionary<char, float> previousSubCommandsParsed = new Dictionary<char, float>();
                while ((line = fileReader.ReadLine()) != null) {
                    // Remove comments
                    int commentIndex = line.IndexOf(';');
                    if (commentIndex > -1) {
                        line = line.Substring(0, commentIndex);
                    }

                    // Skip empty lines
                    line = line.Trim();
                    if (line.Length <= 0) {
                        continue;
                    }

                    // Split individual sub-commands and save them
                    string[] subCommands = line.Split(' ');
                    Dictionary<char, float> subCommandsParsed = new Dictionary<char, float>(previousSubCommandsParsed);
                    foreach (var subCommand in subCommands) {
                        float number = 0;
                        if (float.TryParse(subCommand.Substring(1), out number)) {
                            subCommandsParsed[subCommand[0]] = number;
                        } else {
                            Debug.Log("REMOVED LINE:"+subCommand);
                            break;
                        }
                    }
                    if(subCommandsParsed.Count > 0) {
                        newCommands.Add(subCommandsParsed);
                        previousSubCommandsParsed = subCommandsParsed;
                    }
                }
                fileReader.Close();
            }
        } catch (Exception e) {
            Debug.Log(e.Message);
        }
        modelLoaded = true;
    }
}