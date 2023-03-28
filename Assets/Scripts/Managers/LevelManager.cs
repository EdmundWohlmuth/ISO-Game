using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public void LoadGameplay()
    {
        SceneManager.LoadScene(0);
        GameManager.gameManager.Reset();
    }

    public void ExitGame()
    {
        Application.Quit();      
    }
}
