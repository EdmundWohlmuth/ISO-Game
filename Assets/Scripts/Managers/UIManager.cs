using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] Canvas MainMenu;
    [SerializeField] Canvas Gameplay;
    [SerializeField] Canvas Construction;
    [SerializeField] LevelManager levelManager;

    [Header("Construction Button")]
    bool constructionOpen = false;
    GameObject constButton;
    [Header("Decor Button")]
    bool decorationOpen = false;
    GameObject decorButton;
    

    public enum CurrentScreen
    {
        _MainMenu,
        _Gameplay,
        Construction
    }
    public CurrentScreen currentScreen;

    public void MainMenuScreen()
    {
        MainMenu.enabled = true;
        Gameplay.enabled = false;
    }
    public void GameplayScreen()
    {
        MainMenu.enabled = false;
        Gameplay.enabled = true;
    }

    public void StartGame()
    {
        GameplayScreen();
        levelManager.LoadGameplay();
    }


    public void OpenCloseConstruction()
    {
        if (constructionOpen)
        {
            constructionOpen = false;
        }
        else
        {

            constructionOpen = true;
        }
    }
}
