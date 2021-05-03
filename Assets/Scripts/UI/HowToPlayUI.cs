/*
Author: Warren Rose II
Data: 05/02/2021
Summary: Overview of controls, set to update if keys are rebound.
    In the Main Menu Controls page, this allows rebinding of keys.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HowToPlayUI : MonoBehaviour
{
    //Needed for general key bindings to text
    private DisplayKeys _displayKeys;

    //Simple Controls
    private string _jumpKey, _interactKey, _cancelKey;
    private string _fireKey, _zoomKey;

    //Controls with a main key and an alternate trigger
    private string _standardKey, _standardKey2, _brambleKey;
    private string _brambleKey2, _warpKey, _warpKey2, _airburstKey;
    private string _airburstKey2, _crouchKey, _crouchKey2;

    //[0] and [5] display "2D Axis" as that is the type of control
    private string _upKey, _downKey, _leftKey, _rightKey;
    private string _upKey2, _downKey2, _leftKey2, _rightKey2;


    //Specific to Controls UI
    public TMP_Text controlsText;
    public TMP_Text standText;
    public TMP_Text brambText;
    public TMP_Text warpText;
    public TMP_Text airText;

    //Specific to Main Menu Controls UI
    public TMP_Text titleText;
    public TMP_Text buttonText;
    public GameObject[] items = new GameObject[5];
    public TMP_Text[] refs = new TMP_Text[23];
    private bool _secondScreen;
    private string _firstTitle = "How To Play";
    private string _secondTitle = "Rebind Controls";

    /*
    * Awake
    * Creates local copy of DisplayKeys, before Start runs
    */
    void Awake()
    {
        //Needed for general key bindings to text
        _displayKeys = gameObject.AddComponent(typeof(DisplayKeys)) as DisplayKeys;
    }//Awake

    /*
     * Start
     * Preloads strings with the definitions provided by DisplayKeys
     * Fills out the TextMeshPro Text objects.
     */
    void Start()
    {
        UpdateStrings();
        UpdateText();
    }//Start

    /*
    * Toggle Menus
    * Turns on and off the visibilty of menu components
    * Asks for the strings to be updated across the UI
    * This only works on the Main Menu Controls page
    * Fortunately, it's called by a button that
    *   doesn't exist inside of the Pause UI Prefab
    */
    public void ToggleMenus()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].SetActive(!items[i].activeSelf);

        }

        _secondScreen = !_secondScreen;

        if (_secondScreen)
        {
            titleText.text = _secondTitle;
            buttonText.text = _firstTitle;
        }
        else
        {
            titleText.text = _firstTitle;
            buttonText.text = _secondTitle;
        }

        UpdateStrings();
        UpdateText();
    }//ToggleMenus

    /*
    * Update Strings
    * Preloads strings with the definitions provided by DisplayKeys
    */
    public void UpdateStrings()
    {
        //Needed for general key bindings to text

        //Simple Controls
        _jumpKey = _displayKeys.FirstKey("Jump");
        _interactKey = _displayKeys.FirstKey("Interact");
        _cancelKey = _displayKeys.FirstKey("Cancel");
        _fireKey = _displayKeys.FirstKey("Fire");
        _zoomKey = _displayKeys.FirstKey("Zoom");

        //Controls with a main key and an alternate trigger
        _standardKey = _displayKeys.FirstKey("Standard");
        _standardKey2 = _displayKeys.SecondKey("Standard");
        _brambleKey = _displayKeys.FirstKey("Bramble");
        _brambleKey2 = _displayKeys.SecondKey("Bramble");
        _warpKey = _displayKeys.FirstKey("Warp");
        _warpKey2 = _displayKeys.SecondKey("Warp");
        _airburstKey = _displayKeys.FirstKey("Airburst");
        _airburstKey2 = _displayKeys.SecondKey("Airburst");
        _crouchKey = _displayKeys.FirstKey("Crouch");
        _crouchKey2 = _displayKeys.SecondKey("Crouch");

        //[0] and [5] display "2D Axis" as that is the type of control
        _upKey = _displayKeys.ByPart("Movement", 1);
        _downKey = _displayKeys.ByPart("Movement", 2);
        _leftKey = _displayKeys.ByPart("Movement", 3);
        _rightKey = _displayKeys.ByPart("Movement", 4);
        _upKey2 = _displayKeys.ByPart("Movement", 6);
        _downKey2 = _displayKeys.ByPart("Movement", 7);
        _leftKey2 = _displayKeys.ByPart("Movement", 8);
        _rightKey2 = _displayKeys.ByPart("Movement", 9);
    }//UpdateStrings

    /*
    * Update Text
    * Fills out the TextMeshPro Text objects.
    */
    public void UpdateText()
    {
        //Specific to Controls UI

        if (!_secondScreen)
        {
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
        }
        else
        {
            refs[0].text = _upKey; refs[1].text = _downKey;
            refs[2].text = _leftKey; refs[3].text = _rightKey;
            refs[4].text = _upKey2; refs[5].text = _downKey2;
            refs[6].text = _leftKey2; refs[7].text = _rightKey2;

            refs[8].text = _standardKey; refs[9].text = _brambleKey;
            refs[10].text = _warpKey; refs[11].text = _airburstKey;
            refs[12].text = _standardKey2; refs[13].text = _brambleKey2;
            refs[14].text = _warpKey2; refs[15].text = _airburstKey2;

            refs[16].text = _jumpKey; refs[17].text = _interactKey;
            refs[18].text = _cancelKey; refs[19].text = _fireKey;
            refs[20].text = _zoomKey;
            refs[21].text = _crouchKey; refs[22].text = _crouchKey2;
        }
    }//UpdateText
}//HowToPlayUI
