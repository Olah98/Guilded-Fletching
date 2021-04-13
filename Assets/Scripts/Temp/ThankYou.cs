using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ThankYou : MonoBehaviour
{
    public string level;
    private GameObject _blackScreen; //By Warren

    public void Start()
    {
        _blackScreen = GameObject.FindGameObjectWithTag("ScreenShift"); //By Warren
    }

    public void load()
    {
        //SceneManager.LoadScene(level);
        //StartCoroutine(LoadSceneCo(level));//By Warren

        //This starts the LevelEnd Report Scene, the public string level
        //is now read by the Character on hit and entered in the LevelEnd
        //section of the SaveManager.instance
        StartCoroutine(LoadSceneCo("LevelEnd"));//By Warren
    }//load

    /* FUNCTIONS ADDED BY WARREN */
    /*
    * Delay - By Warren 
    * Calls a screen shift and waits for the change, if a target is available
    */
    public IEnumerator Delay()
    {
        if (_blackScreen != null)
        {
            _blackScreen.GetComponent<ScreenShift>().Change();
            yield return new WaitForSeconds(1f);
        }
    }//Delay

    /*
    * Load Scene Co - By Warren 
    * Asks for a delay for a screen fade before loading the target level/menu 
    */
    public IEnumerator LoadSceneCo(string new_level)
    {
        yield return Delay();
        SceneManager.LoadScene(new_level);
    }//LoadSceneCo
}//ThankYou
