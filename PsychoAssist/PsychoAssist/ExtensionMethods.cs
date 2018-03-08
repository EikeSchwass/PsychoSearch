using System.Collections.Generic;

namespace PsychoAssist
{
    public static class ExtensionMethods
    {
        public static bool ListEquals<T>(this object o, List<T> list1, List<T> list2)
        {
            if (list1.Count != list2.Count)
                return false;
            for (int i = 0; i < list1.Count; i++)
            {
                if (!Equals(list1[i], list2[i]))
                    return false;
            }

            return true;
        }

        public static bool SequentialEquals<T>(this IEnumerable<T> source)
        {
            T current = default(T);
            foreach (var next in source)
            {
                if (Equals(current, default(T)))
                {
                    current = next;
                    continue;
                }

                if (!Equals(current, next))
                    return false;
                current = next;
            }

            return true;
        }
    }
}