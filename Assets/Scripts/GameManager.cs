using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GridSpawner GridSpawner { get => gridSpawner; set => gridSpawner = value; }
    public CentralBase CentralBase { get => centralBase; set => centralBase = value; }
    public int CurrentCoin { get => currentCoin; set => currentCoin = value; }
    public HeroMovement HeroMovement { get => heroMovement; set => heroMovement = value; }
    public bool IsFighting = false;

    [SerializeField] GridSpawner gridSpawner;
    [SerializeField] CentralBase centralBase;
    [SerializeField] Enemy enemyPrefab;
    [SerializeField] HeroMovement heroMovement;
    [SerializeField] GameObject plane;
    [SerializeField] Canvas canvasOverlay;
    [SerializeField] int level;                // level hiện tại
    [SerializeField] GameData gameData;
    [SerializeField] MainUI mainUI;
    [SerializeField] Store storeUI;

    int currentCoin;
    int killedCount = 0;
    int currentWaveIndex = 0;
    LevelDataSO levelData;
    public List<Enemy> enemies = new List<Enemy>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(StartLevel());
    }
    IEnumerator StartLevel()
    {
        levelData = gameData.ListLevel.Find(l => l.level == level);
        if (levelData == null) yield break;

        currentCoin = levelData.coinStart;
        mainUI.InitUI(levelData.listWave.Count, currentCoin);

        // 👉 mở Store đầu tiên
        storeUI.gameObject.SetActive(true);
    }

    public IEnumerator SpawnWave(Wave wave, int waveIndex)
    {
        killedCount = 0;
        IsFighting = true;
        mainUI.StartWave(waveIndex, wave.listEnemy.Count);

        foreach (EnemyType type in wave.listEnemy)
        {
            SpawnEnemy(type);
            yield return new WaitForSeconds(wave.spawnInterval);
        }

        // chờ wave kết thúc
        while (enemies.Count > 0)
        {
            yield return null;
        }
        IsFighting = false;
        if (currentWaveIndex >= levelData.listWave.Count)
        {
            Debug.Log("Level hoàn thành!");
            mainUI.ShowResult();
        }
        else
        {
            // 👉 gọi hàm riêng để chuẩn bị UI cho wave kế tiếp
            mainUI.PrepareNextWaveUI(currentWaveIndex, levelData.listWave[currentWaveIndex].listEnemy.Count);
            storeUI.gameObject.SetActive(true);
        }
    }

    public void StartNextWave()
    {
        if (levelData == null) return;
        StartCoroutine(SpawnWave(levelData.listWave[currentWaveIndex], currentWaveIndex)); 
        currentWaveIndex++;
    }

    void SpawnEnemy(EnemyType type)
    {
        // lấy stat từ EnemyDataSO
        EnemyStat stat = gameData.EnemyDataSO.listEnemy.Find(e => e.type == type);
        if (stat == null)
        {
            Debug.LogWarning("Không tìm thấy EnemyStat cho type: " + type);
            return;
        }

        // chọn vị trí spawn ngẫu nhiên ở cạnh plane
        Renderer planeRenderer = plane.GetComponent<Renderer>();
        Bounds bounds = planeRenderer.bounds;
        Vector3 spawnPos = Vector3.zero;
        int edge = Random.Range(0, 4);
        switch (edge)
        {
            case 0: spawnPos = new Vector3(bounds.min.x, bounds.center.y, Random.Range(bounds.min.z, bounds.max.z)); break;
            case 1: spawnPos = new Vector3(bounds.max.x, bounds.center.y, Random.Range(bounds.min.z, bounds.max.z)); break;
            case 2: spawnPos = new Vector3(Random.Range(bounds.min.x, bounds.max.x), bounds.center.y, bounds.min.z); break;
            case 3: spawnPos = new Vector3(Random.Range(bounds.min.x, bounds.max.x), bounds.center.y, bounds.max.z); break;
        }

        // tạo enemy
        Enemy enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        EnemyStat statData = new EnemyStat(stat);
        // gắn model từ EnemyStat
        if (statData.model != null)
        {
            GameObject model = Instantiate(statData.model, enemy.transform);
            model.transform.localPosition = Vector3.zero;
        }
        enemy.Init(statData, centralBase.transform, canvasOverlay.transform);

        enemies.Add(enemy);
    }

    public void RemoveEnemy(Enemy enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);

            killedCount++;
            mainUI.UpdateEnemyKilled(killedCount);
        }
    }
    public void UpdateCoinUI() 
    {
        mainUI.UpdateCoinText(currentCoin);
    }
    public void ShowResult(bool isWin) 
    {
        mainUI.ShowResult();
    }
}
