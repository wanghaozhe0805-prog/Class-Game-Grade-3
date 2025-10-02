using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu( fileName = "newInventory", menuName = "InventorySystem/Inventory")]
public class InventoryBase :ScriptableObject
{
    public List<ItemPlate> allItemPlates;
}

[System.Serializable]
public class ItemPlate
{
    public string itemName;
    public ItemBase item;
    public int count;

    public ItemPlate(ItemBase item)
    {
        this.item=item;
        count=1;
        itemName=item.name;
    }
    
}
