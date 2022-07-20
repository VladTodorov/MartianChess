using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreaScript : MonoBehaviour
{

    RectTransform rectTransform;
    Rect SafeArea;
    Vector2 minAnchor;
    Vector2 maxAnchor;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        SafeArea = Screen.safeArea;

        minAnchor = SafeArea.position;
        maxAnchor = minAnchor + SafeArea.size;

        minAnchor.x /= Screen.width;
        minAnchor.y /= Screen.height;
        maxAnchor.x /= Screen.width;
        maxAnchor.y /= Screen.height;

        rectTransform.anchorMin = minAnchor;
        rectTransform.anchorMax = maxAnchor;
    }
}
