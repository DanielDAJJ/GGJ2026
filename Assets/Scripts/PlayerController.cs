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
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float force = 10f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.5f, 0f);
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private float horizontalInput;
    [SerializeField] private float verticalInput;
    [SerializeField] private float jumpInput;
    [SerializeField] private float extraJumps = 1f;
    [SerializeField] private float reamingJumps;
    [SerializeField] private float dashInput;
    [SerializeField] private float wallJumpTime;
    [Header("-----------------------States")]
    [SerializeField] private bool isAlive = true;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isTouchingWall;

    public enum maskStates {none, monkey, leopard, rhino}
    [SerializeField] private maskStates currentMask = maskStates.none;
    [Header("-----------------------References")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;


    void Awake()
    {
        groundLayer = LayerMask.GetMask("Ground");
        wallLayer = LayerMask.GetMask("Wall");
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
        if (isGrounded || (currentMask == maskStates.monkey && isTouchingWall))
        {
            reamingJumps = extraJumps;
        }
        if (Input.GetButtonDown("Jump"))
        {
            if(isGrounded || ( currentMask == maskStates.monkey && isTouchingWall))
            {
                Jump();
            } else if(currentMask == maskStates.monkey && reamingJumps > 0)
            {
                Jump();
                reamingJumps--;
            }
        }        
    }
    void FixedUpdate()
    {
        PlayerIsGrounded();
        PlayerIsTounchingWall();
        if(currentMask == maskStates.monkey && isTouchingWall)
        {
            ClimbWall();
        }
        else
        {
            rb.gravityScale = 1f;
            MovePlayer(horizontalInput);
        }
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
        isTouchingWall = Physics2D.OverlapBox(wallCheck.position, wallCheckSize, 0f, wallLayer);
    }
    public void MovePlayer(float horizontalInput)
    {
        if (wallJumpTime <= 0)
        {
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
            if (horizontalInput != 0)
            {
                transform.localScale = new Vector2(horizontalInput > 0 ? 1 : -1, 1);
            }
        } 
        else
        {
            wallJumpTime -= Time.deltaTime; 
        }
    }
    public void ClimbWall()
    {
        if (isTouchingWall)
        {
            rb.gravityScale = 0f;
            float vSpeed = verticalInput * moveSpeed;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, vSpeed);
        }
        else
        {
            rb.gravityScale = 1f;
        }
    }
    public void Jump()
    {
        if(currentMask == maskStates.monkey && isTouchingWall && !isGrounded)
        {
            wallJumpTime = 0.2f;
            float pushDirection = transform.localScale.x > 0 ? -1 : 1;
            rb.linearVelocity = new Vector2(pushDirection * moveSpeed * 2f, jumpForce);
            transform.localScale = new Vector2(pushDirection, 1);
        }
        else
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }
    public void Dash()
    {
        
    }
}
