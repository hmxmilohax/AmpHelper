using DtxCS;
using DtxCS.DataTypes;
using System;
using System.IO;

namespace AmpHelper.Library.Helpers
{
    public partial class HelperMethods
    {
        public static T DoWithDtbFile<T>(string file, Func<DataArray, T> action, bool rebuild = true)
        {
            var input = File.OpenRead(file);
            bool encrypted = false;
            int version = DTX.DtbVersion(input, ref encrypted);
            input.Position = 0;
            var dtx = DTX.FromDtb(input);
            input.Dispose();

            var returnValue = action(dtx);

            if (rebuild)
            {
                using (var output = File.Create(file))
                {
                    _ = DTX.ToDtb(dtx, output, version, encrypted);
                }
            }

            return returnValue;
        }
    }
}
