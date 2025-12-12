using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Transform originalParent; // Lưu cha gốc
    public WorldItem itemPrefab;

    ItemData itemData;
    StoreItem storeItem;
    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void SetData(ItemData data)
    {
        itemData = data;
        storeItem = GetComponentInParent<StoreItem>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;

        originalParent = transform.parent; // Lưu lại cha gốc
        transform.SetParent(canvas.transform); // Tách ra khỏi cha để tránh layout ảnh hưởng
        transform.SetAsLastSibling(); // Đảm bảo nằm trên cùng
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Tile") && GameManager.Instance.CurrentCoin >= itemData.price)
            {
                WorldItem obj = Instantiate(itemPrefab, hit.collider.transform.position, Quaternion.identity, hit.collider.transform);
                obj.Init(itemData);
                GameManager.Instance.GridSpawner.worldItems.Add(obj);
                hit.collider.enabled = false;
                SpendCoin();
                return;
            }
            if (hit.collider.CompareTag("WorldItem") && GameManager.Instance.CurrentCoin >= itemData.price) 
            {
                WorldItem existingItem = hit.collider.GetComponentInParent<WorldItem>();
                if (existingItem != null)
                {
                    if (existingItem.ItemData.type == itemData.type && existingItem.Level < existingItem.ItemData.maxLevel)
                    {
                        existingItem.Upgrade();
                        SpendCoin();
                        return;
                    }
                }
            }
        }

        // Reset về cha gốc và vị trí ban đầu
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = Vector2.zero;
    }
    private void SpendCoin()
    {
        GameManager.Instance.CurrentCoin -= itemData.price;
        GameManager.Instance.UpdateCoinUI();
        gameObject.SetActive(false);
        storeItem.HideItem();
    }
}

