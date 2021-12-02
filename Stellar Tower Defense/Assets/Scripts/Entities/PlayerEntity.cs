using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerEntity : Entity
{
    private static PlayerEntity _instance;
    public static PlayerEntity Instance { get { return _instance; } }

    public static int userCurrency = 0;
    public static int level = 0;

    public static List<InformationController.TowerUpgrade> upgrades;

    private void Singleton()
    {
        if (_instance != null)
            Destroy(_instance.gameObject);
        _instance = this;
    }

    private void Awake()
    {
        Singleton();
    }


    public void LoadPlayer()
    {
        HitPoints = 100f;
        InformationController.PlayerInfo info = InformationController.GetPlayer();

        userCurrency = info.currency;
        level = info.level;
        upgrades = info.towerList;

        List<int> aux = new List<int>(upgrades.Count);
        foreach (InformationController.TowerUpgrade tu in upgrades)
            aux.Add(tu.type);

        MenuUIManager.ActivateTowerButtons(aux);
    }

    public void Update()
    {
        UpdateCurrencyUI();

        if (GameManager.applicationPaused) return;

        Shoot();

        if (HitPoints <= 0f)
            GameManager.menu.GameOverMenu(true);
    }

    public bool Buy(int price)
    {
        if (userCurrency < price) return false;

        userCurrency -= price;
        return true;
    }

    public void UpgradeTowers(int type, int upgradeType, int upgradeNumber)
    {
        switch (upgradeType)
        {
            case 0:
                GetTowerInfo(type).upgradeStats.fireRateUpgrades = upgradeNumber;
                break;

            case 1:
                GetTowerInfo(type).upgradeStats.damageUpgrades = upgradeNumber;
                break;

            case 2:
                GetTowerInfo(type).upgradeStats.rangeUpgrades = upgradeNumber;
                break;

            default: return;
        }

        InformationController.ModifyTowers();
    }

    public void AddTower(int type, float theta, float radius)
    {
        GetTowerInfo(type).positions.Add(new InformationController.TowerPosition(theta, radius));
        InformationController.ModifyTowers();
    }

    public static InformationController.TowerUpgrade GetTowerInfo(int type)
    {
        foreach (InformationController.TowerUpgrade tu in upgrades)
            if (tu.type == type) return tu;

        return null;
    }

    private TextMeshProUGUI currency;

    public TextMeshProUGUI Currency
    {
        get
        {
            if (currency == null)
            {
                GameObject aux = GameObject.FindWithTag("Currency");
                if (aux == null) return null;
                currency = aux.GetComponent<TextMeshProUGUI>();
            }

            return currency;
        }
    }

    private void UpdateCurrencyUI()
    {
        if(Currency!=null) Currency.text = "Credits: " + userCurrency.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Enemy")
        {
            EnemyEntity entity = other.gameObject.GetComponent<EnemyEntity>();

            TargetList.Add(entity);
        }
        return;

    }

    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.tag == "Enemy")
        {
            TargetList.Remove(other.gameObject.GetComponent<EnemyEntity>());
        }
        return;

    }

}
