using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 10f;
    public float jumpForce = 6f;
    public float airControl = 0.3f;

    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private bool isGrounded;
    private Vector3 moveInput;
    public bool canMove = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
        
        if (!canMove)
        {
            moveInput = Vector3.zero;
            return;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Transform cam = Camera.main.transform;
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        moveInput = camForward * v + camRight * h;

        if (moveInput.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveInput);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        float currentSpeed = isGrounded ? speed : speed * airControl;

        Vector3 move = moveInput.normalized * currentSpeed;

        Vector3 velocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
        rb.linearVelocity = velocity;
    }
}