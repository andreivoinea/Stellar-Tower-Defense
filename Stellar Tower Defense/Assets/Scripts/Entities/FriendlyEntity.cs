using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class FriendlyEntity : Entity
{
    public int towerType;

    private GameObject AttackRange;

    private static GameObject attackRangePrefab;
    public static GameObject AttackRangePrefab
    {
        get
        {
            if (attackRangePrefab == null)
                attackRangePrefab = (GameObject)Resources.Load("Prefabs/Radius");

            return attackRangePrefab;
        }
    }

    public float attackRangeRadius = 1f;
    public float AttackRangeRadius
    {
        get { return attackRangeRadius; }
        set
        {
            if (attackRangeRadius == value) return;
            attackRangeRadius = value;
            VisualizeAttackRange();
        }
    }

    private Vector3 attackRangeRadiusScale
    {
        get
        {
            return Vector3.one * attackRangeRadius * (1f / transform.localScale.x);
        }
    }

    private void VisualizeAttackRange()
    {
        if (AttackRange == null)
            AttackRange = Instantiate(AttackRangePrefab, transform);

        AttackRange.transform.localScale = attackRangeRadiusScale;
    }

    private new Renderer renderer;

    public Renderer Renderer
    {
        get
        {
            if (renderer == null) renderer = GetComponent<Renderer>();
            return renderer;
        }
    }

    private Material canPlace;
    private Material cannotPlace;
    private Material defaultMaterial;

    public Material CanPlace
    {
        get
        {
            if (canPlace == null)
                canPlace = (Material)Resources.Load("Materials/CanPlace");

            return canPlace;
        }
    }
    public Material CannotPlace
    {
        get
        {
            if (cannotPlace == null)
                cannotPlace = (Material)Resources.Load("Materials/CannotPlace");

            return cannotPlace;
        }
    }
    public Material DefaultMaterial
    {
        get
        {
            if (defaultMaterial == null)
                defaultMaterial = Renderer.material;

            return defaultMaterial;
        }
    }
    public float GetCurrentTheta()
    {
        Point center = new Point();
        center.x = 0f;
        center.y = 0f;

        Point current = new Point();
        current.x = transform.position.x;
        current.y = transform.position.z;

        float realRadius = current.GetDistance(center);

        float negative;
        if (Mathf.Asin(current.y / realRadius) < 0f) negative = -1;
        else negative = 1f;

        return Mathf.Acos(current.x / realRadius) * negative;
    }


    private void Start()
    {
        VisualizeAttackRange();
        FollowMouse();
    }

    public void Build(int towerType)
    {
        this.towerType = towerType;

        InformationController.UpgradeVector info = PlayerEntity.GetTowerInfo(towerType).upgradeStats;
        InformationController.Upgrade upgradeInfo = InformationController.GetUpgrade(towerType);
        fireRate = upgradeInfo.fireRateNumber[info.fireRateUpgrades];
        damage = upgradeInfo.damageNumber[info.damageUpgrades];
        AttackRangeRadius = upgradeInfo.rangeNumber[info.rangeUpgrades];
    }

    private bool placed = false;
    public void Update()
    {
        if (CheckPlacement() && Input.GetMouseButtonUp(0))
            PlaceTower();

        if (GameManager.applicationPaused) return;

        if (placed)
            Shoot();
        else
            FollowMouse();
    }

    private void FollowMouse()
    {
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y);

        Vector3 screenPos = Camera.main.ScreenToWorldPoint(mousePos);

        Vector3 realPos = GetIntersectingYPlanePoint(Camera.main.transform.position, screenPos, 0f);

        transform.position = realPos;
    }

    private Vector3 GetIntersectingYPlanePoint(Vector3 point1, Vector3 point2, float plane)
    {
        float t = (plane - point1.y) / (point2.y - point1.y);

        float x = point1.x + t * (point2.x - point1.x);
        float z = point1.z + t * (point2.z - point1.z);

        return new Vector3(x, plane, z);
    }


    private bool CheckPlacement()
    {
        if (placed) return false;

        _ = DefaultMaterial;

        if (asteroidCount > 0)
        {
            Renderer.material = CanPlace;
            return true;
        }

        Renderer.material = CannotPlace;
        return false;
    }

    private void PlaceTower()
    {
        placed = true;
        Renderer.material = DefaultMaterial;

        asteroidField = asteroidBelt.GetComponent<AsteroidField>();

        Point p = new Point(GetCurrentTheta(), asteroidField.radius);

        transform.localPosition = new Vector3(p.x, transform.localPosition.y, p.y);
        radius = asteroidField.radius;

        transform.parent = asteroidBelt;


        SphereCollider collider = gameObject.AddComponent<SphereCollider>();
        collider.isTrigger = true;
        collider.radius = attackRangeRadiusScale.x;

        Destroy(GetComponent<BoxCollider>());
    }

    private int asteroidCount = 0;
    private Transform asteroidBelt;
    private AsteroidField asteroidField;

    private void OnTriggerEnter(Collider other)
    {
        if (placed)
        {
            if (other.gameObject.tag == "Enemy")
            {
                EnemyEntity entity = other.gameObject.GetComponent<EnemyEntity>();

                TargetList.Add(entity);
            }
            return;
        }

        if (other.gameObject.tag == "Asteroid")
        {
            ++asteroidCount;
            if (asteroidBelt == null)
                asteroidBelt = other.transform.parent.parent;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (placed)
        {
            if (other.gameObject.tag == "Enemy")
            {
                TargetList.Remove(other.gameObject.GetComponent<EnemyEntity>());
            }
            return;
        }

        if (other.gameObject.tag == "Asteroid")
        {
            --asteroidCount;
            if (asteroidCount == 0)
                asteroidBelt = null;
        }
    }
}
