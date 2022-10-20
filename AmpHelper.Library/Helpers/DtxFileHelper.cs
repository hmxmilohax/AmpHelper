using DtxCS.DataTypes;
using System;
using System.IO;

namespace AmpHelper.Library.Helpers
{
    internal class DtxFileHelper<T> : DtxStreamHelper<T>
    {
        protected string Filename { get; set; }
        public DtxFileHelper(string file, Func<DataArray, T> func) : base(File.OpenRead(file), func)
        {
            Filename = file;
            DtxStream.Dispose();
            DtxStream = null;
        }

        public new DtxFileHelper<T> Run(bool dispose = false)
        {
            base.Run(dispose);

            return this;
        }

        public new DtxFileHelper<T> Rebuild()
        {
            base.DtxStream = File.Create(Filename);

            base.Rebuild();

            DtxStream.Dispose();
            DtxStream = null;

            return this;
        }

        public static T Run<T>(string file, Func<DataArray, T> func, bool rebuild = false, bool dispose = false)
        {
            var helper = new DtxFileHelper<T>(file, func);

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
