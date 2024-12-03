using Mirror;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerCameraController : NetworkBehaviour
{
    [Header("Mouse Settings")]
    public float mouseSensitivity = 100f;  // Mouse sensitivity
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

    [Header("Recoil Settings")]
    public float recoilRecoverySpeed = 5f; // Speed at which the recoil recovers

    private float xRotation = 0f;         // Vertical rotation
    private float yRotation = 0f;         // Horizontal rotation
    private float currentFOV;             // Current FOV
    private Vector3 targetRotation;       // Target rotation for smoothing
    private Vector3 headBobOffset;        // Offset for head bobbing

    private Vector2 currentRecoilOffset = Vector2.zero; // Current recoil offset
    private Vector2 targetRecoilOffset = Vector2.zero;  // Target recoil offset

    [SerializeField] private Transform playerBody;         // Reference to the player's body
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Camera _camera;

    private Vector3 rotationVelocity;     // Used for smooth damping

    private float recoilRecoveryDelay = 0.3f;
    private float currentDelay = 0f;


    [SerializeField] private WeaponSO weaponSO;

    void Start()
    {
        _camera = GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentFOV = defaultFOV;
        Camera.main.fieldOfView = defaultFOV;
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            _camera.enabled = false;
            Destroy(this);
            return;
        }
        HandleMouseLook();
        HandleFieldOfView();
        RecoverRecoil();
        // HandleHeadBobbing();
    }




    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

      //  targetRecoilOffset.x -= Mathf.Sign(targetRecoilOffset.x) * mouseX;
      //  targetRecoilOffset.y -= Mathf.Sign(targetRecoilOffset.y) * mouseY;
        targetRecoilOffset.x = Mathf.Clamp(targetRecoilOffset.x, -5f, 5f);
        targetRecoilOffset.y = Mathf.Clamp(targetRecoilOffset.y, -5f, 5f);
        xRotation -=  mouseY;
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);
        yRotation += mouseX;
        Quaternion recoilRotation = Quaternion.Euler(-currentRecoilOffset.y, currentRecoilOffset.x, 0);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f) * recoilRotation;
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

        if (_rigidbody.velocity.magnitude > 0.1f)
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

    public void ApplyRecoil(Vector2 recoilOffset)
    {
        currentDelay = recoilRecoveryDelay;
        targetRecoilOffset += recoilOffset;
    }

    private void RecoverRecoil()
    {
        currentRecoilOffset = Vector2.Lerp(currentRecoilOffset, targetRecoilOffset, weaponSO.FireRate);
        if (currentDelay >= 0)
        {
            currentDelay -= Time.deltaTime;
            return;
        }
        // Smoothly reduce the current recoil offset towards the target recoil offset
       

        // Gradually reduce target recoil to zero for subtle recovery
        targetRecoilOffset = Vector2.Lerp(targetRecoilOffset, Vector2.zero, Time.deltaTime * recoilRecoverySpeed);
    }


    public void SetMouseSensitivity(float sensitivity)
    {
        mouseSensitivity = sensitivity;
    }

    public void ToggleInvertY(bool isEnabled)
    {
       // invertY = isEnabled;
    }

    public void SetFOV(float fov)
    {
        defaultFOV = fov;
        currentFOV = fov;
    }
}
