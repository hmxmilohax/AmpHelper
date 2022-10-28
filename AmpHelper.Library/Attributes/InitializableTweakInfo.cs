using AmpHelper.Interfaces;
using System;

namespace AmpHelper.Attributes
{
    /// <summary>
    /// TweakInfo, but with a <see cref="CreateInstance(string)"/> function.
    /// </summary>
    public class InstantiableTweakInfo : TweakInfo
    {
        /// <summary>
        /// The tweak type to create an instance of.
        /// </summary>
        public Type TweakType { get; protected set; }

        /// <summary>
        /// Copies a TweakInfo instance to a new one while setting the TweakType.
        /// </summary>
        /// <param name="Info"></param>
        /// <param name="TweakType"></param>
        internal InstantiableTweakInfo(TweakInfo Info, Type TweakType) : base()
        {
            if (!typeof(ITweak).IsAssignableFrom(TweakType) || !TweakType.IsClass || TweakType.IsAbstract)
            {
                throw new ArgumentException($"{nameof(TweakType)} is not a valid class implementing ITweak.");
            }

            base.Name = Info.Name;
            base.Verb = Info.Verb;
            base.Description = Info.Description;
            base.EnableText = Info.EnableText;
            base.DisableText = Info.DisableText;
            base.EnabledText = Info.EnabledText;
            base.DisabledText = Info.DisabledText;
            this.TweakType = TweakType;
        }

        /// <summary>
        /// Creates an instance of the tweak
        /// </summary>
        /// <param name="unpackedPath"></param>
        /// <returns></returns>
        public ITweak CreateInstance(string unpackedPath)
        {
            ITweak tweak = Activator.CreateInstance(TweakType) as ITweak;

            tweak?.SetPath(unpackedPath);

            return tweak;
        }
    }
}
