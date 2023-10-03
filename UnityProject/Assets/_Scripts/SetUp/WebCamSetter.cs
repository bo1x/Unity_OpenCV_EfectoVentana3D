using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebCamSetter : MonoBehaviour
{
    Image _image;
    string _webcamName;

    public float durationLerp;

    // Start is called before the first frame update
    void Awake()
    {
        _image = GetComponent<Image>();
        _image.enabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.Instance.GetWebcam() == null || _webcamName == GameManager.Instance.GetWebcam().deviceName)
            return;

        _image.enabled = true;
        _image.material.mainTexture = GameManager.Instance.GetWebcam();
        GameManager.Instance.WebCamConstructor(120, 640, 360);
        _image.SetMaterialDirty();
        _webcamName = GameManager.Instance.GetWebcam().deviceName;

        if (_image.color.a == 0)
            StartCoroutine(AnimateOpacity());
    }

    IEnumerator AnimateOpacity()
    {
        float blend = 0;
        float journey = 0;

        while (journey <= durationLerp)
        {
            journey = journey + Time.deltaTime;
            float percent = Mathf.Clamp01(journey / durationLerp);
            blend = Mathf.Lerp(0, 1, percent);

            _image.color = new Vector4(1, 1, 1, blend);

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }
}
