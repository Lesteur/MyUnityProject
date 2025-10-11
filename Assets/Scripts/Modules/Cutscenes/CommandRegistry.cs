using System;
using System.Collections.Generic;

namespace Cutscenes
{
    /// <summary>
    /// A static registry for associating string keywords with IVNCommand constructors.
    /// Used to retrieve command instances dynamically at runtime.
    /// </summary>
    public static class CommandRegistry
    {
        /// <summary>
        /// Maps command keywords to functions that construct IVNCommand instances.
        /// </summary>
        private static Dictionary<string, Func<IVNCommand>> commandMap = new Dictionary<string, Func<IVNCommand>>();

        /// <summary>
        /// Registers a command with a keyword and a constructor function.
        /// </summary>
        /// <param name="keyword">The string keyword for the command.</param>
        /// <param name="constructor">A function that constructs the command instance.</param>
        public static void Register(string keyword, Func<IVNCommand> constructor)
        {
            if (!commandMap.ContainsKey(keyword))
            {
                commandMap.Add(keyword, constructor);
            }
        }

        /// <summary>
        /// Retrieves a new command instance associated with the given keyword.
        /// </summary>
        /// <param name="keyword">The command keyword.</param>
        /// <returns>An instance of the command.</returns>
        /// <exception cref="Exception">Thrown when the command keyword is not registered.</exception>
        public static IVNCommand GetCommand(string keyword)
        {
            if (commandMap.TryGetValue(keyword, out var constructor))
            {
                return constructor();
            }

            throw new Exception($"Unknown command keyword: {keyword}");
        }
    }
}