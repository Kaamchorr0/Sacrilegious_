using System.Collections;
using UnityEditor.Tilemaps;
using UnityEngine;

public class BossController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
    private float health = 100f;
    private bool isGuard = false;
    private bool isInvincible = false;
    public float dashspeed = 30f;
    public float dashduration = 1.0f;
    public float guardDuration = 3.0f;
    private Transform playerTransform;
    private Coroutine activeDashCoroutine;
    public bool isFight;
    public Transform meleePoint; 
    public float meleeAttackRange = 1.5f;
    public float meleeDamage = 30f;
    public float dashdamage = 40f;
    public LayerMask playerLayer;
    public float maxHealth = 100f;
    private bool hasHealedOnce = false;
    public bool isBusy = false;
    private Vector2 startPosition;
    public HealthBar bossHealthBar;
    public GameObject youWinPanel;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;
        health = maxHealth;
        startPosition = transform.position;
        if (bossHealthBar != null) 
            bossHealthBar.UpdateBar(health, maxHealth);
    }
    void Update()
    {
        if (!isFight)
        {
            return;
        }
        if (isInvincible || isGuard)
        {
            return;
        }
        Flip();
    }
    public void ResetBoss()
    {
        
        StopAllCoroutines();
        activeDashCoroutine = null;

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 3f; 
        transform.position = startPosition;

        health = maxHealth;
        hasHealedOnce = false;

        isGuard = false;
        isInvincible = false;
        isBusy = false;
        isFight = false; 
        anim.SetBool("IsGuard", false);
        anim.Play("Idle");
        if (bossHealthBar != null)
        {
            bossHealthBar.transform.parent.gameObject.SetActive(false);
        }
    }
    void Flip()
    {
        if (playerTransform.position.x > transform.position.x)
        {
            sr.flipX = false;
        }
        else if(playerTransform.position.x< transform.position.x)
        {
            sr.flipX = true;
        }
    }

    public void DoMeleeAtk()
    {
        if (activeDashCoroutine != null)
        {
            StopCoroutine(activeDashCoroutine);
        }
        activeDashCoroutine = StartCoroutine(MeleeAttackRoutine());
    }
    private IEnumerator MeleeAttackRoutine()
    {
        isBusy = true;
        float distance = Mathf.Abs(playerTransform.position.x - transform.position.x);

        if (distance > meleeAttackRange + 1f)
        {
            anim.SetTrigger("Dash"); 

            isInvincible = false; 
            
            float originalGravity = rb.gravityScale;
            rb.gravityScale = 0f;

            while (Mathf.Abs(playerTransform.position.x - transform.position.x) > meleeAttackRange)
            {    
                float dashDir = (playerTransform.position.x > transform.position.x) ? 1f : -1f;
                if (dashDir > 0f) sr.flipX = false;
                else if (dashDir < 0f) sr.flipX = true;
                rb.linearVelocity = new Vector2(dashDir * dashspeed, 0f); 

                yield return null; 
                
                if (playerTransform == null) break; 
            }
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = originalGravity;
        }
        anim.SetTrigger("Attack"); 
        Collider2D hitPlayer = Physics2D.OverlapCircle(meleePoint.position, meleeAttackRange, playerLayer);
        if (hitPlayer != null)
        {
            Phealth player = hitPlayer.GetComponent<Phealth>();
            if(player != null) player.TakeDamage(meleeDamage);
        }
        activeDashCoroutine = null;
        isBusy = false;
    }
    public void DoDash()
    {
        if (activeDashCoroutine != null)
        {
            StopCoroutine(activeDashCoroutine);
        }
        activeDashCoroutine = StartCoroutine(DashRoutine());
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isInvincible && collision.gameObject.CompareTag("Wall"))
        {
            if (activeDashCoroutine != null)
            {
                StopCoroutine(activeDashCoroutine);
            }
            isInvincible = false;
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 3f;
            activeDashCoroutine = null;
            isBusy = false;
        }
    }
    public void DoGuard()
    {
        StartCoroutine(GuardRoutine());
    }
    public void DoHeal()
    {
        if (hasHealedOnce)
        {
            return;
        }
        else
        {
            if (health >= (maxHealth * 0.5f))
            {
                
                return;
            }
            else
            {
                isBusy = true;
                health += 20;
                anim.SetTrigger("Heal");
                hasHealedOnce = true;
                StartCoroutine(ActionCooldown(0.5f));
                if (bossHealthBar != null)
                {
                    bossHealthBar.UpdateBar(health, maxHealth);
                }
            }
        }
    }
    IEnumerator ActionCooldown(float duration)
    {
        yield return new WaitForSeconds(duration);
        isBusy = false; 
    }

    private IEnumerator DashRoutine()
    {
        isBusy = true;
        anim.SetTrigger("Dash");
        isInvincible = true;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        float dashDir = (player.transform.position.x > transform.position.x) ? 1f : -1f;
        if (dashDir > 0f) sr.flipX = false;
        else if (dashDir < 0f) sr.flipX = true;

        float grav = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(dashDir * dashspeed, 0f);

        yield return new WaitForSeconds(dashduration);

        isInvincible = false;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = grav;
        activeDashCoroutine = null;
        isBusy = false;

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isInvincible && other.CompareTag("Player"))
        {
            Phealth player = other.GetComponent<Phealth>();
            if (player != null)
            {
                player.TakeDamage(dashdamage);
            }
        }
    }
    private IEnumerator GuardRoutine()
    {
        isBusy = true;
        anim.SetBool("IsGuard", true);
        isGuard = true;
        yield return new WaitForSeconds(guardDuration);
        isGuard = false;
        anim.SetBool("IsGuard", false);
        isBusy = false;
    }
    public void TakeDamage(float damageAmount)
    {
        if (isGuard)
        {
            return; 
        }

        if (isInvincible)
        {
            return; 
        }

        health -= damageAmount;
        if (bossHealthBar != null)
        {
            bossHealthBar.UpdateBar(health, maxHealth);
        }

        if (health <= 0f)
        {
            health = 0;
            if (bossHealthBar != null)
            {
                bossHealthBar.UpdateBar(health, maxHealth);
            }
            Die();
        }
    }

    private void Die()
    {

        GetComponent<Tactics>().StopAllCoroutines();
        StopAllCoroutines();
        this.enabled = false;
        StartCoroutine(WinRoutine());
    }
    private IEnumerator WinRoutine()
    {
        yield return new WaitForSeconds(2.0f);


        if (youWinPanel != null)
        {
            youWinPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }
    public void QuitGame()
{
    Debug.Log("QUIT");
    Time.timeScale = 1f; 
    Application.Quit();
}

}
