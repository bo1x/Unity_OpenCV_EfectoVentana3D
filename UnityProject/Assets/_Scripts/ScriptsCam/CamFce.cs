using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using Unity.VisualScripting;

public class CamFce : MonoBehaviour
{
    public WebCamTexture wct;
    WebCamDevice[] wdevice;

    CascadeClassifier faceCascade = new CascadeClassifier();
    CascadeClassifier eyeCascade = new CascadeClassifier();

    int AltoCam = 0;
    int AnchoCam = 0;

    // Start is called before the first frame update
    void Start()
    {


        faceCascade.Load(Application.dataPath + "/haarcascades/haarcascade_frontalface_default.xml");
        eyeCascade.Load(Application.dataPath + "/haarcascades/haarcascade_eye.xml");

        wdevice = WebCamTexture.devices;
        foreach (WebCamDevice device in wdevice)
        {
            Debug.Log("name" + device.name);
            Debug.Log("");
        }

        wct = new WebCamTexture(wdevice[0].name, 640, 480);
        wct.Play();


        AltoCam = wct.height;
        AnchoCam = wct.width;


    }

    // Update is called once per frame
    void Update()
    {
        Texture2D text = GameManager.Instance.WebcamToTexture2D(wct);
        SaveTextureAsPNG(text, Application.dataPath, wct);

        using var src = new Mat(Application.dataPath + "textura.png", ImreadModes.Grayscale);
        using var dst = new Mat();



        var faces = faceCascade.DetectMultiScale(src, 1.3, 5);
        foreach (var face in faces)
        {



            Debug.Log(face.Location+" "+face.Height);



        }

       
        



       

    }
    void OnDestroy()
    {
        if (wct != null)
        {
            wct.Stop();
        }
    }

    public static void SaveTextureAsPNG(Texture2D _texture, string _fullPath, WebCamTexture wct)
    {
        byte[] _bytes = _texture.EncodeToPNG();
        _texture.SetPixels(wct.GetPixels());
        _texture.Apply();
        System.IO.File.WriteAllBytes(_fullPath + "textura.png", _texture.EncodeToPNG());

    }





}
