using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInCommand : IVNCommand
{
    public IEnumerator Execute(List<string> args)
    {
        // You can customize the fade logic here
        Debug.Log("Fading in...");
        yield return new WaitForSeconds(1f); // simulate fade
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Register()
    {
        CommandRegistry.Register("fadeIn", () => new FadeInCommand());
    }
}