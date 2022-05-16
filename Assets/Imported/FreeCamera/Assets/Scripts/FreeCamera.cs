using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SimpleCameraInput))]
public class FreeCamera : MonoBehaviour
{
    [Header("FreeCamera Settings")]
    [Tooltip("Acctual camera speed")]
    public float CameraSpeed = 2;
    [Tooltip("How much faster the camera should move after pressing the selected button")]
    public float CameraSpeedMultiplier = 2;
    [Tooltip("Maximum camera speed")]
    public float MaxCameraSpeed = 10;
    [Tooltip("Mouse sensitivity")]
    public float MouseSensitivity = 5;
    [Tooltip("Possibility to enable and disable free camera in the game by pressing binded key")]
    public bool CameraSwitch = true;

    private SimpleCameraInput _cameraInput;
    private bool _isSpeedUp;
    private bool _switchOn = true;
    private bool _isLocked = false;

    private float _xInput;
    private float _yInput;
    private float _zInput;

    private float _mouseXInput;
    private float _mouseYInput;
    private float _rotationX, _rotationY;

    private void Start()
    {
        _cameraInput = GetComponent<SimpleCameraInput>();
    }
    private void Update()
    {
        CheckKeyboardInput();

        if (Input.GetButton("Fire2"))
        {
            if(!_isLocked)
                LockCursor();
            CheckMouseInput();
        }
        else
        {
            if (_isLocked)
                LockCursor();
        }
    }
    private void LateUpdate()
    {
        if(_switchOn)
        {
            MouseMovement();
            CameraMovement();
        }
    }
    private void CheckKeyboardInput()
    {
        _xInput = _cameraInput.GetAxis(_cameraInput.xAxis);
        _yInput = _cameraInput.GetAxis(_cameraInput.yAxis);
        _zInput = _cameraInput.GetAxis(_cameraInput.zAxis);
        _isSpeedUp = Input.GetKey(_cameraInput.BoostSpeedKey);
        if (CameraSwitch && Input.GetKeyDown(_cameraInput.SwitchOnKey)) SwitchCamera();
    }
    private void CameraMovement()
    {
            if (!_isSpeedUp)
            {
                transform.position += transform.forward * _yInput * CameraSpeed * Time.deltaTime;
                transform.position += transform.right * _xInput * CameraSpeed * Time.deltaTime;
                transform.position += transform.up * _zInput * CameraSpeed * Time.deltaTime;
            }
            else
            {
                transform.position += transform.forward * _yInput * CameraSpeed * CameraSpeedMultiplier * Time.deltaTime;
                transform.position += transform.right * _xInput * CameraSpeed * CameraSpeedMultiplier * Time.deltaTime;
                transform.position += transform.up * _zInput * CameraSpeed * CameraSpeedMultiplier * Time.deltaTime;
            }
    }
    private void CheckMouseInput()
    {
        _mouseXInput = Input.GetAxis("Mouse X");
        _mouseYInput = Input.GetAxis("Mouse Y");
        if (Input.mouseScrollDelta != Vector2.zero) SetCameraSpeed();

    }
    private void MouseMovement()
    {
        _rotationX += _mouseXInput * MouseSensitivity;
        _rotationY += _mouseYInput * MouseSensitivity;

        transform.localRotation = Quaternion.AngleAxis(_rotationX, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(_rotationY, Vector3.left);
    }
    private void SwitchCamera()
    {
        _switchOn = !_switchOn;
    }
    private void SetCameraSpeed()
    {
        if (CameraSpeed >= 0 && CameraSpeed <= MaxCameraSpeed)
        {
            CameraSpeed = CameraSpeed + Input.mouseScrollDelta.y * 0.5f;
            if (CameraSpeed < 0) CameraSpeed = 0;
            if (CameraSpeed > MaxCameraSpeed) CameraSpeed = MaxCameraSpeed;
        }
    }
    private void LockCursor()
    {
        if (!_isLocked)
        {
            _isLocked = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
        }
        else
        {
            _isLocked = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

    }
}
