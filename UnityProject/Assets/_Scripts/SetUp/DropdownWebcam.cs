using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DropdownWebcam : MonoBehaviour
{
    public TMP_Dropdown _dropdown;
    private int _realValue;
    private bool _wasOpened = false;

    // Start is called before the first frame update
    void Awake()
    {
        _realValue = 0;
        _dropdown = GetComponent<TMP_Dropdown>();
        SetNewList();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_dropdown.IsExpanded)
        {
            if (_wasOpened == true)
                SetCamera();

            _realValue = _dropdown.value;
            _wasOpened = _dropdown.IsExpanded;
            return;
        }
        SetNewList();
    }

    void SetCamera()
    {
        WebCamTexture webcamTexture = new WebCamTexture();

        webcamTexture.deviceName = _dropdown.captionText.text;

        GameManager.Instance.SetWebcam(webcamTexture);
    }

    void SetNewList()
    {
        if (_wasOpened)
            return;

        _dropdown.ClearOptions();
        List<string> deviceList = new List<string>();
        WebCamDevice[] devices = WebCamTexture.devices;

        for (int i = 0; i < devices.Length; i++)
            deviceList.Add(devices[i].name);

        foreach (string t in deviceList)
        {
            _dropdown.options.Add(new TMP_Dropdown.OptionData() { text = t });
        }
        _dropdown.value = _realValue;
        _wasOpened = _dropdown.IsExpanded;
    }
}
