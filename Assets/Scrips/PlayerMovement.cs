using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSmoothTime = 0.1f;
    public float jumpForce = 5f;
    public float groundCheckDistance = 0.7f;
    public LayerMask groundLayers = ~0;

    [Header("References")]
    public Transform cameraTransform;
    public Animator animator;

    private Rigidbody rb;
    private float rotationVelocity;
    private bool isGrounded;
    private bool jumpRequest;

    [HideInInspector]
    public bool movementEnabled;

    private WandSpell wandSpellScript;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        movementEnabled = true;

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(transform.position + Vector3.down * 0.3f, 0.3f, groundLayers);

        if (Input.GetButtonDown("Jump"))
            jumpRequest = true;

        // Cast spell on C key
        if (Input.GetKeyDown(KeyCode.C) && movementEnabled)
        {
            StartCoroutine(CastSpellRoutine());
        }
    }

    void FixedUpdate()
    {
        if (movementEnabled)
        {
            MovePlayer();
        }
        else
        {
            // Stop horizontal movement while casting (preserve vertical velocity)
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }

        HandleJump();
        jumpRequest = false;
    }

    void MovePlayer()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * vertical + camRight * horizontal).normalized;
        float speed = moveDir.magnitude;

        animator.SetFloat("Speed", speed);

        if (moveDir.sqrMagnitude > 0.001f)
        {
            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
            float smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle,
                                                        ref rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);

            Vector3 move = moveDir * moveSpeed;
            rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
        }
        else
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
    }

    void HandleJump()
    {
        if (jumpRequest && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
    }

    private IEnumerator CastSpellRoutine()
    {
        movementEnabled = false;

        // Immediately stop horizontal movement
        rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);

        animator.SetTrigger("Cast");

        // Optional: wandSpellScript.CastSpell();

        yield return new WaitForSeconds(4.5f); // Adjust for your animation length

        movementEnabled = true;
    }
}
