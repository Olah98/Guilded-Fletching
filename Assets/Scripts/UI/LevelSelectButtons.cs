/*
Author: Warren Rose II
Data: 04/6/2021
Summary: Controls level select screen navigation.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelSelectButtons : MonoBehaviour
{
    public GameObject Buttons1, Buttons2, Buttons3;
    public Button LeftButton, RightButton;

    private int _scrollMax = 2; //Current levels (12) divided by 4, rounded up

    private int _set = 1;
    // Start is called before the first frame update
    void Start()
    {
    }

    

    /*
    * Scroll
    * Shows and hides buttons to simulate pages turning
    * int direction changes visible set
    */
    public void Scroll(int direction)
    {
        RightButton.GetComponent<Button>().interactable = false;
        LeftButton.GetComponent<Button>().interactable = false;
        Buttons1.SetActive(false);
        Buttons2.SetActive(false);
        Buttons3.SetActive(false);

        _set += direction;

        if (_set < 1)
        {
            _set = 1;
        } else if (_set > _scrollMax)
        {
            _set = _scrollMax;
        }

        if (_set == 1)
        {
            Buttons1.SetActive(true);
            RightButton.GetComponent<Button>().interactable = true;
        } else if (_set == 2)
        {
            Buttons2.SetActive(true);
            LeftButton.GetComponent<Button>().interactable = true;
            //RightButton.GetComponent<Button>().interactable = true;
            RightButton.GetComponent<Button>().interactable = false;
        }
        else if (_set == 3)
        {
            Buttons3.SetActive(true);
            LeftButton.GetComponent<Button>().interactable = true;
        }
    }//Scroll
}//LevelSelectButtons