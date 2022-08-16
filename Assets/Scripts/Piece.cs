using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public GameObject target;

    private Vector3 startPos;
    private Vector3 desiredPos;
    private float elapsedTime;
    private float desiredDuration = 0.5f;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("PieceLookAt");


        if (target != null)    //unneeded?
        {
            transform.forward = new Vector3(0, 90, 90);
            desiredPos = transform.position;
        }
        else
        {

            transform.forward = new Vector3(0, 90, 0);
            transform.Rotate(new Vector3(0, 45, 0));
            desiredPos = transform.position;
        }

    }

    void Update()
    {
        //transform.position = desiredPos;

        elapsedTime += Time.deltaTime;
        float percentComplete = elapsedTime / desiredDuration;

        transform.position = Vector3.Lerp(startPos, desiredPos, Mathf.SmoothStep(0, 1, percentComplete));



        if(target != null)
        {
            transform.LookAt(target.transform);
            transform.Rotate(new Vector3(-90, 0, 0));
            transform.Rotate(new Vector3(0, -45, 0));
        }
            
    }

    public void SetPosition(Vector3 newPos)
    {
        startPos = transform.position;
        desiredPos = newPos;
        elapsedTime = 0;
    }

    //public


}
