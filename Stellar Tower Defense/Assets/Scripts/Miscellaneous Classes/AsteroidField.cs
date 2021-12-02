using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class AsteroidField : MonoBehaviour
{
    private static GameObject prefab;
    public static GameObject Prefab
    {
        get
        {
            if (prefab == null)
                prefab = (GameObject)Resources.Load("Prefabs/Asteroid");

            return prefab;
        }
    }

    public const float doublePI = 2 * Mathf.PI;

    private static float MinimumDifferenceBetweenAsteroids
    {
        get
        {
            return Prefab.transform.localScale.x * Mathf.Sqrt(2);
        }
    }

    private int clusterSize;
    private List<Transform> clusters; 

    private int length;
    public int radius;
    public int speed=1;

    private int gapLength = 10;
    private List<int> gapList;
    private List<int> gapCenters;

    public List<Gap> gaps;

    public void Build(int radius, int gapNumber)
    {
        gameObject.name = "Asteroid Field_" + radius.ToString();
        transform.SetParent(null);

        clusterSize = Random.Range(2, 4);
        clusters = new List<Transform>(clusterSize);
        this.radius = radius;

        BuildField(gapNumber);
    }

    private void BuildField(int gapNumber)
    {
        float theta = Random.Range(0f, doublePI);

        float maxNumberOfAsteroids = doublePI * radius / MinimumDifferenceBetweenAsteroids;
        float angleDifferenceBetweenAsteroids = doublePI / maxNumberOfAsteroids;

        length = (int)Mathf.Floor(maxNumberOfAsteroids);

        
        Point pc1 = new Point(theta, radius);
        BuildCluster(1);
        
        Point pc2 = new Point(); float radius2 = radius + MinimumDifferenceBetweenAsteroids;
        if (clusterSize > 1)
        {
            pc2 = new Point(theta, radius2);
            BuildCluster(2);
        }

        Point pc3 = new Point(); float radius3 = radius - MinimumDifferenceBetweenAsteroids;
        if (clusterSize > 2)
        {
            pc3 = new Point(theta, radius3);
            BuildCluster(3);
        }

        if (gapNumber > length / (2 * (gapLength + 1))) gapNumber = length / (2 * (gapLength + 1));

        GenerateGaps(gapNumber);

        for (int i = 0; i < length; ++i)
        {
            theta += angleDifferenceBetweenAsteroids;
            if (theta > doublePI) theta -= doublePI;

            if (gapList.Contains(i))
            {
                if (gapCenters.Contains(i))
                    BuildGap(theta, radius);

                continue;
            }

            BuildAsteroid(i.ToString(), theta, radius, ref pc1,0);

            BuildAsteroid(i.ToString(), theta, radius2, ref pc2, 1, true);

            BuildAsteroid(i.ToString(), theta, radius3, ref pc3, 2, true);

        }

        AnimateField();

    }

    private void BuildCluster(int i)
    {
        GameObject cluster = new GameObject();
        cluster.name = "Cluster_" + i.ToString();
        cluster.transform.parent = transform;

        clusters.Add(cluster.transform);
    }

    private void BuildAsteroid(string name,float theta, float radius, ref Point p,int clusterNumber, bool chanceToGenerate = false)
    {
        if (!p.isInstantiated()) return;

        if (chanceToGenerate)
        {
            float coinFlip = Random.Range(-1f, 1f);
            if (coinFlip < 0f)
            {
                p = new Point(theta, radius);
                return;
            }
        }

        GameObject asteroid = Instantiate(Prefab);

        float height = Random.Range(radius - this.radius, this.radius - radius);

        asteroid.transform.localPosition = new Vector3(p.x, asteroid.transform.localPosition.y + height, p.y);

        asteroid.transform.parent = clusters[clusterNumber];
        asteroid.name = "Asteroid_" + name;

        p = new Point(theta, radius);
    }

    private void BuildGap(float theta, float radius)
    {
        Point p = new Point(theta, radius);

        GameObject gapObject = new GameObject();
        gapObject.transform.parent = transform;

        gapObject.transform.localPosition = new Vector3(p.x, gapObject.transform.localPosition.y, p.y);

        gapObject.name = "Gap_" + gaps.Count.ToString();

        Gap gap = new Gap(this,gapObject, radius);

        gaps.Add(gap);
    }

    private void GenerateGaps(int gapNumber)
    {
        gapList = new List<int>(gapNumber);
        gaps = new List<Gap>(gapNumber);

        if (gapNumber == 0) return;

        int i = 0;
        List<int> possibleGaps = new List<int>(length);

        while (i < length)
        {
            possibleGaps.Add(0);
            ++i;
        }

        i = 0;
        while (gapNumber > 0)
        {
            ++i; if (i == 10000) { Debug.Log("Error " + radius); break; }
            int newGap = Random.Range(0, length);

            if (possibleGaps[newGap] == 1) continue;

            gapList.Add(newGap);

            int startingIndex = newGap - gapLength * 2;
            int count = gapLength * 4 + 1;

            if (startingIndex < 0)
            {
                for (i = length + startingIndex; i < length; i++)
                    possibleGaps[i] = 1;

                count += startingIndex;
                startingIndex = 0;
            }
            else if (startingIndex + count > length)
            {
                for (i = 0; i < startingIndex + count - length; i++)
                    possibleGaps[i] = 1;

                count -= i;
            }

            while (count > 0)
            {
                possibleGaps[startingIndex] = 1;
                ++startingIndex;
                --count;
            }


            --gapNumber;
        }

        List<int> aux = new List<int>(gapNumber * gapLength);
        gapCenters = gapList;

        foreach (int gap in gapList)
        {
            i = -gapLength / 2;

            while (i <= gapLength / 2)
            {
                if (gap + i < 0) aux.Add(length - gap - i);
                else if (gap + i >= length) aux.Add(gap + i - length);
                else aux.Add(gap + i);

                ++i;
            }
        }

        aux.Sort();
        gapList = aux;
    }

    public Gap GetClosestGap(GameObject seeker)
    {
        float min = Mathf.Infinity;
        Gap result = null;

        Point seekerPosition = new Point();

        seekerPosition.x = seeker.transform.position.x;
        seekerPosition.y = seeker.transform.position.z;

        Point targetPosition;float distance;

        foreach(Gap gap in gaps)
        {
            targetPosition = new Point();
            targetPosition.x = gap.instance.transform.position.x;
            targetPosition.y = gap.instance.transform.position.z;

            distance = seekerPosition.GetDistance(targetPosition);

            if (min == Mathf.Infinity || min > distance)
            {
                result = gap;
                min = distance;
            }
        }

        return result;
    }

    private void AnimateField()
    {
        _ = Rotate();
    }

    public float RotationSpeed { get { return .01f * radius * speed; } }

    private async Task<bool> Rotate()
    {
        if (!await GameManager.ApplicationStatus()) return false;

        await Task.Delay(15);

        transform.Rotate(Vector3.up, RotationSpeed);

        _ = Rotate();

        return true;
    }
}
