/*
Author: Warren Rose II
Data: 04/12/2021
Summary: Reporting the end of level stats saved in SaveManager.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class LevelEndReport : MonoBehaviour
{
    public Button nextLevelButton;
    public TMP_Text report;
    private int[,] _lastLoadout;
    private float _lastTime;
    private string _lastLocation;
    private string _nextLocation = "";

    //enum for checking stats
    enum Ammo : int
    {
        Standard, Bramble, Warp, Airburst
    }

    //Bay for Constants
    //UNREADY and READY track the accesibility of the arrow types
    //ACCESS and RECORD are shortcuts for the loadout array slots
    public const int UNREADY = 3, READY = 4, ACCESS = 0, RECORD = 1;

    /*
    * Start
    * Start is called before the first frame update
    * If a SaveManager is found and it shows temp values from  a cleared level,
    * this clears the read values and generates a report
    */
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (GameObject.FindGameObjectWithTag("Save Manager") == null)
        {
            Debug.Log("Failed to load report");
            return;
        }
        if (SaveManager.instance.levelBeaten == true)
        {
            _lastLoadout = SaveManager.instance.lastLoadout;
            _lastTime = SaveManager.instance.lastTime;
            _lastLocation = SaveManager.instance.lastLocation;
            _nextLocation = SaveManager.instance.nextLocation;
            SaveManager.instance.ResetLevelEndStats();
            if ((_nextLocation != "") && (_nextLocation != "MainMenu"))
            {
                nextLevelButton.interactable = true;
            }
            var minutes = _lastTime / 60;
            var seconds = _lastTime % 60;

            report.text = "You completed the level in " + 
                string.Format("{0:00} : {1:00}", minutes, seconds) + "\n" +
                "Standard Arrows Used: " + 
                    GetArrowTypeShot((int)Ammo.Standard) + "\n" +
                "Bramble Arrows Used: " + 
                    GetArrowTypeShot((int)Ammo.Bramble) + "\n" +
                "Warp Arrows Used: " + 
                    GetArrowTypeShot((int)Ammo.Warp) + "\n" +
                "Airburst Arrows Used: " + 
                    GetArrowTypeShot((int)Ammo.Airburst) + "\n";
        } else
        {
            Debug.Log("No report saved");
        }
    }//Start

    /*
    * Next Level
    * Heads to the next room if one is loaded
    * This button is disabled unless a location has been specified
    */
    public void NextLevel()
    {
        this.GetComponent<MainMenuButtons>().LoadScene(_nextLocation);
    }//NextLevel

    /*
    * Get Arrow Type Shot (Int Overload)
    * Returns int value of selected ammo type used this loadout
    */
    public int GetArrowTypeShot(int arrows)
    {
        if (arrows == (int)Ammo.Standard)
        {
            return _lastLoadout[(int)Ammo.Standard, RECORD];
        }
        else if (arrows == (int)Ammo.Bramble)
        {
            return _lastLoadout[(int)Ammo.Bramble, RECORD];
        }
        if (arrows == (int)Ammo.Warp)
        {
            return _lastLoadout[(int)Ammo.Warp, RECORD];
        }
        if (arrows == (int)Ammo.Airburst)
        {
            return _lastLoadout[(int)Ammo.Airburst, RECORD];
        }
        else
        {
            Debug.Log("Invalid arrow type.");
            return -1;
        }
    }//GetArrowTypeShot
}//LevelEndReport
