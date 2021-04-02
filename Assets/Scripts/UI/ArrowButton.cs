/*
Author: Warren Rose II
Data: 03/9/2021
Summary: Toggle for Arrow UI tranparency and image.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ArrowButton : MonoBehaviour
{
    public Quiver myQuiver;
    private Button myButton;
    public int type;
    public Sprite imageOn;
    public Sprite imageOff;
    public Text myText;
    // Start is called before the first frame update
    void Start()
    {
        myButton = GetComponent<Button>();
        myButton.image.sprite = imageOff;
    }

    // Update is called once per frame
    void Update()
    {
        if (myQuiver.GetComponent<Quiver>().GetArrowTypeAccess(type) == false)
        {
            myButton.image.enabled = false;
            myText.text = "";
        }
        else
        {
            myButton.image.enabled = true;
            if (!myButton.interactable)
            {
                if (myQuiver.GetComponent<Quiver>().getEquipped == type)
                {
                    myButton.interactable = true;
                    myButton.image.sprite = imageOn;
                    myButton.image.enabled = true;
                }
            }
            else
            {
                if (myQuiver.GetComponent<Quiver>().getEquipped != type)
                {
                    myButton.interactable = false;
                    myButton.image.sprite = imageOff;
                }
            }
            if (myButton.interactable)
            {
                myText.text = "" + myQuiver.GetComponent<Quiver>().GetArrowTypeShot(type);
            }
        }
    }
}
