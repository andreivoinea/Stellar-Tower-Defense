using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public static MenuUIManager menu { get { return Instance.GetComponent<MenuUIManager>(); } }

    private static Transform environment;

    public static Transform Environment
    {
        get
        {
            if(environment == null) environment = GameObject.FindWithTag("Environment").transform;
            return environment;
        }
    }

    public int Level
    {
        get
        {
            return PlayerEntity.level;
        }
    }

    public GameObject MenuUI;
    public GameObject ExtraUI;

    private void Singleton()
    {
        if (_instance != null)
            Destroy(_instance.gameObject);
        _instance = this;
    }

    private static PlayerEntity Player { get { return PlayerEntity.Instance; } }

    private void Awake()
    {
        Singleton();
    }

    private void Start()
    {
        Menu();
    }

    public void StartLevel()
    {
        MenuUI.SetActive(false);
        ExtraUI.SetActive(true);
        MenuUIManager.WinScreen(false);

        ClearMap();
        FollowPlayer();

        InformationController.Level LevelInfo = InformationController.GetLevel(Level);

        GenerateMap(LevelInfo.gapNumber);
        SpawnEnemies(LevelInfo.enemySequence, LevelInfo.enemiesPerSpawn);
    }

    public void Menu()
    {
        ClearMap();
        GenerateMap(0);

        MenuUI.SetActive(true);
        MenuUIManager.UpgradeMenu(false);
        MenuUIManager.WinScreen(false);
        ExtraUI.SetActive(false);
        menu.GameOverMenu(false);

        Player.LoadPlayer();
    }

    public void NewGame()
    {
        InformationController.DeletePlayer();
        StartLevel();
    }

    public void ContinueGame()
    {
        Player.LoadPlayer();
        StartLevel();
    }

    public void SpawnEnemies(List<int> enemySequence, int enemiesPerSpawn) {

        AsteroidField lastBelt = asteroidFields[asteroidFields.Count - 1];

        GameObject spawner = new GameObject();

        spawner.transform.parent = Environment;
        spawner.name = "Spawner";

        MobSpawner script = spawner.AddComponent<MobSpawner>();

        script.Spawn(lastBelt, enemySequence, enemiesPerSpawn);
    }

    public static List<FriendlyEntity> towers = new List<FriendlyEntity>();
    public static void CreateTower(int towerType)
    {
        GameObject tower = Instantiate((GameObject)Resources.Load("Prefabs/Towers/Tower" + towerType.ToString()), null);
        FriendlyEntity script = tower.AddComponent<FriendlyEntity>();

        script.Build(towerType);

        towers.Add(script);
    }

    public void UpgradeTowers(int type, int upgradeType, int value)
    {
        foreach (FriendlyEntity tower in towers)
        {
            if (tower.towerType != type) continue;

            switch (upgradeType)
            {
                case 0:
                    tower.fireRate = value;
                    break;

                case 1:
                    tower.damage = value;
                    break;

                case 2:
                    tower.AttackRangeRadius = value;
                    break;

                default: return;
            }
        }
    }

    public Gap GetClosestGap(EnemyEntity seeker)
    {
        int i;
        for (i = asteroidFields.Count - 1; i >= 0; --i)
        {
            if (asteroidFields[i].radius < seeker.radius) break;
        }

        if (i != -1)
            return asteroidFields[i].GetClosestGap(seeker.gameObject);
        else
            return null;
    }

    private void FollowPlayer()
    {
        if (PlayerEntity.Instance == null) return;

        Camera.main.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, Player.transform.position.z);

        Camera.main.transform.position += new Vector3(8f, 10f, -8f);

        Camera.main.transform.LookAt(Player.transform);

        Camera.main.transform.position -= Camera.main.transform.TransformDirection(Vector3.up) * 4f;
    }

    private void MoveCamera()
    {
        float xAxisValue = Input.GetAxis("Horizontal");
        float zAxisValue = Input.GetAxis("Vertical");

        Camera.main.transform.position+= Quaternion.Euler(0, -45, 0) * (new Vector3(xAxisValue, 0f, zAxisValue)) * 0.1f;
    }

    public List<AsteroidField> asteroidFields = new List<AsteroidField>(9);
    private void GenerateMap(int level)
    {
        int i = 3;
        while (i < 12)
        {
            AsteroidField asteroidField = new GameObject().AddComponent<AsteroidField>();
            asteroidField.Build(i, level);
            asteroidField.transform.parent = Environment;

            asteroidFields.Add(asteroidField);

            ++i;
        }
    }
    private void ClearMap()
    {
        foreach (AsteroidField field in asteroidFields)
        {
            Destroy(field.gameObject);
        }

        asteroidFields.Clear();

        MobSpawner spawner = FindObjectOfType<MobSpawner>();

        if (spawner != null) spawner.Clear();
    }

    public static bool applicationRunning = true;
    public static bool applicationPaused = false;

    private void OnApplicationQuit()
    {
        applicationRunning = false;
    }

    public static async Task<bool> ApplicationStatus()
    {
        if (!applicationRunning) return false;
        while (applicationPaused) await Task.Delay(15);

        return true;
    }

    public static bool Buy(int price)
    {
        return Player.Buy(price);
    }

    private void Update()
    {
        if(Input.GetKeyDown("space")) applicationPaused = !applicationPaused;
        MoveCamera();
    }
}
