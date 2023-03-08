using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] Canvas MainMenu;
    [SerializeField] Canvas Gameplay;
    [SerializeField] LevelManager levelManager;

    [Header("Construction Button")]
    bool constructionOpen = false;
    public GameObject constButton;
    [Header("Decor Button")]
    bool decorationOpen = false;
    public GameObject decorButton;

    private void Start()
    {
        //MainMenuScreen();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            OpenCloseConstruction();
        }
    }

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
            constButton.transform.position = new Vector3(30f, 0, 0);
        }
        else
        {
            constructionOpen = true;
            constButton.transform.position = new Vector3(30f, 190, 0);
        }
    }
}
