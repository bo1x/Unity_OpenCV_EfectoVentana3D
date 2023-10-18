using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AlwaysLookObject : MonoBehaviour
{
    public GameObject objeto;

    public GameObject alien;

    private Vector3 _Position;

    private float maxnear = 2f;

    private float x, y, z;

    private float ManualSensibilityX, ManualSensibilityY, ManualSensibilityZ;

    private float SensivilityX, SensivilityY, SensivilityZ;
    private float offsetY, offsetZ;

    public TMP_Text sensibilityText;

    private void Awake()
    {
        _Position = gameObject.transform.position;
    }

    private void Update()
    {
        if (!GameManager.Instance.CanDevelopeMode())
        {
            sensibilityText.gameObject.SetActive(false);
            return;
        }
        sensibilityText.gameObject.SetActive(true);

        if(GameManager.Instance.IsgGuinado())
            DarVoltereta();

        if (Input.GetKey(KeyCode.H))
        {
            ManualSensibilityX = 176.7f;
            ManualSensibilityY = 96.8f;
            ManualSensibilityZ = 121.2f;

            offsetZ = -1.2f;

        }


        if (Input.GetKey(KeyCode.Q))
            ManualSensibilityX = Mathf.Clamp(ManualSensibilityX + 0.1f, 0, Mathf.Infinity);

        if (Input.GetKey(KeyCode.A))
            ManualSensibilityX = Mathf.Clamp(ManualSensibilityX - 0.1f, 0, Mathf.Infinity);

        if (Input.GetKey(KeyCode.W))
            ManualSensibilityY = Mathf.Clamp(ManualSensibilityY + 0.1f, 0, Mathf.Infinity);

        if (Input.GetKey(KeyCode.S))
            ManualSensibilityY = Mathf.Clamp(ManualSensibilityY - 0.1f, 0, Mathf.Infinity);

        if (Input.GetKey(KeyCode.E))
            ManualSensibilityZ = Mathf.Clamp(ManualSensibilityZ + 0.1f, 0, Mathf.Infinity);

        if (Input.GetKey(KeyCode.D))
            ManualSensibilityZ = Mathf.Clamp(ManualSensibilityZ - 0.1f, 0, Mathf.Infinity);

        if(Input.GetKey(KeyCode.R))
            offsetY = Mathf.Clamp(offsetY + 0.1f, 0, Mathf.Infinity);

        if (Input.GetKey(KeyCode.F))
            offsetY = Mathf.Clamp(offsetY - 0.1f, 0, Mathf.Infinity);

        if (Input.GetKey(KeyCode.T))
            offsetZ = offsetZ + 0.1f;

        if (Input.GetKey(KeyCode.G))
            offsetZ = offsetZ - 0.1f;

        return;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.GetPause())
            return;

        CameraRotate();
    }

    void CameraRotate()
    {
        GameManager.Instance.OpenCVFace();
        Vector3 axis = GameManager.Instance.getOpenCVAxis();
        SensivilityX = ManualSensibilityX == 0 ? GameManager.Instance.GetSensibility().x : ManualSensibilityX;
        SensivilityY = ManualSensibilityY == 0 ? GameManager.Instance.GetSensibility().y : ManualSensibilityY;
        SensivilityZ = ManualSensibilityZ == 0 ? GameManager.Instance.GetSensibility().z : ManualSensibilityZ;

        if(GameManager.Instance.CanDevelopeMode())
            sensibilityText.text = "Sensibility <br> X: " + Mathf.Round(SensivilityX * 10)/10 +" Y: " + Mathf.Round(SensivilityY * 10) / 10 + " Z: " + Mathf.Round(SensivilityZ * 10) / 10 + "<br>Offset: <br>Y: " + Mathf.Round(offsetY * 10) / 10 + " Z: " + Mathf.Round(offsetZ * 10) / 10;

        x = _Position.x + axis.x / SensivilityX;
        y = Mathf.Clamp(_Position.y + offsetY + axis.y / SensivilityY, 8.8f, Mathf.Infinity);
        z = GameManager.Instance.HaveAxisZ() ? Mathf.Clamp(_Position.z + offsetZ + axis.z / Mathf.Clamp(SensivilityZ, 0,30), maxnear, 15 - maxnear) : _Position.z;
       // Debug.Log(axis.x);
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, new Vector3(x, y, z), 0.1f);
        gameObject.transform.LookAt(objeto.transform);
    }

    void DarVoltereta()
    {
        Debug.Log(GameManager.Instance.iZquierdo());
        alien.transform.Rotate(360 * (GameManager.Instance.iZquierdo() ? Vector3.up : Vector3.down ) * Time.deltaTime, Space.Self);
    }
}
