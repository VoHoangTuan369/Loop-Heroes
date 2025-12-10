using UnityEngine;

public class CentralBase : MonoBehaviour, IHealth
{
    [SerializeField] float maxHealth = 100f;
    private float currentHealth;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    public HealthBarUI HealthBar { get => healthBar; set => healthBar = value; }

    [SerializeField] HealthBarUI healthBar;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.gameObject.SetActive(true);
        healthBar.Init(this);
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return;

        currentHealth = Mathf.Max(currentHealth - damage, 0f);
        healthBar.UpdateHealth();

        if (currentHealth == 0f)
        {
            Die();
        }
    }

    public Vector3 GetHeadPosition()
    {
        return transform.position;
    }

    public void UpdateHpPos()
    {
        healthBar.UpdatePosition();
    }
    public void IncreasedHealth(float amount)
    {
        maxHealth += amount;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth); // không vượt quá max
        healthBar.UpdateHealth();
    }
    private void Die()
    {
        Debug.Log("Central Base destroyed!");
        // Thêm hiệu ứng nổ, kết thúc game, v.v.
    }
}
