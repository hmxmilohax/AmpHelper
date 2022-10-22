using DtxCS.DataTypes;
using System;
using System.IO;

namespace AmpHelper.Library.Helpers
{
    /// <summary>
    /// Makes working with dtx streams a little easier.
    /// </summary>
    /// <typeparam name="T">The return type of <see cref="DtxFileHelper{T}.Run(bool)"/></typeparam>
    internal class DtxFileHelper<T> : DtxStreamHelper<T>
    {
        /// <summary>
        /// The value returned after invoking <see cref="Run(bool)"/>.
        /// </summary>
        public new T ReturnValue
        {
            get => base.ReturnValue;
            protected set => base.ReturnValue = value;
        }
        protected string Filename { get; set; }

        /// <summary>
        /// Initializes the class.
        /// </summary>
        /// <param name="file">Path to the dta or dtb file.</param>
        /// <param name="func">A function that is executed when <see cref="Run(bool)"/> is invoked.</param>
        public DtxFileHelper(string file, Func<DataArray, T> func) : base(File.OpenRead(file), func)
        {
            Filename = file;
            DtxStream.Dispose();
            DtxStream = null;
        }

        /// <summary>
        /// Runs the function and returns the current instance for chaining.
        /// 
        /// The return value can be found on the <see cref="ReturnValue"/> property.
        /// </summary>
        /// <param name="dispose">Only exists to hide <see cref="DtxStreamHelper{T}.Run(bool)"/>.</param>
        /// <returns>The current instance for chaining.</returns>
        public new DtxFileHelper<T> Run(bool dispose = false)
        {
            base.Run(dispose);

            return this;
        }

        /// <summary>
        /// Rebuilds the dtb file and writes it to the original location.
        /// </summary>
        /// <returns>The current instance for chaining.</returns>
        public new DtxFileHelper<T> Rebuild()
        {
            base.DtxStream = File.Create(Filename);

            base.Rebuild();

            DtxStream.Dispose();
            DtxStream = null;

            return this;
        }

        /// <summary>
        /// Opens and parses a dta or dtb file, runs a function on it, and optionally rebuilds the file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file">The source file</param>
        /// <param name="func">The function to call after parsing the file.</param>
        /// <param name="rebuild">Rebuild the file after</param>
        /// <returns></returns>
        public static T Run<T>(string file, Func<DataArray, T> func, bool rebuild = false)
        {
            var helper = new DtxFileHelper<T>(file, func);

            helper.Run();

            if (rebuild)
            {
                helper.Rebuild();
            }

            return helper.ReturnValue;
        }
    }
}
