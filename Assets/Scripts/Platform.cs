using UnityEngine;

public class Platform : MonoBehaviour
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
