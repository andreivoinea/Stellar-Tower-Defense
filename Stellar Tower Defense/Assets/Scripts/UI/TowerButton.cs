using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerButton : CustomButton
{
    public int towerType;
    public Button BuyButton;

    private int price=-2;
    public int Price
    {
        get
        {
            if (price == -2) price = InformationController.GetTowerPrice(towerType);
            return price;
        }
    }

    private void Start()
    {
        Button.onClick.AddListener(CreateTower);
        BuyButton.onClick.AddListener(BuyType);

        BuyButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Buy Tower " + towerType.ToString() + "\nPrice:\n" + (Price/2).ToString();
    }

    private void CreateTower()
    {
        if (!GameManager.Buy(Price)) return;

        GameManager.CreateTower(towerType);
    }

    public void BuyType()
    {
        if (!GameManager.Buy(Price / 2)) return;

        InformationController.TowerUpgrade newType = new InformationController.TowerUpgrade(towerType);

        PlayerEntity.upgrades.Add(newType);
        Button.interactable = true;

        BuyButton.gameObject.SetActive(false);

        UpgradeMenu.UpdateUI();
    }

}
