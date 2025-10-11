using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cutscenes
{
    /// <summary>
    /// Visual novel command that fades in the scene or UI element.
    /// </summary>
    public class FadeInCommand : IVNCommand
    {
        /// <summary>
        /// Executes the fade-in command asynchronously.
        /// </summary>
        /// <param name="args">List of string arguments passed to the command.</param>
        /// <returns>Coroutine IEnumerator.</returns>
        public IEnumerator Execute(List<string> args)
        {
            // You can customize the fade logic here
            Debug.Log("Fading in...");
            yield return new WaitForSeconds(1f); // simulate fade
        }

        /// <summary>
        /// Registers the fade-in command with the command registry before the scene loads.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Register()
        {
            CommandRegistry.Register("fadeIn", () => new FadeInCommand());
        }
    }
}