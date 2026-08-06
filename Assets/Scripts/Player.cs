using System;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField]
    private float speed = 8;
    [SerializeField]
    private float jumpHeight = 3;
    [SerializeField, Range(0.01f, 0.1f)]
    private float coyoteJumpTime = .05f;
    [SerializeField, Range(0.5f, 0.2f)]
    private float jumpBufferTime = .1f;
    [SerializeField]
    private float gravityScale = 1;
    [SerializeField, Range(0.01f, 0.05f)]
    private float skinWidth = .03f;
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private Transform cinemachineCamera;
    [SerializeField, Range(0.03f, 0.1f)]
    private float playerRotateDampening = 0.06f;

    private Rigidbody rb;
    private new CapsuleCollider collider;
    private float coyoteJumpTimer;
    private float jumpBufferTimer;
    private float colliderRadius;
    private Quaternion lastRotation;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
    }

    void OnEnable()
    {
        GameInput.Instance.OnJumpPerformed += Jump_Performed;
    }

    void Start()
    {
        if (IsOwner)
        {
            GetComponentInChildren<Renderer>().material.color = Color.red;
        }

        Physics.gravity *= gravityScale;
        colliderRadius = collider.radius;
    }

    void Update()
    {
        if (!IsOwner) return;

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
        if (!IsOwner) return;

        Vector2 inputDirection = GameInput.Instance.GetMovementDirection();
        if (inputDirection.magnitude >= 0.05f)
        {
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg + cinemachineCamera.eulerAngles.y;
            float _ = 0;
            float smoothTargetAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _, playerRotateDampening);

            transform.rotation = lastRotation = Quaternion.Euler(0, smoothTargetAngle, 0);

            Vector3 movementVector = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward * speed;
            movementVector.y = rb.linearVelocity.y;
            rb.linearVelocity = movementVector;
        }
        else
        {
            transform.rotation = lastRotation;
        }
    }

    private void Jump_Performed(object _sender, EventArgs _event)
    {
        jumpBufferTimer = jumpBufferTime;
    }

    private bool IsOnObjectWithLayerMask(LayerMask _layerMask)
    {
        Vector3 halfExtents = new Vector3(colliderRadius, (skinWidth + 0.01f) * 2, colliderRadius);
        return Physics.CheckBox(transform.position, halfExtents, Quaternion.identity, _layerMask);
    }

    void OnDisable()
    {
        GameInput.Instance.OnJumpPerformed -= Jump_Performed;
    }
}
