using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Visual novel command that waits for a specified number of seconds before continuing.
/// </summary>
public class WaitSecondsCommand : IVNCommand
{
    /// <summary>
    /// Executes the wait seconds command asynchronously.
    /// </summary>
    /// <param name="args">List of string arguments passed to the command. First argument should be the number of seconds to wait.</param>
    /// <returns>Coroutine IEnumerator.</returns>
    public IEnumerator Execute(List<string> args)
    {
        Debug.Log("Wait...");

        if (args.Count == 0 || !float.TryParse(args[0], out float seconds))
        {
            Debug.LogError("WaitSecondsCommand: invalid argument");
            yield break;
        }

        yield return new WaitForSeconds(seconds);
    }

    /// <summary>
    /// Registers the wait seconds command with the command registry before the scene loads.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Register()
    {
        CommandRegistry.Register("waitSeconds", () => new WaitSecondsCommand());
    }
}