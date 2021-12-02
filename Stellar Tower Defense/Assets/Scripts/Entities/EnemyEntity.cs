using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEntity : Entity
{
    public static PlayerEntity Player { get { return PlayerEntity.Instance; } }

    private Gap target;

    public Gap Target
    {
        get { return target; }
        set
        {
            if (target == value) return;
            target = value;

            if(target == null)
            {
                direction = Direction.Base;
                return;
            }

            if (Theta * target.Theta > 0f)
            {
                if (Theta > target.Theta)
                    direction = Direction.Right;
                else direction = Direction.Left;
            }
            else
            {
                if (Theta < ReverseTheta(target.Theta))
                    direction = Direction.Right;
                else direction = Direction.Left;
            }
        }
    }

    public void Update()
    {
        if (!loaded) return;
        if (GameManager.applicationPaused) return;

        Death();
        Move();
        Shoot();
    }

    private enum Direction { Left, Right, Null, Passing,Base }
    private Direction direction = Direction.Null;

    private void Move()
    {
        switch (direction)
        {
            case Direction.Null:
                GetTarget(); return;
            case Direction.Passing:
                PassThroughGap();return;
            case Direction.Base:
                TargetList.Add(Player);
                break;
            default:
                break;
        }

        Theta += AngleDifference * speed * (direction == Direction.Left ? 1f : -1f);

        if (target == null) transform.LookAt(Player.transform);
        else if (Mathf.Abs(Theta - target.Theta) < 0.01f) PassThroughGap();
    }

    private void GetTarget()
    {
        Target = GameManager.Instance.GetClosestGap(this);
    }

    private void PassThroughGap()
    {
        if (direction != Direction.Passing)
        {
            direction = Direction.Passing;
            Theta = target.Theta;
        }

        if (radius > target.ExitRadius)
        {
            radius -= velocity;

            if (radius < target.ExitRadius) { radius = target.ExitRadius; direction = Direction.Null; }

            Theta = target.Theta;
        }
    }

    private bool loaded = false;
    public void Load(InformationController.Entity info)
    {
        prefabName = info.prefabName;
        projectilePrefabName = info.projectilePrefabName;
        HitPoints = info.hitPoints;
        speed = info.speed;
        fireRate = info.fireRate;
        damage = info.damage;
        price = info.price;

        loaded = true;
    }

    public void Death()
    {
        if (HitPoints > 0f) return;

        MobSpawner.enemeiesSpawned.Remove(this);

        /*
        foreach (Projectile p in projectileList)
            DestroyImmediate(p.gameObject);
        */
        Destroy(gameObject);

        PlayerEntity.userCurrency += price;
    }

}
