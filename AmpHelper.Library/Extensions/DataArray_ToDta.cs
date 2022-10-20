using DtxCS.DataTypes;
using System.Text;

namespace AmpHelper.Library.Extensions
{
    public static class DataArray_ToDta
    {
        public static string ToDta(this DataArray array)
        {
            var sb = new StringBuilder();

            foreach (var child in array.Children)
            {
                sb.AppendLine(child.ToString(0));
            }

            return sb.ToString();
        }
    }
}
