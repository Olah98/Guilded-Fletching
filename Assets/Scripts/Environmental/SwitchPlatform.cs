/*
Author: Christian Mullins & Warren Rose II
Date: 03/23/2021
Summary: Class that interacts with and triggers the CountingPlatform script class.
*/
using UnityEngine;

public class SwitchPlatform : MonoBehaviour {
    [Tooltip("Can be any object that needs a triggered movement.")]
    public CountingPlatform myPlatform; // can be a platform
    public bool isFlipped;
    [Tooltip("Check if the player is able to interact with this, or it can" +
            " only be triggered by arrows.")]
    public bool isInteractable;
    public bool triggerByArrow;

    //By Warren
    //Edit to change whether color or material is being affected.
    //public Material m_isOn;
    //public Material m_isOff;
    private Color isOn = Color.green;
    private Color isOff = Color.red;
    private MeshRenderer rend;

    void Start() {

        //By Warren
        rend = GetComponent<MeshRenderer>();
        UpdateColor();


        // push this Switch into the Switch List for myPlatform
       // myPlatform.mySwitches.Add(this);

        if (!isFlipped)
        {
            myPlatform.UpdateColor(false); //By Warren, updates color
        }
    }

    /// <summary>
    /// Function to call when switch is hit by an arrow or interacting with.
    /// </summary>
    public void HitSwitch() {
        if (isFlipped) return;

        isFlipped = true;
        UpdateColor();//By Warren
    }

    /// <summary>
    /// Check if an arrow hits the switch, then activate.
    /// </summary>
    /// <param name="other">Object hitting the switch.</param>
    private void OnCollisionEnter(Collision other) {
        if (other.transform.tag == "Arrow" && triggerByArrow) {
            HitSwitch();
        }
    }

    /// <summary>
    /// Function to call when changing color or material.
    /// Edit to change whether color or material is being affected.
    /// By Warren
    /// </summary>
    private void UpdateColor()
    {
        Color change;
        //Material m_change;
        if (isFlipped)
        {
            change = isOn;
            //m_change = m_isOn;
        } else
        {
            change = isOff;
            //m_change = m_isOff;
        }
        rend.material.SetColor("_Color", change);
        //rend.material = m_change;
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
}