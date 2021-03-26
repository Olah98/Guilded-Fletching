/*
Author: Christian Mullins
Date: 02/15/2021
Summary: Class that interacts with and triggers the Door script class.
*/
using UnityEngine;
using UnityEditor;

public enum SwitchType {
    DoorOrLadder, Bridge, Platform
}

[System.Serializable]
public class Switch : MonoBehaviour {
    [HideInInspector] public Door myDoor = null;
    [HideInInspector] public Bridge myBridge = null;
    [HideInInspector] public CountingPlatform myPlatform = null;
    
    public bool isFlipped;
    [Tooltip("Check if the player is able to interact with this, or it can" +
            " only be triggered by arrows.")]
    public bool isInteractable;
    public bool triggerByArrow;

    //By Warren
    //Edit to change whether color or material is being affected.
    private Color _isOn = Color.green;
    private Color _isOff = Color.red;
    private MeshRenderer _rend;

    private void Start() {

        //By Warren
        _rend = GetComponent<MeshRenderer>();
        UpdateColor();

        // push this Switch into the Switch list of the object we're switching
        // if it exists
        myDoor?.mySwitches.Add(this);
        myBridge?.mySwitches.Add(this);
        myPlatform?.mySwitches.Add(this);


        if (!isFlipped) {
            myDoor?.UpdateColor(false); //By Warren, updates color
            myBridge?.UpdateColor(false);
            myPlatform?.UpdateColor(false);
        }
    }

    /// <summary>
    /// Function to call when switch is hit by an arrow or interacting with.
    /// </summary>
    public virtual void HitSwitch() {
        if (isFlipped) return;

        isFlipped = true;
        UpdateColor();//By Warren
        // handle what is 
        if (myDoor != null && myDoor.IsAllSwitchesFlipped()) {
            StartCoroutine(myDoor.Open());
        }
        if (myBridge != null && myBridge.IsAllSwitchesFlipped()) {
            StartCoroutine(myBridge.Open());
        }
        if (myPlatform != null && myPlatform.IsAllSwitchesFlipped()) {
            myPlatform.UnlockPlatform();
        }
    }
    
    /// <summary>
    /// Function to call when switch is reset, such as by a timer running out.
    /// By Warren
    /// </summary>
    public void ResetSwitch()
    {
        if (!isFlipped) return;

        isFlipped = false;
        UpdateColor();
    }

    /// <summary>
    /// Check if an arrow hits the switch, then activate.
    /// </summary>
    /// <param name="other">Object hitting the switch.</param>
    protected void OnCollisionEnter(Collision other) {
        if (other.transform.tag == "Arrow" && triggerByArrow) {
            HitSwitch();
        }
    }

    /// <summary>
    /// Function to call when changing color or material.
    /// Edit to change whether color or material is being affected.
    /// By Warren
    /// </summary>
    protected void UpdateColor() {
        Color change;
        if (isFlipped) {
            change = _isOn;
        } 
        else {
            change = _isOff;
        }
        _rend.material.SetColor("_Color", change);
    }
}

// editor class to hand Inspector UI for the Switch class
[CustomEditor(typeof(Switch))]
public class SwitchEditor : Editor {
    public SwitchType switchType;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        // create enum property
        switchType = (SwitchType) EditorGUILayout.EnumPopup("SwitchType", switchType);
        // set display to selected enum
        switch (switchType) {
            case SwitchType.DoorOrLadder:
                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("myDoor"));
                break;
            case SwitchType.Bridge:
                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("myBridge"));
                break;
            case SwitchType.Platform:
                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("myPlatform"));
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }
}