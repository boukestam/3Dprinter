using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;

public class GcodeLoader : MonoBehaviour {

    private List<string[]> commands = new List<string[]>();
    int commandoIndex = 0;

    void Awake() {
        //LoadCommands("Assets/Bunny.gcode", commands);
        //Debug.Log(commands.Count);
        //commandoIndex = 0;
    }
	
	void Update () {
        /*if (IsFree()) {
            SendCommando(commands[commandoIndex]);
            commandoIndex++;
        }*/
	}

    private void SendCommando(string[] commando) {
        switch (commando[0]) {
            case Gcodes.MOVE0:
            case Gcodes.MOVE1:
                PopulateSettings(commando, new List<Tuple<string, float>>() { Gcodes.X, Gcodes.Y, Gcodes.Z, Gcodes.E, Gcodes.F });
                break;

        }
    }

    private void PopulateSettings(string[] commando, List<Tuple<string, float>> targets) {
        foreach(var target in targets) {
            foreach(var subCommando in commando) {
                if (subCommando.StartsWith(target.Key)) {
                    //target.Value = ;
                    break;
                }
                
            }
        }
    }

    private bool IsFree() {
        return true;
    }

    private void LoadCommands(string filename, List<string[]> newCommands) {
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
                    if (subCommands.Length > 0) {
                        newCommands.Add(subCommands);
                    }
                }
                fileReader.Close();
            }
        } catch (Exception e) {
            Debug.Log(e.Message);
        }
    }
}