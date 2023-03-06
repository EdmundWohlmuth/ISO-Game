using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] Canvas MainMenu_;
    [SerializeField] Canvas Gameplay_;
    [SerializeField] Canvas Construction_;

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
}
