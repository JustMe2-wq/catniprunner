using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    private PlayerControls controls;

    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float wallSlideSpeed = 1f;
    [SerializeField] private float wallJumpForce = 7f;

    [Header("Checks")]
    [SerializeField] private Transform wallCheckLeft;
    [SerializeField] private Transform wallCheckRight;
    [SerializeField] private float wallCheckRadius = 0.2f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private float moveInput;
    private bool isGrounded = true;
    private bool isTouchingWallLeft;
    private bool isTouchingWallRight;
    private bool isWallSliding;
    private bool isFacingRight = true;
    private float wallJumpLockTime = 0.2f;
    private float wallJumpLockCounter = 0f;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 2f;
        body.freezeRotation = true;
        controls = new PlayerControls();
        controls.Player.Move.performed += OnMove;
        controls.Player.Move.canceled += OnMove;
        controls.Player.Jump.performed += OnJump;
        controls.Enable();
    }

    private void OnDestroy()
    {
        controls.Disable();
    }

    private void FixedUpdate()
    {
        if (wallJumpLockCounter > 0)
            wallJumpLockCounter -= Time.fixedDeltaTime;

        isTouchingWallLeft = Physics2D.OverlapCircle(wallCheckLeft.position, wallCheckRadius, wallLayer);
        isTouchingWallRight = Physics2D.OverlapCircle(wallCheckRight.position, wallCheckRadius, wallLayer);
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        Debug.Log("isGrounded: " + isGrounded);
        Debug.Log("isTouchingWallLeft: " + isTouchingWallLeft);

        isWallSliding = false;
        if (!isGrounded)
        {
            if (isTouchingWallLeft && moveInput < 0)
                isWallSliding = true;
            else if (isTouchingWallRight && moveInput > 0)
                isWallSliding = true;
        }

        if (isWallSliding)
        {
            body.velocity = new Vector2(body.velocity.x, Mathf.Clamp(body.velocity.y, -wallSlideSpeed, float.MaxValue));
        }
        else
        {
            body.velocity = new Vector2(moveInput * speed, body.velocity.y);
        }

        if ((moveInput > 0 && !isFacingRight) || (moveInput < 0 && isFacingRight))
        {
            Flip();
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>().x;
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (isWallSliding && (isTouchingWallLeft || isTouchingWallRight))
        {
            if (isTouchingWallLeft)
                body.velocity = new Vector2(wallJumpForce, jumpForce);
            else if (isTouchingWallRight)
                body.velocity = new Vector2(-wallJumpForce, jumpForce);

            isWallSliding = false;
            wallJumpLockCounter = wallJumpLockTime;
        }
        else if (isGrounded)
        {
            body.velocity = new Vector2(body.velocity.x, jumpForce);
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    private void OnDrawGizmosSelected()
    {
        if (wallCheckLeft != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(wallCheckLeft.position, wallCheckRadius);
        }
    }
}
