using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasGame : MonoBehaviour
{
    [SerializeField] private Button _button;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _button.gameObject.SetActive(!GameManager.Instance.GetPause());
    }
}
