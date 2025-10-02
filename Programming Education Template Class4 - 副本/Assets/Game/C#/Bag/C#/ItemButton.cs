using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class ItemButton : MonoBehaviour
{
    private ItemBase item;
    public Image image;
    private InventoryUIPlayer uiManager;
    public Text text;

    public void Init(ItemBase item, InventoryUIPlayer uiManager,int count)
    {
        this.item = item;
        image.sprite = item.sprite;
        this.uiManager= uiManager;
        text.text = count.ToString();
    }

    public void Init(ItemBase item)
    {
        this.item = item;
        image.sprite = item.sprite;
    }
}