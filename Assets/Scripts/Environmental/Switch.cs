/*
Author: Christian Mullins
Date: 02/15/2021
Summary: Class that interacts with and triggers the Door script class.
*/
using UnityEngine;

public class Switch : MonoBehaviour {
    [Tooltip("Can be any object that needs a triggered movement.")]
    public Door myDoor; // can be a door or ladder
    public bool isFlipped;

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


        // push this Switch into the Switch List for myDoor
        myDoor.mySwitches.Add(this);

        if (!isFlipped)
        {
            myDoor.UpdateColor(false); //By Warren, updates color
        }
    }

    /// <summary>
    /// Function to call when switch is hit by an arrow or interacting with.
    /// </summary>
    public void HitSwitch() {
        if (isFlipped) return;

        isFlipped = true;
        UpdateColor();//By Warren
        if (myDoor.IsAllSwitchesFlipped()) {
            StartCoroutine(myDoor.SlideOpen());
        }
    }

    /// <summary>
    /// Check if an arrow hits the switch, then activate.
    /// </summary>
    /// <param name="other">Object hitting the switch.</param>
    private void OnCollisionEnter(Collision other) {
        if (other.transform.tag == "Arrow") {
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
}