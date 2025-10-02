using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBagManager : InventoryManager
{
    public List<ItemBase> items;
    private void Start()
    {
         inventory.allItemPlates.Clear();
         foreach (var item in items)
         {
              AddItem(item);
         }
    }
}
