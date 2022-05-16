using UnityEngine;
using UnityEngine.UI;

public class FreeLookCam : MonoBehaviour
{
    public float movementSpeed = 10f, fastZoomSensitivity = 50f, fastMovementSpeed = 100f, freeLookSensitivity = 3f, zoomSensitivity = 10f;

    public GameObject crosshair;

    private bool looking = false;
    private GameSettings settings;
    private Transform rig;
    [SerializeField] private Vector3 rigPos, rigRot;
    public Transform target;
    public int tick1 = 250;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    public GameUI ui;

    private void Start()
    {
        rig = transform.parent;
        settings = GameObject.Find("GameManager").GetComponent<GameSettings>();
    }

    private void Update()
    {
        //Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //crosshair.transform.position = Input.mousePosition;

        if(target != null)
        {
           settings.mode = 2;
        }


        if (settings.mode == 0)
        { 
            transform.localPosition = rigPos;
            transform.localEulerAngles = rigRot;
            transform.parent = rig;
        }
        else if (settings.mode == 1)
        {
            transform.parent = null;
            var fastMode = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            var movementSpeed = fastMode ? this.fastMovementSpeed : this.movementSpeed;

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                transform.position = transform.position + (-transform.right * movementSpeed * 1/100);
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                transform.position = transform.position + (transform.right * movementSpeed * 1/100);
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                transform.position = transform.position + (transform.forward * movementSpeed * 1/100);
            }

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                transform.position = transform.position + (-transform.forward * movementSpeed * 1/100);
            }

            if (Input.GetKey(KeyCode.Q))
            {
                transform.position = transform.position + (-transform.up * movementSpeed * 1/100);
            }

            if (Input.GetKey(KeyCode.E))
            {
                transform.position = transform.position + (transform.up * movementSpeed * 1/100);
            }

         /*   if (Input.GetKey(KeyCode.R) || Input.GetKey(KeyCode.PageUp))
            {
                transform.position = transform.position + (Vector3.up * movementSpeed * 1/100);
            }

            if (Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.PageDown))
            {
                transform.position = transform.position + (-Vector3.up * movementSpeed * 1/100);
            }*/

            if (looking)
            {
                Cursor.visible = true;
                float newRotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * freeLookSensitivity;
                float newRotationY = transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * freeLookSensitivity;
                transform.localEulerAngles = new Vector3(newRotationY, newRotationX, 0f);
            }

            float axis = Input.GetAxis("Mouse ScrollWheel");
            if (axis != 0)
            {
                var zoomSensitivity = fastMode ? this.fastZoomSensitivity : this.zoomSensitivity;
                transform.position = transform.position + transform.forward * axis * zoomSensitivity;
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                StartLooking();
            }
            else if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                StopLooking();
            }
        }
        else if (settings.mode == 2)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
            transform.LookAt(target);
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                settings.mode = 1;
                target = null;
                ui.scene = 0;
            }
        }
    }

    void OnDisable()
    {
        StopLooking();
    }

    /// <summary>
    /// Enable free looking.
    /// </summary>
    public void StartLooking()
    {
        looking = true;
        Debug.Log("what");
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Disable free looking.
    /// </summary>
    public void StopLooking()
    {
        looking = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}