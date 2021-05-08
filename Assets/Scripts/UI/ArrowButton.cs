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
using TMPro;

public class ArrowButton : MonoBehaviour
{
    public Quiver myQuiver;
    private Button myButton;
    public int type;
    public Sprite imageOn;
    public Sprite imageOff;
    public Image overlay;
    private Color _lerpedColor;

    //public TMP_Text myText;
    // Start is called before the first frame update
    void Start()
    {
        myButton = GetComponent<Button>();
        myButton.image.sprite = imageOff;
        _lerpedColor = Color.clear;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (myQuiver.GetArrowTypeAccess(type) == false)
        {
            myButton.image.enabled = false;
            //myText.text = "";
        }
        else
        {
            myButton.image.enabled = true;
            if (!myButton.interactable)
            {
                if (myQuiver.getEquipped == type)
                {
                    myButton.interactable = true;
                    myButton.image.sprite = imageOn;
                    myButton.image.enabled = true;
                    overlay.GetComponent<Image>().enabled = true;
                    _lerpedColor = Color.clear;
                    overlay.GetComponent<Image>().color = _lerpedColor;
                }
            }
            else
            {
                _lerpedColor = Color.Lerp(Color.white, Color.clear, Mathf.PingPong(Time.time, 2f));
                overlay.GetComponent<Image>().color = _lerpedColor;
                if (myQuiver.getEquipped != type)
                {
                    myButton.interactable = false;
                    myButton.image.sprite = imageOff;
                    _lerpedColor = Color.clear;
                    overlay.GetComponent<Image>().color = _lerpedColor;
                    overlay.GetComponent<Image>().enabled = false;
                }
            }
            /*
            // Removed for current build
            if (myButton.interactable)
            {
                myText.text = "" + myQuiver.GetArrowTypeShot(type);
            }
            */
        }
    }
}
