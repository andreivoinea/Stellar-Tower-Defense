using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class InformationController : MonoBehaviour
{
    private static LevelInfo levelInformation;
    public static LevelInfo LevelInformation
    {
        get
        {
            if (levelInformation == null) levelInformation = (LevelInfo)ReadInfo("LevelInformation", typeof(LevelInfo));
            return levelInformation;
        }
    }

    [Serializable]
    public class LevelInfo
    {
        public List<Level> levelList;
    }

    [Serializable]
    public class Level
    {
        public int gapNumber;
        public int enemiesPerSpawn;
        public List<int> enemySequence;
    }

    private static EntityInfo enemyInformation;
    public static EntityInfo EnemyInformation
    {
        get
        {
            if (enemyInformation == null) enemyInformation = (EntityInfo)ReadInfo("EnemyInformation", typeof(EntityInfo));
            return enemyInformation;
        }
    }

    private static EntityInfo towerInformation;
    public static EntityInfo TowerInformation
    {
        get
        {
            if (towerInformation == null) towerInformation = (EntityInfo)ReadInfo("TowerInformation", typeof(EntityInfo));
            return towerInformation;
        }
    }


    [Serializable]
    public class EntityInfo
    {
        public List<Entity> entityList;
    }

    [Serializable]
    public class Entity
    {
        public string prefabName;
        public string projectilePrefabName;
        public float hitPoints;
        public float speed;
        public int fireRate;
        public float damage;
        public int price;

        public Entity(string prefabName,string projectilePrefabName,float hitPoints,float speed,int fireRate,float damage,int price)
        {
            this.prefabName = prefabName;
            this.projectilePrefabName = projectilePrefabName;
            this.hitPoints = hitPoints;
            this.speed = speed;
            this.fireRate = fireRate;
            this.damage = damage;
            this.price = price;
        }
    }

    private static PlayerInfo playerInformation;
    public static PlayerInfo PlayerInformation
    {
        get
        {
            if (playerInformation == null) playerInformation = LoadPlayer();
            return playerInformation;
        }
    }

    [Serializable]
    public class PlayerInfo
    {      
        public int currency;
        public int level;

        public List<TowerUpgrade> towerList;
    }

    [Serializable]
    public class TowerUpgrade
    {
        public int type;

        public UpgradeVector upgradeStats;

        public List<TowerPosition> positions;

        public TowerUpgrade(int type)
        {
            this.type = type;
            upgradeStats = new UpgradeVector();
            positions = new List<TowerPosition>();
        }
    }

    [Serializable]
    public class UpgradeVector
    {
        public int fireRateUpgrades;
        public int damageUpgrades;
        public int rangeUpgrades;

        public UpgradeVector()
        {
            fireRateUpgrades = 0;
            damageUpgrades = 0;
            rangeUpgrades = 0;
        }
    }

    [Serializable]
    public class TowerPosition
    {
        public float theta;
        public float radius;

        public TowerPosition(float theta,float radius)
        {
            this.theta = theta;
            this.radius = radius;
        }
    }

    private static UpgradeInfo upgradeInformation;
    public static UpgradeInfo UpgradeInformation
    {
        get
        {
            if (upgradeInformation == null) upgradeInformation = (UpgradeInfo)ReadInfo("UpgradeInformation", typeof(UpgradeInfo));
            return upgradeInformation;
        }
    }

    [Serializable]
    public class UpgradeInfo
    {
        public List<Upgrade> upgradeList;
    }
    [Serializable]
    public class Upgrade
    {
        public int price;

        public List<int> fireRateNumber;
        public List<int> fireRatePrice;

        public List<int> damageNumber;
        public List<int> damagePrice;

        public List<int> rangeNumber;
        public List<int> rangePrice;
    }

    private static object ReadInfo(string file, Type t)
    {
        string json = Resources.Load<TextAsset>(file).text;

        return JsonUtility.FromJson(json,t);
    }

    private static PlayerInfo LoadPlayer()
    {
        string path = Application.persistentDataPath + "/PlayerInformation.json";

        if (!File.Exists(path))
            return SavePlayer((PlayerInfo)ReadInfo("PlayerInformation", typeof(PlayerInfo)));

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open);

        PlayerInfo aux = formatter.Deserialize(stream) as PlayerInfo;

        stream.Close();

        return aux;
    }

    public static PlayerInfo SavePlayer(PlayerInfo info = null)
    {
        if (info == null) info = PlayerInformation;

        string path = Application.persistentDataPath + "/PlayerInformation.json";
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, info);

        stream.Close();

        return info;
    }

    public static void DeletePlayer()
    {
        string path = Application.persistentDataPath + "/PlayerInformation.json";

        if (!File.Exists(path)) return;

        File.Delete(path);

        playerInformation = LoadPlayer();
    }

    public static Level GetLevel(int level)
    {
        return LevelInformation.levelList[level - 1];
    }

    
    public static Entity GetEnemy(int number)
    {
        if (PlayerEntity.level == 0) return null;

        int type = LevelInformation.levelList[PlayerEntity.level - 1].enemySequence[number];
        return EnemyInformation.entityList[type];
    }

    public static Upgrade GetUpgrade(int number)
    { 
        return UpgradeInformation.upgradeList[number];
    }

    public static int GetTowerPrice(int number)
    {
        if (number >= UpgradeInformation.upgradeList.Count) return -1;

        return UpgradeInformation.upgradeList[number].price;
    }

    public static PlayerInfo GetPlayer()
    {
        return PlayerInformation;
    }

    public static void ModifyTowers()
    {
        PlayerInformation.towerList = PlayerEntity.upgrades;
    }

}
