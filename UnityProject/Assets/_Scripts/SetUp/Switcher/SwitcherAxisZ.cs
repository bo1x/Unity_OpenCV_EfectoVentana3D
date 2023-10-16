 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitcherAxisZ : Switcher
{

    private void Start()
    {
        if (GameManager.Instance.HaveAxisZ())
            ButtonOn();
        else
            ButtonOff();
    }

    public void OnClick()
    {
        GameManager.Instance.HaveAxisZ(!GameManager.Instance.HaveAxisZ());
        if (GameManager.Instance.HaveAxisZ())
            ButtonOn();
        else
            ButtonOff();
    }
}
