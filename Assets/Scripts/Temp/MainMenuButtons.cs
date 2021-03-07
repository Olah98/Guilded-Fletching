using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuButtons : MonoBehaviour
{
    
    public void LoadScene(string level)
    {
        SceneManager.LoadScene(level);
    }

    public void OpenLink(string url)
    {
        Application.OpenURL(url);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void LoadOptions() {
        SceneManager.LoadScene("Options");
    }


}
