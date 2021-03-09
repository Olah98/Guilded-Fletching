using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuButtons : MonoBehaviour
{
    public Button cont;
    private void Start()
    {
        
        SaveManager.instance.Load();
        if (cont != null)
        {
            cont.GetComponent<Button>().interactable = false;
            int sceneCount = SceneManager.sceneCountInBuildSettings;
            for ( int i=0; i< sceneCount; i++)
            {
                
                
                if (SaveManager.instance.activeSave.sceneName == sceneName(i))
                {
                    Debug.Log("true!");
                    cont.GetComponent<Button>().interactable = true;
                    break;
                }
                
            }
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

        public void LoadOptions()
        {
            SceneManager.LoadScene("Options");
        }

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
                SceneManager.LoadScene(SaveManager.instance.activeSave.sceneName);
            }

        }

    private static string sceneName(int i)
    {
        string path = SceneUtility.GetScenePathByBuildIndex(i);
        int slash = path.LastIndexOf('/');
        string name = path.Substring(slash + 1);
        int dot = name.LastIndexOf('.');
        return name.Substring(0, dot);
    }
     



}
