using AmpHelper.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmpHelper.Interfaces
{
    /// <summary>
    /// The base interface for all tweaks.
    /// 
    /// Classes implementing this interface should add the <see cref="Attributes.TweakInfo"/> attribute.
    /// </summary>
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

        /// <summary>
        /// Finds all classes conforming to ITweak in the domain.
        /// </summary>
        /// <param name="domain">The domain to search, or the current domain if left null.</param>
        /// <returns></returns>
        public static IEnumerable<InstantiableTweakInfo> GetTweaks(AppDomain domain = null)
        {
            return (domain ?? AppDomain.CurrentDomain).GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(ITweak).IsAssignableFrom(p) && p.IsClass && !p.IsAbstract)
                .Select(tweak =>
                {
                    var info = (Attribute.GetCustomAttribute(tweak, typeof(TweakInfo)) as TweakInfo) ?? new TweakInfo(tweak.GetType().Name, tweak.GetType().Name);
                    return new InstantiableTweakInfo(info, tweak);
                }).OrderBy(t => t.Verb);
        }
    }
}
