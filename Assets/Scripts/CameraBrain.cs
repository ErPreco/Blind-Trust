using UnityEngine;

public class CameraBrain : MonoBehaviour
{
    [SerializeField]
    private Transform cameraOrigin;

    void Start()
    {
        transform.SetPositionAndRotation(cameraOrigin.position, cameraOrigin.rotation);
    }
}
