using UnityEngine;
using System.Collections;

public static class FloatExtensions 
{
    public static float Squared(this float f)
    {
        f *= f;
        return f;
    }
}
