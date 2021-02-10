﻿/*
Author: Warren Rose II
Data: 02/04/2021
Summary: Quiver loading and storage.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quiver : MonoBehaviour
{

    //First segment is Ammo Type
    //Second segment is the integer values for current and max capacity
    private int[,] loadout = new int[4, 2];
    private int equipped;

    //enum for checking stats
    enum Ammo : int
    {
        Standard, Bramble, Warp, Airburst
    }

    //Bay for Constants
    //CURRENT and MAX are array indexes
    //TEST and MAX_CAPACITY are manually set values, please change as needed
    public const int CURRENT = 0, MAX = 1, TEST = 25, MAX_CAPACITY = 99;

    /*
    * Start
    * Initializing loadout
    */
    void Start()
    {
        //Initial loadout
        //Load("Empty"); //Main Gameplay
        Load("All"); //For testing purposes, sets ammo to the above TEST value

        //Base value
        equipped = (int)Ammo.Standard;
    }//Start


    /*
    * Update 
    * Called once per frame
    * Listens for keyboard selection, changes equipped selection based on ammo
    */
    void Update()
    {
        CheckKeyboardInput();
    }//Update

    /*
    * Check Keyboard Input
    * Called by Update
    * Listens for keyboard selection, changes equipped selection based on ammo
    */
    void CheckKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)
            || Input.GetKeyDown(KeyCode.Keypad1))
        {
            if (loadout[(int)Ammo.Standard, CURRENT] > 0)
            {
                if (equipped != (int)Ammo.Standard)
                {
                    equipped = (int)Ammo.Standard;
                    Debug.Log("Equipped " +
                        loadout[(int)Ammo.Standard, CURRENT] +
                        " Standard arrows");
                    return; //handles key-mashing
                }
                else
                {
                    Debug.Log(loadout[(int)Ammo.Standard, CURRENT] + 
                        " Standard type arrows already equipped");
                }
            }
            else
            {
                Debug.Log("You're out of Standard arrows");
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)
            || Input.GetKeyDown(KeyCode.Keypad2))
        {
            if (loadout[(int)Ammo.Bramble, CURRENT] > 0)
            {
                if (equipped != (int)Ammo.Bramble)
                {
                    equipped = (int)Ammo.Bramble;
                    Debug.Log("Equipped " +
                        loadout[(int)Ammo.Bramble, CURRENT] +
                        " Bramble arrows");
                    return; //handles key-mashing
                }
                else
                {
                    Debug.Log(loadout[(int)Ammo.Bramble, CURRENT] +
                        " Bramble type arrows already equipped");
                }
            }
            else
            {
                Debug.Log("You're out of Bramble arrows");
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)
            || Input.GetKeyDown(KeyCode.Keypad3))
        {
            if (loadout[(int)Ammo.Warp, CURRENT] > 0)
            {
                if (equipped != (int)Ammo.Warp)
                {
                    equipped = (int)Ammo.Warp;
                    Debug.Log("Equipped " +
                        loadout[(int)Ammo.Warp, CURRENT] +
                        " Warp arrows");
                    return; //handles key-mashing
                }
                else
                {
                    Debug.Log(loadout[(int)Ammo.Warp, CURRENT] +
                        " Warp type arrows already equipped");
                }
            }
            else
            {
                Debug.Log("You're out of Warp arrows");
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)
            || Input.GetKeyDown(KeyCode.Keypad4))
        {
            if (loadout[(int)Ammo.Airburst, CURRENT] > 0)
            {
                if (equipped != (int)Ammo.Airburst)
                {
                    equipped = (int)Ammo.Airburst;
                    Debug.Log("Equipped " +
                        loadout[(int)Ammo.Airburst, CURRENT] +
                        " Airburst arrows");
                    return; //handles key-mashing
                }
                else
                {
                    Debug.Log(loadout[(int)Ammo.Airburst, CURRENT] +
                        " Airburst type arrows already equipped");
                }
            }
            else
            {
                Debug.Log("You're out of Airburst arrows");
            }
        }
    }//CheckKeyboardInput


    /*
    * Load
    * Fills out the current loadout based on called parameters
    */
    void Load(string arrows)
    {
        if (arrows == "Empty")
        {
            //Initial loadout
            loadout[(int)Ammo.Standard, CURRENT] = 0;
            loadout[(int)Ammo.Standard, MAX] = MAX_CAPACITY;
            loadout[(int)Ammo.Bramble, CURRENT] = 0;
            loadout[(int)Ammo.Bramble, MAX] = MAX_CAPACITY;
            loadout[(int)Ammo.Warp, CURRENT] = 0;
            loadout[(int)Ammo.Warp, MAX] = MAX_CAPACITY;
            loadout[(int)Ammo.Airburst, CURRENT] = 0;
            loadout[(int)Ammo.Airburst, MAX] = MAX_CAPACITY;
            Debug.Log("Load(Empty). Quiver is empty";
        }
        else if (arrows == "Standard")
        {
            //Initial loadout
            loadout[(int)Ammo.Standard, CURRENT] = TEST;
            loadout[(int)Ammo.Standard, MAX] = MAX_CAPACITY;
            loadout[(int)Ammo.Bramble, CURRENT] = 0;
            loadout[(int)Ammo.Bramble, MAX] = MAX_CAPACITY;
            loadout[(int)Ammo.Warp, CURRENT] = 0;
            loadout[(int)Ammo.Warp, MAX] = MAX_CAPACITY;
            loadout[(int)Ammo.Airburst, CURRENT] = 0;
            loadout[(int)Ammo.Airburst, MAX] = MAX_CAPACITY;
            Debug.Log("Load(Standard). Selected type, " +
               "default TEST amount of " + TEST);
        }
        else if (arrows == "Bramble")
        {
            //Initial loadout
            loadout[(int)Ammo.Standard, CURRENT] = 0;
            loadout[(int)Ammo.Standard, MAX] = MAX_CAPACITY;
            loadout[(int)Ammo.Bramble, CURRENT] = TEST;
            loadout[(int)Ammo.Bramble, MAX] = MAX_CAPACITY;
            loadout[(int)Ammo.Warp, CURRENT] = 0;
            loadout[(int)Ammo.Warp, MAX] = MAX_CAPACITY;
            loadout[(int)Ammo.Airburst, CURRENT] = 0;
            loadout[(int)Ammo.Airburst, MAX] = MAX_CAPACITY;
            Debug.Log("Load(Bramble). Selected type, " +
               "default TEST amount of " + TEST);
        }
        else if (arrows == "Warp")
        {
            //Initial loadout
            loadout[(int)Ammo.Standard, CURRENT] = 0;
            loadout[(int)Ammo.Standard, MAX] = MAX_CAPACITY;
            loadout[(int)Ammo.Bramble, CURRENT] = 0;
            loadout[(int)Ammo.Bramble, MAX] = MAX_CAPACITY;
            loadout[(int)Ammo.Warp, CURRENT] = TEST;
            loadout[(int)Ammo.Warp, MAX] = MAX_CAPACITY;
            loadout[(int)Ammo.Airburst, CURRENT] = 0;
            loadout[(int)Ammo.Airburst, MAX] = MAX_CAPACITY;
            Debug.Log("Load(Warp). Selected type, " +
               "default TEST amount of " + TEST);
        }
        else if (arrows == "Airburst")
        {
            //Initial loadout
            loadout[(int)Ammo.Standard, CURRENT] = 0;
            loadout[(int)Ammo.Standard, MAX] = MAX_CAPACITY;
            loadout[(int)Ammo.Bramble, CURRENT] = 0;
            loadout[(int)Ammo.Bramble, MAX] = MAX_CAPACITY;
            loadout[(int)Ammo.Warp, CURRENT] = 0;
            loadout[(int)Ammo.Warp, MAX] = MAX_CAPACITY;
            loadout[(int)Ammo.Airburst, CURRENT] = TEST;
            loadout[(int)Ammo.Airburst, MAX] = MAX_CAPACITY;
            Debug.Log("Load(Airburst). Selected type, " +
               "default TEST amount of " + TEST);
        }
        else if (arrows == "All")
        {
            //Initial loadout
            loadout[(int)Ammo.Standard, CURRENT] = TEST;
            loadout[(int)Ammo.Standard, MAX] = MAX_CAPACITY;
            loadout[(int)Ammo.Bramble, CURRENT] = TEST;
            loadout[(int)Ammo.Bramble, MAX] = MAX_CAPACITY;
            loadout[(int)Ammo.Warp, CURRENT] = TEST;
            loadout[(int)Ammo.Warp, MAX] = MAX_CAPACITY;
            loadout[(int)Ammo.Airburst, CURRENT] = TEST;
            loadout[(int)Ammo.Airburst, MAX] = MAX_CAPACITY;
            Debug.Log("Load(All). All four types, " +
                "default TEST amount of " + TEST);
        }
        else if (arrows == "Max")
        {
            //Initial loadout
            loadout[(int)Ammo.Standard, CURRENT] = MAX_CAPACITY;
            loadout[(int)Ammo.Standard, MAX] = MAX_CAPACITY;
            loadout[(int)Ammo.Bramble, CURRENT] = MAX_CAPACITY;
            loadout[(int)Ammo.Bramble, MAX] = MAX_CAPACITY;
            loadout[(int)Ammo.Warp, CURRENT] = MAX_CAPACITY;
            loadout[(int)Ammo.Warp, MAX] = MAX_CAPACITY;
            loadout[(int)Ammo.Airburst, CURRENT] = MAX_CAPACITY;
            loadout[(int)Ammo.Airburst, MAX] = MAX_CAPACITY;
            Debug.Log("Load(Max). All four types, " +
                "MAX_CAPACITY of " + MAX_CAPACITY);
        }
        else
        {
            Debug.Log("Invalid Load(). Try the ammo type, Empty, All, or Max");
        }
    }//Load

    /*
    * Get Arrow Type
    * Returns the equipped type as int value
    * Standard = 1, Bramble = 2, Warp = 3, Airburst = 4
    */
    public int GetArrowType()
    {
        return (equipped + 1);
    }//GetArrowType

    /*
    * Get Arrow Number
    * Returns int value of equipped ammo type
    */
    public int GetArrowNumber()
    {
        return loadout[equipped, CURRENT];
    }//GetArrowNumber

    /*
    * Get Arrow Number Max
    * Returns maximum carryable of equipped ammo type
    */
    public int GetArrowNumberMax()
    {
        return loadout[equipped, MAX];
    }//GetArrowNumberMax

    /*
    * Fire
    * If ammo is available, this returns true and decreases count by 1
    * Returns false otherwise
    */
    bool Fire()
    {
        bool success = false;
        switch (equipped)
        {
            case (int)Ammo.Standard:
                success = FireStandard();
                break;
            case (int)Ammo.Bramble:
                success = FireBramble();
                break;
            case (int)Ammo.Airburst:
                success = FireAirburst();
                break;
            case (int)Ammo.Warp:
                success = FireWarp();
                break;
            default:
                break;
        }
        return success;
    }//Fire

    /*
    * Fire Ammo Type
    * If ammo is available, this returns true and decreases count by 1
    * Returns false otherwise
    */
    bool FireStandard()
    {
        if (loadout[(int)Ammo.Standard, CURRENT] > 0)
        {
            loadout[(int)Ammo.Standard, CURRENT]--;
            return true;
        }
        return false;
    }

    bool FireBramble()
    {
        if (loadout[(int)Ammo.Bramble, CURRENT] > 0)
        {
            loadout[(int)Ammo.Bramble, CURRENT]--;
            return true;
        }
        return false;
    }

    bool FireAirburst()
    {
        if (loadout[(int)Ammo.Airburst, CURRENT] > 0)
        {
            loadout[(int)Ammo.Airburst, CURRENT]--;
            return true;
        }
        return false;
    }

    bool FireWarp()
    {
        if (loadout[(int)Ammo.Warp, CURRENT] > 0)
        {
            loadout[(int)Ammo.Warp, CURRENT]--;
            return true;
        }
        return false;
    }//End Fire Ammo Type

    /*
    * Add Ammo Type
    * Checks incoming number and limits new loadout to max carryable
    */
    void AddStandard(int count)
    {
        if (loadout[(int)Ammo.Standard, CURRENT] + count < loadout[(int)Ammo.Standard, MAX])
        {
            loadout[(int)Ammo.Standard, CURRENT] += count;
        }
        else
        {
            loadout[(int)Ammo.Standard, CURRENT] = loadout[(int)Ammo.Standard, MAX];
        }
    }

    void AddBramble(int count)
    {
        if (loadout[(int)Ammo.Bramble, CURRENT] + count < loadout[(int)Ammo.Bramble, MAX])
        {
            loadout[(int)Ammo.Bramble, CURRENT] += count;
        }
        else
        {
            loadout[(int)Ammo.Bramble, CURRENT] = loadout[(int)Ammo.Bramble, MAX];
        }
    }

    void AddWarp(int count)
    {
        if (loadout[(int)Ammo.Warp, CURRENT] + count < loadout[(int)Ammo.Warp, MAX])
        {
            loadout[(int)Ammo.Warp, CURRENT] += count;
        }
        else
        {
            loadout[(int)Ammo.Warp, CURRENT] = loadout[(int)Ammo.Warp, MAX];
        }
    }

    void AddAirburst(int count)
    {
        if (loadout[(int)Ammo.Airburst, CURRENT] + count < loadout[(int)Ammo.Airburst, MAX])
        {
            loadout[(int)Ammo.Airburst, CURRENT] += count;
        }
        else
        {
            loadout[(int)Ammo.Airburst, CURRENT] = loadout[(int)Ammo.Airburst, MAX];
        }
    }//End Add Ammo Type

    /*
    * Drop Ammo Type
    * Checks incoming number and limits new loadout to zero
    */
    void DropStandard(int count)
    {
        if (loadout[(int)Ammo.Standard, CURRENT] - count > 0)
        {
            loadout[(int)Ammo.Standard, CURRENT] -= count;
        }
        else
        {
            loadout[(int)Ammo.Standard, CURRENT] = 0;
        }
    }

    void DropBramble(int count)
    {
        if (loadout[(int)Ammo.Bramble, CURRENT] - count > 0)
        {
            loadout[(int)Ammo.Bramble, CURRENT] -= count;
        }
        else
        {
            loadout[(int)Ammo.Bramble, CURRENT] = 0;
        }
    }

    void DropWarp(int count)
    {
        if (loadout[(int)Ammo.Warp, CURRENT] - count > 0)
        {
            loadout[(int)Ammo.Warp, CURRENT] -= count;
        }
        else
        {
            loadout[(int)Ammo.Warp, CURRENT] = 0;
        }
    }

    void DropAirburst(int count)
    {
        if (loadout[(int)Ammo.Airburst, CURRENT] - count > 0)
        {
            loadout[(int)Ammo.Airburst, CURRENT] -= count;
        }
        else
        {
            loadout[(int)Ammo.Airburst, CURRENT] = 0;
        }
    }//End Drop Ammo Type
}