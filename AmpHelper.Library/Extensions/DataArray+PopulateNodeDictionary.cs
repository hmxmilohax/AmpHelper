using DtxCS.DataTypes;
using System.Collections.Generic;
using System.Linq;

namespace AmpHelper.Extensions
{
    internal static class DataArray_PopulateNodeDictionary
    {
        /// <summary>
        /// Creates or appends a <see cref="Dictionary{String, DataArray}"/> with <see cref="DataArray"/>s with the key being the first symbol in each data array.
        /// </summary>
        /// <param name="dtx"></param>
        /// <param name="dict"></param>
        /// <param name="basename"></param>
        /// <returns></returns>
        public static Dictionary<string, DataArray> PopulateNodeDictionary(this DataArray dtx, Dictionary<string, DataArray> dict = null, string basename = null)
        {
            if (dict == null)
            {
                dict = new Dictionary<string, DataArray>();
            }

            foreach (var child in dtx.Children.OfType<DataArray>().Where(e => e.Children.Count >= 1 && e.Children[0] is DataSymbol && e.Children[0].ToString().Length > 0))
            {
                if (dict.ContainsKey($"{basename}/"))
                {
                    continue;
                }

                var name = $"{basename}/{child.Children[0].ToString()}";

                PopulateNodeDictionary(child, dict, name);

                if (!dict.ContainsKey(name))
                {
                    dict.Add(name, child);
                }
            }

            if (string.IsNullOrEmpty(basename))
            {
                dict.Add($"{basename}/", dtx);
            }

            return dict;
        }
    }
}
