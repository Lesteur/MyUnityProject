using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Interface for asynchronous visual novel commands that yield control using IEnumerator.
/// </summary>
public interface IVNCommand
{
    /// <summary>
    /// Executes the command asynchronously.
    /// </summary>
    /// <param name="args">List of string arguments passed to the command.</param>
    /// <returns>Coroutine IEnumerator.</returns>
    IEnumerator Execute(List<string> args);
}

/// <summary>
/// Interface for synchronous visual novel commands that execute immediately.
/// </summary>
public interface IVNCommandSync
{
    /// <summary>
    /// Executes the command immediately without yielding.
    /// </summary>
    /// <param name="args">List of string arguments passed to the command.</param>
    void ExecuteImmediate(List<string> args);
}