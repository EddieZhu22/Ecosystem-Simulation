using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, 10);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "food")
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }
    }
}
