using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisualNovel
{
    /// <summary>
    /// Interprets and executes commands from a script file.
    /// The script file should be in JSON format, containing a list of commands and their arguments.
    /// Each command can be synchronous or asynchronous.
    /// </summary>
    public class CommandInterpreter : Singleton<CommandInterpreter>
    {
        /// <summary>
        /// List of all actor instances available to commands.
        /// This is shared across all interpreters.
        /// </summary>
        public static List<Actor> Actors = new();

        /// <summary>
        /// JSON script file containing a list of serialized commands.
        /// This should be assigned in the Unity Inspector.
        /// </summary>
        [SerializeField] private TextAsset _scriptFile;

        /// <summary>
        /// List of parsed commands loaded from the script file.
        /// </summary>
        private List<SerializedCommand> _commandsList = new();

        /// <summary>
        /// Represents a single command from the script file, with its name and arguments.
        /// </summary>
        [Serializable]
        public class SerializedCommand
        {
            /// <summary>
            /// The keyword/name of the command.
            /// </summary>
            public string command;
            /// <summary>
            /// Arguments for the command.
            /// </summary>
            public List<string> args;
        }

        /// <summary>
        /// Wrapper for deserializing a list of commands from JSON.
        /// </summary>
        [Serializable]
        public class SerializedCommandList
        {
            /// <summary>
            /// The list of commands.
            /// </summary>
            public List<SerializedCommand> commands;
        }

        /// <summary>
        /// Unity lifecycle method.
        /// Called once when the GameObject is initialized.
        /// </summary>
        private void Start()
        {
            LoadScript(_scriptFile.text);
            StartCoroutine(RunScript());
        }

        /// <summary>
        /// Parses the JSON-formatted script file into a list of commands.
        /// </summary>
        /// <param name="text">JSON text of the script file.</param>
        private void LoadScript(string text)
        {
            SerializedCommandList list = JsonUtility.FromJson<SerializedCommandList>(text);
            if (list != null && list.commands != null)
            {
                _commandsList = list.commands;
            }
            else
            {
                Debug.LogWarning("Failed to parse script file or no commands found.");
                _commandsList = new List<SerializedCommand>();
            }
        }

        /// <summary>
        /// Executes all commands from the loaded script.
        /// Synchronous commands run immediately; asynchronous commands are awaited via coroutine.
        /// </summary>
        private IEnumerator RunScript()
        {
            foreach (var serializedCommand in _commandsList)
            {
                string keyword = serializedCommand.command;
                List<string> args = serializedCommand.args;

                IVNCommand command = CommandRegistry.GetCommand(keyword);

                if (command == null)
                {
                    Debug.LogWarning($"Command not found: {keyword}");
                    continue;
                }

                if (command is IVNCommandSync syncCommand)
                {
                    Debug.Log($"Executing synchronous command: {keyword}");
                    syncCommand.ExecuteImmediate(args);
                }
                else
                {
                    Debug.Log($"Executing coroutine command: {keyword}");
                    yield return StartCoroutine(command.Execute(args));
                }
            }
        }
    }
}