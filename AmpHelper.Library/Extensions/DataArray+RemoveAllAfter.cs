using DtxCS.DataTypes;
using System;
using System.Linq;

namespace AmpHelper.Library.Extensions
{
    internal static class DataArray_RemoveAllAfter
    {
        /// <summary>
        /// Removes all children after the specified index and returns the input <see cref="DataArray"/>.
        /// </summary>
        /// <param name="array">The input <see cref="DataArray"/>.</param>
        /// <param name="index">The last index to keep.</param>
        /// <returns></returns>
        public static DataArray RemoveAllAfter(this DataArray array, int index)
        {
            var keep = array.Children.Take(Math.Min(index + 1, array.Children.Count)).ToArray();
            array.Children.Clear();
            array.Children.AddRange(keep);

            return array;
        }
    }
}
