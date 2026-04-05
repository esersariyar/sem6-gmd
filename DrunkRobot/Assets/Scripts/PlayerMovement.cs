using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 0.5f;
    public float jumpForce = 8f;
    public float airControl = 0.3f;

    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundLayer;

    public int maxJumps = 2;

    private Rigidbody rb;
    private bool isGrounded;
    private Vector3 moveInput;
    public bool canMove = false;
    private int jumpsRemaining;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jumpsRemaining = maxJumps;
    }

    void Update()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);

        if (isGrounded && !wasGrounded)
        {
            jumpsRemaining = maxJumps;
        }

        if (!canMove)
        {
            moveInput = Vector3.zero;
            return;
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(h) < 0.1f) h = 0f;
        if (Mathf.Abs(v) < 0.1f) v = 0f;

        Transform cam = Camera.main.transform;

        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        moveInput = camForward * v + camRight * h;

        if (v > 0f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(camForward);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        if (Input.GetKeyDown(KeyCode.Space) && jumpsRemaining > 0)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpsRemaining--;
        }
    }

    void FixedUpdate()
    {
        float currentSpeed = isGrounded ? speed : speed * airControl;

        Vector3 move = moveInput.normalized * currentSpeed;

        Vector3 velocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
        rb.linearVelocity = velocity;
    }

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y > 0.3f)
            {
                jumpsRemaining = maxJumps;
                return;
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (isGrounded) return;
        
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Mathf.Abs(contact.normal.x) > 0.5f || Mathf.Abs(contact.normal.z) > 0.5f)
            {
                if (jumpsRemaining < 1)
                    jumpsRemaining = 1;
                return;
            }
        }
    }
}