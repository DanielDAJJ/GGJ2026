using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    [Header("Player Settings")]
    [Header("-----------------------Stats")]
    [SerializeField] private float health = 10f;
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float dashforce = 15f;
    [SerializeField] private float dashCooldown = 1.5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float force = 10f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.5f, 0f);
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private float horizontalInput;
    [SerializeField] private float verticalInput;
    [SerializeField] private float extraJumps = 1f;
    [SerializeField] private float reamingJumps;
    [SerializeField] private float wallJumpTime;
    [Header("-----------------------States")]
    [SerializeField] private bool isAlive = true;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isTouchingWall;
    [SerializeField] private bool isDashing = false;
    [SerializeField] private bool canDash = true;
    [SerializeField] private bool isHanging;
    

    public enum maskStates {none, monkey, leopard, rhino}
    [SerializeField] private maskStates currentMask = maskStates.none;
    [Header("-----------------------References")]
    [SerializeField] private LayerMask lianaLayer;
    [SerializeField] private Transform currentLiana;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;


    void Awake()
    {
        lianaLayer = LayerMask.GetMask("Liana");
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
        if (isGrounded || (currentMask == maskStates.monkey && isTouchingWall) || isHanging)
        {
            reamingJumps = extraJumps;
        }
        if (Input.GetButtonDown("Jump"))
        {
            if (isHanging)
            {
                isHanging = false;
                currentLiana = null;
                Jump();
                return;
            }
            if(isGrounded || ( currentMask == maskStates.monkey && isTouchingWall))
            {
                Jump();
            } else if(currentMask == maskStates.monkey && reamingJumps > 0)
            {
                Jump();
                reamingJumps--;
            }
        }
        if(Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isDashing && currentMask == maskStates.leopard)
        {
            StartCoroutine(DashCoroutine());
        }
    }
    void FixedUpdate()
    {
        if(isDashing) return;
        PlayerIsGrounded();
        PlayerIsTounchingWall();
        if (isHanging)
        {
            HangingOnLina();
        }
        else if(currentMask == maskStates.monkey && isTouchingWall)
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (currentMask == maskStates.monkey && collision.CompareTag("Liana"))
        {
            isHanging = true;
            currentLiana = collision.transform;
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Liana"))
        {
            isHanging = false;
            currentLiana = null;
            rb.gravityScale = 1f;
        }
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
    public void HangingOnLina()
    {
        if(isHanging && currentLiana != null)
        {
            rb.gravityScale = 0f;
            float vSpeed = verticalInput * moveSpeed;
            rb.linearVelocity =  new Vector2(0, vSpeed);
            if(horizontalInput != 0)
            {
                transform.localScale = new Vector2(horizontalInput > 0 ? 1 : -1, 1);
                float offset = 0.3f;
                float targetx = currentLiana.position.x + (horizontalInput * offset);
                transform.position = new Vector2(targetx, transform.position.y);
            }
        }
    }
    public IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        float dPosition = transform.localScale.x * dashforce;
        rb.linearVelocity = new Vector2(dPosition, 0f);
        yield return new WaitForSeconds(0.25f);
        isDashing = false;
        rb.gravityScale = originalGravity;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    } 
}
