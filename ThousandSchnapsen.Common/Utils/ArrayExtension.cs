using System;

namespace ThousandSchnapsen.Common.Utils
{
    public static class ArrayExtension
    {
        public static T[] Populate<T>(this T[] array, Func<T> provider)
        {
            for (var i = 0; i < array.Length; i++)
                array[i] = provider();
            return array;
        }
    }
}