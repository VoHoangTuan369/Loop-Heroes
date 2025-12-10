using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GridSpawner GridSpawner { get => gridSpawner; set => gridSpawner = value; }
    public CentralBase CentralBase { get => centralBase; set => centralBase = value; }
    public int CurrentCoin { get => currentCoin; set => currentCoin = value; }
    public HeroMovement HeroMovement { get => heroMovement; set => heroMovement = value; }
    public int CurrentWaveIndex { get => currentWaveIndex; set => currentWaveIndex = value; }

    public bool IsFighting = false;

    [SerializeField] GridSpawner gridSpawner;
    [SerializeField] CentralBase centralBase;
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
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }
    public static int GetLoopedLevelIndex(int levelIndexValue, int totalLevelLength, int startLoopIndex = 10)
    {
        // Độ dài của vòng lặp con (ví dụ: 50 - 10 = 40)
        int loopLength = totalLevelLength - startLoopIndex;

        // Sử dụng Toán tử Tam phân (Ternary Operator) để chọn logic
        return (levelIndexValue < totalLevelLength)
            ? levelIndexValue // Vòng lặp ĐẦU TIÊN (0 -> 49)
            : startLoopIndex + ((levelIndexValue - totalLevelLength) % loopLength); // Các vòng lặp SAU (10 -> 49 -> 10 ...)
    }
    private void Start()
    {
        StartLevel();
    }

    void StartLevel()
    {
        level = LevelLoader.SelectedLevel;
        levelData = gameData.ListLevel.Find(l => l.level == level);
        if (levelData == null)
        {
            levelData = gameData.ListLevel[GetLoopedLevelIndex(level, gameData.ListLevel.Count, 5)];
        }

        currentCoin = levelData.coinStart;
        mainUI.InitUI(levelData.listWave.Count, currentCoin);
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
            mainUI.ShowResult(true);

            level++;
            PlayerPrefs.SetInt("CurrentLevel", level);
            PlayerPrefs.Save();
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
        if (planeRenderer == null) return;
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

        // 👉 lấy Enemy từ Pool thay vì Instantiate
        Enemy enemy = PoolManager.Instance.GetEnemy(type, spawnPos, Quaternion.identity, canvasOverlay.transform);
        enemy.ResetEnemy();
        // reset stat và init
        EnemyStat statData = new EnemyStat(stat);
        enemy.Init(statData, centralBase.transform, canvasOverlay.transform, edge);

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
        mainUI.ShowResult(isWin);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}
