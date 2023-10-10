using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using Unity.VisualScripting;

public class FaceDetection : MonoBehaviour
{
    public WebCamTexture wct;
    WebCamDevice[] wdevice;

    CascadeClassifier faceCascade = new CascadeClassifier();
    CascadeClassifier eyeCascade = new CascadeClassifier();

    Window ventana;
    Window cara;

    Mat caraSeparada;


    // Start is called before the first frame update
    void Start()
    {
        cara = new Window("cara");
        ventana = new Window("ventana");


        faceCascade.Load(Application.dataPath + "/haarcascades/haarcascade_frontalface_default.xml");
        eyeCascade.Load(Application.dataPath + "/haarcascades/haarcascade_eye.xml");

        wdevice = WebCamTexture.devices;
        foreach (WebCamDevice device in wdevice)
        {
            Debug.Log("name" + device.name);
            Debug.Log("");
        }

        wct = new WebCamTexture(wdevice[1].name, 640, 480);
        wct.Play();





    }

    // Update is called once per frame
    void Update()
    {


        Texture2D text = GameManager.Instance.WebcamToTexture2D(wct);
        SaveTextureAsPNG(text, Application.dataPath, wct);

        using var src = new Mat(Application.dataPath + "textura.png", ImreadModes.Grayscale);
        using var dst = new Mat();

        ventana.ShowImage(src);

        var faces = faceCascade.DetectMultiScale(src, 1.3, 5);
        foreach (var face in faces)
        {

            Debug.Log("TopLEFT" + face.TopLeft + " " + (face.TopLeft.Y-face.Height).ToString());
            Debug.Log("TopLEFTY" + face.TopLeft.Y+ " "+face.Height);
            Debug.Log("TopLEFTX" + face.TopLeft.X+ " "+face.Width);

               Range[] rangos=new Range[2];
               rangos[0] = new OpenCvSharp.Range(face.TopLeft.Y, face.TopLeft.Y - face.Height);
               rangos[1] = new OpenCvSharp.Range(face.BottomRight.X, face.BottomRight.X + face.Width);
               caraSeparada = new Mat(src,rangos);
               
            
            var eyes = eyeCascade.DetectMultiScale(caraSeparada, 1.3, 5);
            foreach (var eye in eyes)
            {
                
            }
        }






            
            
            cara.ShowImage(caraSeparada);

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
