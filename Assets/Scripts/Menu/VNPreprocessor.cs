using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class VNPreprocessor
{
    [MenuItem("VN/Compile Scripts")]
    public static void CompileAllScripts()
    {
        string sourceFolder = "Assets/Resources/VisualNovelScripts";
        string outputFolder = "Assets/Resources/JSONFiles/VisualNovelScripts";

        // Find all text files in the source folder and the subfolders
        foreach (string file in Directory.GetFiles(sourceFolder, "*.txt", SearchOption.AllDirectories))
        {
            string[] lines = File.ReadAllLines(file);
            List<SerializedCommand> commands = new();

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (!line.StartsWith("@")) continue;

                string[] parts = line.Substring(1).Split(' ', 2);
                string command = parts[0];
                string argLine = parts.Length > 1 ? parts[1] : "";

                List<string> args = ParseArguments(argLine);
                commands.Add(new SerializedCommand { command = command, args = args });
            }

            string json = JsonUtility.ToJson(new CommandListWrapper { commands = commands }, true);
            string outputName = Path.GetFileNameWithoutExtension(file) + ".json";
            File.WriteAllText(Path.Combine(outputFolder, outputName), json);
        }

        AssetDatabase.Refresh();
        Debug.Log("VN Scripts compiled.");
    }

    static List<string> ParseArguments(string input)
    {
        List<string> args = new();
        bool inQuotes = false;
        string current = "";

        foreach (char c in input)
        {
            if (c == '"') { inQuotes = !inQuotes; continue; }
            if (char.IsWhiteSpace(c) && !inQuotes)
            {
                if (!string.IsNullOrEmpty(current))
                {
                    args.Add(current);
                    current = "";
                }
            }
            else current += c;
        }

        if (!string.IsNullOrEmpty(current))
            args.Add(current);

        return args;
    }

    [System.Serializable]
    public class SerializedCommand
    {
        public string command;
        public List<string> args;
    }

    [System.Serializable]
    public class CommandListWrapper
    {
        public List<SerializedCommand> commands;
    }
}