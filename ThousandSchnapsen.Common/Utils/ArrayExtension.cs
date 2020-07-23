using System;

namespace ThousandSchnapsen.Common.Utils
{
    public static class ArrayExtension
    {
        public static T[] Populate<T>(this T[] array, Func<int, T> provider)
        {
            for (var i = 0; i < array.Length; i++)
                array[i] = provider(i);
            return array;
        }
    }
}