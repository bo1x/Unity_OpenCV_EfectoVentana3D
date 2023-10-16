using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class AlwaysLookObject : MonoBehaviour
{
    public GameObject objeto;

    private void FixedUpdate()
    {
        CameraRotate();
    }

    void CameraRotate()
    {
        GameManager.Instance.OpenCVFace();
        Vector3 axis = GameManager.Instance.getOpenCVAxis();
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, new Vector3(0 + axis.x / 30, 0 + axis.y / 30, 0 + axis.z / 30), 0.3f);
        gameObject.transform.LookAt(objeto.transform);
    }
}
