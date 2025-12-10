using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreItem : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI nameText, priceText;
    [SerializeField] DragItem dragItem;
    ItemData itemData;
    public void InitData(ItemData data) 
    {
        itemData = data;
        icon.sprite = itemData.icon;
        nameText.text = itemData.itemName;
        priceText.text = itemData.price.ToString();
        dragItem.SetData(itemData);
    }
    public void HideItem() 
    {
        gameObject.SetActive(false);
    }
}
