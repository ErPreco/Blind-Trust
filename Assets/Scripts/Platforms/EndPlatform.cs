using UnityEngine;

public class EndPlatform : MonoBehaviour
{
    [SerializeField]
    private LayerMask agentLayer;

    private PlatformColliderFitter platformColliderFitter;
    private Vector3 size;
    private Vector3 center;

    void Start()
    {
        platformColliderFitter = GetComponent<PlatformColliderFitter>();

        size = platformColliderFitter.Size * 0.85f;
        size.y = 0.3f;
        float centerOffset = (platformColliderFitter.Size.y + size.y) / 2;
        center = platformColliderFitter.Center + Vector3.up * centerOffset;
    }

    void Update()
    {
        if (Physics.CheckBox(center, size / 2, transform.rotation, agentLayer))
        {
            GameManager.Instance.WinConditionMet();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(center, size);
    }
}
