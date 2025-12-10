using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelPanel : MonoBehaviour
{
    [SerializeField] Button levelBtnPrefab;
    [SerializeField] int levelCount = 20;
    [SerializeField] int columnCount = 5;
    [SerializeField] float spacingX = 150f;
    [SerializeField] float spacingY = 150f;
    [SerializeField] Image lineImagePrefab;

    List<RectTransform> levelButtons = new List<RectTransform>();

    void Start()
    {
        //PlayerPrefs.DeleteAll();
        SaveLevel();
        GenerateLevelButtons();
        DrawLineBetweenLevels();
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
        Debug.Log("unlockedLevel" + unlockedLevel);

        int totalRows = Mathf.CeilToInt((float)levelCount / columnCount);
        float totalHeight = (totalRows - 1) * spacingY;
        float offsetY = totalHeight / 2f;
        float totalWidth = (columnCount - 1) * spacingX;
        float offsetX = totalWidth / 2f;

        for (int i = 0; i < levelCount; i++)
        {
            int levelNumber = i + 1;

            Button btn = Instantiate(levelBtnPrefab, transform);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = $"Level {levelNumber}";

            RectTransform rt = btn.GetComponent<RectTransform>();
            levelButtons.Add(rt);

            int row = i / columnCount;
            int col = i % columnCount;
            if (row % 2 == 1) col = columnCount - 1 - col;

            float posX = col * spacingX - offsetX;
            float posY = -(row * spacingY - offsetY);
            rt.anchoredPosition = new Vector2(posX, posY);

            // xử lý mở khóa
            Image lockIcon = btn.transform.Find("lockIcon")?.GetComponent<Image>();
            bool isUnlocked = levelNumber <= unlockedLevel;

            if (lockIcon != null)
                lockIcon.gameObject.SetActive(!isUnlocked);

            btn.interactable = isUnlocked;

            if (isUnlocked)
            {
                int selectedLevel = levelNumber; // cần biến riêng để tránh closure bug
                btn.onClick.AddListener(() =>
                {
                    LevelLoader.SelectedLevel = selectedLevel;
                    SceneManager.LoadScene("GameScene");
                });
            }
        }
    }

    void DrawLineBetweenLevels()
    {
        for (int i = 0; i < levelButtons.Count - 1; i++)
        {
            RectTransform a = levelButtons[i];
            RectTransform b = levelButtons[i + 1];

            // tạo line
            Image line = Instantiate(lineImagePrefab, transform);
            RectTransform lineRt = line.GetComponent<RectTransform>();

            // hướng vector
            Vector2 dir = (b.anchoredPosition - a.anchoredPosition).normalized;

            // bán kính theo hướng (giả sử button hình vuông/tròn, lấy nửa width)
            float aRadius = a.sizeDelta.x * 0.5f;
            float bRadius = b.sizeDelta.x * 0.5f;

            // điểm bắt đầu và kết thúc từ rìa
            Vector2 startPos = a.anchoredPosition + dir * aRadius;
            Vector2 endPos = b.anchoredPosition - dir * bRadius;

            // trung điểm
            Vector2 midPoint = (startPos + endPos) / 2f;
            lineRt.anchoredPosition = midPoint;

            // độ dài
            float length = Vector2.Distance(startPos, endPos);

            // scale line theo chiều dài
            lineRt.sizeDelta = new Vector2(length, lineRt.sizeDelta.y);

            // xoay line theo hướng
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            lineRt.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
public static class LevelLoader
{
    public static int SelectedLevel = 1;
}
