﻿using System;

namespace AmpHelper.Attributes
{
    /// <summary>
    /// Describes a tweak.
    /// </summary>
    public class TweakInfo : Attribute
    {
        /// <summary>
        /// The name of the tweak
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// The verb used to activate the tweak
        /// </summary>
        public string Verb { get; protected set; }

        /// <summary>
        /// A description of the tweak
        /// </summary>
        public string Description { get; protected set; }

        /// <summary>
        /// Text used when the tweak is enabled
        /// </summary>
        public string EnableText { get; protected set; }

        /// <summary>
        /// Text used when the tweak is disabled
        /// </summary>
        public string DisableText { get; protected set; }

        /// <summary>
        /// Status text used when the tweak has been enabled
        /// </summary>
        public string EnabledText { get; protected set; }

        /// <summary>
        /// Status text used when the tweak has been disabled
        /// </summary>
        public string DisabledText { get; protected set; }

        /// <summary>
        /// Instantiates a TweakInfo object.
        /// </summary>
        /// <param name="Name">The name of the tweak</param>
        /// <param name="Verb">The verb used to activate the tweak</param>
        /// <param name="Description">A description of the tweak</param>
        /// <param name="EnableText">Text used when the tweak is enabled</param>
        /// <param name="DisableText">Text used when the tweak is disabled</param>
        /// <param name="EnabledText">Status text used when the tweak has been enabled</param>
        /// <param name="DisabledText">Status text used when the tweak has been disabled</param>
        public TweakInfo(string Name, string Verb, string Description = null, string EnableText = null, string DisableText = null, string EnabledText = null, string DisabledText = null)
        {
            this.Name = Name;
            this.Verb = Verb;
            this.Description = Description;
            this.EnableText = EnableText;
            this.DisableText = DisableText;
            this.EnabledText = EnabledText;
            this.DisabledText = DisabledText;
        }

        protected TweakInfo() { }
    }
}
