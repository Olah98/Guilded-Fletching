/*
Author: Christian Mullins
Date: 02/15/2021
Summary: The class called from the Switch to open the Door's GameObject.
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Door : MonoBehaviour {
    public float openSpeed;
    [HideInInspector]
    public List<Switch> mySwitches;
    private Transform slideTo;

    //By Warren
    //Edit to change whether color or material is being affected.
    //public Material m_isOn;
    //public Material m_isOff;
    private Color isOn = Color.green;
    private Color isOff = Color.red;
    private MeshRenderer rend;


    void Start() {
        slideTo = transform.GetChild(0);
       
        //By Warren
        rend = GetComponent<MeshRenderer>();
        UpdateColor(true);
        IsAllSwitchesFlipped();
    }

    /// <summary>
    /// Iterate through every Switch to check if they are all flipped.
    /// </summary>
    /// <returns>True if all switches are flipped.</returns>
    public bool IsAllSwitchesFlipped() {
        foreach (var s in mySwitches) {
            if (!s.isFlipped)
            {
                UpdateColor(false);
                return false;
            }
        }
        UpdateColor(true);
        return true;
    }

    /// <summary>
    /// Coroutine to move the Door outside of using Update().
    /// </summary>
    public IEnumerator SlideOpen() {
        // detach slideTo child
        slideTo.parent = null;
        do {
            Vector3 moveTo = (slideTo.position - transform.position).normalized;
            transform.Translate(moveTo * openSpeed * Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
        } while (!IsPositionApproximateTo(slideTo.position));
        // re-attach slideTo child
        slideTo.parent = transform;
        yield return null;
    }

    /// <summary>
    /// Substitute this in for "==" between Vector3s, operator is too exact.
    /// </summary>
    /// <param name="compareTo">Destination position.</param>
    /// <returns>True if the distance is almost 0f.</returns>
    private bool IsPositionApproximateTo(Vector3 compareTo) {
        if (Vector3.Distance(transform.position, compareTo) < 0.25f) 
            return true;

        return false;
    }

    /// <summary>
    /// Function to call when changing color or material.
    /// Edit to change whether color or material is being affected.
    /// By Warren
    /// </summary>
    public void UpdateColor(bool isActive)
    {
        Color change;
        //Material m_change;
        if (isActive)
        {
            change = isOn;
            //m_change = m_isOn;
        }
        else
        {
            change = isOff;
            //m_change = m_isOff;
        }
        if (rend != null)
        {
            rend.material.SetColor("_Color", change);
            //rend.material = m_change;
        }
    }
}