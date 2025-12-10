using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IHealth
{
    [SerializeField] HealthBarUI healthBarPrefab;
    [SerializeField] float rushSpeedMultiplier = 3f; // tốc độ gấp 3 lần khi rush
    [SerializeField] float rushDistance = 10f;       // chạy nhanh trong 10 đơn vị rồi bình thường
    private HealthBarUI healthBarInstance;

    private Transform target;
    private CentralBase baseObj;
    private bool isAttacking = false;
    EnemyStat data;
    float maxHealth;
    float currentHealth;
    Animator animator;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    public void Init(EnemyStat statData, Transform centralBase, Transform canvasTransform)
    {
        target = centralBase;
        baseObj = centralBase.GetComponent<CentralBase>();
        SetStats(statData);

        if (healthBarPrefab != null && canvasTransform != null)
        {
            healthBarInstance = Instantiate(healthBarPrefab, canvasTransform);
            healthBarInstance.isEnemyHP = true;
            healthBarInstance.Init(this);
        }

        StartCoroutine(MoveToTarget());
        animator = GetComponentInChildren<Animator>();
    }
    public void SetStats(EnemyStat statData)
    {
        data = statData;
        maxHealth = data.maxHealth;
        currentHealth = maxHealth;
    }
    IEnumerator MoveToTarget()
    {
        float currentSpeed = data.speed * rushSpeedMultiplier;
        float rushedDistance = 0f;

        while (target != null && !isAttacking)
        {
            // hướng tới base
            Vector3 direction = (target.position - transform.position).normalized;

            // di chuyển
            Vector3 oldPos = transform.position;
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                currentSpeed * Time.deltaTime
            );
            rushedDistance += Vector3.Distance(transform.position, oldPos);

            // xoay theo hướng di chuyển
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            // cập nhật health bar
            if (healthBarInstance != null)
                healthBarInstance.UpdatePosition();

            // sau khi chạy nhanh đủ khoảng cách thì về tốc độ thường
            if (rushedDistance >= rushDistance)
            {
                currentSpeed = data.speed;
            }

            // kiểm tra khoảng cách để bắt đầu tấn công
            if (Vector3.Distance(transform.position, target.position) <= data.attackRange)
            {
                Debug.Log("Enemy bắt đầu tấn công CentralBase!");
                isAttacking = true;
                StartCoroutine(AttackBase(baseObj));
                yield break;
            }

            yield return null;
        }
    }

    IEnumerator AttackBase(CentralBase baseObj)
    {
        while (baseObj != null && baseObj.CurrentHealth > 0 && currentHealth > 0)
        {
            baseObj.TakeDamage(data.damage);
            if (animator != null)
                animator.SetBool("IsAttacking", true);
            yield return new WaitForSeconds(data.attackSpeed);
        }
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);

        if (healthBarInstance != null)
            healthBarInstance.UpdateHealth();

        if (currentHealth == 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (healthBarInstance != null)
            Destroy(healthBarInstance.gameObject);

        GameManager.Instance.CurrentCoin += data.price;
        GameManager.Instance.UpdateCoinUI();

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RemoveEnemy(this);
        }
    }

    public Vector3 GetHeadPosition()
    {
        Collider col = GetComponentInChildren<Collider>();
        if (col != null)
            return col.bounds.center + new Vector3(0, col.bounds.extents.y, 0);

        return transform.position + Vector3.up * 2f;
    }
}
