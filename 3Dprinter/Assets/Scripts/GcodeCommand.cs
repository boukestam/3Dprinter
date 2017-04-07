using System.Collections;
using System.Collections.Generic;

/// <summary>
///     This class stores a single gcode command.
/// </summary>
public class GcodeCommand {
    private int Type;
    private bool Valid = true;

    public float X = Gcodes.INVALID_NUMBER, Y = Gcodes.INVALID_NUMBER, Z = Gcodes.INVALID_NUMBER, E = Gcodes.INVALID_NUMBER, F = Gcodes.INVALID_NUMBER;

    /// <summary>
    ///     This constructor will convert a string of text into a GcodeCommand object if a single gcode command represents the text.
    /// </summary>
    /// <param name="text">The line of text representing a single gcode command. Example: "G1 X1.27 Y5.44 Z5.55 E0.44 F400.00"</param>
    public GcodeCommand(string text) {
        // Remove comments in the command
        int commentIndex = text.IndexOf(';');
        if (commentIndex > -1) {
            text = text.Substring(0, commentIndex);
        }

        // Skip empty lines
        text = text.Trim();
        if (text.Length <= 0) {
            Valid = false;
            return;
        }

        // Split individual sub-commands and save them into the dictionary.
        string[] subCommandsUnchanged = text.Split(' ');
        
        int addCount = 0;
        foreach (var subCommand in subCommandsUnchanged) {
            float number = 0;
            if (float.TryParse(subCommand.Substring(1), out number)) {
                if(addCount == 0) {
                    Type = (int)number;
                }

                if(subCommand[0] == 'X') {
                    X = number;
                }else if (subCommand[0] == 'Y') {
                    Y = number;
                } else if (subCommand[0] == 'Z') {
                    Z = number;
                } else if (subCommand[0] == 'E') {
                    E = number;
                } else if (subCommand[0] == 'F') {
                    F = number;
                }
                
                addCount++;
            } else {
                Valid = false;
                return;
            }
        }
        if(addCount == 0) {
            Valid = false;
        }
    }

    /// <summary>
    ///     Returns true if a valid GcodeCommand has been initialized.
    /// </summary>
    public bool IsValid() {
        return Valid;
    }

    /// <summary>
    ///     Returns the type of GcodeCommand. This type determines what type of action the gcode command is.
    /// </summary>
    public int GetCommandType() {
        return Type;
    }
}
