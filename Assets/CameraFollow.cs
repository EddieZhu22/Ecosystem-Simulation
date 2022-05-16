using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public Vector3 ForwardspectateSpeed;
    public Vector3 SidespectateSpeed;
    public Vector3 UpspectateSpeed;
    void Start()
    {
        
    }

    void Update()
    {
        if(target != null)
        {
            transform.position = target.position + offset;
        }
        else
        {
            if (Input.GetKey("w"))
            {
                transform.position = transform.position + ForwardspectateSpeed;
            }
            if (Input.GetKey("s"))
            {
                transform.position = transform.position - ForwardspectateSpeed;

            }
            if (Input.GetKey("a"))
            {
                transform.position = transform.position - SidespectateSpeed;
            }
            if (Input.GetKey("d"))
            {
                transform.position = transform.position + SidespectateSpeed;
            }
            if (Input.GetKey("q"))
            {
                transform.position = transform.position - UpspectateSpeed;
            }
            if (Input.GetKey("e"))
            {
                transform.position = transform.position + UpspectateSpeed;
            }
        }
    }
}
