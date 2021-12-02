using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour
{
    public void NewGame()
    {
        GameManager.Instance.NewGame();
    }

    public void ContinueGame()
    {
        GameManager.Instance.ContinueGame();
    }

    public void Settings()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }

    private static UpgradeMenu upgradeMenuUI;
    private static UpgradeMenu UpgradeMenuUI
    {
        get
        {
            if (upgradeMenuUI == null) upgradeMenuUI = FindObjectOfType<UpgradeMenu>(true);
            return upgradeMenuUI;
        }
    }

    public static void UpgradeMenu(bool status)
    {
        UpgradeMenuUI.gameObject.SetActive(status);

        if (!UpgradeMenuUI.WinScreen.activeSelf)
            GameManager.applicationPaused = status;
    }

    public static void WinScreen(bool status)
    {
        UpgradeMenuUI.WinScreen.SetActive(status);
        UpgradeMenuUI.gameObject.SetActive(status);
        GameManager.applicationPaused = status;
    }


    public GameObject GameOverUI;
    public void GameOverMenu(bool status)
    {
        GameOverUI.SetActive(status);

        GameManager.applicationPaused = status;
    }

    public static List<TowerButton> towerButtonList = new List<TowerButton>();
    public static void ActivateTowerButtons(int type)
    {
        if (towerButtonList.Count == 0) towerButtonList = new List<TowerButton>(FindObjectsOfType<TowerButton>(true));

        foreach(TowerButton tb in towerButtonList)
        {
            if (tb.towerType == type) tb.Button.interactable = true;
        }
    }

    public static void ActivateTowerButtons(List<int> types)
    {
        if (towerButtonList.Count == 0) towerButtonList = new List<TowerButton>(FindObjectsOfType<TowerButton>(true));

        foreach (TowerButton tb in towerButtonList)
        {
            if (types.Contains(tb.towerType)) tb.Button.interactable = true;
        }
    }

}
