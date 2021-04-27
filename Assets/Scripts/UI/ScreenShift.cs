/*
Author: Warren Rose II
Data: 4/2/2021
Summary: Enabling screen transitions and HP damage
Using Reference:
** https://turbofuture.com/graphic-design-video/How-to-Fade-to-Black-in-Unity
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenShift : MonoBehaviour
{
    private Image _image;
    private RectTransform _rectangle;
    private Color _color;
    private Color _solid;
    private Color _clear;
    private float _red, _green, _blue, _alpha;
    public bool fadeScreen;
    public bool goDark;
    public bool showDamage;
    public float speed;
    public Image redBorder;
    private Image _redBorder;
    private RectTransform _rectangleRB;

    /*
    * Start
    * Called before the first frame update
    * Loads stats from the local game objects 
    */
    void Start()
    {
        _image = GetComponent<Image>();
        _color = _image.color; _solid = _image.color;
        _red = _color.r; _green = _color.g; _blue = _color.b; _alpha = _color.a;
        _clear = new Color(_red, _green, _blue, 0);
        _rectangle = GetComponent<RectTransform>();
        _rectangle.sizeDelta = new Vector2(Screen.width, Screen.height);

        _redBorder = redBorder.GetComponent<Image>();
        _rectangleRB = redBorder.GetComponent<RectTransform>();
        _rectangleRB.sizeDelta = new Vector2(Screen.width, Screen.height);
        Change();
    }//Start

    /*
    * Change
    * Calls fade ins and fade outs based on variables set in the Inspector
    */
    public void Change()
    {
        if (fadeScreen)
        {
            if (goDark)
            {
                _image.color = _clear;
            }
            else
            {
                _image.color = _solid;
            }
            StartCoroutine(FadeScreen(goDark));
            goDark = !goDark;
        }
    }//Change

    /*
    * ToggleDamage
    * Turns on and off Damage Indicators
    */
    public void ToggleDamage()
    {
        _redBorder.enabled = !_redBorder.enabled;
    }//ToggleDamage

    /*
    * FadeScreen
    * Starts a process to change the overlay's opacity
    */
    public IEnumerator FadeScreen(bool becomingSolid)
    {
        if (becomingSolid)
        {
            _image.enabled = true;
        }
        Color _localColor = _image.color;
        float fadeAmount;

        if (becomingSolid)
        {
            while (_localColor.a < _alpha)
            {
                fadeAmount = _localColor.a + (speed * Time.deltaTime);
                _localColor = new Color(_red, _green, _blue, fadeAmount);
                _image.color = _localColor;
                yield return null;
            }
        }
        else
        {
            while (_localColor.a > 0)
            {
                fadeAmount = _localColor.a - (speed * Time.deltaTime);
                _localColor = new Color(_red, _green, _blue, fadeAmount);
                _image.color = _localColor;
                yield return null;
            }
        }
        if (!becomingSolid)
        {
            _image.enabled = false;
        }
    }//FadeScreen
}//ScreenShift
