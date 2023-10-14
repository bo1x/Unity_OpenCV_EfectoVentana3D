 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EyeTrackingSwitch : Switcher
{

    private void Start()
    {
        if (GameManager.Instance.CanEyeTracking())
            ButtonOn();
        else
            ButtonOff();
    }

    public void OnClick()
    {
        GameManager.Instance.SetEyeTracking(!GameManager.Instance.CanEyeTracking());
        if (GameManager.Instance.CanEyeTracking())
            ButtonOn();
        else
            ButtonOff();
    }
}
