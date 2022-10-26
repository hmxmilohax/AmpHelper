using AmpHelper.Extensions;
using DtxCS;
using DtxCS.DataTypes;
using System;
using System.IO;
using System.Text;

namespace AmpHelper.Helpers
{
    /// <summary>
    /// Makes working with dtx streams a little easier.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class DtxStreamHelper<T> : IDisposable
    {
        /// <summary>
        /// The value returned after invoking <see cref="Run(bool)"/>.
        /// </summary>
        public T ReturnValue { get; protected set; }
        protected Func<DataArray, T> Func;
        protected Stream DtxStream
        {
            get;
            set;
        }
        protected long InitialPosition { get; set; }
        protected DataArray Dtx { get; set; }
        protected bool Binary { get; set; }
        protected bool Encrypted { get; set; }
        protected int Version { get; set; }

        /// <summary>
        /// Initializes the class.
        /// </summary>
        /// <param name="stream">A seekable stream of dta or dtb data.</param>
        /// <param name="func">A function that is executed when <see cref="Run(bool)"/> is invoked.</param>
        public DtxStreamHelper(Stream stream, Func<DataArray, T> func)
        {
            Func = func;
            DtxStream = stream;
            bool encrypted = false;
            Binary = DtxStream.ReadByte() == 1;
            DtxStream.Position = 0;
            Encrypted = false;
            Version = Binary ? DTX.DtbVersion(DtxStream, ref encrypted) : 3;
            Encrypted = encrypted;
            DtxStream.Position = 0;
            Dtx = Binary ? DTX.FromDtb(DtxStream) : DTX.FromDtaStream(DtxStream);
            DtxStream.Position = InitialPosition;
        }

        /// <summary>
        /// Runs the function and returns the current instance for chaining.
        /// 
        /// The return value can be found on the <see cref="ReturnValue"/> property.
        /// </summary>
        /// <param name="dispose">Whether to dispose of the stream.  <see cref="Rebuild"/> can't be used if this is set to true.</param>
        /// <returns>The current instance for chaining.</returns>
        public DtxStreamHelper<T> Run(bool dispose = false)
        {
            if (dispose)
            {
                Dispose();
            }

            ReturnValue = Func(Dtx);

            return this;
        }

        /// <summary>
        /// Clears the stream and rebuilds it.
        /// </summary>
        /// <returns>The current instance for chaining.</returns>
        public DtxStreamHelper<T> Rebuild()
        {
            DtxStream.Position = 0;
            DtxStream.SetLength(0);

            if (Binary)
            {
                _ = DTX.ToDtb(Dtx, DtxStream, Version, Encrypted);
            }
            else
            {
                DtxStream.Write(Encoding.UTF8.GetBytes(Dtx.ToDta()));
            }

            return this;
        }

        public void Dispose() => DtxStream?.Dispose();

        /// <summary>
        /// Parses a dta or dtb stream, runs a function on it, and optionally rebuilds it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream">A seekable dta or dtb stream.</param>
        /// <param name="func">The function to invoke after parsing the stream.</param>
        /// <param name="rebuild">Rebuild the stream after.</param>
        /// <param name="dispose">Whether or not to dispose of the stream.</param>
        /// <returns></returns>
        public static T Run<T>(Stream stream, Func<DataArray, T> func, bool rebuild = false, bool dispose = false)
        {
            var helper = new DtxStreamHelper<T>(stream, func);

            helper.Run();

            if (rebuild)
            {
                helper.Rebuild();
            }

            if (dispose)
            {
                helper.Dispose();
            }

            return helper.ReturnValue;
        }
    }
}
