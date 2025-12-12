using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelPanel : MonoBehaviour
{
    [SerializeField] Button levelBtnPrefab;
    [SerializeField] int levelCount = 20;
    [SerializeField] Transform content;

    List<RectTransform> levelButtons = new List<RectTransform>();

    void Start()
    {
        //PlayerPrefs.DeleteAll();
        SaveLevel();
        GenerateLevelButtons();
    }
    void SaveLevel()
    {
        if (PlayerPrefs.GetInt("CurrentLevel") > 0) return;
        PlayerPrefs.SetInt("CurrentLevel", 1);
        PlayerPrefs.Save();
    }

    void GenerateLevelButtons()
    {
        int unlockedLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        Debug.Log("unlockedLevel " + unlockedLevel);

        for (int i = 0; i < levelCount; i++)
        {
            int levelNumber = i + 1;

            // tạo button trong content
            Button btn = Instantiate(levelBtnPrefab, content);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = $"{levelNumber}";

            RectTransform rt = btn.GetComponent<RectTransform>();
            levelButtons.Add(rt);

            // xử lý mở khóa
            Image lockIcon = btn.transform.Find("lockIcon")?.GetComponent<Image>();
            bool isUnlocked = levelNumber <= unlockedLevel;

            if (lockIcon != null)
                lockIcon.gameObject.SetActive(!isUnlocked);

            btn.interactable = isUnlocked;

            if (isUnlocked)
            {
                int selectedLevel = levelNumber; // tránh closure bug
                btn.onClick.AddListener(() =>
                {
                    LevelLoader.SelectedLevel = selectedLevel;
                    SceneManager.LoadScene("GameScene");
                });
            }
        }
    }
}
public static class LevelLoader
{
    public static int SelectedLevel = 1;
}
