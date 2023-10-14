 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlipSwitch : Switcher
{

    private void Start()
    {
        if (GameManager.Instance.isWebcamFliped())
            ButtonOn();
        else
            ButtonOff();
    }

    public void OnClick()
    {
        GameManager.Instance.SetWebcamFlip(!GameManager.Instance.isWebcamFliped());
        if (GameManager.Instance.isWebcamFliped())
            ButtonOn();
        else
            ButtonOff();
    }
}
