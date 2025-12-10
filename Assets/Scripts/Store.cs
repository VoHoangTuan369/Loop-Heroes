using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Store : MonoBehaviour
{
    [SerializeField] StoreItem itemPrefab;
    [SerializeField] Transform content;
    [SerializeField] Button rerollBtn, fightBtn;
    [SerializeField] ItemDataSO itemDatabase;
    [SerializeField] TextMeshProUGUI rerollText;

    private Vector3 originalCamPos;   // lưu vị trí gốc của camera
    private Camera mainCam;
    bool hasRerolled = false;
    int priceReroll = 2;

    private void Awake()
    {
        mainCam = Camera.main;
        if (mainCam != null)
            originalCamPos = mainCam.transform.position;
    }

    private void OnEnable()
    {
        if (mainCam != null)
        {
            Vector3 newPos = originalCamPos;
            newPos.y = 2f;
            mainCam.transform.position = newPos;
            GameManager.Instance.CentralBase.UpdateHpPos();
        }
        hasRerolled = false;
        RerollItems();
    }

    private void OnDisable()
    {
        if (mainCam != null)
        {
            mainCam.transform.position = originalCamPos;
            if(GameManager.Instance.CentralBase != null)
                GameManager.Instance.CentralBase.UpdateHpPos();
        }
    }

    private void Start()
    {
        rerollBtn.onClick.AddListener(RerollItems);
        fightBtn.onClick.AddListener(OnFightClicked);
    }

    private void OnFightClicked()
    {
        gameObject.SetActive(false);
        GameManager.Instance.StartNextWave();
    }

    void ClearStore()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }

    void RerollItems()
    {
        if (hasRerolled && GameManager.Instance.CurrentCoin < priceReroll) return;

        ClearStore();

        // kiểm tra wave hiện tại
        if (GameManager.Instance.CurrentWaveIndex == 0) // wave đầu tiên
        {
            // chỉ spawn 1 item Arrow
            ItemData arrowItem = itemDatabase.listItem.Find(i => i.type == ItemType.Arrow);
            if (arrowItem != null)
            {
                StoreItem newItem = Instantiate(itemPrefab, content);
                newItem.InitData(arrowItem);
            }
            rerollBtn.gameObject.SetActive(false);
        }
        else
        {
            // random 3 item khác nhau
            List<ItemData> allItems = new List<ItemData>(itemDatabase.listItem);
            HashSet<int> usedIndices = new HashSet<int>();

            for (int i = 0; i < 3; i++)
            {
                int randIndex;
                do
                {
                    randIndex = Random.Range(0, allItems.Count);
                } while (usedIndices.Contains(randIndex));

                usedIndices.Add(randIndex);

                ItemData randomItem = allItems[randIndex];
                StoreItem newItem = Instantiate(itemPrefab, content);
                newItem.InitData(randomItem);
            }
            rerollBtn.gameObject.SetActive(true);
        }

        // trừ coin khi reroll (trừ lần free đầu tiên)
        if (rerollText.text != "Free")
        {
            GameManager.Instance.CurrentCoin -= priceReroll;
            GameManager.Instance.UpdateCoinUI();
        }

        UpdateRerollButton();
        hasRerolled = true;
    }

    private void UpdateRerollButton()
    {
        if (hasRerolled)
            rerollText.text = priceReroll.ToString();
        else
            rerollText.text = "Free";
    }
}
