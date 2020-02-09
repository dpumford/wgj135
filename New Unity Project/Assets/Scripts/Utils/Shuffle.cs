using System;
using System.Collections.Generic;

public static class CollectionExtensions
   {
    private static Random rng = new Random();

    //FROM: https://stackoverflow.com/a/1262619
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
