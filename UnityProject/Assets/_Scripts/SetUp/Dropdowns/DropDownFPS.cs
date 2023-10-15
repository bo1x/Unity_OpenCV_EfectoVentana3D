using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropDownFPS : DropDown
{
    void Start()
    {
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
