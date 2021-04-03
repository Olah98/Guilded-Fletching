/*
Author: Warren Rose II
Data: 4/2/2021
Summary: Enabling screen transitions
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
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        _image = GetComponent<Image>();
        _color = _image.color; _solid = _image.color;
        _red = _color.r; _green = _color.g; _blue = _color.b; _alpha = _color.a;
        _clear = new Color(_red, _green, _blue, 0);
        _rectangle = GetComponent<RectTransform>();
        _rectangle.sizeDelta = new Vector2(Screen.width, Screen.height);
        Change();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
    }

    public IEnumerator FadeScreen(bool becomingSolid)
    {
        Color _localColor = _image.color;
        float fadeAmount;

        if (becomingSolid)
        {
            while (_localColor.a < _alpha)
            {
                fadeAmount =_localColor.a + (speed * Time.deltaTime);
                _localColor = new Color(_red, _green, _blue, fadeAmount);
                _image.color = _localColor;
                yield return null;
            }
        } else
        {
            while (_localColor.a > 0)
            {
                fadeAmount = _localColor.a - (speed * Time.deltaTime);
                _localColor = new Color(_red, _green, _blue, fadeAmount);
                _image.color = _localColor;
                yield return null;
            }
        }
    }
}
