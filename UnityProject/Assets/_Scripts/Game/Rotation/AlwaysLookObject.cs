using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class AlwaysLookObject : MonoBehaviour
{
    public GameObject objeto;

    private Vector3 _Position;

    private float maxnear = 2f;

    private float x, y, z;

    private void Awake()
    {
        _Position = gameObject.transform.position;
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
        x = _Position.x + axis.x / GameManager.Instance.GetSensibility().x;
        y = _Position.y + axis.y / GameManager.Instance.GetSensibility().y;
        z = GameManager.Instance.HaveAxisZ() ? Mathf.Clamp(_Position.z + axis.z / GameManager.Instance.GetSensibility().z, maxnear, Mathf.Infinity) : _Position.z;
        Debug.Log(axis.x);
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, new Vector3(x, y, z), 0.1f);
        gameObject.transform.LookAt(objeto.transform);
    }
}
