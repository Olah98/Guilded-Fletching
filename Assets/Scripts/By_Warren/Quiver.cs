/*
Author: Warren Rose II
Data: 02/12/2021
Summary: Quiver loading and record-keeping for unlimited ammo.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quiver : MonoBehaviour
{

    //First segment is Ammo Type
    //Second segment is the integer values for access status and amount used
    private int[,] loadout = new int[4, 2];
    private int equipped;

    //enum for checking stats
    enum Ammo : int
    {
        Standard, Bramble, Warp, Airburst
    }

    //Bay for Constants
    //INACTIVE and ACTIVE track the accesibility of the arrow types
    //ACCESS and RECORD are shortcuts for the loadout array slots
    public const int INACTIVE = 0, ACTIVE = 1, ACCESS = 0, RECORD = 1;

    /*
    * Start
    * Initializing loadout
    */
    void Start()
    {
        //Initial loadout
        //Load("Standard"); //Main Gameplay
        //Load("FirstCombo"); //Standard and Bramble
        //Load("SecondCombo"); //All but Airburst
        Load("All"); //For testing purposes, grants access to everything

        //Base value
        equipped = (int)Ammo.Standard; //Might add check for non-standard level
    }//Start


    /*
    * Update 
    * Called once per frame
    * Listens for keyboard selection, changes equipped selection if accessible
    */
    void Update()
    {
        CheckKeyboardInput();
    }//Update

    /*
    * Check Keyboard Input
    * Called by Update
    * Listens for keyboard selection, changes equipped selection if accessible
    */
    void CheckKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)
            || Input.GetKeyDown(KeyCode.Keypad1))
        {
            if (loadout[(int)Ammo.Standard, ACCESS] == ACTIVE)
            {
                if (equipped != (int)Ammo.Standard)
                {
                    equipped = (int)Ammo.Standard;
                    Debug.Log("Equipped. You have used " +
                        loadout[(int)Ammo.Standard, RECORD] +
                        " Standard arrows");
                    return; //handles key-mashing
                }
                else
                {
                    Debug.Log(loadout[(int)Ammo.Standard, RECORD] +
                        " Standard type arrows used so far");
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
            if (loadout[(int)Ammo.Bramble, ACCESS] == ACTIVE)
            {
                if (equipped != (int)Ammo.Bramble)
                {
                    equipped = (int)Ammo.Bramble;
                    Debug.Log("Equipped. You have used " +
                        loadout[(int)Ammo.Bramble, RECORD] +
                        " Bramble arrows");
                    return; //handles key-mashing
                }
                else
                {
                    Debug.Log(loadout[(int)Ammo.Bramble, RECORD] +
                        " Bramble type arrows used so far");
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
            if (loadout[(int)Ammo.Warp, ACCESS] == ACTIVE)
            {
                if (equipped != (int)Ammo.Warp)
                {
                    equipped = (int)Ammo.Warp;
                    Debug.Log("Equipped. You have used " +
                        loadout[(int)Ammo.Warp, RECORD] +
                        " Warp arrows");
                    return; //handles key-mashing
                }
                else
                {
                    Debug.Log(loadout[(int)Ammo.Warp, RECORD] +
                        " Warp type arrows used so far");
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
            if (loadout[(int)Ammo.Airburst, ACCESS] == ACTIVE)
            {
                if (equipped != (int)Ammo.Airburst)
                {
                    equipped = (int)Ammo.Airburst;
                    Debug.Log("Equipped. You have used " +
                        loadout[(int)Ammo.Airburst, RECORD] +
                        " Airburst arrows");
                    return; //handles key-mashing
                }
                else
                {
                    Debug.Log(loadout[(int)Ammo.Airburst, RECORD] +
                        " Airburst type arrows used so far");
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
    * Fills out the ACCESS loadout based on called parameters
    * Clears all RECORD. May be modified later to keep track?
    */
    void Load(string arrows)
    {
        if (arrows == "Empty")
        {
            //Initial loadout
            loadout[(int)Ammo.Standard, ACCESS] = INACTIVE;
            loadout[(int)Ammo.Standard, RECORD] = 0;
            loadout[(int)Ammo.Bramble, ACCESS] = INACTIVE;
            loadout[(int)Ammo.Bramble, RECORD] = 0;
            loadout[(int)Ammo.Warp, ACCESS] = INACTIVE;
            loadout[(int)Ammo.Warp, RECORD] = 0;
            loadout[(int)Ammo.Airburst, ACCESS] = INACTIVE;
            loadout[(int)Ammo.Airburst, RECORD] = 0;
            Debug.Log("Load(Empty). Quiver is empty");
        }
        else if (arrows == "Standard")
        {
            //Initial loadout
            loadout[(int)Ammo.Standard, ACCESS] = ACTIVE;
            loadout[(int)Ammo.Standard, RECORD] = 0;
            loadout[(int)Ammo.Bramble, ACCESS] = INACTIVE;
            loadout[(int)Ammo.Bramble, RECORD] = 0;
            loadout[(int)Ammo.Warp, ACCESS] = INACTIVE;
            loadout[(int)Ammo.Warp, RECORD] = 0;
            loadout[(int)Ammo.Airburst, ACCESS] = INACTIVE;
            loadout[(int)Ammo.Airburst, RECORD] = 0;
            Debug.Log("Load(Standard). Selected type only.");
        }
        else if (arrows == "Bramble")
        {
            //Initial loadout
            loadout[(int)Ammo.Standard, ACCESS] = INACTIVE;
            loadout[(int)Ammo.Standard, RECORD] = 0;
            loadout[(int)Ammo.Bramble, ACCESS] = ACTIVE;
            loadout[(int)Ammo.Bramble, RECORD] = 0;
            loadout[(int)Ammo.Warp, ACCESS] = INACTIVE;
            loadout[(int)Ammo.Warp, RECORD] = 0;
            loadout[(int)Ammo.Airburst, ACCESS] = INACTIVE;
            loadout[(int)Ammo.Airburst, RECORD] = 0;
            Debug.Log("Load(Bramble). Selected type only.");
        }
        else if (arrows == "Warp")
        {
            //Initial loadout
            loadout[(int)Ammo.Standard, ACCESS] = INACTIVE;
            loadout[(int)Ammo.Standard, RECORD] = 0;
            loadout[(int)Ammo.Bramble, ACCESS] = INACTIVE;
            loadout[(int)Ammo.Bramble, RECORD] = 0;
            loadout[(int)Ammo.Warp, ACCESS] = ACTIVE;
            loadout[(int)Ammo.Warp, RECORD] = 0;
            loadout[(int)Ammo.Airburst, ACCESS] = INACTIVE;
            loadout[(int)Ammo.Airburst, RECORD] = 0;
            Debug.Log("Load(Warp). Selected type only.");
        }
        else if (arrows == "Airburst")
        {
            //Initial loadout
            loadout[(int)Ammo.Standard, ACCESS] = INACTIVE;
            loadout[(int)Ammo.Standard, RECORD] = 0;
            loadout[(int)Ammo.Bramble, ACCESS] = INACTIVE;
            loadout[(int)Ammo.Bramble, RECORD] = 0;
            loadout[(int)Ammo.Warp, ACCESS] = INACTIVE;
            loadout[(int)Ammo.Warp, RECORD] = 0;
            loadout[(int)Ammo.Airburst, ACCESS] = ACTIVE;
            loadout[(int)Ammo.Airburst, RECORD] = 0;
            Debug.Log("Load(Airburst). Selected type only.");
        }
        else if (arrows == "All")
        {
            //Initial loadout
            loadout[(int)Ammo.Standard, ACCESS] = ACTIVE;
            loadout[(int)Ammo.Standard, RECORD] = 0;
            loadout[(int)Ammo.Bramble, ACCESS] = ACTIVE;
            loadout[(int)Ammo.Bramble, RECORD] = 0;
            loadout[(int)Ammo.Warp, ACCESS] = ACTIVE;
            loadout[(int)Ammo.Warp, RECORD] = 0;
            loadout[(int)Ammo.Airburst, ACCESS] = ACTIVE;
            loadout[(int)Ammo.Airburst, RECORD] = 0;
            Debug.Log("Load(All). All four types ready to go.");
        }
        else if (arrows == "FirstCombo")
        {
            //Initial loadout
            loadout[(int)Ammo.Standard, ACCESS] = ACTIVE;
            loadout[(int)Ammo.Standard, RECORD] = 0;
            loadout[(int)Ammo.Bramble, ACCESS] = ACTIVE;
            loadout[(int)Ammo.Bramble, RECORD] = 0;
            loadout[(int)Ammo.Warp, ACCESS] = INACTIVE;
            loadout[(int)Ammo.Warp, RECORD] = 0;
            loadout[(int)Ammo.Airburst, ACCESS] = INACTIVE;
            loadout[(int)Ammo.Airburst, RECORD] = 0;
            Debug.Log("Load(FirstCombo). Standard and Bramble ready to go");
        }
        else if (arrows == "SecondCombo")
        {
            //Initial loadout
            loadout[(int)Ammo.Standard, ACCESS] = ACTIVE;
            loadout[(int)Ammo.Standard, RECORD] = 0;
            loadout[(int)Ammo.Bramble, ACCESS] = ACTIVE;
            loadout[(int)Ammo.Bramble, RECORD] = 0;
            loadout[(int)Ammo.Warp, ACCESS] = ACTIVE;
            loadout[(int)Ammo.Warp, RECORD] = 0;
            loadout[(int)Ammo.Airburst, ACCESS] = INACTIVE;
            loadout[(int)Ammo.Airburst, RECORD] = 0;
            Debug.Log("Load(SecondCombo). Ready to go with all but Airburst");
        }
        else
        {
            Debug.Log("Invalid Load(). Try the ammo type, Empty, All, FirstCombo or SecondCombo");
        }
    }//Load

    /*
    * Get Arrow Type
    * Returns the equipped type as int value
    * //Standard = 1, Bramble = 2, Warp = 3, Airburst = 4
    * Edit by Christian: Start index at 0 to work better with arrays.
    */
    public int GetArrowType()
    {
        return equipped;
    }//GetArrowType

    /*
    * Get Arrow Shot
    * Returns int value of equipped ammo type used this loadout
    */
    public int GetArrowShot()
    {
        return loadout[equipped, RECORD];
    }//GetArrowShot

    /*
    * Get Arrow Type Access
    * Returns true if arrow type is accessible
    */
    public bool GetArrowTypeAccess(string arrows)
    {
        if (arrows == "Standard")
        {
            return (loadout[(int)Ammo.Standard, ACCESS] == ACTIVE);
        }
        else if (arrows == "Bramble")
        {
            return (loadout[(int)Ammo.Bramble, ACCESS] == ACTIVE);
        }
        if (arrows == "Warp")
        {
            return (loadout[(int)Ammo.Warp, ACCESS] == ACTIVE);
        }
        if (arrows == "Airburst")
        {
            return (loadout[(int)Ammo.Airburst, ACCESS] == ACTIVE);
        }
        else
        {
            Debug.Log("Invalid arrow type.");
            return false;
        }
    }//GetArrowTypeAccess

    /*
    * Get Arrow Type Shot
    * Returns int value of selected ammo type used this loadout
    */
    public int GetArrowTypeShot(string arrows)
    {
        if (arrows == "Standard")
        {
            return loadout[(int)Ammo.Standard, RECORD];
        }
        else if (arrows == "Bramble")
        {
            return loadout[(int)Ammo.Bramble, RECORD];
        }
        if (arrows == "Warp")
        {
            return loadout[(int)Ammo.Warp, RECORD];
        }
        if (arrows == "Airburst")
        {
            return loadout[(int)Ammo.Airburst, RECORD];
        }
        else
        {
            Debug.Log("Invalid arrow type.");
            return -1;
        }
    }//GetArrowTypeShot

    /*
    * Fire
    * If ammo type is available, this returns true and increases count by 1
    * Returns false otherwise
    */
    public bool Fire()
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
            case (int)Ammo.Warp:
                success = FireWarp();
                break;
            case (int)Ammo.Airburst:
                success = FireAirburst();
                break;
            default:
                break;
        }
        return success;
    }//Fire

    /*
    * Fire Ammo Type
    * If ammo is available, this returns true and increases count by 1
    * Returns false otherwise
    */
    bool FireStandard()
    {
        if (loadout[(int)Ammo.Standard, ACCESS] == ACTIVE)
        {
            loadout[(int)Ammo.Standard, RECORD]++;
            return true;
        }
        return false;
    }

    bool FireBramble()
    {
        if (loadout[(int)Ammo.Bramble, ACCESS] == ACTIVE)
        {
            loadout[(int)Ammo.Bramble, RECORD]++;
            return true;
        }
        return false;
    }

    bool FireWarp()
    {
        if (loadout[(int)Ammo.Warp, ACCESS] == ACTIVE)
        {
            loadout[(int)Ammo.Warp, RECORD]++;
            return true;
        }
        return false;
    }

    bool FireAirburst()
    {
        if (loadout[(int)Ammo.Airburst, ACCESS] == ACTIVE)
        {
            loadout[(int)Ammo.Airburst, RECORD]++;
            return true;
        }
        return false;
    }//End Fire Ammo Type
}