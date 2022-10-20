using AmpHelper.Library.Extensions;
using DtxCS;
using DtxCS.DataTypes;
using System;
using System.IO;
using System.Text;

namespace AmpHelper.Library.Helpers
{
    internal class DtxStreamHelper<T> : IDisposable
    {
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

        public DtxStreamHelper<T> Run(bool dispose = false)
        {
            if (dispose)
            {
                Dispose();
            }

            ReturnValue = Func(Dtx);

            return this;
        }

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
