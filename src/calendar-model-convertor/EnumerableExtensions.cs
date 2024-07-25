using System;
using System.Collections.Generic;
using System.Linq;

namespace Calendar.Model.Convertor
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var element in enumerable)
            {
                action(element);
            }
        }

        public static List<List<T>> Split<T>(this IEnumerable<T> collection, int maxLength)
        {
            if (maxLength < 1)
            {
                throw new ArgumentException("Invalid length.");
            }
            var result = new List<List<T>>();
            var list = collection.ToList();
            if (list.Count == 0) //optimization 1.
            {
                return result;
            }
            if (list.Count <= maxLength)  //optimization 2.
            {
                result.Add(list);
                return result;
            }
            for (var startIndex = 0; startIndex < list.Count;)
            {
                var remain = list.Count - startIndex;
                var count = remain > maxLength ? maxLength : remain;
                result.Add(list.GetRange(startIndex, count));
                startIndex += count;
            }
            return result;
        }

        public static IEnumerable<T> NotNullElements<T>(this IEnumerable<T> enumerable) where T : class
        {
            return enumerable.Where(element => element != null);
        }
    }
}