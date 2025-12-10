using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI waveText, coinText;
    [SerializeField] Slider waveSlider;
    [SerializeField] ResultPanel resultPanel;
    [SerializeField] Button settingBtn;
    [SerializeField] SettingPanel settingPanel;
    [SerializeField] Store storeUI;

    private int totalWaves;

    private void Start()
    {
        resultPanel.gameObject.SetActive(false);
        settingBtn.onClick.AddListener(settingPanel.ShowPanel);
    }
    public void InitUI(int totalWaveCount, int startCoin)
    {
        totalWaves = totalWaveCount;
        UpdateCoinText(startCoin);
        waveText.text = $"Wave {1}/{totalWaves}";
        waveSlider.value = 0;
        storeUI.gameObject.SetActive(true);
    }

    // gọi khi bắt đầu wave mới
    public void StartWave(int currentWaveIndex, int enemyCount)
    {
        waveText.text = $"Wave {currentWaveIndex + 1}/{totalWaves}";
        waveSlider.maxValue = enemyCount;
        waveSlider.value = 0;
    }

    // gọi khi 1 enemy bị tiêu diệt
    public void UpdateEnemyKilled(int killedCount)
    {
        waveSlider.value = killedCount;
    }

    public void UpdateCoinText(int coin)
    {
        coinText.text = coin.ToString();
    }
    public void ShowResult(bool isWin) 
    {
        resultPanel.gameObject.SetActive(true);
        resultPanel.ShowResult(isWin);
    }
    public void PrepareNextWaveUI(int nextWaveIndex, int enemyCount)
    {
        waveText.text = $"Wave {nextWaveIndex + 1}/{totalWaves}";
        waveSlider.maxValue = enemyCount;
        waveSlider.value = 0;
    }
}
