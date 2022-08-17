using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiTweening : MonoBehaviour
{
    [SerializeField] GameObject mars, ufo;

    void Start()
    {
        LeanTween.rotateAround(mars, Vector3.forward, -360, 30.0f)
            .setLoopClamp();

       // LeanTween.moveLocal(ufo, Vector3.up * 0.2f, 2.0f)
        LeanTween.moveLocal(ufo, Vector3.Lerp(ufo.transform.localPosition, Vector3.zero, 0.2f), 2.0f)
            .setEaseInOutQuad()
            //.setEaseOutSine()
            //.setEaseInSine()
            //.setLoopClamp();
            .setLoopPingPong();
    }

}
