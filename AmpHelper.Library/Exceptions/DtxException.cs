using System;

namespace AmpHelper.Exceptions
{
    /// <summary>
    /// An exception describing a failure related to DTX handling.
    /// </summary>
    public class DtxException : Exception
    {
        public DtxException(string message) : base(message) { }
    }
}
