using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeMenu : MonoBehaviour
{
    public GameObject WinScreen;

    private GameObject upgradeButton;

    public GameObject UpgradeButton
    {
        get
        {
            if (upgradeButton == null)
                upgradeButton = (GameObject)Resources.Load("Prefabs/UI/UpgradeButton");

            return upgradeButton;
        }
    }

    private void Start()
    {
        BuildUpgradeMenu();
    }

    public enum UpgradeType { FireRate, Damage, Range }

    public void BuildUpgradeMenu()
    {
        for (int i = 0; i < transform.GetChild(0).childCount; ++i)
                BuildUpgradeButtons(i);
    }

    public static List<UpgradeButton> allButtons = new List<UpgradeButton>();

    public void BuildUpgradeButtons(int towerNumber)
    {
        InformationController.Upgrade info = InformationController.GetUpgrade(towerNumber);

        int j = 0;

        int max = info.fireRateNumber.Count;
        if (info.damageNumber.Count > max) max = info.damageNumber.Count;
        if (info.rangeNumber.Count > max) max = info.rangeNumber.Count;

        for (j = 0; j < max; ++j)
        {
            if (j < info.fireRateNumber.Count) allButtons.Add(BuildUpgradeButton(towerNumber, j, UpgradeType.FireRate));
            if (j < info.damageNumber.Count) allButtons.Add(BuildUpgradeButton(towerNumber, j, UpgradeType.Damage));
            if (j < info.rangeNumber.Count) allButtons.Add(BuildUpgradeButton(towerNumber, j, UpgradeType.Range));
        }

    }

    public UpgradeButton BuildUpgradeButton(int towerNumber, int upgradeNumber, UpgradeType type)
    {
        Transform parent = transform.GetChild(0).GetChild(towerNumber).GetChild((int)type);

        UpgradeButton aux = Instantiate(UpgradeButton,parent).GetComponent<UpgradeButton>();

        aux.gameObject.name = "Upgrade" + upgradeNumber.ToString();
        aux.Build(towerNumber, upgradeNumber, type);

        RectTransform rect = aux.GetComponent<RectTransform>();
        RectTransform parentRect = parent.GetComponent<RectTransform>();
        rect.anchoredPosition += Vector2.right * 40 * upgradeNumber;
        if (parentRect.anchorMin.x == 1f) rect.anchoredPosition *= -1f;

        return aux;
    }

    public static bool Upgrade(int towerNumber,int upgradeNumber,UpgradeType type)
    {
        InformationController.Upgrade info = InformationController.GetUpgrade(towerNumber);

        int value;

        switch (type)
        {
            case UpgradeType.FireRate:
                if (!GameManager.Buy(info.fireRatePrice[upgradeNumber])) return false;
                value = info.fireRateNumber[upgradeNumber];
                break;

            case UpgradeType.Damage:
                if (!GameManager.Buy(info.damagePrice[upgradeNumber])) return false;
                value = info.damageNumber[upgradeNumber];
                break;

            case UpgradeType.Range:
                if (!GameManager.Buy(info.rangePrice[upgradeNumber])) return false;
                value = info.rangeNumber[upgradeNumber];
                break;

            default:
                return false;
        }

        GameManager.Instance.UpgradeTowers(towerNumber, (int)type, value);
        PlayerEntity.Instance.UpgradeTowers(towerNumber, (int)type, upgradeNumber);

        return true;
    }

    public static void UpdateUI()
    {
        foreach (UpgradeButton ub in allButtons)
            ub.Check();
    }


}
