using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public GameObject target;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("PieceLookAt");
        transform.forward = new Vector3(0, 90, 90);
    }

    void Update()
    {
        transform.LookAt(target.transform);
        transform.Rotate(new Vector3(90,0,0));
    }
}
