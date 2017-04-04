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
    private List<List<Tuple<char, float>>> commands = new List<List<Tuple<char, float>>>();
    int commandoIndex = 0;

    void Awake() {
        if (loadModelOnAwake) {
            LoadGcode("Assets/Bunny.gcode");
        }
        commandoIndex = 0;
    }
	
	public void NextGcodeCommand(Printer printer) {
        if (modelLoaded && forwardPassGcode) {
            if (commandoIndex == 0) { Debug.Log("GCODE_PASS##############################"); Debug.Log("START_PASS_GCODE!!!"); }
            if (commandoIndex >= commands.Count) {
                return;
            }

            SendCommando(commands[commandoIndex], printer);
            commandoIndex++;
            if (commandoIndex >= commands.Count) { Debug.Log("DONE_PASS_CGODE!!!"); Debug.Log("GCODE_PASS##############################"); }
        }
    }

    static List<WriteTuple<char, float>> fullParameters = new List<WriteTuple<char, float>>() { Gcodes.X, Gcodes.Y, Gcodes.Z, Gcodes.E, Gcodes.F };
    static List<WriteTuple<char, float>> positionParameters = new List<WriteTuple<char, float>>() { Gcodes.X, Gcodes.Y, Gcodes.Z };
    static List<WriteTuple<char, float>> setParameters = new List<WriteTuple<char, float>>() { Gcodes.S };
    private void SendCommando(List<Tuple<char, float>> commando, Printer printer) {
        string startCommand = commando[0].Key.ToString() + (int)commando[0].Value;
        switch (startCommand) {
            case Gcodes.MOVE0:
            case Gcodes.MOVE1:
                PopulateSettings(commando, fullParameters);
                printer.Move(Gcodes.X.Value, Gcodes.Y.Value, Gcodes.Z.Value, Gcodes.E.Value, Gcodes.F.Value);
                break;
            case Gcodes.HOME_AXIS:
                PopulateSettings(commando, positionParameters);
                printer.HomeAllAxis();
                break;
            case Gcodes.SET_ABOSLUTE:
                //printer.HomeAllAxis();
                break;
            case Gcodes.SET_CURRENT_POS:
                PopulateSettings(commando, fullParameters);
                break;
            case Gcodes.FAN_ON:
                PopulateSettings(commando, setParameters);
                break;
            case Gcodes.FAN_OFF:
                PopulateSettings(commando, setParameters);
                break;
            case Gcodes.SET_EXTRUDER_TEMP:
                PopulateSettings(commando, setParameters);
                break;
            case Gcodes.SET_BED_TEMP:
                PopulateSettings(commando, setParameters);
                break;
            default:
                //Debug.Log("Unknown command:"+ commando[0]);
                break;
        }
    }

    private void PopulateSettings(List<Tuple<char, float>> commando, List<WriteTuple<char, float>> targets) {
        foreach(var target in targets) {
            target.Value = Gcodes.INVALID_NUMBER;
            //string key = target.Key;
            foreach (var subCommando in commando) {
                if(subCommando.Key == target.Key) {
                    target.Value = subCommando.Value;
                    break;
                }
            }
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

    private void LoadCommands(string filename, List<List<Tuple<char, float>>> newCommands) {
        newCommands.Clear();
        try {
            StreamReader fileReader = new StreamReader(filename, Encoding.Default);
            using (fileReader) {
                string line;
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
                    List<Tuple<char, float>> subCommandsParsed = new List<Tuple<char, float>>();
                    foreach (var subCommand in subCommands) {
                        double number = 0;
                        if (double.TryParse(subCommand.Substring(1), out number)) {
                            subCommandsParsed.Add(new Tuple<char, float>(subCommand[0], (float)number));
                        } else {
                            Debug.Log("REMOVED LINE:"+subCommand);
                            break;
                        }
                    }
                    if(subCommandsParsed.Count > 0) {
                        newCommands.Add(subCommandsParsed);
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