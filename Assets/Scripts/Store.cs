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

        List<ItemData> allItems = itemDatabase.listItem;
        for (int i = 0; i < 3; i++)
        {
            ItemData randomItem = allItems[Random.Range(0, allItems.Count)];
            StoreItem newItem = Instantiate(itemPrefab, content);
            newItem.InitData(randomItem);
        }
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
