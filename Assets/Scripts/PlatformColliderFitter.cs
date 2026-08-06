using UnityEngine;

public class PlatformColliderFitter : MonoBehaviour
{
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
