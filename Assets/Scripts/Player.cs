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

    private InputSystem inputSystem;
    private Rigidbody rb;
    private new CapsuleCollider collider;
    private float coyoteJumpTimer;
    private float jumpBufferTimer;
    private float colliderRadius;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
    }

    void OnEnable()
    {
        inputSystem = new InputSystem();
        inputSystem.Player.Enable();
        inputSystem.Player.Jump.performed += Jump_Performed;
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

        Vector2 inputVector = inputSystem.Player.Move.ReadValue<Vector2>();
        Vector3 velocity = new Vector3(inputVector.x * speed, rb.linearVelocity.y, inputVector.y * speed);

        rb.linearVelocity = velocity;
    }

    private void Jump_Performed(UnityEngine.InputSystem.InputAction.CallbackContext _context)
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
        inputSystem.Player.Jump.performed -= Jump_Performed;
        inputSystem.Player.Disable();
    }
}
