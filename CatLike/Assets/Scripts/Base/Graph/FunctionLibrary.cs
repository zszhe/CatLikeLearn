using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FunctionLibrary
{
    //public static GraphFunction[] graphFunctions =
    //  { SineFunction, Sine2DFunction, MultiSineFunction, MultiSine2DFunction, Ripple, Cylinder, Torus };
    public static GraphFunction[] graphFunctions =
     { SineFunction, MultiSineFunction, Ripple, Cylinder, Torus };
    const float pi = Mathf.PI;

    public static int functionCount => graphFunctions.Length;

    #region Test
    public static float Wave(float x , float t)
    {
        return Mathf.Sin(Mathf.PI * (x + t));
    }
    #endregion

    public static Vector3 Morph(float u, float v, float t, GraphFunction from, GraphFunction to, float progress)
    {
        return Vector3.LerpUnclamped(from(u, v, t), to(u, v, t), Mathf.SmoothStep(0f, 1f, progress));
    }

    public static GraphFunctionName GetRandomGraphFunctionName(GraphFunctionName name)
    {
        var choice = (GraphFunctionName)Random.Range(1, graphFunctions.Length);
        return choice == name ? 0 : choice;
    }

    public static GraphFunctionName GetGraphFunctionName(GraphFunctionName name)
    {
        return (int)name < graphFunctions.Length - 1 ? name + 1 : 0;
    }

    static Vector3 SineFunction(float x, float z, float t)
    {
        Vector3 point;
        point.x = x;
        point.z = z;
        point.y = Mathf.Sin(pi * (x + t));
        return point;
    }

    static Vector3 MultiSineFunction(float x, float z, float t)
    {
        Vector3 point;
        point.x = x;
        point.z = z;
        float y = Mathf.Sin(pi * (x + t));
        y += Mathf.Sin(2f * pi * (x + 2 * t)) / 2f;
        y *= 2 / 3f;
        point.y = y;
        return point;
    }

    static Vector3 Sine2DFunction(float x, float z, float t)
    {
        Vector3 point;
        point.x = x;
        point.z = z;
        point.y = Mathf.Sin(pi * (x + z + t));
        return point;
    }

    static Vector3 MultiSine2DFunction(float x, float z, float t)
    {
        Vector3 point;
        point.x = x;
        point.z = z;
        float y = 4f * Mathf.Sin(pi * (x + z + t * 0.5f));
        y += Mathf.Sin(pi * (x + t));
        y += Mathf.Sin(2f * pi * (z + 2f * t) * 0.5f);
        y *= 1f / 5.5f;
        point.y = y;
        return point;
    }

    static Vector3 Ripple(float x, float z, float t)
    {
        Vector3 point;
        point.x = x;
        point.z = z;
        float d = Mathf.Sqrt(x * x + z * z);
        float y = Mathf.Sin(pi * (4f * d - t));
        y /= 1 + 10 * d;
        point.y = y;
        return point;
    }

    static Vector3 Cylinder(float u, float v, float t)
    {
        float r = 0.9f + 0.1f * Mathf.Sin(pi * (6f * u + 4f * v + t));
        float s = r * Mathf.Cos(0.5f * pi * v);
        Vector3 point;
        point.x = s * Mathf.Sin(pi * u);
        point.y = r * Mathf.Sin(pi * 0.5f * v);
        point.z = s * Mathf.Cos(pi * u);
        return point;
    }

    static Vector3 Torus(float u, float v, float t)
    {
        float r1 = 0.7f + 0.1f * Mathf.Sin(pi * (6f * u + 0.5f * t));
        float r2 = 0.15f + 0.05f * Mathf.Sin(pi * (8f * u + 4f * v + 2f * t));
        float s = r1 + r2 * Mathf.Cos(pi * v);
        Vector3 point;
        point.x = s * Mathf.Sin(pi * u);
        point.y = r2 * Mathf.Sin(pi * v);
        point.z = s * Mathf.Cos(pi * u);
        return point;
    }
}
