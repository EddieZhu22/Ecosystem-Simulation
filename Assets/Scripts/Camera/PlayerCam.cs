using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCam : MonoBehaviour
{

    public Transform target;
    public int tick1 = 250;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
        transform.LookAt(target);
    }

}
