using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillImage;
    public void SetMaxHealth(float maxHealth)
    {
        fillImage.fillAmount = 1f;
    }
    
    public void UpdateBar(float currentHealth, float maxHealth)
    {
        fillImage.fillAmount = currentHealth / maxHealth;
    }
}
