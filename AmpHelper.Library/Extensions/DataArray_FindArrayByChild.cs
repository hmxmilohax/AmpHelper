using DtxCS.DataTypes;
using System.Collections.Generic;
using System.Linq;

namespace AmpHelper.Extensions
{
    internal static class DataArray_FindArrayByChild
    {
        /// <summary>
        /// Finds <see cref="DataArray"/> objects with a child matching the string provided.
        /// </summary>
        /// <param name="dtx">The input <see cref="DataArray"/>.</param>
        /// <param name="stringValue">Value to match against.</param>
        /// <param name="index">The index of the child to check.</param>
        /// <returns></returns>
        public static IEnumerable<DataArray> FindArrayByChild(this DataArray dtx, string stringValue, int index = 0)
        {
            return dtx.Children.OfType<DataArray>().Where(child =>
            {
                if (index + 1 > child.Children.Count)
                {
                    return false;
                }

                if (child.Children[index].ToString() == stringValue)
                {
                    return true;
                }

                return false;
            });
        }
    }
}
