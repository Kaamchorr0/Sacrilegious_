using System;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Rigidbody2D rb;
    private float movement;
    public float jumpForce = 10f;
    public float moveForce = 10f;
    private bool isGrounded = false;
    private SpriteRenderer sr;
    private Animator anim;
    public GameObject pPrefab;
    public Transform fPoint;
    public Transform meleePoint;
    public float meleeAttackRange = 0.5f;
    public float meleeDamage = 5f;
    public LayerMask bossLayer;
    private Vector2 lastGroundedPosition;
    public float fallRespawnThreshold = -0.6f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGrounded)
        {
            lastGroundedPosition = transform.position;
        }

        if (transform.position.y < fallRespawnThreshold)
        {
            Vector2 respawnpos = transform.position;
            transform.position = lastGroundedPosition;
            respawnpos.y += 3f;
            rb.linearVelocity = Vector2.zero;
        }

        movement = Input.GetAxisRaw("Horizontal");
        //Attack
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("isAttack");
            PerformMelee();
        }
        //Long Range Attack
        if (Input.GetMouseButtonDown(1) && movement == 0f)
        {
            anim.SetTrigger("isAttack2");
        }
        //Jump& animation
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            isGrounded = false;
            Debug.Log("Jump");
            anim.SetBool("isJump", true);
        }
        //Walk& animation
        if (movement > 0)
        {
            anim.SetBool("isWalk", true);
            sr.flipX = false;
        }
        else if (movement < 0)
        {
            anim.SetBool("isWalk", true);
            sr.flipX = true;
        }
        else
        {
            anim.SetBool("isWalk", false);
        }

    }
    void PerformMelee()
    {
        Collider2D hitBoss = Physics2D.OverlapCircle(meleePoint.position, meleeAttackRange, bossLayer);
        if (hitBoss != null)
        {
            BossController boss = hitBoss.GetComponent<BossController>(); 
            AttackTracker tracker = GameObject.FindGameObjectWithTag("Player").GetComponent<AttackTracker>();      
            boss.TakeDamage(meleeDamage);
            tracker.LogCloseAtk();
        }
    }
    public void FireProjectile()
    {
        float spawnDistance = 0.3f;
        bool isRight = !sr.flipX;
        float Offset = isRight ? spawnDistance : -spawnDistance;
        Vector2 spawnPosition = new Vector2(transform.position.x+ Offset, fPoint.position.y);
        GameObject bullet = Instantiate(pPrefab, spawnPosition, Quaternion.identity);
        Fire projscript = bullet.GetComponent<Fire>();
        
        projscript.Launch(isRight);
    }
    void FixedUpdate() 
    {
        rb.linearVelocity = new Vector2(movement * moveForce, rb.linearVelocity.y);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            Debug.Log("Enter");
            anim.SetBool("isJump", false);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
    
}
