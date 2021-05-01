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

    private DisplayKeys _displayKeys;

    void Awake()
    {
        _displayKeys = gameObject.AddComponent(typeof (DisplayKeys)) as DisplayKeys;
    }

    // Start is called before the first frame update
    void Start()
    {
        string _jumpKey = _displayKeys.FirstKey("Jump");
        string _interactKey = _displayKeys.FirstKey("Interact");
        string _standardKey = _displayKeys.FirstKey("Standard");
        string _brambleKey = _displayKeys.FirstKey("Bramble");
        string _warpKey = _displayKeys.FirstKey("Warp");
        string _airburstKey = _displayKeys.FirstKey("Airburst");
        string _cancelKey = _displayKeys.FirstKey("Cancel");
        //string _movementKey = _displayKeys.FirstKey("Movement");
        string _movementKey = "WASD";
        //string _lookKey = _displayKeys.FirstKey("Look");
        string _fireKey = _displayKeys.FirstKey("Fire");
        string _crouchKey = _displayKeys.FirstKey("Crouch");
        string _zoomKey = _displayKeys.FirstKey("Zoom");

        controlsText.text = _movementKey + ": To Move\n" + 
            _interactKey + ": Interact\n" +
            _fireKey + ": Hold to Fire\n" + _zoomKey + ": Zoom\n" + 
            _jumpKey + ": Jump\n" + _cancelKey + ": Pause Game\n" +
           _standardKey + ", " + _brambleKey + ", "+ _warpKey + ", " +
           _airburstKey + ": Equip Arrows\n" +
             _crouchKey + ": Crouch";

        standText.text = 
            "Shoot targets with the normal arrow. It is on the " + _standardKey + " key.";
        brambText.text = 
            "Bind moving objects in place for seconds with the bramble arrow. " +
            "Switch using the " + _brambleKey + " key. Try in level 2.";
        warpText.text = 
            "Teleport to certain targets by hitting them with the warp arrow. " +
            "Switch using the " + _warpKey + " key. Needed to navigate level 5.";
        airText.text = 
            "Knock over light objects with the airburst arrow. " +
            "Switch using the " + _airburstKey + " key.";
    }
}
