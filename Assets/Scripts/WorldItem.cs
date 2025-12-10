using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class WorldItem : MonoBehaviour
{
    [SerializeField] Renderer quadRenderer;
    [SerializeField] TextMeshPro levelText;
    [SerializeField] Projectile arrowPrefab, bulletPrefab;
    [SerializeField] Image cooldown;

    private ItemData itemData;
    int level = 1;
    bool isOnCooldown = false;

    public ItemData ItemData { get => itemData; set => itemData = value; }
    public int Level { get => level; set => level = value; }

    public void Init(ItemData data)
    {
        itemData = data;
        level = Mathf.Max(level, 1);
        quadRenderer.material.mainTexture = itemData.icon.texture;
        levelText.text = $"Lv{level}";
        cooldown.fillAmount = 0f;
    }

    void AttackType(bool isArrow)
    {
        StartCoroutine(ShootProjectiles(isArrow));
    }

    private IEnumerator ShootProjectiles(bool isArrow)
    {
        Enemy nearestEnemy = GetNearestEnemy();
        if (nearestEnemy == null) yield break;

        Vector3 dir = (nearestEnemy.transform.position - transform.position).normalized;
        Projectile prefab = isArrow ? arrowPrefab : bulletPrefab;
        if (prefab == null)
        {
            Debug.LogWarning("Prefab chưa được gán!");
            yield break;
        }

        for (int i = 0; i < level; i++)
        {
            Quaternion rotation = Quaternion.LookRotation(dir);
            Vector3 spawnPos = transform.position + Vector3.up * 0.5f;

            // 👉 lấy projectile từ pool thay vì Instantiate
            Projectile projectile = PoolManager.Instance.GetProjectile(prefab, spawnPos, rotation);
            if (projectile != null)
            {
                projectile.Init(dir);
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    Enemy GetNearestEnemy()
    {
        Enemy nearest = null;
        float minDist = Mathf.Infinity;

        foreach (Enemy e in GameManager.Instance.enemies)
        {
            if (e == null) continue;
            float dist = Vector3.Distance(transform.position, e.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = e;
            }
        }
        return nearest;
    }

    void Healing()
    {
        if (!gameObject.activeSelf) return;
        float healAmount = level * 10f;
        GameManager.Instance.CentralBase.IncreasedHealth(healAmount);
        gameObject.SetActive(false);
    }
    void SpeedBoost()
    {
        if (!gameObject.activeSelf) return;

        // gọi hàm tăng tốc độ trong HeroMovement
        GameManager.Instance.StartCoroutine(
            GameManager.Instance.HeroMovement.ApplySpeedBoost(level)
        );
    }
    void Dash()
    {
        if (!gameObject.activeSelf) return;

        GameManager.Instance.StartCoroutine(
            GameManager.Instance.HeroMovement.ApplyDash()
        );
    }

    public void ActivateItem()
    {
        if (isOnCooldown || !GameManager.Instance.IsFighting) return;

        switch (itemData.type)
        {
            case ItemType.Arrow:
                AttackType(true);
                break;
            case ItemType.Bullet:
                AttackType(false);
                break;
            case ItemType.Health:
                Healing();
                break;
            case ItemType.SpeedBoost:
                SpeedBoost();
                break;
            case ItemType.Dash:
                Dash();
                break;
        }
        SoundMN.Instance.PlayShoot();
        if (!gameObject.activeSelf) return;
        StartCoroutine(StartCooldown());
    }
    public void Upgrade()
    {
        if (level == itemData.maxLevel) return;
        level++;
        levelText.text = $"Lv{level}";
    }
    private IEnumerator StartCooldown()
    {
        isOnCooldown = true;
        cooldown.fillAmount = 1f; // bắt đầu đầy

        float elapsed = 0f;
        while (elapsed < itemData.cooldown)
        {
            elapsed += Time.deltaTime;
            cooldown.fillAmount = 1f - (elapsed / itemData.cooldown); // giảm dần
            yield return null;
        }

        cooldown.fillAmount = 0f; // hết hồi chiêu
        isOnCooldown = false;
    }
}
