using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuButtons : MonoBehaviour
{
    public Button cont;
    private void Awake()
    {
        SaveManager.instance.Load();
        if (SaveManager.instance.activeSave.sceneName!=null)
        {
            cont.GetComponent<Button>().interactable = false;
        }
    }

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

    public void Continue()
    {
        SaveManager.instance.Load();
        string sceneToLoad = SaveManager.instance.activeSave.sceneName;
        if (sceneToLoad !=null &&
            (sceneToLoad != "AlexOlahTest" ||
            sceneToLoad != "Christian's Test Scene" ||
            sceneToLoad != "Jon_Test" ||
            sceneToLoad != "MilesGomezTest" ||
            sceneToLoad != "TylerMunstockTest" ||
            sceneToLoad != "Warren_Test"))
        {
            SceneManager.LoadScene(SaveManager.instance.activeSave.sceneName);
        }
        
    }


}
