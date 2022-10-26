namespace AmpHelper
{
    /// <summary>
    /// Various information about the game.
    /// </summary>
    public static class AmplitudeData
    {
        /// <summary>
        /// PS3 header files start with this.
        /// </summary>
        public const uint PS3EncryptedVersion = 0xc64eed30;

        /// <summary>
        /// PS4 header files start with this.
        /// </summary>
        public const uint PS4EncryptedVersion = 0x6f303f55;

        /// <summary>
        /// The key used to decrypt the PS3 header file.
        /// </summary>
        public const int PS3Key = -967906000;  // 0xc64eed30 Unsigned

        /// <summary>
        /// The key used to decrypt the PS4 header file.
        /// </summary>
        public const int PS4Key = -1865432917; // 0x90cfc0ab Unsigned

        /// <summary>
        /// Extra data set in the PS3 header file entries.
        /// </summary>
        public const uint PS3Extra = 0x7D401F60;

        /// <summary>
        /// Extra data set in the PS4 header file entries.
        /// </summary>
        public const uint PS4Extra = 0xDDB682F0;

        /// <summary>
        /// All of the song folder names included with the base game.
        /// </summary>
        public static readonly string[] BaseSongs = new string[] {
            "allthetime", "assault_on", "astrosight", "breakforme", "concept", "crazy_ride", "credits", "crystal",
            "dalatecht", "decode_me", "digitalparalysis", "donot", "dreamer", "energize", "entomophobia", "forcequit",
            "humanlove", "impossible", "iseeyou", "lights", "magpie", "muze", "necrodancer", "perfectbrain",
            "phantoms", "recession", "redgiant", "supraspatial", "synthesized2014", "tut0", "tut1", "tutc",
            "unfinished", "wayfarer", "wetware"
        };
    }
}
