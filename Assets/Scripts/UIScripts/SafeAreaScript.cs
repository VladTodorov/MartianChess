using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreaScript : MonoBehaviour
{

    RectTransform rectTransform;
    Rect SafeArea;
    Vector2 minAnchor;
    Vector2 maxAnchor;

    public bool isFlush;

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

        if(maxAnchor.y == 1)
            isFlush = true;
        else
            isFlush = false;

        
        //print(isFlush);
        //print(minAnchor);
        //print(maxAnchor);
        //print(Screen.width);
        print(Screen.height);
        print(SafeArea.size);
        
    }
}
