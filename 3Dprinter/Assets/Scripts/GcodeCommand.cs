using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class stores a single gcode command.
/// </summary>
public class GcodeCommand {
    [HideInInspector]
    /// <param name="X">This float is one of the possible parameters for a single G-code command.</param>
    public float X = GMcodes.InvalidNumber;
    /// <param name="Y">This float is one of the possible parameters for a single G-code command.</param>
    public float Y = GMcodes.InvalidNumber;
    /// <param name="Z">This float is one of the possible parameters for a single G-code command.</param>
    public float Z = GMcodes.InvalidNumber;
    /// <param name="E">This float is one of the possible parameters for a single G-code command.</param>
    public float E = GMcodes.InvalidNumber;
    /// <param name="F">This float is one of the possible parameters for a single G-code command.</param>
    public float F = GMcodes.InvalidNumber;
    /// <param name="S">This float is one of the possible parameters for a single G-code command.</param>
    public float S = GMcodes.InvalidNumber;

    /// <param name="Type">Contains the value identified the type of G-code command.</param>
    private int Type;

    /// <param name="Valid">Boolean that represenents true if this is a valid G-code command.</param>
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
        var subCommandsUnchanged = new List<string>(text.Split(' '));
        if(subCommandsUnchanged.Count == 1) {
            subCommandsUnchanged = NumberSplit(text);
        }

        int addCount = 0;
        foreach (var subCommand in subCommandsUnchanged) {
            float number = StringToFloat(subCommand.Substring(1));
            if(addCount == 0) {
                Type = (int)number;
            }
            else if(subCommand[0] == 'X') {
                X = number;
            }else if (subCommand[0] == 'Y') {
                Y = number;
            } else if (subCommand[0] == 'Z') {
                Z = number;
            } else if (subCommand[0] == 'E') {
                E = number;
            } else if (subCommand[0] == 'F') {
                F = number;
            } else if (subCommand[0] == 'S') {
                S = number;
            }
            addCount++;
        }
        if(addCount == 0) {
            Valid = false;
        }
    }

    /// <summary>
    ///     Returns true if a valid GcodeCommand has been initialized.
    /// </summary>
    /// <returns>The boolean containing if the GcodeCommand has been initialized if true, otherwise false.</returns>
    public bool IsValid() {
        return Valid;
    }

    /// <summary>
    ///     This function gives back the type of GcodeCommand.
    /// </summary>
    /// <returns>Returns the type of GcodeCommand. This type determines what type of action the gcode command is.</returns>
    public int GetCommandType() {
        return Type;
    }

    /// <summary>
    ///     An optimized string to float conversion method, without error checking.
    /// </summary>
    /// <param name="input">The string that will be converted to the float.</param>
    /// <returns>Returns the resulting float from the string input.</returns>
    private static float StringToFloat(string input) {
        float number = 0;
        int length = input.Length;
        int decimalPosition = length;
        bool negative = input[0] == '-';
        int begin = (negative ? 1 : 0);
        for (int i = begin; i < length; i++) {
            char character = input[i];

            if (character == '.') {
                decimalPosition = i + 1;
            } else {
                number = (number * 10) + (character - '0');
            }
        }

        float result = ((negative ? -1 : 1) * number) / Mathf.Pow(10, length - decimalPosition);
        return result;
    }

    /// <summary>
    ///     This function converts a string to a List of floating numbers that are still in string form.
    /// </summary>
    /// <param name="text">The input text that will be converted.</param>
    /// <returns>Returns the list of numbers that were split from the input text.</returns>
    private List<string> NumberSplit(string text) {
        var command = new List<string>();
        int start = 0;
        for (int i = 0; i < text.Length; i++) {
            if (char.IsUpper(text[i])) {
                if (start < i) {
                    command.Add(text.Substring(start, i - start).Trim());
                }
                start = i;
            }
        }
        command.Add(text.Substring(start, text.Length - start).Trim());

        return command;
    }
}
