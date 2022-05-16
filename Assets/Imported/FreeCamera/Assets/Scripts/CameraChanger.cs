using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChanger : MonoBehaviour
{
    [Header("List of available cameras")]
    public GameObject[] Cameras;

    [Header("CameraChanger Inputs")]
    public KeyCode Save;
    public KeyCode[] SelectCamera;

    private int _cameraID = 0;
    public void Update()
    {
        if (Cameras != null)
        {
            if(!Input.GetKey(Save))
            {
                for (int i = 0; i < Cameras.Length; i++)
                {
                    if (Input.GetKeyDown(SelectCamera[i]))
                    {
                        ChooseCamera(i);
                    }
                }
            }
            else
            {
                for (int i = 0; i < Cameras.Length; i++)
                {
                    if (Input.GetKeyDown(SelectCamera[i]))
                    {
                        SetCameraPosition(i);
                    }
                }
            }
        }
    }
    void ChooseCamera(int _idCam)
    {
        if (_idCam >= 0 && _idCam < Cameras.Length)
        {
            foreach (GameObject Cam in Cameras)
            {
                Cam.SetActive(false);
            }
            Cameras[_idCam].SetActive(true);
        }
    }
    void SetCameraPosition(int _idCam)
    {
        if (_idCam != _cameraID)
        {
            Cameras[_idCam].transform.localPosition = Cameras[_cameraID].transform.localPosition;
            Cameras[_idCam].transform.localRotation = Cameras[_cameraID].transform.localRotation;
        }
    }
}
