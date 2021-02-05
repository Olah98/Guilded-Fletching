/*
Author: Warren Rose II
Data: 02/04/2021
Summary: Quiver loading and storage.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quiver : MonoBehaviour
{

    //enum for checking stats
    enum Ammo : int
    {
        Standard, Bramble, Warp, Airburst
    }

    //Bay for Constants
    public const int CURRENT = 0, MAX = 1, MAX_INVENTORY = 99;

    /*
    * Start
    * //
    */
    void Start()
    {
        
    }


    /*
    * FixedUpdate 
    * Called once per fixed framerate frame
    * //
    */
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {

        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {

        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {

        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {

        }
    }
}
