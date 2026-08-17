using UnityEngine;

public class ColoredPlatform : MonoBehaviour
{
    [SerializeField]
    private GameObject visuals;

    public void Show()
    {
        visuals.SetActive(true);
    }

    public void Hide()
    {
        visuals.SetActive(false);
    }
}
