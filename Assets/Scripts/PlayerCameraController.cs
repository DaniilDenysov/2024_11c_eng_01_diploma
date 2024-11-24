using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerCameraController : MonoBehaviour
{
    [Header("Mouse Settings")]
    public float mouseSensitivity = 100f;  // Mouse sensitivity
    public bool invertY = false;           // Option to invert Y-axis
    public float rotationSmoothTime = 0.1f; // Smoothing for rotation

    [Header("Field of View")]
    public float defaultFOV = 60f;        // Default FOV
    public float zoomFOV = 30f;           // FOV during zoom
    public float zoomSpeed = 10f;         // Speed of FOV transition

    [Header("Head Bobbing (Optional)")]
    public bool enableHeadBob = true;
    public float bobFrequency = 1.5f;
    public float bobAmplitude = 0.05f;

    [Header("Clamp Settings")]
    public float minVerticalAngle = -90f; // Minimum pitch angle
    public float maxVerticalAngle = 90f;  // Maximum pitch angle

    private float xRotation = 0f;         // Vertical rotation
    private float yRotation = 0f;         // Horizontal rotation
    private float currentFOV;             // Current FOV
    private Vector3 targetRotation;       // Target rotation for smoothing
    private Vector3 headBobOffset;        // Offset for head bobbing

    [SerializeField] private Transform playerBody;         // Reference to the player's body
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private Camera camera;

    private Vector3 rotationVelocity;     // Used for smooth damping

    void Start()
    {
        camera = GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        currentFOV = defaultFOV;
        Camera.main.fieldOfView = defaultFOV;
    }

    void Update()
    {
        HandleMouseLook();
        HandleFieldOfView();
        HandleHeadBobbing();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= (invertY ? -mouseY : mouseY);
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);
        yRotation += mouseX;
        transform.localEulerAngles = new Vector3(xRotation, 0f, 0f);
        playerBody.localEulerAngles = new Vector3(0f, yRotation, 0f);
    }

    void HandleFieldOfView()
    {
        float targetFOV = Input.GetMouseButton(1) ? zoomFOV : defaultFOV;
        currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * zoomSpeed);
        Camera.main.fieldOfView = currentFOV;
    }

    void HandleHeadBobbing()
    {
        if (!enableHeadBob) return;

        if (rigidbody.velocity.magnitude > 0.1f)
        {
            float bobOffset = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
            headBobOffset = new Vector3(0f, bobOffset, 0f);
        }
        else
        {
            headBobOffset = Vector3.zero;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, headBobOffset, Time.deltaTime * 5f);
    }

    public void SetMouseSensitivity(float sensitivity)
    {
        mouseSensitivity = sensitivity;
    }

    public void ToggleInvertY(bool isEnabled)
    {
        invertY = isEnabled;
    }

    public void SetFOV(float fov)
    {
        defaultFOV = fov;
        currentFOV = fov;
    }
}
