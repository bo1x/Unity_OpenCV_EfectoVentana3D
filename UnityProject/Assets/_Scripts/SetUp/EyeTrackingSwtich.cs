using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EyeTrackingSwtich : MonoBehaviour
{
    private Image _Image;

    // Start is called before the first frame update
    void Awake()
    {
        _Image = GetComponent<Image>();
    }

    private void Start()
    {
        if (GameManager.Instance.CanEyeTracking())
            ButtonOn();
        else
            ButtonOff();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        GameManager.Instance.SetEyeTracking(!GameManager.Instance.CanEyeTracking());
        if (GameManager.Instance.CanEyeTracking())
            ButtonOn();
        else
            ButtonOff();
    }

    private void ButtonOn()
    {
        _Image.color = Color.green;
    }

    private void ButtonOff()
    {
        _Image.color = Color.red;
    }
}
