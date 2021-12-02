using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Projectile : MonoBehaviour
{
    public Entity spawner;

    public float Theta
    {
        set
        {
            Point p = new Point(value, Radius);

            transform.position = new Vector3(p.x, transform.position.y, p.y);
        }
    }

    private float radius = Mathf.Infinity;

    public float Radius
    {
        get
        {
            if (radius == Mathf.Infinity)
                radius = spawner.radius;

            return radius;
        }

        set
        {
            if (radius == value) return;
            radius = value;
        }
    }

    private float Velocity
    {
        get
        {
            return spawner.radius / Mathf.Ceil(spawner.timeBetweenShots / 10);
        }
    }

    private new Renderer renderer;

    public Renderer Renderer
    {
        get
        {
            if(renderer==null) renderer = GetComponent<Renderer>();
            return renderer;
        }
    }

    public void Shoot()
    {
        Renderer.enabled = true;
        _ = MoveTowardsTarget();

    }

    private bool ReturnToSpawner()
    {
        Renderer.enabled = false;
        projectileHit = false;
        transform.localPosition = new Vector3();

        return projectileHit;
    }

    int count = 0;
    private async Task<bool> MoveTowardsTarget()
    {
        if (projectileHit)
            return ReturnToSpawner();

        Theta = spawner.Theta;

        Radius -= Velocity;
        count++;

        await Task.Delay(10);

        _ = MoveTowardsTarget();

        return true;
    }

    private bool projectileHit = false;
    private void OnTriggerEnter(Collider other)
    {

        if(other.gameObject.tag == "Player")
        {
            projectileHit = true;
        }
    }


}
