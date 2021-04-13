using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEngine.Audio;

public class UI : MonoBehaviour
{
    public GameObject pauseMenu;
    //public GameObject controlMenu;
    public GameObject optionMenu;
   // public GameObject restartPrompt;
    public GameObject mainMenuPrompt;
    public GameObject controlsPrompt; //By Warren
    public GameObject quitPrompt;
    public GameObject pauseBG;
   // public GameObject howToPlay;

    // Character ref to aid in persistant data
    private Character character;
    private GameObject pausePrompt;

    public static UI Instance;

    private bool _isPaused;

    public bool PausedStatus { get { return _isPaused; } }

    private GameObject blackScreen; //By Warren

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;

        character = GameObject.FindGameObjectWithTag("Player")
                                .GetComponent<Character>();
        pausePrompt = mainMenuPrompt.transform.parent.gameObject;
    }

    private void Start()
    {
        Application.targetFrameRate = Screen.currentResolution.refreshRate;


        Time.timeScale = 1;

        blackScreen = GameObject.FindGameObjectWithTag("ScreenShift"); //By Warren
    }

    // Update is called once per frame
    void Update()
    {
        //Altered by Warren - Input Manager's default "Cancel" was already ESC
        //if (Input.GetKeyDown(KeyCode.Escape))
        if (Input.GetButtonDown("Cancel"))
        {
            if (_isPaused)
            {
                UnPause();
            }
            else
            {
                
                Pause();
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }



    /// <summary>
    /// Pause Game
    /// </summary>
    public void Pause()
    {
        ShowMenu(pauseBG);
        ShowMenu(pauseMenu);
        _isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    /// <summary>
    /// Unpause Game
    /// </summary>
    public void UnPause()
    {
        HideAll();
        Hide(pauseBG);
        _isPaused = false;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (optionMenu.activeInHierarchy)
            ToggleOptions(false);
    }

    /// <summary>
    /// Disables the Specified Menu
    /// </summary>
    /// <param name="menuObject">Menu to Disable</param>
    public void Hide(GameObject menuObject)
    {
        menuObject.SetActive(false);
    }

    /// <summary>
    /// Hides All Menus
    /// </summary>
    public void HideAll()
    {
        //Hide(controlMenu);
        Hide(pauseMenu);
        Hide(quitPrompt);
        Hide(mainMenuPrompt);
        Hide(controlsPrompt);
        // Hide(restartPrompt);
        // Hide(optionMenu);
        //Hide(howToPlay);
    }

    /// <summary>
    /// Restarts the Level
    /// </summary>
    public void Restart()
    {
        
        Debug.Log("Restart Level(Currently just loads current active scene for testing purposes");
        Time.timeScale = 1;
        SaveManager.instance.DeleteSave();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //StartCoroutine(LoadSceneCo(SceneManager.GetActiveScene().name));//By Warren
        //No fade here, has bugs
    }

    public void RetryAtCheckpoint()
    {
        Debug.Log("Restart Level(Currently just loads current active scene for testing purposes");
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //StartCoroutine(LoadSceneCo(SceneManager.GetActiveScene().name));//By Warren
        //No fade here, has bugs
    }

    /// <summary>
    /// Quits The Game
    /// </summary>
    public void Quit()
    {
        var curData = character.UpdateAndGetSaveData();
        StartCoroutine(SavedData.CutCurrentPlayTime(curData));

        //store values!!
        Debug.Log("Quit Game");
        //Application.Quit();
        StartCoroutine(ExitGameCo()); //By Warren
    }

    /// <summary>
    /// Either Returns to the Main Menu or Shows the Pause Menu based on value
    /// </summary>
    /// <param name="value">True = Go to Main Menu, False = Go Back to Pause Menu</param>
    public void ReturnToMain(bool value)
    {
        if (value)
        {
            // persisten data handling (by Christian)
            var curData = character.UpdateAndGetSaveData();
            StartCoroutine(SavedData.CutCurrentPlayTime(curData));

            Time.timeScale = 1;
            //SceneManager.LoadScene("MainMenu");
            StartCoroutine(LoadSceneCo("MainMenu"));//By Warren
            Debug.Log("Go To Main Menu");
        }
        else
        {
            ShowMenu(pauseMenu);
        }

    }

    /// <summary>
    /// Hides Other Menus and Shows the Menu that is Input
    /// </summary>
    /// <param name="menu"> Menu to Show</param>
    public void ShowMenu(GameObject menu)
    {
        //HideAll();
        menu.SetActive(true);
    }

    /// <summary>
    /// Opens Link Specified by input string "url"
    /// </summary>
    /// <param name="url">URL to be opened</param>
    public void OpenLink(string url)
    {
        Application.OpenURL(url);
    }

    /* FUNCTIONS ADDED BY CHRISTIAN */
    /// <summary>
    /// Enable or disable showing the options screen based on parameter.
    /// </summary>
    /// <param name="showing">Bool to activate Options.</param>
    public void ToggleOptions(bool showing) {
        if (!showing) {
            var oc = optionMenu.GetComponent<OptionsController>();
            SavedData.SetOptionsInScene(oc.PackControllerOptions());
            oc.SaveOptionsToCurrentData();
        }
        optionMenu.SetActive(showing);
        pausePrompt.SetActive(!showing);
    }

    /* FUNCTIONS ADDED BY WARREN */
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

    /*
    * Load Scene Co - By Warren 
    * Asks for a delay for a screen fade before loading the target level/menu 
    */
    public IEnumerator LoadSceneCo(string level)
    {
        yield return Delay();
        SceneManager.LoadScene(level);
    }//LoadSceneCo
    /*
    * Exit Game Co - By Warren 
    * Asks for a delay for a screen fade before closing the game
    */
    public IEnumerator ExitGameCo()
    {
        yield return Delay();
        Application.Quit();
    }//ExitGameCo
}