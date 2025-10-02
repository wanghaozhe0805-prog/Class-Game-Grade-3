using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<ItemBase> allItems;
    public InventoryBase inventory;
    public InventoryUIBase uiManager;

    public void AddItem(ItemBase item)
    {
        bool isHave = false;
        foreach (var itemPlate in inventory.allItemPlates)
        {
            if (itemPlate.item == item)
            {
                itemPlate.count++;
                isHave = true;
            }
        }

        if (!isHave)
            inventory.allItemPlates.Add(new ItemPlate(item));

        uiManager.RefreshUI(inventory);
    }

    public void AddItem(List<ItemBase> items)
    {
        foreach (var item in items)
        {
            bool isHave = false;
            foreach (var itemPlate in inventory.allItemPlates)
            {
                if (itemPlate.item == item)
                {
                    itemPlate.count++;
                    isHave = true;
                }
            }

            if (!isHave)
                inventory.allItemPlates.Add(new ItemPlate(item));
        }

        uiManager.RefreshUI(inventory);
    }

    public void RemoveItem(ItemBase item)
    {
        List<ItemPlate> removeList = new List<ItemPlate>();
        foreach (var itemPlate in inventory.allItemPlates)
        {
            if (itemPlate.item == item)
                itemPlate.count--;

            if (itemPlate.count <= 0)
            {
                removeList.Add(itemPlate);
            }
        }

        foreach (var itemPlate in removeList)
        {
            inventory.allItemPlates.Remove(itemPlate);
        }

        uiManager.RefreshUI(inventory);
    }
    
    public void RemoveItem(ItemPlate itemPlate)
    { 
        
    }

    public int FindItemCount(ItemBase item)
    {
        int count = 0;
        foreach (var itemPlate in inventory.allItemPlates)
        {
            if (itemPlate.item == item)
                count = itemPlate.count;
        }

        return count;
    }

    public int FindItemCount(string itemName)
    {
        int count = 0;
        foreach (var item in allItems)
        {
            if (item.itemName.ToString() == itemName)
                count = FindItemCount(item);
        }
        return count;
    }
    
    public ItemBase FindItem(string itemName)
    {
        foreach (var item in allItems)
        {
            if (item.itemName.ToString() == itemName)
                return item;
        }
        return null;
    }
}