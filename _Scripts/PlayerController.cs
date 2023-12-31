using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Physics")]
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D bodyCollider;
    private AudioSource audioSource;
    private float defaultVolume;
    [SerializeField] GameObject groundCheckBox;
    [SerializeField] GameObject ceilingCheckBox;
    private Vector2 groundCheckSize = new Vector3(0.75f, 0.1f);
    [SerializeField] LayerMask groundLayer;
    
    [Header("Death")]
    [SerializeField] float deathJumpHieght;
    [SerializeField] AudioClip deathSound;
    public bool IsDead { get; private set; }
    public bool isFrozen;

    [Header("Movement")]
    [SerializeField] float speed = 3;
    [SerializeField] float acceleration = 3f;
    [SerializeField] float deceleration = 3f;
    [SerializeField] float velPower = 0.9f;
    [SerializeField] float stoppingFriction = 0.2f;
    private float horizontalInput;

    [Header("Jump")]
    [SerializeField] AudioClip jumpSound;
    [SerializeField] int maxJumps;
    [SerializeField] float jumpForce;
    [SerializeField] float maxJumpTime;
    [SerializeField] float holdForce;
    [SerializeField] float maxJumpSpeed;
    [SerializeField] float maxFallSpeed;
    [SerializeField] float fallSpeed;
    [SerializeField] float jumpCoyoteTime = 0.5f;
    [SerializeField] float fallGravityMultiplier;
    private bool jumpPressed;
    private bool jumpHeld;
    private bool isJumping;
    private float jumpHoldTime;
    private float originalGravityScale;
    private float lastGroundedTime;
    private int numberOfJumpsLeft;

    [Header("Dash")]
    [SerializeField] AudioClip dashSound;
    [SerializeField] float dashDelay;
    [SerializeField] float dashStrength;
    private float dashTimer;
    private bool canDash;

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
        if (Time.timeScale < 1)
        {
            return;
        }

        if (IsDead)
        {
            return;
        }

        // Used to stop player from moving after reaching the goal.
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

        // Face player sprite in direction they are moving.
        if (horizontalInput < 0)
        {
            spriteRenderer.flipX = true;
        }
        if (horizontalInput > 0)
        {
            spriteRenderer.flipX = false;
        }
        
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
        IsGrounded();

        #endregion

        #region Dashing
        dashTimer += Time.deltaTime;
        if (dashTimer >= dashDelay)
        {
            canDash = true;
        }

        if (Input.GetButtonDown("Dash") && canDash)
        {
            canDash = false;
            jumpHeld = false;
            dashTimer = 0;
            Dash();
        }
        #endregion
    }

    private void FixedUpdate()
    {
        if (IsDead || isFrozen)
        {
            return;
        }

        IsJumping();

        #region Movement
        // Set direction and velocity
        float targetSpeed = horizontalInput * speed;
        // Difference between desired velocity and current velocity
        float speedDifference = targetSpeed - rb.velocity.x;
        // If target speed is > 0, accelerate otherwise deccelerate
        float accel = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        // Applies the accel to speed difference and raises to a set power, multiplies by sign to maintain direction
        float movement = Mathf.Pow(Mathf.Abs(speedDifference) * accel, velPower) * Mathf.Sign(speedDifference);

        rb.AddForce(movement * Vector2.right);
        #endregion

        #region Stopping
        // Apply a slowing force to player when stopping.
        if (Mathf.Abs(horizontalInput) < 0.01f)
        {
            // Choose friction amount or velocity (if stopped)
            float stopPower = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(stoppingFriction));
            // Use sign to determine movement direction
            stopPower *= Mathf.Sign(rb.velocity.x);
            // Apply opposite force to stop movement.
            rb.AddForce(Vector2.right * -stopPower, ForceMode2D.Impulse);
        }
        #endregion

    }

    // Check if player is attempting to jump
    void CheckForJump()
    {
        if (!jumpPressed)
        {
            return;
        }

        // If player hasn't been grounded for longer than the Coyote time and still has max jumps, they are falling
        if (lastGroundedTime < 0 && numberOfJumpsLeft == maxJumps)
        {
            isJumping = false;
            return;
        }
   
        // Remove a jump, if player had at least 1 jump left, reset gravity velocity and jumptime so air jumps act like grounded jumps
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
        {
            rb.velocity = new Vector2(rb.velocity.x, maxJumpSpeed);
        }
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
        {
            rb.gravityScale = fallGravityMultiplier;
        }
        if (rb.velocity.y < maxFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);
        }
    }

    // Apply impulse force to the player in the direction they are moving
    void Dash()
    {
        // Play SFX if the player is moving
        if (Mathf.Abs(horizontalInput) > 0)
        {
            audioSource.PlayOneShot(dashSound);        
        }

        rb.AddForce(dashStrength * Mathf.Round(horizontalInput) * Vector2.right, ForceMode2D.Impulse);
    }

    // Check if the player is touching the ground
    private void IsGrounded()
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

    // Play Death animation and reset player location
    private IEnumerator PlayerDeath()
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Reset number of jumps when feet collider tough the ground
        if (collision.gameObject.layer == 6)
        {
            numberOfJumpsLeft = maxJumps;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hazard") && !IsDead)
        {
            audioSource.volume = 1f;
            audioSource.PlayOneShot(deathSound);
            IsDead = true;
            StartCoroutine(PlayerDeath());
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
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

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Stairs"))
        {
            rb.constraints &= ~RigidbodyConstraints2D.FreezePosition;
        }
    }
}
