using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gap
{
    public GameObject instance;
    public AsteroidField field;
    public float radius;

    public float ExitRadius
    {
        get
        {
            if (radius != 3f) return radius - 0.5f;

            return 1.5f;
        }
    }

    public float Theta
    {
        get
        {
            float negative;
            if (Mathf.Asin(instance.transform.position.z / radius) < 0f) negative = -1;
            else negative = 1f;
            return Mathf.Acos(instance.transform.position.x / radius)* negative;
        }
    }

    public Gap(AsteroidField field,GameObject instance, float radius)
    {
        this.field = field;
        this.instance = instance;
        this.radius = radius;
    }
}
