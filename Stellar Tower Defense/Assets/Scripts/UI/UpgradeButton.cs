using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : CustomButton
{
    public int towerNumber;
    public UpgradeMenu.UpgradeType type;
    public int upgradeNumber;

    public void Upgrade()
    {
        if (!UpgradeMenu.Upgrade(towerNumber,upgradeNumber,type)) return;

        DisableCurrentUpgrade(true);
        EnableNextUpgrade();
    }

    private void DisableCurrentUpgrade(bool upgraded)
    {
        if (upgraded)
        {
            ColorBlock newColor = ColorBlock.defaultColorBlock;
            newColor.disabledColor = Color.yellow;

            Button.colors = newColor;
        }

        Button.interactable = false;
    }

    private void EnableNextUpgrade()
    {
        int hierachyOrder = transform.GetSiblingIndex();

        if (hierachyOrder == transform.parent.childCount - 1) return;

        transform.parent.GetChild(hierachyOrder + 1).GetComponent<Button>().interactable = true;
    }

    public void Check()
    {
       
        int compareValue;

        InformationController.TowerUpgrade info = PlayerEntity.GetTowerInfo(towerNumber);

        if (info == null) compareValue = -2;
        else
        {
            switch ((int)type)
            {
                case 0:
                    compareValue = info.upgradeStats.fireRateUpgrades;
                    break;

                case 1:
                    compareValue = info.upgradeStats.damageUpgrades;
                    break;

                case 2:
                    compareValue = info.upgradeStats.rangeUpgrades;
                    break;

                default: return;
            }
        }

        ++compareValue;

        if (upgradeNumber == compareValue)
        {
            Button.interactable = true;
            return;
        }

        if (upgradeNumber < compareValue) 
            DisableCurrentUpgrade(true);
        else
            DisableCurrentUpgrade(false);
    }

    public void Build(int towerNumber,int upgradeNumber, UpgradeMenu.UpgradeType type)
    {
        this.towerNumber = towerNumber;
        this.upgradeNumber = upgradeNumber;
        this.type = type;

        Check();
    }
}
