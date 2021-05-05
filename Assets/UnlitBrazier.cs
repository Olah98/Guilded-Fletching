/*
Author: Warren Rose II
Data: 05/04/2021
Summary: When the part of the UnlitBrazier that is tagged as Burnable is destroyed
    by an arrow on fire, turn on the mesh renderer of its parent and enable Fire FX.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlitBrazier : MonoBehaviour
{
    public GameObject fire;
    public GameObject fireAudio;
    private MeshRenderer _parentMeshRenderer;

    /*
    * Start
    * Grab MeshRenderer Component
    */
    public void Start()
    {
        _parentMeshRenderer = transform.parent.GetComponent<MeshRenderer>();
    }//Start

    /*
    * OnDestroy
    * Triggered by the logic of the arrow on fire hitting a Burnable Tag
    */
    public void OnDestroy()
    {
        fire.SetActive(true);
        fireAudio.SetActive(true);
        _parentMeshRenderer.enabled = true;
    }//OnDestroy
}//UnlitBrazier
