using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bin2Coe
{
    public static class ListExtensions
    {
        public static IEnumerable<IEnumerable<T>> SplitIntoSets<T>
    (this IEnumerable<T> source, int itemsPerSet)
    {
        var sourceList = source as List<T> ?? source.ToList();
        for (var index = 0; index < sourceList.Count; index += itemsPerSet)
        {
            yield return sourceList.Skip(index).Take(itemsPerSet);
        }
    }
}
}
