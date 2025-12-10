using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataSO", menuName = "Game Data/ItemDataSO")]
public class ItemDataSO : ScriptableObject
{
    public List<ItemData> listItem;
}
[Serializable]
public class ItemData 
{
    public ItemType type;
    public string itemName;
    public Sprite icon;
    public int price;
    public int cooldown;
    public int maxLevel;
}
public enum ItemType 
{
    Arrow, Bullet, Dash, SpeedBoost, Health
}
