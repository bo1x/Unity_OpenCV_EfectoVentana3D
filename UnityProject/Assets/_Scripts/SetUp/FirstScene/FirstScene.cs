using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstScene : MonoBehaviour
{
    public float durationLerp;
    public Image endFade;
    public RectTransform StarAnimationObject;
    private

    void Awake()
    {
        StartCoroutine(startingAnimation());
    }

    IEnumerator startingAnimation()
    {
        float journey = 0;

        while (journey <= durationLerp)
        {
            journey = journey + Time.deltaTime;
            float percent = Mathf.Clamp01(journey / durationLerp);
            float size = Mathf.Lerp(0.93f,1,percent);
            if(endFade.color.a != 1)
                endFade.color = new Color (endFade.color.r, endFade.color.g, endFade.color.b, 1 - (percent * 3.4f));
            StarAnimationObject.gameObject.transform.localScale = Vector2.one * size;


            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }
}
