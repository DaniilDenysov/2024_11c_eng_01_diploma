using UnityEngine;

public class WeaponWobble : MonoBehaviour
{
    [Header("Wobble Settings")]
    public float intensity = 1f;
    public float smoothness = 5f;
    public float maxOffset = 0.05f; 

    private Vector3 initialPosition;
    private Quaternion initialRotation; 

    void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    void Update()
    {

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Vector3 offset = new Vector3(-mouseX, -mouseY, 0) * intensity;
        offset = Vector3.ClampMagnitude(offset, maxOffset); 


        Quaternion rotationOffset = Quaternion.Euler(
            new Vector3(mouseY * intensity, mouseX * intensity, 0)
        );

        transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition + offset, Time.deltaTime * smoothness);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, initialRotation * rotationOffset, Time.deltaTime * smoothness);
    }
}
