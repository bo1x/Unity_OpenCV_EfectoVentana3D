using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropDownSize : DropDown
{
    void Start()
    {
        if (GameManager.Instance.GetWebcam() != null)
            switch (GameManager.Instance.GetWebcam().requestedWidth)
            {
                case 144:
                    dropDown.value = 0;
                    break;
                case 240:
                    dropDown.value = 1;
                    break;
                case 360:
                    dropDown.value = 2;
                    break;
                case 480:
                    dropDown.value = 3;
                    break;
                case 720:
                    dropDown.value = 4;
                    break;
            }

        OnChanged();
    }

    public void OnChanged()
    {
        switch (dropDown.value)
        {
            case 0:
                GameManager.Instance.WebCamConstructor((int)GameManager.Instance.RequestedFps(), 256, 144);
                break;
            case 1:
                GameManager.Instance.WebCamConstructor((int)GameManager.Instance.RequestedFps(), 426, 240);
                break;
            case 2:
                GameManager.Instance.WebCamConstructor((int)GameManager.Instance.RequestedFps(), 640, 360);
                break;
            case 3:
                GameManager.Instance.WebCamConstructor((int)GameManager.Instance.RequestedFps(), 854, 480);
                break;
            case 4:
                GameManager.Instance.WebCamConstructor((int)GameManager.Instance.RequestedFps(), 1280, 720);
                break;
        }
    }
}
