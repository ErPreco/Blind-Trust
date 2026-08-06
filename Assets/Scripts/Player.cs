using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private InputSystem inputSystem;

    void OnEnable()
    {
        inputSystem = new InputSystem();
        inputSystem.Player.Enable();
    }

    void Start()
    {
        if (IsOwner)
        {
            GetComponentInChildren<Renderer>().material.color = Color.red;
        }
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;

        Vector2 inputVector = inputSystem.Player.Move.ReadValue<Vector2>();
        Vector3 direction = new Vector3(inputVector.x, 0, inputVector.y);
        transform.Translate(3 * Time.fixedDeltaTime * direction);
    }

    void OnDisable()
    {
        inputSystem.Player.Disable();
    }
}
