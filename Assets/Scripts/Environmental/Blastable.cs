/*
Author: Christian Mullins
Date: 4/04/2021
Summary: Script to easily discern how Airburst Arrow's blast will handle
    objects inside its blast radius.
*/
using UnityEngine;

/*
    TO DO:
        -Check if the MonoBehavior interface is required for Inspector 
        interaction.
*/
//[System.Serializable]
public class Blastable : MonoBehaviour {
    [HideInInspector] public  bool isDestroyable => _isDestroyable;
    [SerializeField]  private bool _isDestroyable;
}
