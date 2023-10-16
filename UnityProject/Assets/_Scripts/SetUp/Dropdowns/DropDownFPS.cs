using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownFPS : DropDown
{
    void Start()
    {
        if(GameManager.Instance.GetWebcam() != null)
            switch (GameManager.Instance.GetWebcam().requestedFPS)
            {
                case 15:
                    dropDown.value = 0;
                    break;
                case 30:
                    dropDown.value = 1;
                    break;
                case 60:
                    dropDown.value = 2;
                    break;
            }
        OnChanged();
    }
    
    public void OnChanged()
    {
        switch (dropDown.value){
            case 0:
                GameManager.Instance.WebCamConstructor(15, GameManager.Instance.RequestedSize().x, GameManager.Instance.RequestedSize().y);
                break;
            case 1:
                GameManager.Instance.WebCamConstructor(30, GameManager.Instance.RequestedSize().x, GameManager.Instance.RequestedSize().y);
                break;
            case 2:
                GameManager.Instance.WebCamConstructor(60, GameManager.Instance.RequestedSize().x, GameManager.Instance.RequestedSize().y);
                break;
        }
    }  
}
