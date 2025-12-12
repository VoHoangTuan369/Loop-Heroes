using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IHealth
{
    [SerializeField] HealthBarUI healthBarPrefab;
    [SerializeField] float rushSpeedMultiplier = 3f; // tốc độ gấp 3 lần khi rush
    [SerializeField] float rushDistance = 10f;       // chạy nhanh trong 10 đơn vị rồi bình thường
    
    HealthBarUI healthBarInstance;
    Transform target;
    CentralBase baseObj;
    bool isAttacking = false;
    EnemyStat data;
    float maxHealth;
    float currentHealth;
    Animator animator;
    GameObject model;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    public EnemyStat Data { get => data; set => data = value; }
    public void Init(EnemyStat statData, Transform centralBase, Transform canvasTransform, int edge)
    {
        target = centralBase;
        baseObj = centralBase.GetComponent<CentralBase>();
        SetStats(statData);

        if (model == null) 
        {
            model = Instantiate(statData.model, transform);
            model.transform.localPosition = Vector3.zero;
        }

        if (healthBarPrefab != null && canvasTransform != null)
        {
            healthBarInstance = Instantiate(healthBarPrefab, canvasTransform);
            healthBarInstance.isEnemyHP = true;
            healthBarInstance.Init(this);
            healthBarInstance.gameObject.SetActive(false);
        }

        // 👉 chỉnh rushDistance theo cạnh spawn
        if (edge == 2 || edge == 3) // trên hoặc dưới
            rushDistance *= 2f;     // ví dụ gấp đôi
        else
            rushDistance *= 1f;     // giữ nguyên

        StartCoroutine(MoveToTarget());
        animator = GetComponentInChildren<Animator>();
        SoundMN.Instance.PlayEnemySound();
    }
    public void SetStats(EnemyStat statData)
    {
        data = statData;
        maxHealth = data.maxHealth;
        currentHealth = maxHealth;
    }

    public void AttachHealthBar(HealthBarUI bar)
    {
        healthBarInstance = bar;
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
            SoundMN.Instance.PlayBaseHit();
            yield return new WaitForSeconds(data.attackSpeed);
        }
        healthBarInstance.gameObject.SetActive(false);
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return;
        healthBarInstance.gameObject.SetActive(true);
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);
        SoundMN.Instance.PlayEnemyHit();

        if (healthBarInstance != null)
            healthBarInstance.UpdateHealth();

        if (CameraShake.Instance != null)
            StartCoroutine(CameraShake.Instance.Shake(0.15f, 0.1f));
        if (currentHealth == 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.Instance.CurrentCoin += data.price;
        GameManager.Instance.UpdateCoinUI();
        GameManager.Instance.RemoveEnemy(this);

        // 👉 trả healthbar về pool
        if (healthBarInstance != null)
            PoolManager.Instance.ReturnHealthBar(healthBarInstance);

        // 👉 trả enemy về pool
        PoolManager.Instance.ReturnEnemy(this);
    }

    public Vector3 GetHeadPosition()
    {
        Collider col = GetComponentInChildren<Collider>();
        if (col != null)
            return col.bounds.center + new Vector3(0, col.bounds.extents.y, 0);

        return transform.position + Vector3.up * 2f;
    }
    public void ResetEnemy()
    {
        // reset máu
        currentHealth = maxHealth;
        isAttacking = false;

        // reset rushDistance về giá trị gốc
        rushDistance = 3f; // hoặc lưu giá trị gốc trong một biến khác rồi gán lại

        // reset animator
        if (animator != null)
        {
            animator.SetBool("IsAttacking", false);
        }

        // reset hướng quay
        transform.rotation = Quaternion.identity;

        // dừng tất cả coroutine cũ
        StopAllCoroutines();
    }
}
