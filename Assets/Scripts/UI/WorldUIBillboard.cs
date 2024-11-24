using UnityEngine;

public class WorldUIBillboard : MonoBehaviour
{
    [Header("Settings")]
    public bool lockX = false;
    public bool lockY = false;
    public bool lockZ = false;

    private void LateUpdate()
    {
        var camera = NetworkPlayer.LocalPlayerInstance.GetPlayerCamera();
        if (camera == null) return;
        Vector3 directionToFace = camera.transform.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(-directionToFace);
        Vector3 eulerRotation = targetRotation.eulerAngles;
        if (lockX) eulerRotation.x = transform.eulerAngles.x;
        if (lockY) eulerRotation.y = transform.eulerAngles.y;
        if (lockZ) eulerRotation.z = transform.eulerAngles.z;
        transform.rotation = Quaternion.Euler(eulerRotation);
    }
}
