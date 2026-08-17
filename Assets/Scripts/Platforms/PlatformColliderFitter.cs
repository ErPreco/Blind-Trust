using UnityEngine;

public class PlatformColliderFitter : MonoBehaviour
{
    public Vector3 Center => cube.position;
    public Vector3 Size => cube.localScale;

    [SerializeField]
    private Transform cube;

    private BoxCollider boxCollider;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();

        boxCollider.center = cube.localPosition;
        boxCollider.size = cube.localScale;
    }
}
