using UnityEngine;
using System.Collections;

public static class ColorExtensions 
{
    public static Color Slerp(this Color a, Color b, float t)
    {
        return (HSBColor.Lerp(HSBColor.FromColor(a), HSBColor.FromColor(b), t)).ToColor();
    }

    public static void H(this Color c, int hue0to360)
    {
        HSBColor temp = HSBColor.FromColor(c);
        temp.h = (hue0to360 / 360.0f);
        c = HSBColor.ToColor(temp);
    }

    public static void Foo(this Color c, ref Color col)
    {
        col.b = 1.0f;
    }
    public static Color Bar(this Color c)
    {
        c.b = 1.0f;
        return c;
    }

    public static void H(this Color c, float hue0to1, ref Color thisColor)
    {
        HSBColor temp = HSBColor.FromColor(thisColor);
        temp.h = hue0to1;
        thisColor = HSBColor.ToColor(temp);
    }

    public static void S(this Color c, int saturation0to360, ref Color thisColor)
    {
        HSBColor temp = HSBColor.FromColor(thisColor);
        temp.s = saturation0to360 / 360.0f;
        thisColor = HSBColor.ToColor(temp);
    }

    public static void S(this Color c, float saturation0to1, ref Color thisColor)
    {
        HSBColor temp = HSBColor.FromColor(thisColor);
        temp.s = saturation0to1;
        thisColor = HSBColor.ToColor(temp);
    }

    public static void B(this Color c, int brightness0to360, ref Color thisColor)
    {
        HSBColor temp = HSBColor.FromColor(thisColor);
        temp.b = brightness0to360 / 360.0f;
        thisColor = HSBColor.ToColor(temp);
    }

    public static void B(this Color c, float brightness0to1, ref Color thisColor)
    {
        HSBColor temp = HSBColor.FromColor(thisColor);
        temp.b = brightness0to1;
        thisColor = HSBColor.ToColor(temp);
    }

    public static void V(this Color c, int value0to360, ref Color thisColor)
    {
        HSBColor temp = HSBColor.FromColor(thisColor);
        temp.b = value0to360 / 360.0f;
        thisColor = HSBColor.ToColor(temp);
    }

    public static void V(this Color c, float value0to1, ref Color thisColor)
    {
        HSBColor temp = HSBColor.FromColor(thisColor);
        temp.b = value0to1;
        thisColor = HSBColor.ToColor(temp);
    }
}
