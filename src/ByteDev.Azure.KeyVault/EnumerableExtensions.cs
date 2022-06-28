using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ByteDev.Azure.KeyVault
{
    internal static class EnumerableExtensions
    {
        public static Dictionary<string, string> ToDictionary(this IEnumerable<Tuple<string, Task<string>>> source)
        {
            var dictionary = new Dictionary<string, string>();

            if (source == null)
                return dictionary;

            foreach (var nameTask in source)
            {
                dictionary.Add(nameTask.Item1, nameTask.Item2.Result);
            }

            return dictionary;
        }
    }
}