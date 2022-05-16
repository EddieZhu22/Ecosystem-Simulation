using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class normal : MonoBehaviour
{
    public LayerMask rayLayer;
    private RaycastHit hit; void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(transform.position + new Vector3(0, 1000, 0), -Vector3.up, out hit, Mathf.Infinity, rayLayer))
        {
            transform.rotation = Quaternion.FromToRotation(Vector3.forward,hit.normal);
            Debug.DrawLine(transform.position + new Vector3(0, 1000, 0), hit.point, Color.cyan);
        }
    }
}
