using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    [Header("Player Settings")]
    [Header("-----------------------Stats")]
    [SerializeField] private float health = 10f;
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float dashforce = 5f;
    [SerializeField] private float dashCooldown = 2f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float force = 10f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.5f, 0f);
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private float horizontalInput;
    [SerializeField] private float verticalInput;
    [SerializeField] private float jumpInput;
    [SerializeField] private float dashInput;
    [Header("-----------------------States")]
    [SerializeField] private bool isAlive = true;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isTouchingWall;

    public enum maskStates {none, monkey, leopard, rhino}
    [SerializeField] private maskStates currentMask = maskStates.none;
    [Header("-----------------------References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive) return;
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        jumpInput = Input.GetButton("Jump") ? 1 : 0;
        //dashInput = Input.GetButton("Shift") ? 1 : 0;
    }
    void FixedUpdate()
    {
        PlayerIsGrounded();
        PlayerIsTounchingWall();
        MovePlayer(horizontalInput);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawWireCube(wallCheck.position, wallCheckSize);
    }
    public void PlayerIsGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
    public void PlayerIsTounchingWall()
    {
        isTouchingWall = Physics2D.OverlapBox(wallCheck.position, wallCheckSize, 0f, groundLayer);
    }
    public void MovePlayer(float horizontalInput)
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        if (horizontalInput != 0)
        {
            transform.localScale = new Vector2(horizontalInput > 0 ? 1 : -1, 1);
        }
        if (currentMask == maskStates.monkey) ClimbWall();
    }
    public void ClimbWall()
    {
        if(isTouchingWall && verticalInput != 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, verticalInput * moveSpeed);
            rb.gravityScale = 0;
        } else
        {
            rb.gravityScale = 1;
        }
    }
    public void Dash()
    {
        
    }
}
