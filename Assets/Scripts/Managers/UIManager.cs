using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] Canvas MainMenu_;
    [SerializeField] Canvas Gameplay_;
    [SerializeField] Canvas Construction_;

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
