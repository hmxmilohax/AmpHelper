using AmpHelper.Library.Delegates;
using DtxCS.DataTypes;
using System;

namespace AmpHelper.Library.Extensions
{
    internal static class DataArray_GetChild
    {
        /// <summary>
        /// Gets a child node, or null if it doesn't exist.
        /// </summary>
        /// <param name="dtx">The input <see cref="DataArray"/>.</param>
        /// <param name="index">The index of the child node.</param>
        /// <returns></returns>
        public static DataNode GetChild(this DataArray dtx, UInt16 index)
        {
            if (dtx.Children.Count >= index + 1)
            {
                return dtx.Children[index];
            }

            return null;
        }

        /// <summary>
        /// Gets a child node as the specified type, or null if failure.
        /// </summary>
        /// <typeparam name="T">String or a subclass of <see cref="DataNode"/>DataNode.</typeparam>
        /// <param name="dtx">The input <see cref="DataArray"/>.</param>
        /// <param name="index">The index of the child node.</param>
        /// <returns></returns>
        public static T GetChild<T>(this DataArray dtx, UInt16 index)
        {
            var child = dtx.GetChild(index);

            if (child == null)
            {
                return default(T);
            }

            if (typeof(T) == typeof(string))
            {
                if (child.GetType() == typeof(DataAtom) && (child as DataAtom).Type == DataType.STRING)
                {
                    return (T)Convert.ChangeType((child as DataAtom).String, typeof(T));
                }
                return (T)Convert.ChangeType(child.ToString(), typeof(T));
            }

            try
            {
                return (T)Convert.ChangeType(child, typeof(T));
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        /// <summary>
        /// Gets a child node as the specified type while making use of a transformation function, or null if the function fails.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dtx">The input <see cref="DataArray"/>.</param>
        /// <param name="index">The index of the child node.</param>
        /// <param name="transformer">A function to transform the <see cref="DataNode"/> into the desired type.</param>
        /// <param name="transformerDefault"></param>
        /// <returns></returns>
        public static T GetChild<T>(this DataArray dtx, UInt16 index, DataNodeTransformer<T> transformer, T transformerDefault = default(T))
        {
            var child = dtx.GetChild(index);

            if (child == null)
            {
                return default(T);
            }

            try
            {
                return transformer.Invoke(child);
            }
            catch (Exception)
            {
                return transformerDefault;
            }
        }
    }
}
