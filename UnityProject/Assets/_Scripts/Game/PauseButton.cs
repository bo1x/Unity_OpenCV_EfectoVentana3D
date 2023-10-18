using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButton : MonoBehaviour
{
    public bool pause;

    public void onclick() {
        GameManager.Instance.Pause(pause);
    }
}
