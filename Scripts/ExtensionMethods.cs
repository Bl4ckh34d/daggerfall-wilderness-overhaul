using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WildernessOverhaul
{
    public static class ExtensionMethods
    {
        public static bool In2DArrayBounds(this byte[,] array, int x, int y)
        {
            if (
                x < array.GetLowerBound(0) ||
                x > array.GetUpperBound(0) ||
                y < array.GetLowerBound(1) ||
                y > array.GetUpperBound(1))
            {
                return false;
            }
            return true;
        }
 
        public static bool InArrayBounds(this float[] array, int x)
        {
            if (
                x < array.GetLowerBound(0) ||
                x > array.GetUpperBound(0))
            {
                return false;
            }
            return true;
        }
    }
}
