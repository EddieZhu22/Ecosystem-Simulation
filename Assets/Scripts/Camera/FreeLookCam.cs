using UnityEngine;
using UnityEngine.UI;

public class FreeLookCam : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 10f, fastZoomSensitivity = 50f, fastMovementSpeed = 100f, freeLookSensitivity = 3f, zoomSensitivity = 10f;
    private bool looking = false;
    [SerializeField] private GameManager manager;
    [SerializeField] private Transform rig;
    [SerializeField] private Vector3 rigPos, rigRot;
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset;

    [SerializeField] private int type;
    [SerializeField] private GameObject creatureText, plantText;
    private void Start()
    {
        rig = transform.parent;
    }

    private void Update()
    {
        //Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //crosshair.transform.position = Input.mousePosition;

        //if (target == null) manager.settings.CameraMode = GameSettings.mode.Free;

        switch (manager.settings.CameraMode)
        {
            case GameSettings.mode.Rotate:
                transform.parent = null;
                CameraMovement();

                break;

            case GameSettings.mode.Free:
                transform.parent = null;
                CameraMovement();
                break;

            case GameSettings.mode.Attatched:
                Vector3 desiredPosition;
                if (target == null)
                {
                    manager.settings.CameraMode = GameSettings.mode.Free;
                    target = null;
                    manager.UI.scene = 0;
                }
                // Check if the target is rotated 90 degrees
                if (Mathf.Approximately(Mathf.Abs(target.eulerAngles.y), 90.0f))
                {
                    // Transform the offset to the local space of the target object if it's rotated 90 degrees
                    desiredPosition = target.TransformPoint(offset);
                }
                else
                {
                    // Otherwise, calculate the desired position normally
                    desiredPosition = target.position + offset;
                }

                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
                transform.position = smoothedPosition;
                transform.LookAt(target);

                manager.UI.type = type;

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    manager.settings.CameraMode = GameSettings.mode.Free;
                    target = null;
                    manager.UI.scene = 0;

                }
   
                break;
        }
    }

    private void CameraMovement()
    {
        var fastMode = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        var movementSpeed = fastMode ? this.fastMovementSpeed : this.movementSpeed;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position = transform.position + (-transform.right * movementSpeed / Time.timeScale * 100 * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.position = transform.position + (transform.right * movementSpeed / Time.timeScale * 100 * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            transform.position = transform.position + (transform.forward * movementSpeed / Time.timeScale * 100 * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            transform.position = transform.position + (-transform.forward * movementSpeed / Time.timeScale * 100 * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            transform.position = transform.position + (-transform.up * movementSpeed / Time.timeScale * 100 * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.position = transform.position + (transform.up * movementSpeed / Time.timeScale * 100 * Time.deltaTime);
        }

        if (looking)
        {
            Cursor.visible = true;
            float newRotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * freeLookSensitivity;
            float newRotationY = transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * freeLookSensitivity;
            transform.localEulerAngles = new Vector3(newRotationY, newRotationX, 0f);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Create a ray from the cursor's screen position into the world in the camera's look direction
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Define layer mask for "Collectible" and "Creatures and Environment"
            int layerMask = (1 << 3) | (1 << 6); // 3 for "Collectible", 6 for "Creatures and Environment"


            // Perform the raycast
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                // If the ray hits, set the camera mode to "Attached" and set the target to the hit GameObject
                manager.settings.CameraMode = GameSettings.mode.Attatched; // Note: You have 'Attatched' spelled with two 't's, make sure it's intended
                target = hit.collider.transform; // Assuming you have a 'Target' field to set in your settings
                manager.UI.scene = 3;
                type = hit.collider.gameObject.layer;
                if (type == 3) { manager.UI.inspectorPlant = target.GetComponent<Plant>(); }
                if (type == 6) { manager.UI.inspectorCreature = target.GetComponent<Creature>(); }
            }
            else
            {
                // Reset to default camera mode and nullify the target if you wish to do so
                manager.settings.CameraMode = GameSettings.mode.Free; // Replace 'Default' with your actual default mode
                target = null;
                manager.UI.scene = 0; // Reset the scene index to your default
            }
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

    public void StartLooking()
    {
        looking = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void StopLooking()
    {
        looking = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}