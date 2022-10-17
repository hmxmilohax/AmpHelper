using DtxCS.DataTypes;
using System.Collections.Generic;
using System.Linq;

namespace AmpHelper.Library.Extensions
{
    internal static class DataArray_FindByChild
    {
        public static IEnumerable<DataNode> FindArrayByChild(this DataArray dtx, string stringValue, int index = 0)
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
