using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform Bossdest;
    public Tactics bossBrain;
    public Transform Cam;
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player"))
        {
            other.transform.position = Bossdest.position;
            Cam.position = new Vector3(9.76f, 1.11f, -10f);
            Debug.Log("Teleport");
            bossBrain.StartFight();
        }
    }
}
