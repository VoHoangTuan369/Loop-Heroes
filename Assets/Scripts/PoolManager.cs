using UnityEngine;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField] Enemy enemyPrefab;
    [SerializeField] Projectile arrowPrefab, bulletPrefab;
    [SerializeField] HealthBarUI healthBarPrefab;

    // Pool cho Enemy theo type
    private Dictionary<EnemyType, List<Enemy>> enemyPools = new Dictionary<EnemyType, List<Enemy>>();
    // Pool cho Projectile
    private Dictionary<Projectile, List<Projectile>> projectilePools = new Dictionary<Projectile, List<Projectile>>();
    // Pool cho HealthBar
    private List<HealthBarUI> healthBarPool = new List<HealthBarUI>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ===== ENEMY =====
    public Enemy GetEnemy(EnemyType type, Vector3 position, Quaternion rotation, Transform canvasTransform)
    {
        if (!enemyPools.ContainsKey(type))
            enemyPools[type] = new List<Enemy>();

        Enemy enemy = null;
        foreach (var e in enemyPools[type])
        {
            if (e == null) continue; // bỏ qua object đã bị Destroy
            if (!e.gameObject.activeInHierarchy)
            {
                enemy = e;
                break;
            }
        }

        if (enemy == null)
        {
            enemy = Instantiate(enemyPrefab);
            enemyPools[type].Add(enemy);
        }

        enemy.transform.position = position;
        enemy.transform.rotation = rotation;
        enemy.gameObject.SetActive(true);

        // lấy healthbar từ pool
        HealthBarUI hpBar = GetHealthBar(canvasTransform);
        hpBar.isEnemyHP = true;
        hpBar.Init(enemy);
        hpBar.gameObject.SetActive(false); // ẩn cho tới khi enemy bị đánh

        enemy.AttachHealthBar(hpBar); // viết hàm trong Enemy để gắn healthbar

        return enemy;
    }

    public void ReturnEnemy(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
    }

    // ===== PROJECTILE =====
    public Projectile GetProjectile(Projectile prefab, Vector3 position, Quaternion rotation)
    {
        if (!projectilePools.ContainsKey(prefab))
            projectilePools[prefab] = new List<Projectile>();

        Projectile proj = null;
        foreach (var p in projectilePools[prefab])
        {
            if (p == null) continue;
            if (!p.gameObject.activeInHierarchy)
            {
                proj = p;
                break;
            }
        }

        if (proj == null)
        {
            proj = Instantiate(prefab);
            projectilePools[prefab].Add(proj);
        }

        proj.transform.SetPositionAndRotation(position, rotation);
        proj.gameObject.SetActive(true);
        return proj;
    }

    public void ReturnProjectile(Projectile proj)
    {
        proj.gameObject.SetActive(false);
    }

    // ===== HEALTHBAR =====
    public HealthBarUI GetHealthBar(Transform canvasTransform)
    {
        HealthBarUI bar = null;
        foreach (var b in healthBarPool)
        {
            if (b == null) continue;
            if (!b.gameObject.activeInHierarchy)
            {
                bar = b;
                break;
            }
        }

        if (bar == null)
        {
            bar = Instantiate(healthBarPrefab, canvasTransform);
            healthBarPool.Add(bar);
        }

        bar.gameObject.SetActive(true);
        return bar;
    }

    public void ReturnHealthBar(HealthBarUI bar)
    {
        bar.gameObject.SetActive(false);
    }
}
