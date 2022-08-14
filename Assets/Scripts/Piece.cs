using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public GameObject target;

    private Vector3 desiredPos;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("PieceLookAt");

        transform.forward = new Vector3(0, 90, 90);

        //desiredPos = transform.position;

        if(target != null)
        {
            transform.forward = new Vector3(0, 90, 90);
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
        transform.position = desiredPos;
        //transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * 50f);

        if(target != null)
        {
            transform.LookAt(target.transform);
            transform.Rotate(new Vector3(-90, 0, 0));
            transform.Rotate(new Vector3(0, -45, 0));
        }
            
    }

    public void SetPosition(Vector3 newPos)
    {
        desiredPos = newPos;
    }



}
