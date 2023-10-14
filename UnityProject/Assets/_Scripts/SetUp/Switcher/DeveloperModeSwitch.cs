using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeveloperModeSwitch : Switcher
{

    private void Start()
    {
        if (GameManager.Instance.CanDevelopeMode())
            ButtonOn();
        else
            ButtonOff();
    }

    public void OnClick()
    {
        GameManager.Instance.SetDevelopeMode(!GameManager.Instance.CanDevelopeMode());
        if (GameManager.Instance.CanDevelopeMode())
            ButtonOn();
        else
            ButtonOff();
    }
}
