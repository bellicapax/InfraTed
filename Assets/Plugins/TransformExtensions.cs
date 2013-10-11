using UnityEngine;
using System.Collections;

public static class TransformExtensions 
{
    public static void XRotation(this Transform t, float eulerX)
    {
        t.rotation = Quaternion.Euler(new Vector3(eulerX, t.rotation.eulerAngles.y, t.rotation.eulerAngles.z));
    }

    public static void YRotation(this Transform t, float eulerY)
    {
        t.rotation = Quaternion.Euler(new Vector3(t.rotation.eulerAngles.x, eulerY, t.rotation.eulerAngles.z));
    }

    public static void ZRotation(this Transform t, float eulerZ)
    {
        t.rotation = Quaternion.Euler(new Vector3(t.rotation.eulerAngles.x, t.rotation.eulerAngles.y, eulerZ));
    }

    public static void XPosition(this Transform t, float x)
    {
        t.position = new Vector3(x, t.position.y, t.position.z);
    }

    public static void YPosition(this Transform t, float y)
    {
        t.position = new Vector3(t.position.x, y, t.position.z);
    }

    public static void ZPosition(this Transform t, float z)
    {
        t.position = new Vector3(t.position.x, t.position.y, z);
    }
}
