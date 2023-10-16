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
        _scrollbar.value = (GameManager.Instance.GetSensibility().x - 10) / 40;
    }

    public void onchange()
    {
        GameManager.Instance.SetSensibility((_scrollbar.value * 40) + 10);
    }
}
