using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Switcher: MonoBehaviour
{
    private Image _Image;

    // Start is called before the first frame update
    void Awake()
    {
        _Image = GetComponent<Image>();
    }

    public void ButtonOn()
    {
        _Image.color = Color.green;
    }

    public void ButtonOff()
    {
        _Image.color = Color.red;
    }
}
