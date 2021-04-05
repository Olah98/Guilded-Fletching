using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuButtons : MonoBehaviour
{
    public Button cont;
    private GameObject blackScreen;

    private void Start()
    {
        blackScreen = GameObject.FindGameObjectWithTag("ScreenShift"); //By Warren
        SaveManager.instance.Load();
        if (cont != null)
        {
            cont.GetComponent<Button>().interactable = false;
            int sceneCount = SceneManager.sceneCountInBuildSettings;
            for (int i = 0; i < sceneCount; i++)
            {
                if (SaveManager.instance.activeSave.sceneName == sceneName(i))
                {
                    Debug.Log("true!");
                    cont.GetComponent<Button>().interactable = true;
                    break;
                }

            }
        }
    }//Start

    /*
    * Delay - By Warren 
    * Calls a screen shift and waits for the change, if a target is available
    */
    public IEnumerator Delay()
    {
        if (blackScreen != null)
        {
            blackScreen.GetComponent<ScreenShift>().Change();
            yield return new WaitForSeconds(1f);
        }
    }//Delay

    public void LoadScene(string level)
    {
        StartCoroutine(LoadSceneCo(level)); //By Warren
        //SceneManager.LoadScene(level);
    }//LoadScene

    /*
    * Load Scene Co - By Warren 
    * Asks for a delay for a screen fade before loading the target level/menu 
    */
    public IEnumerator LoadSceneCo(string level)
    {
        yield return Delay();
        SceneManager.LoadScene(level);
    }//LoadSceneCo

    public void OpenLink(string url)
    {
        Application.OpenURL(url);
    }//OpenLink

    public void ExitGame()
    {
        StartCoroutine(ExitGameCo()); //By Warren
        //Application.Quit();
    }//ExitGame

    /*
    * Exit Game Co - By Warren 
    * Asks for a delay for a screen fade before closing the game
    */
    public IEnumerator ExitGameCo()
    {
        yield return Delay();
        Application.Quit();
    }//ExitGameCo

    public void LoadOptions()
    {
        StartCoroutine(LoadSceneCo("Options")); //By Warren
        //SceneManager.LoadScene("Options");
    }//LoadOptions

    public void Continue()
    {
        SaveManager.instance.Load();
        string sceneToLoad = SaveManager.instance.activeSave.sceneName;
        if (sceneToLoad != null &&
            (sceneToLoad != "AlexOlahTest" ||
            sceneToLoad != "Christian's Test Scene" ||
            sceneToLoad != "Jon_Test" ||
            sceneToLoad != "MilesGomezTest" ||
            sceneToLoad != "TylerMunstockTest" ||
            sceneToLoad != "Warren_Test"))
        {
            //SceneManager.LoadScene(SaveManager.instance.activeSave.sceneName);
            StartCoroutine(LoadSceneCo(sceneToLoad)); //By Warren
        }
    }//Continue

    private static string sceneName(int i)
    {
        string path = SceneUtility.GetScenePathByBuildIndex(i);
        int slash = path.LastIndexOf('/');
        string name = path.Substring(slash + 1);
        int dot = name.LastIndexOf('.');
        return name.Substring(0, dot);
    }//sceneName
}
