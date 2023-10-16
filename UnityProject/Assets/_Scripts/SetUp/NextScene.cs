using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextScene : MonoBehaviour
{
    public void onclick()
    {
        GameManager.Instance.NextScene(1);
    }
}
