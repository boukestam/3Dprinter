using System.Collections;
using System.Collections.Generic;

/// <summary>
///     This class stores a single gcode command.
/// </summary>
public class GcodeCommand {
    private int Type;
    private Dictionary<char, float> SubCommands;
    private bool Valid = true;

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
        SubCommands = new Dictionary<char, float>() { { 'X', Gcodes.INVALID_NUMBER }, { 'Y', Gcodes.INVALID_NUMBER }, { 'Z', Gcodes.INVALID_NUMBER }, { 'E', Gcodes.INVALID_NUMBER }, { 'F', Gcodes.INVALID_NUMBER }, { 'S', Gcodes.INVALID_NUMBER } };
        int addCount = 0;
        foreach (var subCommand in subCommandsUnchanged) {
            float number = 0;
            if (float.TryParse(subCommand.Substring(1), out number)) {
                if(addCount == 0) {
                    Type = (int)number;
                }
                SubCommands[subCommand[0]] = number;
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

    /// <summary>
    ///     Returns a float matching the giving key in the GcodeCommand.
    ///     For example in the GcodeCommand "G1 X1.5454 Y5.64", using Get('Y') returns 5.64.
    /// </summary>
    /// <param name="key">The key matching the value of the subpart of the gcommand.</param>
    public float Get(char key) {
        return SubCommands[key];
    }
}
