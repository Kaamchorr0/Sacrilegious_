using UnityEngine;

public class Fire : MonoBehaviour
{
    public float speed = 10f;
    public float LifeTime = 2f;
    public Rigidbody2D rb;
    public float damage = 10f; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Boss")) 
        {
            BossController boss = other.gameObject.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
            }
            
            AttackTracker tracker = GameObject.FindGameObjectWithTag("Player").GetComponent<AttackTracker>();
            if (tracker != null)
            {
                tracker.LogLongAtk();
            }
            Destroy(gameObject);
        }
        
        if (!other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
    public void Launch(bool isRight)
    {
        if (isRight)
        {
            rb.linearVelocity = new Vector2(speed, 0);
        }
        else
        {
            rb.linearVelocity = new Vector2(-speed, 0);
        }
        Destroy(gameObject, LifeTime);
    }
}
