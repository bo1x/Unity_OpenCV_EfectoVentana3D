using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestVideo : MonoBehaviour
{
    GameObject _parent;
    [SerializeField] private GameObject _OtherParent;

    private void Awake()
    {
        _parent = gameObject.transform.parent.gameObject;
    }

    public void onclick()
    {
        GameManager.Instance.CanShowWebcam(!GameManager.Instance.CanShowWebcam());

        showWebcam();

    }

    private void showWebcam()
    {
        _parent.SetActive(false);
        _OtherParent.SetActive(true);
    }
}
