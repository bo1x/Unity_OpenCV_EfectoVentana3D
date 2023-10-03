using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebCamSetter : MonoBehaviour
{
    Image _image;
    string _webcamName;

    // Start is called before the first frame update
    void Awake()
    {
        _image = GetComponent<Image>();
        _image.enabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.Instance.GetWebcam() == null || _webcamName  == GameManager.Instance.GetWebcam().deviceName)
            return;

        _image.enabled = true;
        _image.material.mainTexture = GameManager.Instance.GetWebcam();
        GameManager.Instance.WebCamConstructor(120, 640, 360);
        _image.SetMaterialDirty();
        _webcamName = GameManager.Instance.GetWebcam().deviceName;
    }
}
