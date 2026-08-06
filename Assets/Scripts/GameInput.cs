using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<GameInput>();
            }

            return instance;
        }
    }

    private static GameInput instance;

    public event EventHandler OnJumpPerformed;

    private InputActions inputActions;

    void OnEnable()
    {
        inputActions = new InputActions();
        inputActions.Player.Enable();

        inputActions.Player.Jump.performed += Jump_Performed;
    }

    private void Jump_Performed(InputAction.CallbackContext _context)
    {
        OnJumpPerformed?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementDirection()
    {
        return inputActions.Player.Move.ReadValue<Vector2>();
    }

    void OnDisable()
    {
        inputActions.Player.Jump.performed -= Jump_Performed;
    }
}
