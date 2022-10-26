namespace AmpHelper.Enums
{
    public enum UnlockRequirement
    {
        None,

        /// <summary>
        /// unlock_requirement_boss
        /// </summary>
        Boss,

        /// <summary>
        /// unlock_requirement_playcount
        /// </summary>
        PlayCount,

        /// <summary>
        /// unlock_requirement_world1
        /// </summary>
        World1,

        /// <summary>
        /// unlock_requirement_world2
        /// </summary>
        World2,

        /// <summary>
        /// unlock_requirement_world3
        /// </summary>
        World3,

        /// <summary>
        /// unlock_requirement_bonus
        /// </summary>
        Bonus,
        Unknown
    }

    /// <summary>
    /// Extensions for <see cref="UnlockRequirement"/>.
    /// </summary>
    public static class UnlockRequirement_Extensions
    {
        /// <summary>
        /// Converts <see cref="UnlockRequirement"/> to a string representing the symbol used in the game files.
        /// </summary>
        /// <param name="requirement"></param>
        /// <returns></returns>
        public static string ToSymbol(this UnlockRequirement requirement)
        {
            switch (requirement)
            {
                case UnlockRequirement.Boss:
                    return "unlock_requirement_boss";

                case UnlockRequirement.PlayCount:
                    return "unlock_requirement_playcount";

                case UnlockRequirement.World1:
                    return "unlock_requirement_world1";

                case UnlockRequirement.World2:
                    return "unlock_requirement_world2";

                case UnlockRequirement.World3:
                    return "unlock_requirement_world3";

                case UnlockRequirement.Bonus:
                    return "unlock_requirement_bonus";
            }

            return null;
        }
    }
}
