using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point
{
    public float x;
    public float y;

    public Point(float theta, float radius)
    {
        x = radius * Mathf.Cos(theta); y = radius * Mathf.Sin(theta);
    }

    public Point()
    {
        x = Mathf.Infinity;
        y = Mathf.Infinity;
    }

    public bool isInstantiated()
    {
        if (x == Mathf.Infinity || y == Mathf.Infinity) return false;
        return true;
    }

    public float GetDistance(Point target)
    {
        float x2 = x - target.x;
        float y2 = y - target.y;

        x2 *= x2;
        y2 *= y2;

        return Mathf.Sqrt(x2 + y2);
    }

}
