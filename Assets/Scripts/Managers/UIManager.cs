using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Refrences")]
    [SerializeField] GameManager GM;
    [Header("Canvas'")]
    [SerializeField] Canvas MainMenu;
    [SerializeField] Canvas Gameplay;
    [SerializeField] LevelManager levelManager;

    [Header("UI Elements")]
    [Header("Construction Button")]
    bool constructionOpen = false;
    public GameObject constButton;
    [Header("Decor Button")]
    bool decorationOpen = false;
    public GameObject decorButton;
    [Header("Resources")]
    public TMP_Text woodPool;
    public TMP_Text stonePool;
    public TMP_Text foodPool;

    public enum CurrentScreen
    {
        _MainMenu,
        _Gameplay,
        Construction
    }
    public CurrentScreen currentScreen;

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

        if (currentScreen == CurrentScreen._Gameplay)
        {
            UpdateHUD();
        }
    }

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
            decorButton.SetActive(true);
            constButton.transform.position = new Vector3(30f, 0, 0);
        }
        else
        {
            constructionOpen = true;
            decorButton.SetActive(false);
            constButton.transform.position = new Vector3(30f, 190, 0);
        }
    }

    void UpdateHUD()
    {
        woodPool.text = "Wood: " + GM.woodPool.ToString();
        stonePool.text = "Stone: " + GM.stonePool.ToString();
        foodPool.text = "Food: " + GM.foodPool.ToString();
    }
}
