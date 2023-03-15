using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Layer Masks")]
    [SerializeField] LayerMask groundLayer;

    [Header("Ground Collision Variables")]
    [SerializeField] private float groundRaycastLength;
    [SerializeField] private Vector3 groundRaycastOffset;
    private bool onGround;


    [Header("Movement Variables")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float linearDrag;
    private float horizontalDirection;
    private bool facingRight = true;

    [Header("Jump Variables")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float airLinearDrag;
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float lowJumpMultiplier;
    private bool jumpRequest;
    private bool canJump => Input.GetButtonDown("Jump") && onGround;

    private float verticalDirection;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canJump) jumpRequest = true;

        if (facingRight && rb.velocity.x < 0 && anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1") == false
            || !facingRight && rb.velocity.x > 0 && anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1") == false) Flip();

        //Animation
        anim.SetBool("isGrounded", onGround);
        anim.SetFloat("horizontalDirection", Mathf.Abs(horizontalDirection));

        if(rb.velocity.y > 0)
        {
            anim.SetBool("isJumping", true);
            anim.SetBool("isFalling", false);
        }
        else
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("isFalling", true);
        }

        // saldırı yaparken yürüme hızının yavaşlaması
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1") == true)
        {
            rb.velocity = new Vector2(rb.velocity.x * 0.8f, rb.velocity.y);
        }
            
        
    }

    private void FixedUpdate()
    {
        horizontalDirection = GetInput().x;

        CheckCollisions();
        Movement();

        if (onGround)
        {
            GroundLinearDrag();
            anim.SetBool("isJumping", false);
            anim.SetBool("isFalling", false);

        }
        else
        {
            AirLinearDrag();
            ApplyFallMultiplier();            
        }

        if (jumpRequest)
        {
            Jump();
            jumpRequest = false;
        }            
        
        
    }

    /*Yön tuşları için -1 ve 1 değerlerinde çıktı alınıyor.*/
    public Vector2 GetInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void Movement()
    {
        // yatay yönde sürekli olarak hızlanma değişkeni kadar kuvvet uygulanıyor.

        rb.AddForce(new Vector2(horizontalDirection, 0) * acceleration);

        // hızlanma değeri ile hız sonsuza kadar çıkabileceği için
        // hızın maksimum hızı aşması durumunda hızı maksimum hıza eşitlemek gerekmektedir.

        if(Mathf.Abs(rb.velocity.x) > maxSpeed)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
        }
    }

    private void GroundLinearDrag()
    {
        if(Mathf.Abs(horizontalDirection) < 0.4f)
        {
            rb.drag = linearDrag;
        }
        else
        {
            rb.drag = 0;
        }
        
    }

    private void AirLinearDrag()
    {
            rb.drag = airLinearDrag;        
    }

    private void ApplyFallMultiplier()
    {
        if (rb.velocity.y < 2.5f)
        {
            rb.gravityScale = fallMultiplier;
        }        
        else if(rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.gravityScale = lowJumpMultiplier;
        }
        else
        {
            rb.gravityScale = 1f;
        }
       
    }

    private void Flip()
    {
        facingRight = !facingRight;

        rb.transform.Rotate(0, 180, 0);
    }
    
    private void Jump()
    {
        AirLinearDrag();
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        anim.SetBool("isJumping", true);
        anim.SetBool("isFalling", false);


    }

    

    private void CheckCollisions()
    {      
        onGround = Physics2D.Raycast(transform.position + groundRaycastOffset, Vector2.down, groundRaycastLength, groundLayer) ||
            Physics2D.Raycast(transform.position - groundRaycastOffset, Vector2.down, groundRaycastLength, groundLayer);        
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position + groundRaycastOffset, transform.position + groundRaycastOffset + Vector3.down * groundRaycastLength);
        Gizmos.DrawLine(transform.position - groundRaycastOffset, transform.position - groundRaycastOffset + Vector3.down * groundRaycastLength);
    }

}
