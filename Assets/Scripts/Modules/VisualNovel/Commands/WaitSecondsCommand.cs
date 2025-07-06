using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitSecondsCommand : IVNCommand
{
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

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Register()
    {
        CommandRegistry.Register("waitSeconds", () => new WaitSecondsCommand());
    }
}