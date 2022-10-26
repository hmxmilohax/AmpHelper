namespace AmpHelper.Delegates
{
    /// <summary>
    /// Desribed an action that receives a message and progress values.
    /// </summary>
    /// <param name="message">Describes the current operation, can be null.</param>
    /// <param name="current">The current progress value.</param>
    /// <param name="total">The maximum progress value.</param>
    public delegate void ProgressAction(string message, long current, long total);
}
