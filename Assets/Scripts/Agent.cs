using System;
using Unity.Netcode;
using UnityEngine;

public class Agent : NetworkBehaviour
{
    [SerializeField]
    private float walkSpeed = 8;
    [SerializeField, Range(3, 6)]
    private float turningSpeed = 5;
    [SerializeField]
    private float jumpHeight = 3;
    [SerializeField, Range(0.01f, 0.1f)]
    private float coyoteJumpTime = 0.05f;
    [SerializeField, Range(0.05f, 0.2f)]
    private float jumpBufferTime = 0.1f;
    [SerializeField]
    private float gravityScale = 3;
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private Transform cinemachineCamera;

    enum MovementHandler
    {
        None,
        Host,
        Client
    }
    private NetworkVariable<MovementHandler> movementHandler = new(MovementHandler.None);

    private NetworkObject networkObject;
    private CharacterController characterController;
    private bool canMove;
    private bool isMovementRequestSent;
    private float gravityMagnitude;
    private float verticalVelocity;
    private float jumpVelocity;
    private bool overrideVerticalVelocity;
    private float coyoteJumpTimer;
    private float jumpBufferTimer;

    void OnEnable()
    {
        GameInput.Instance.OnJumpPerformed += Jump_Performed;
    }

    void Start()
    {
        networkObject = GetComponent<NetworkObject>();
        characterController = GetComponent<CharacterController>();

        gravityMagnitude = Physics.gravity.y * gravityScale * -1;
        jumpVelocity = Mathf.Sqrt(jumpHeight * gravityMagnitude * 2);
    }

    void Update()
    {
        CheckJump();
        Movement();
        Turn();
    }

    private void Jump_Performed(object _sender, EventArgs _event)
    {
        if (IsMovementHandlerOtherClient()) return;

        jumpBufferTimer = jumpBufferTime;
    }

    [Rpc(SendTo.NotMe)]
    private void RequestMovementRpc()
    {
        // The other player is requesting to move the agent, so acknowledge it
        // TODO: If IsClient and isMovementRequestSent, drop my request and ack this one

        RequestMovementAckRpc();
    }

    [Rpc(SendTo.NotMe)]
    private void RequestMovementAckRpc()
    {
        // The other player acknowledged the request, so allow control on agent
        ChangeOwnershipRpc(NetworkManager.Singleton.LocalClientId, true);

        canMove = true;
    }

    [Rpc(SendTo.Server)]
    private void ReleaseMovementRpc()
    {
        movementHandler.Value = MovementHandler.None;
    }

    [Rpc(SendTo.Server)]
    private void RequestJumpRpc(ulong _ownerId)
    {
        ChangeOwnershipRpc(_ownerId, false);

        RequestJumpAckRpc(RpcTarget.Single(_ownerId, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void RequestJumpAckRpc(RpcParams _rpcParams)
    {
        overrideVerticalVelocity = true;
    }

    [Rpc(SendTo.Server)]
    private void ChangeOwnershipRpc(ulong _ownerId, bool _setMovementHandler)
    {
        networkObject.ChangeOwnership(_ownerId);
        if (_setMovementHandler)
        {
            movementHandler.Value = (_ownerId == NetworkManager.Singleton.LocalClientId) ? MovementHandler.Host : MovementHandler.Client;
        }
    }

    private void CheckJump()
    {
        if (IsGrounded())
        {
            coyoteJumpTimer = coyoteJumpTime;
        }
        else
        {
            coyoteJumpTimer -= Time.deltaTime;
        }
        jumpBufferTimer -= Time.deltaTime;

        if (coyoteJumpTimer > 0 && jumpBufferTimer > 0)
        {
            if (IsMovementHandlerOtherClient()) return;

            RequestJumpRpc(NetworkManager.Singleton.LocalClientId);

            jumpBufferTimer = 0;
        }
    }

    private void Movement()
    {
        Vector3 inputDirection = GameInput.Instance.GetMovementDirection();

        // float ignoreInputDirectionMagnitudeThreshold = 0.01f;
        if (inputDirection.magnitude > 0)
        {
            // The player is trying to move the agent
            if (movementHandler.Value == MovementHandler.None && !isMovementRequestSent)
            {
                // The agent is standstill (the other player is not controlling it),
                // and it is the first attempt to move
                isMovementRequestSent = true;

                // Send the request to the other player
                RequestMovementRpc();
            }
        }
        else if (isMovementRequestSent)
        {
            // The player has just released the agent control, so notify the other player
            isMovementRequestSent = false;
            canMove = false;
            ReleaseMovementRpc();
        }

        Vector3 movementVector = Vector3.zero;
        if (canMove)
        {
            movementVector = cinemachineCamera.TransformDirection(inputDirection);
            movementVector *= walkSpeed;
        }

        movementVector.y = VerticalForceCalculation();
        characterController.Move(movementVector * Time.deltaTime);
    }

    private void Turn()
    {
        if (GameInput.Instance.GetMovementDirection().magnitude > 0)
        {
            Vector3 currentLookDirection = characterController.velocity.normalized;
            currentLookDirection.y = 0;

            currentLookDirection.Normalize();

            if (currentLookDirection.magnitude > 0)
            {
                Quaternion targetRotation = Quaternion.LookRotation(currentLookDirection);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turningSpeed * Time.deltaTime);
            }
        }
    }

    private float VerticalForceCalculation()
    {
        if (overrideVerticalVelocity)
        {
            overrideVerticalVelocity = false;
            verticalVelocity = jumpVelocity;
            return jumpVelocity;
        }

        if (IsGrounded() && verticalVelocity < 0)
        {
            verticalVelocity = -1;
        }
        else
        {
            verticalVelocity -= gravityMagnitude * Time.deltaTime;
        }

        return verticalVelocity;
    }

    private bool IsGrounded()
    {
        Vector3 halfExtents = new(characterController.radius, (characterController.skinWidth + 0.01f) * 2, characterController.radius);
        return Physics.CheckBox(transform.position, halfExtents, Quaternion.identity, groundLayer);
    }

    private bool IsMovementHandlerOtherClient()
    {
        return (IsHost && movementHandler.Value == MovementHandler.Client) ||
            (IsClient && !IsHost && movementHandler.Value == MovementHandler.Host);
    }

    void OnDisable()
    {
        GameInput.Instance.OnJumpPerformed -= Jump_Performed;
    }
}
