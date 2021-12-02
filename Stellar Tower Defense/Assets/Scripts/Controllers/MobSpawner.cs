using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class MobSpawner : MonoBehaviour
{
    public static List<EnemyEntity> enemeiesSpawned; 

    public void Spawn(float radius, InformationController.Entity info)
    {
        if (info == null) return;

        GameObject prefab = GetPrefab(info.prefabName);

        float theta = Random.Range(0f, AsteroidField.doublePI);
        Point p = new Point(theta, radius + 0.5f);

        Vector3 pos = new Vector3(p.x, prefab.transform.localPosition.y, p.y);

        EnemyEntity enemy = Instantiate(prefab, pos, Quaternion.identity,transform).AddComponent<EnemyEntity>();

        enemy.Load(info);

        enemy.radius = radius + 0.5f;

        enemeiesSpawned.Add(enemy);
    }

    public void Spawn(AsteroidField field, List<int> enemySequence, int enemyFrequency)
    {
        _ = AsyncSpawn(field, enemySequence, enemyFrequency);
    }

    public async Task<bool> AsyncSpawn(AsteroidField field, List<int> enemySequence, int enemyFrequency)
    {
        int i = 0; int j;

        enemeiesSpawned = new List<EnemyEntity>(enemySequence.Count);

        while (i < enemySequence.Count)
        {
            await Task.Delay(2000);

            if (!await GameManager.ApplicationStatus()) return false;

            for (j=0; j < enemyFrequency; ++j)
                Spawn(field.radius, InformationController.GetEnemy(i+j));


            i += enemyFrequency;
        }

        loaded = true;
        return true;
    }

    public GameObject GetPrefab(string prefabName)
    {
        return (GameObject)Resources.Load("Prefabs/" + prefabName);
    }

    private bool loaded = false;
    private void Update()
    {
        if (!loaded) return;
        if (enemeiesSpawned.Count > 0) return;

        MenuUIManager.WinScreen(true);
        ++PlayerEntity.level;
        loaded = false;
    }

    public void Clear()
    {
        foreach (EnemyEntity enemy in enemeiesSpawned)
        {
            Destroy(enemy.gameObject);
        }

        Destroy(gameObject);
    }
}
