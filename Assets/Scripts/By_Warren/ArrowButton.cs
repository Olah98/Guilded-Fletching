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
    // Start is called before the first frame update
    void Start()
    {
        myButton = GetComponent<Button>();
        myButton.image.sprite = imageOff;
    }

    // Update is called once per frame
    void Update()
    {
        if (!myButton.interactable)
        {
            if (myQuiver.GetComponent<Quiver>().getEquipped == type)
            {
                myButton.interactable = true;
                myButton.image.sprite = imageOn;
            }
        } else
        {
            if (myQuiver.GetComponent<Quiver>().getEquipped != type)
            {
                myButton.interactable = false;
                myButton.image.sprite = imageOff;
            }
        }
    }
}
