using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using Unity.VisualScripting;

public class EyeDetection : MonoBehaviour
{
    public WebCamTexture wct;
    WebCamDevice[] wdevice;
  
    CascadeClassifier faceCascade = new CascadeClassifier();

    Window ventana;
    Window cara;

    int _ojos = 0;


    // Start is called before the first frame update
    void Start()
    {
        cara = new Window("cara");
        ventana = new Window("capturawebcam");


        faceCascade.Load(Application.dataPath + "/haarcascades/haarcascade_eye.xml");

        wdevice = WebCamTexture.devices;
        foreach (WebCamDevice device in wdevice)
        {
            Debug.Log("name" + device.name);
            Debug.Log("");
        }

        wct = new WebCamTexture(wdevice[0].name, 640, 480);
        wct.Play();


       


    }

    // Update is called once per frame
    void Update()
    {



        Texture2D text = GameManager.Instance.WebcamToTexture2D(wct);
        SaveTextureAsPNG(text, Application.dataPath, wct);

        using var src = new Mat(Application.dataPath + "textura.png", ImreadModes.Grayscale);
        using var dst = new Mat();



        var faces = faceCascade.DetectMultiScale(src, 1.3, 5);

        if (_ojos < faces.Length)
        {
            Debug.Log("Guiño");
            
        }
        foreach (var face in faces)
        {
            _ojos = faces.Length;
            Debug.Log(faces.Length);
            //Debug.Log(_ojos);
        }





        

        Cv2.ImWrite(Application.dataPath + "ImagenFinal.png", src);
        ventana.ShowImage(src);

    }
    void OnDestroy()
    {
        ventana.Close();
        cara.Close();
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
