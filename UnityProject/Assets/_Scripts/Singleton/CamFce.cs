using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using Unity.VisualScripting;
using UnityEngine.XR;
using static UnityEngine.EventSystems.EventTrigger;

//Obsoleto no se usa ya este script

public class CamFce : MonoBehaviour
{
    public WebCamTexture wct;
    WebCamDevice[] wdevice;

    CascadeClassifier faceCascade = new CascadeClassifier();
    CascadeClassifier eyeCascade = new CascadeClassifier();

    int AltoCam = 0;
    int AnchoCam = 0;

    public int X, Y, Z;

    Mat faceMat;

    Window a;
    // Start is called before the first frame update
    void Start()
    {
        //Esto en una funcion ncon un awake y debe estar asociado a variables
        a = new Window("capturawebcam");
        faceCascade.Load(Application.dataPath + "/haarcascades/haarcascade_frontalface_default.xml");
        eyeCascade.Load(Application.dataPath + "/haarcascades/haarcascade_eye.xml");

        //Esto tocara ponerlo con la mca de GameManager
        wdevice = WebCamTexture.devices;
        foreach (WebCamDevice device in wdevice)
        {
            Debug.Log("name" + device.name);
            Debug.Log("");
        }
        wct = new WebCamTexture(wdevice[1].name, 640, 480);
        wct.Play();
        AltoCam = wct.height;
        AnchoCam = wct.width;


    }

    // Update is called once per frame
    void Update()
    {
        Texture2D text = GameManager.Instance.WebcamToTexture2D(wct);
        //SaveTextureAsPNG(text, Application.dataPath, wct);

        //Hacer que todo sea grayscale
        faceMat = new Mat(Application.dataPath + "textura.png", ImreadModes.Grayscale);

        //El flip ya esta incorporado asi que remover
        Cv2.Flip(faceMat, faceMat, FlipMode.Y);
        //no la usa asi que se puede remover lmao
        using var dst = new Mat();

        //funcion de llamar caras de opencv
        var faces = faceCascade.DetectMultiScale(faceMat, 1.3, 5);
        //revisar
        foreach (var face in faces)
        {

            X = face.Left + face.Width / 2; 
            X = X - wct.width / 2;
            Y = face.Top + face.Height / 2;
            Y = Y - wct.height / 2;
            Z = face.Height;
            Y = -Y;
            
            
            Point centro = new Point(face.Left + face.Width / 2, face.Top + face.Height / 2);
            Scalar color = new Scalar(0, 0, 255);
            Cv2.Circle(faceMat, centro, 1, color, 5);
            //poner un cuadrado con una condicional
            Cv2.Rectangle(faceMat, face.TopLeft, face.BottomRight, new Scalar(255, 0, 0), 5 );
        }



        //gameObject.transform.position = new Vector3(0+X / 30, 0+Y / 30, 0 + Z / 30);
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, new Vector3(0 + X / 30, 0 + Y / 30, 0 + Z / 30), 0.3f);


        //la ventana
        a.ShowImage(faceMat);
    }
    void OnDestroy()
    {
        a.Close();
        if (wct != null)
        {
            wct.Stop();
        }
    }
    /*
    public static void SaveTextureAsPNG(Texture2D _texture, string _fullPath, WebCamTexture wct)
    {
        byte[] _bytes = _texture.EncodeToPNG();
        _texture.SetPixels(wct.GetPixels());
        _texture.Apply();
        System.IO.File.WriteAllBytes(_fullPath + "textura.png", _texture.EncodeToPNG());

    }
    */





}
