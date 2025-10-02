using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIPlayer : InventoryUIBase
{
    public Transform itemParent;
    public GameObject itemPrefab;


    private void Start()
    {
    }

    public override void RefreshUI(InventoryBase inventory)
    {
        for (int i = 0; i < itemParent.childCount; i++)
        {
             Destroy(itemParent.GetChild(i).gameObject);
        }
        foreach (var item in inventory.allItemPlates)
        {
            var itemObj = Instantiate(itemPrefab, itemParent);
            itemObj.GetComponent<ItemButton>().Init(item.item, this, item.count);
        }
    }
}