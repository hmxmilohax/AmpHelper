using DtxCS.DataTypes;
using System;
using System.Linq;

namespace AmpHelper.Extensions
{
    internal static class DataArray_DeleteArraysByName
    {
        /// <summary>
        /// Deletes all <see cref="DataArray"/> nodes whos first child node string value matches one of the names provided.
        /// </summary>
        /// <param name="dtx">The input <see cref="DataArray"/>.</param>
        /// <param name="index">The index to check.</param>
        /// <param name="values">String values to check against.</param>
        public static void DeleteDataArraysByValue(this DataArray dtx, int index, params string[] values)
        {
            foreach (var child in dtx.Children.ToArray().OfType<DataArray>().Where(e => e.Children.Count >= index + 1 && values.Contains(e.Children[index].ToString())))
            {
                dtx.Children.Remove(child);
            }
        }

        public static void DeleteDataArraysByValue(this DataArray dtx, params string[] values) => dtx.DeleteDataArraysByValue(0, values);
    }
}
