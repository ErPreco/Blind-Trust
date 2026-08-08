using System;
using Unity.Netcode;
using UnityEngine;

public class Agent : NetworkBehaviour
{
    [SerializeField]
    private float speed = 8;
    [SerializeField]
    private float jumpHeight = 3;
    [SerializeField, Range(0.01f, 0.1f)]
    private float coyoteJumpTime = 0.05f;
    [SerializeField, Range(0.05f, 0.2f)]
    private float jumpBufferTime = 0.1f;
    [SerializeField]
    private float gravityScale = 1;
    [SerializeField, Range(0.01f, 0.05f)]
    private float skinWidth = 0.03f;
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private Transform cinemachineCamera;
    [SerializeField, Range(0.03f, 0.1f)]
    private float playerRotateDampening = 0.06f;

    private NetworkObject networkObject;
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private bool canMove;
    private bool isMovementRequestSent;
    private bool isMovementHandledByOtherClient;
    private float coyoteJumpTimer;
    private float jumpBufferTimer;
    private float colliderRadius;
    private Quaternion lastRotation;

    void OnEnable()
    {
        GameInput.Instance.OnJumpPerformed += Jump_Performed;
    }

    void Start()
    {
        networkObject = GetComponent<NetworkObject>();
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        Physics.gravity *= gravityScale;
        colliderRadius = capsuleCollider.radius;
    }

    void Update()
    {
        if (IsOnObjectWithLayerMask(groundLayer))
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
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0);

            float jumpForce = Mathf.Sqrt(-2 * Physics.gravity.y * jumpHeight) - (Time.deltaTime * Physics.gravity.y * .5f);
            rb.AddForce(new Vector2(0, jumpForce), ForceMode.VelocityChange);

            jumpBufferTimer = 0;
        }
    }

    void FixedUpdate()
    {
        Vector2 inputDirection = GameInput.Instance.GetMovementDirection();

        float ignoreInputDirectionMagnitudeThreshold = 0.01f;
        if (inputDirection.magnitude >= ignoreInputDirectionMagnitudeThreshold)
        {
            // The player is trying to move the agent
            if (!isMovementHandledByOtherClient && !isMovementRequestSent)
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

        if (!canMove)
        {
            if (IsOwner)
            {
                // Makes the agent stop spinning after control is released
                transform.rotation = lastRotation;
            }

            return;
        }

        if (inputDirection.magnitude >= ignoreInputDirectionMagnitudeThreshold)
        {
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg + cinemachineCamera.eulerAngles.y;
            float _ = 0;
            float smoothTargetAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _, playerRotateDampening);

            transform.rotation = lastRotation = Quaternion.Euler(0, smoothTargetAngle, 0);

            Vector3 movementVector = (Quaternion.Euler(0, targetAngle, 0) * Vector3.forward).normalized * speed;
            movementVector.y = rb.linearVelocity.y;
            rb.linearVelocity = movementVector;
        }
    }

    private void Jump_Performed(object _sender, EventArgs _event)
    {
        if (isMovementHandledByOtherClient) return;

        RequestJumpRpc(NetworkManager.Singleton.LocalClientId);
    }

    [Rpc(SendTo.NotMe)]
    private void RequestMovementRpc()
    {
        // The other player is requesting to move the agent, so acknowledge it
        // TODO: If IsClient and isMovementRequestSent, drop my request and ack this one

        isMovementHandledByOtherClient = true;

        RequestMovementAckRpc();
    }

    [Rpc(SendTo.NotMe)]
    private void RequestMovementAckRpc()
    {
        // The other player acknowledged the request, so allow control on agent
        ChangeOwnershipToLocalClientRpc(NetworkManager.Singleton.LocalClientId);
        canMove = true;
    }

    [Rpc(SendTo.NotMe)]
    private void ReleaseMovementRpc()
    {
        isMovementHandledByOtherClient = false;
    }

    [Rpc(SendTo.Server)]
    private void RequestJumpRpc(ulong _ownerId)
    {
        ChangeOwnershipToLocalClientRpc(_ownerId);

        RequestJumpAckRpc(RpcTarget.Single(_ownerId, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void RequestJumpAckRpc(RpcParams _rpcParams)
    {
        jumpBufferTimer = jumpBufferTime;
    }

    [Rpc(SendTo.Server)]
    private void ChangeOwnershipToLocalClientRpc(ulong _ownerId)
    {
        networkObject.ChangeOwnership(_ownerId);
    }

    private bool IsOnObjectWithLayerMask(LayerMask _layerMask)
    {
        Vector3 halfExtents = new(colliderRadius, (skinWidth + 0.01f) * 2, colliderRadius);
        return Physics.CheckBox(transform.position, halfExtents, Quaternion.identity, _layerMask);
    }

    void OnDisable()
    {
        GameInput.Instance.OnJumpPerformed -= Jump_Performed;
    }
}
