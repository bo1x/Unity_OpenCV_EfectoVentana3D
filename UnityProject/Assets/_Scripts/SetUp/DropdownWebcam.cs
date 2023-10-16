using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DropdownWebcam : MonoBehaviour
{
    public TMP_Dropdown _dropdown;
    private bool _wasOpened = false;
    private string _text;
    private string _none = "(none)";
    public static List<TMP_Dropdown.OptionData> list;
    [SerializeField] private Button _button;

    // Start is called before the first frame update
    void Awake()
    {

    }

    private void Start()
    {
        _dropdown = GetComponent<TMP_Dropdown>();

        if (list == null)
            SetNewList();
        else
        {
            _dropdown.options = list;
            _dropdown.value = GameManager.Instance.whatdropdown();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_dropdown.IsExpanded)
        {
            if (_wasOpened == true && _dropdown.value != 0)
            {
                SetCamera();
            }
            else if (_dropdown.value == 0)
            {
                GameManager.Instance.StopWebcam();
                _button.interactable = false;
            }
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

        _button.interactable = true;
    }

    void SetNewList()
    {
        if (_wasOpened)
            return;

        _text = _dropdown.captionText.text;

        _dropdown.ClearOptions();
        List<string> deviceList = new List<string>();
        WebCamDevice[] devices = WebCamTexture.devices;

        for (int i = 0; i < devices.Length; i++)
            deviceList.Add(devices[i].name);

        _dropdown.options.Add(new TMP_Dropdown.OptionData() { text = _none });

        foreach (string t in deviceList)
        {
            _dropdown.options.Add(new TMP_Dropdown.OptionData() { text = t });
        }
        GameManager.Instance.setdropdown(_dropdown.value);
        _wasOpened = _dropdown.IsExpanded;

        list = _dropdown.options;

        _dropdown.captionText.text = _text;
    }
}
