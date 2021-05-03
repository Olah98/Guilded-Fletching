/*
Author: Warren Rose II
Data: 05/02/2021
Summary: Overview of controls, set to update if keys are rebound.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HowToPlayUI : MonoBehaviour
{
    public TMP_Text controlsText;
    public TMP_Text standText;
    public TMP_Text brambText;
    public TMP_Text warpText;
    public TMP_Text airText;
    public TMP_Text titleText;
    public TMP_Text buttonText;
    public GameObject[] items = new GameObject[2];

    private DisplayKeys _displayKeys;
    private bool _secondScreen;
    private string _firstTitle = "How To Play";
    private string _secondTitle = "Rebind Controls";

    /*
    * Awake
    * Creates local copy of DisplayKeys, before Start runs
    */
    void Awake()
    {
        _displayKeys = gameObject.AddComponent(typeof(DisplayKeys)) as DisplayKeys;
    }//Awake

    /*
     * Start
     * Preloads strings with the definitions provided by DisplayKeys
     * Fills out the TextMeshPro Text objects.
     */
    void Start()
    {
        //Simple Controls
        string _jumpKey = _displayKeys.FirstKey("Jump");
        string _interactKey = _displayKeys.FirstKey("Interact");
        string _cancelKey = _displayKeys.FirstKey("Cancel");
        string _fireKey = _displayKeys.FirstKey("Fire");
        string _zoomKey = _displayKeys.FirstKey("Zoom");

        //Controls with a main key and an alternate trigger
        string _standardKey = _displayKeys.FirstKey("Standard");
        string _standardKey2 = _displayKeys.SecondKey("Standard");
        string _brambleKey = _displayKeys.FirstKey("Bramble");
        string _brambleKey2 = _displayKeys.SecondKey("Bramble");
        string _warpKey = _displayKeys.FirstKey("Warp");
        string _warpKey2 = _displayKeys.SecondKey("Warp");
        string _airburstKey = _displayKeys.FirstKey("Airburst");
        string _airburstKey2 = _displayKeys.SecondKey("Airburst");
        string _crouchKey = _displayKeys.FirstKey("Crouch");
        string _crouchKey2 = _displayKeys.SecondKey("Crouch");

        //[0] and [5] display "2D Axis" as that is the type of control
        string _upKey = _displayKeys.ByPart("Movement", 1);
        string _downKey = _displayKeys.ByPart("Movement", 2);
        string _leftKey = _displayKeys.ByPart("Movement", 3);
        string _rightKey = _displayKeys.ByPart("Movement", 4);
        string _upKey2 = _displayKeys.ByPart("Movement", 6);
        string _downKey2 = _displayKeys.ByPart("Movement", 7);
        string _leftKey2 = _displayKeys.ByPart("Movement", 8);
        string _rightKey2 = _displayKeys.ByPart("Movement", 9);

        controlsText.text = "<b>To Move:</b> " + _upKey + " or " + _upKey2 + ", " +
            _downKey + " or " + _downKey2 + ", " + _leftKey + " or " + _leftKey2 + ", "
            + _rightKey + " or " + _rightKey2 + "\n" + "<b>Interact:</b> " + _interactKey + "\n" +
            "<b>Hold to Fire:</b> " + _fireKey + "\n" + "<b>Zoom:</b> " + _zoomKey + "\n" +
            "<b>Jump:</b> " + _jumpKey + "\n" + "<b>Pause Game:</b> " + _cancelKey + "\n" +
            "<b>Equip Special Arrows:</b> " + _standardKey + " or " + _standardKey2 + ", " +
            _brambleKey + " or " + _brambleKey2 + ", " + _warpKey + " or " + _warpKey2 + ", "
            + _airburstKey + " or " + _airburstKey2 + "\n" + "<b>Crouch:</b> " + _crouchKey +
            " or " + _crouchKey2;

        standText.text =
            "Shoot targets with the normal arrow. It is on the " + _standardKey + " key.";
        brambText.text =
            "Bind moving objects in place for seconds with the bramble arrow. " +
            "Switch using the " + _brambleKey + " key. Try in level 4.";
        warpText.text =
            "Teleport to certain targets by hitting them with the warp arrow. " +
            "Switch using the " + _warpKey + " key. Needed to navigate level 5.";
        airText.text =
            "Knock over light objects with the airburst arrow. " +
            "Switch using the " + _airburstKey + " key.";
    }//Start

    public void ToggleMenus()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].active = !items[i].active;
        }
        _secondScreen = !_secondScreen;

        if (_secondScreen)
        {
            titleText.text = _secondTitle;
            buttonText.text = _firstTitle;
        } else
        {
            titleText.text = _firstTitle;
            buttonText.text = _secondTitle;
        }
    }

}//HowToPlayUI
