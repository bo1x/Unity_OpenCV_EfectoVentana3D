using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private WebCamTexture _webcam;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public void SetWebcam( WebCamTexture actualWebcam )
    {
        if (_webcam != null)
        {
            if (_webcam.deviceName == actualWebcam.deviceName)
                return;

            _webcam.Stop();
        }

        _webcam = actualWebcam;

        GameManager.Instance.WebCamConstructor(60, 640, 360);
        _webcam.Play();
        Debug.Log(_webcam.deviceName);
        //mas paridas para el inicio de la webcam
    }

    public void NoAvaibleWebcam()
    {
        Debug.LogError("No hay webcams disponibles");
        //Mandarte a la escena o ponerte el prefab para configurar la webcam

    }

    public void StopWebcam()
    {
        if (_webcam == null)
            return;

        _webcam.Stop();
        _webcam = null;
    }


    public void WebCamConstructor(int fps, int width, int height) {
        _webcam.requestedFPS = fps;
        _webcam.requestedHeight = height;
        _webcam.requestedWidth = width;
    }

    public WebCamTexture GetWebcam()
    {
        return _webcam;
    }

}
