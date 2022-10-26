using System;

namespace AmpHelper.Extensions
{
    internal static class Object_Transform
    {
        /// <summary>
        /// Transforms an object with a transformer, or returns errorValue if an exception is encountered.
        /// </summary>
        /// <typeparam name="O"></typeparam>
        /// <param name="input"></param>
        /// <param name="transformer"></param>
        /// <param name="errorValue">Returned if transformer encounters an exception.</param>
        /// <returns></returns>
        public static O Transform<O>(this object input, Func<object, O> transformer, O errorValue = default(O))
        {
            try
            {
                return transformer.Invoke(input);
            }
            catch (Exception)
            {
                return errorValue;
            }
        }
    }
}
