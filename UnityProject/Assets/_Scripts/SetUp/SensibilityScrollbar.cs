using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SensibilityScrollbar : MonoBehaviour
{

    Scrollbar _scrollbar;

    private void Awake()
    {
        _scrollbar = gameObject.GetComponent<Scrollbar>();
    }

    private void Start()
    {
        _scrollbar.value = Mathf.Abs(((GameManager.Instance.GetSensibility().x - 10) / 40) - 1);
    }

    public void onchange()
    {
        GameManager.Instance.SetSensibility((Mathf.Abs((_scrollbar.value - 1)) * 40) + 10);
    }
}
