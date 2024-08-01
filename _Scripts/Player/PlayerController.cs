using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Wall and Ground Checks")]
    [SerializeField] Collider2D playerCollider;
    [SerializeField] GameObject groundCheckBox;
    [SerializeField] GameObject ceilingCheckBox;
    Vector2 groundCheckSize = new Vector3(0.75f, 0.1f);
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float wallCheckPoints = 5;
    [SerializeField] float gizmoBuffer = 0.1f;
    [SerializeField] float wallDetectionDistance = 0.5f;
    [SerializeField] bool IsTouchingRight;
    [SerializeField] bool IsTouchingLeft;
    
    [Header("Death Effect")]
    [SerializeField] float deathJumpHieght;
    [SerializeField] AudioClip deathSound;
    public bool IsDead { get; private set; }
    public bool isFrozen;

    [Header("Movement Values")]
    [SerializeField] float speed = 12;
    [SerializeField] float acceleration = 50f;
    float horizontalInput;
    float horizontal;

    [Header("Jump Values")]
    [SerializeField] AudioClip jumpSound;
    [SerializeField] int maxJumps = 2;
    [SerializeField] float jumpForce;
    [SerializeField] float maxJumpTime;
    [SerializeField] float holdForce;
    [SerializeField] float maxJumpSpeed;
    [SerializeField] float maxFallSpeed;
    [SerializeField] float fallSpeed;
    [SerializeField] float jumpCoyoteTime = 0.5f;
    [SerializeField] float fallGravityMultiplier;
    bool jumpPressed;
    bool jumpHeld;
    bool isJumping;
    float jumpHoldTime;
    float originalGravityScale;
    float lastGroundedTime;
    int numberOfJumpsLeft;

    [Header("Dash Values")]
    [SerializeField] AudioClip dashSound;
    [SerializeField] float dashDelay = 1;
    [SerializeField] float dashStrength = 60;
    [SerializeField] float dashDuration;
    float dashTimer;
    bool canDash;
    bool isDashing;

    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D bodyCollider;
    AudioSource audioSource;
    float defaultVolume;
    RaycastHit2D[] results = new RaycastHit2D[100];

    public delegate void MovingEvent(bool isMoving);
    public static event MovingEvent PlayerMoving;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        bodyCollider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
        defaultVolume = audioSource.volume;
        jumpHoldTime = maxJumpTime;
        originalGravityScale = rb.gravityScale;
        numberOfJumpsLeft = maxJumps;
    }
 
    void Update()
    {
        // Do nothing if game is paused or player is dead/dying
        if (Time.timeScale < 1 || IsDead)
        {
            return;
        }

        // Stop player from moving after reaching the goal.
        if (isFrozen)
        {
            animator.SetFloat("Speed", 0);
            animator.SetBool("Jump", false);
            animator.SetBool("Landing", false);
            animator.SetTrigger("Grounded");
            return;
        }

        #region Movement
        horizontalInput = Input.GetAxis("Horizontal");
        lastGroundedTime -= Time.deltaTime;

        UpdateGrounding();
        UpdateWallTouching();
        UpdateSpriteDirection();
        UpdateMovement();

        // Send out movement event when the player moves
        if (rb.velocity.magnitude > 0.01f)
        {
            PlayerMoving(true);
            animator.SetFloat("Speed", 1);
        }
        else
        {
            PlayerMoving(false);
            animator.SetFloat("Speed", 0);
        }
        #endregion

        #region Jumping
        jumpPressed = Input.GetButtonDown("Jump");
        jumpHeld = Input.GetButton("Jump");
        CheckForJump();
        #endregion

        #region Dashing
        dashTimer += Time.deltaTime;
        canDash = dashTimer >= dashDelay;

        #endregion
    }


    void FixedUpdate()
    {
        if (IsDead || isFrozen)
            return;

        IsJumping();
    }

    // Accelerate player in desired direction
    void UpdateMovement()
    {
        if (Input.GetButtonDown("Dash") && canDash)
            isDashing = true;

        float desiredHorizontal = horizontalInput * speed;

        if (desiredHorizontal > horizontal)
        {
            horizontal += acceleration * Time.deltaTime;
            if (horizontal > desiredHorizontal)
                horizontal = desiredHorizontal;
        }
        else if (desiredHorizontal < horizontal)
        {
            horizontal -= acceleration * Time.deltaTime;
            if (horizontal < desiredHorizontal)
                horizontal = desiredHorizontal;
        }

        // Stop movement if walking into a wall
        if (IsTouchingRight && horizontalInput > 0)
            horizontal = 0;
        if (IsTouchingLeft && horizontalInput < 0)
            horizontal = 0;
        
        if (isDashing)
            Dash();
        else
            rb.velocity = new Vector2(horizontal, rb.velocity.y);
    }

    // Face player sprite in direction they are moving.
    void UpdateSpriteDirection()
    {
        if (horizontalInput < 0)
            spriteRenderer.flipX = true;
        if (horizontalInput > 0)
            spriteRenderer.flipX = false;
    }

    // Check if player is attempting to jump
    void CheckForJump()
    {
        if (!jumpPressed)
            return;

        // If player hasn't been grounded for longer than the Coyote time and still has max jumps, they are falling
        if (lastGroundedTime < 0 && numberOfJumpsLeft == maxJumps)
        {
            isJumping = false;
            return;
        }
   
        // Reset gravity, velocity and jumptime so air jumps act like grounded jumps
        numberOfJumpsLeft--;
        if (numberOfJumpsLeft >= 0)
        {
            audioSource.PlayOneShot(jumpSound);
            isJumping = true;
            rb.gravityScale = originalGravityScale;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            jumpHoldTime = maxJumpTime;
        }
    }

    // Apply inititial jumping force. Check for held jumps and falling
    void IsJumping()
    {
        if (isJumping)
        {
            rb.AddForce(Vector2.up * jumpForce);
            HoldingJump();
        }
        if (rb.velocity.y > maxJumpSpeed)
            rb.velocity = new Vector2(rb.velocity.x, maxJumpSpeed);

        Falling();
    }

    // Apply upward force to player as long as jump is held up the max jump time.
    void HoldingJump()
    {
        if (!jumpHeld)
        {
            isJumping = false;
            return;
        }

        jumpHoldTime -= Time.deltaTime;
        if (jumpHoldTime <= 0)
        {
            animator.SetBool("Jump", false);
            animator.SetBool("Landing", true);
            jumpHoldTime = 0;
            isJumping = false;
        }
        else
        {
            rb.AddForce(Vector2.up * holdForce);
            animator.SetBool("Jump", true);
            animator.SetBool("Landing", false);
        }
    }

    // Increase player's gravity scale if they are falling.
    void Falling()
    {
        if (!isJumping && rb.velocity.y < fallSpeed)
            rb.gravityScale = fallGravityMultiplier;

        if (rb.velocity.y < maxFallSpeed)
            rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);
    }

    // Apply impulse force to the player in the direction they are moving
    void Dash()
    {
        if (!canDash)
            return;

        isDashing = true;
        canDash = false;
        jumpHeld = false;
        dashTimer = 0;

        if (Mathf.Abs(horizontalInput) > 0)
        {
            audioSource.PlayOneShot(dashSound);        
            rb.AddForce(dashStrength * Mathf.Sign(horizontalInput) * Vector2.right, ForceMode2D.Impulse);
        }

        StartCoroutine(DashDuration());
    }

    IEnumerator DashDuration()
    {
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        rb.velocity = new Vector2(horizontal, rb.velocity.y);
    }


    // Check if the player is touching the ground
    void UpdateGrounding()
    {
        if (Physics2D.OverlapBox(groundCheckBox.transform.position, groundCheckSize, 0, groundLayer))
        {
            lastGroundedTime = jumpCoyoteTime;
            rb.gravityScale = originalGravityScale;
            animator.SetBool("Landing", false);
            animator.SetBool("Jump", false);
            animator.SetTrigger("Grounded");
            return;
        }

        // Stop player's jump if they hit a ceiling
        if (Physics2D.OverlapBox(ceilingCheckBox.transform.position, groundCheckSize, 0, groundLayer))
        {
            animator.SetBool("Jump", false);
            animator.SetBool("Landing", true);
            jumpHoldTime = 0;
        }
    }

    private void UpdateWallTouching()
    {
        IsTouchingRight = CheckForWall(Vector2.right);
        IsTouchingLeft = CheckForWall(Vector2.left);
    }

    // Checks given direction for wall collisions
    bool CheckForWall(Vector2 direction)
    {
        float colliderHeight = playerCollider.bounds.size.y - 2 * gizmoBuffer;
        float segmentSize = colliderHeight / (wallCheckPoints - 1);

        for (int i = 0; i < wallCheckPoints; i++)
        {
            Vector3 origin = playerCollider.bounds.center; 
            origin += new Vector3(0, (i - (wallCheckPoints - 1) * 0.5f) * segmentSize, 0);
            origin += (Vector3)direction * wallDetectionDistance;

            // Store raycast hits in results
            int hits = Physics2D.Raycast(origin,
                direction,
                new ContactFilter2D() { layerMask = groundLayer, useLayerMask = true },
                results,
                 0.1f);

            for (int hitIndex = 0; hitIndex < hits; hitIndex++)
            {
                var hit = results[hitIndex];
                if (hit.collider && hit.collider.isTrigger == false && !hit.transform.CompareTag("Stairs"))
                    return true;
            }
        }
        return false;
    }

    // Play Death animation and reset player location
    IEnumerator PlayerDeath()
    {
        animator.SetBool("isDead", true);
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(.1f);
        rb.gravityScale = fallGravityMultiplier;
        rb.AddForce(Vector2.up * deathJumpHieght, ForceMode2D.Impulse);
        bodyCollider.enabled = false;
        yield return new WaitForSeconds(1.5f);
        bodyCollider.enabled = true;
        animator.SetBool("isDead", false);
        yield return new WaitForSeconds(.2f);
        IsDead = false;
        audioSource.volume = defaultVolume;
        rb.velocity = Vector2.zero;
        GameManager.instance.ResetPlayerPosition();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Reset number of jumps when player touches the ground
        if (collision.gameObject.layer == 6)
            numberOfJumpsLeft = maxJumps;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hazard") && !IsDead)
        {
            audioSource.volume = 1f;
            audioSource.PlayOneShot(deathSound);
            IsDead = true;
            StartCoroutine(PlayerDeath());
        }
    }

    // Prevents player from sliding down stair when not moving
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Stairs") && Mathf.Abs(horizontalInput) < 0.001 && !jumpHeld)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else
        {
            rb.constraints &= ~RigidbodyConstraints2D.FreezePosition;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Stairs"))
        {
            rb.constraints &= ~RigidbodyConstraints2D.FreezePosition;
        }
    }

    // Draw side detection gizmos on player
    void OnDrawGizmos()
    {
        DrawGizmosForSide(Vector2.right);
        DrawGizmosForSide(Vector2.left);
    }

    void DrawGizmosForSide(Vector2 direction)
    {
        float colliderHeight = playerCollider.bounds.size.y - 2 * gizmoBuffer; // Subtract buffer from both top and bottom
        float segmentSize = colliderHeight / (wallCheckPoints - 1); // Adjust for one less segment

        for (int i = 0; i < wallCheckPoints; i++)
        {
            Vector3 origin = playerCollider.bounds.center; // Use collider center as the origin
            origin += new Vector3(0, (i - (wallCheckPoints - 1) * 0.5f) * segmentSize, 0); // Adjust origin calculation for buffer
            origin += (Vector3)direction * wallDetectionDistance;
            Gizmos.DrawWireSphere(origin, 0.05f);
        }
    }
}
