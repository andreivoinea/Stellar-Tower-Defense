using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Entity : MonoBehaviour
{
    public string prefabName;
    public string projectilePrefabName;

    private GameObject prefab;
    public GameObject Prefab
    {
        get
        {
            if (prefab == null)
                prefab = (GameObject)Resources.Load("Prefabs/" + prefabName);

            return prefab;
        }
    }

    private  GameObject projectilePrefab;

    public GameObject ProjectilePrefab
    {
        get
        {
            if (projectilePrefab == null)
                projectilePrefab = (GameObject)Resources.Load("Prefabs/" + projectilePrefabName);

            return projectilePrefab;
        }
    }

    private float hitPoints = -1f;
    public float HitPoints
    {
        get { return hitPoints; }
        set
        {
            if (hitPoints == value) return;
            hitPoints = value;
        }
    }

    public float Theta
    {
        get
        {
            if (radius == Mathf.Infinity)
            {
                Debug.Log("radius is infinity");
                return Mathf.Infinity;
            }

            float negative;
            if (Mathf.Asin(transform.position.z / radius) < 0f) negative = -1;
            else negative = 1f;

            return Mathf.Acos(transform.position.x / radius) * negative;
        }

        set
        {
            if (radius == Mathf.Infinity) return;

            Point p = new Point(value, radius);

            if (velocity == Mathf.Infinity)
            {
                velocity = radius * Mathf.Sqrt(2) * Mathf.Sqrt(1 - Mathf.Cos(value - Theta));
            }

            transform.localPosition = new Vector3(p.x, transform.localPosition.y, p.y);
        }
    }

    public float speed = 1;
    public int fireRate = 1;
    public float damage = 1;

    public int price = -1;

    public int timeBetweenShots
    {
        get
        {
            return (int)Mathf.Ceil(1000 / fireRate);
        }
    }

    public float ReverseTheta(float theta)
    {
        if (theta >= 0f)
            return -Mathf.PI + theta;

        return Mathf.PI + theta;
    }

    public float AngleDifference
    {
        get
        {
            return 0.1f / radius;
        }
    }

    public float velocity = Mathf.Infinity;

    public float radius = Mathf.Infinity;

    public List<Entity> TargetList = new List<Entity>();

    public bool shootingState = false;
    public void Shoot()
    {
        if (TargetList.Count == 0) return;

        if (shootingState) return;

        shootingState = true;

        _ = AsyncShooting(TargetList[0]);
    }

    public async Task<bool> AsyncShooting(Entity target)
    {
        //InstantiateProjectiles();

        //int count = 0;
        /*
        if (count < projectileList.Count)
            projectileList[count].Shoot();
        ++count;*/

        if (target.HitPoints - damage < 0) TargetList.Remove(target);

        target.HitPoints -= damage;

        await Task.Delay(timeBetweenShots);

        if (!await GameManager.ApplicationStatus())
            return false;

        shootingState = false;

        return true;
    }

    public List<Projectile> projectileList;

    public void InstantiateProjectiles()
    {
        projectileList = new List<Projectile>(fireRate);

        GameObject projectile; Projectile script;

        int i = 0;
        while (i < fireRate)
        {
            projectile = Instantiate(ProjectilePrefab, transform);
            script = projectile.AddComponent<Projectile>();
            script.spawner = this;

            projectileList.Add(script);
            ++i;
        }
    }
}
