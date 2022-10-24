using DtxCS.DataTypes;
using System;
using System.Linq;

namespace AmpHelper.Library.Extensions
{
    internal static class DataArray_DeleteArraysByName
    {
        /// <summary>
        /// Deletes all <see cref="DataArray"/> nodes whos first child node string value matches one of the names provided.
        /// </summary>
        /// <param name="dtx">The input <see cref="DataArray"/>.</param>
        /// <param name="values">String values to check against.</param>
        public static void DeleteDataArraysByNValue(this DataArray dtx, params string[] values)
        {
            var children = dtx.Children.ToArray();

            dtx.Children.Clear();
            dtx.Children.AddRange(children.Where(child =>
            {
                if (child.GetType() == typeof(DataArray) && (child as DataArray).Children.Count >= 1 && values.Contains((child as DataArray).Children[0].ToString()))
                {
                    return false;
                }

                return true;
            }));
        }
    }
}
