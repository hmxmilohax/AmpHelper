namespace AmpHelper.Library.Interfaces
{
    public interface ITweak
    {
        /// <summary>
        /// Sets the path to the unpacked game files
        /// </summary>
        /// <param name="path">Path to the unpacked game files</param>
        /// <returns>The current instance of the tweak</returns>
        ITweak SetPath(string path);

        /// <summary>
        /// Checks if the tweak is enabled
        /// </summary>
        /// <returns>The status of the tweak</returns>
        bool IsEnabled();

        /// <summary>
        /// Enables the tweak
        /// </summary>
        void EnableTweak();

        /// <summary>
        /// Disables the tweak
        /// </summary>
        void DisableTweak();
    }
}
