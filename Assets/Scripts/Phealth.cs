using UnityEngine;

public class Phealth : MonoBehaviour
{
    public Transform respawnPoint; 
    public BossController bossController; 
    public Tactics bossBrain;
    public float health;
    public float maxHealth = 100f;
    public Transform Cam;
    public HealthBar playerHealthBar;

    void Start()
    {
        health = maxHealth;
        
        if (playerHealthBar != null)
            playerHealthBar.UpdateBar(health, maxHealth);
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;

        if (playerHealthBar != null)
            playerHealthBar.UpdateBar(health, maxHealth);
        
        if (health <= 0f)
        {
            health = 0;
            if (playerHealthBar != null)
            {
                playerHealthBar.UpdateBar(health, maxHealth);
            }
            Die();
        }
    }

    private void Die()
    {
        health = maxHealth;
        if (playerHealthBar != null)
        {
            playerHealthBar.UpdateBar(health, maxHealth);
        }
        
        transform.position = respawnPoint.position;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        Cam.position = new Vector3(0f, 0f, -10f);

        if (bossController != null)
        {
            bossController.ResetBoss();
        }

        if (bossBrain != null)
        {
            StartCoroutine(bossBrain.ResetAndRethink());
        }
    }
}
