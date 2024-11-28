using System.Collections.Generic;
using System.Linq;

public static class ListExtension
{
    public static List<T> Shuffle<T>(this List<T> source)
    {
        System.Random rng = new System.Random();
        return source.OrderBy(item => rng.Next()).ToList();
    }
}