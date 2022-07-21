using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoPosisiton : MonoBehaviour
{
    RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        bool isFlush = GetComponentInParent<SafeAreaScript>().isFlush;

        print(isFlush);

        if (!isFlush)
        {
            Vector2 pos = rectTransform.position;
            pos.y += 26;
            rectTransform.position = pos;
        }
    }
}
