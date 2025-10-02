using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ComposeManager : MonoBehaviour
{
    private GameObject _player;
    private PlayerBagManager _bagManager;
    public List<ComposeBase> composeBaseList;
    public GameObject composePrefab;
    public Transform composeParent;
    public GameObject composeUI;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _bagManager = _player.GetComponent<PlayerBagManager>();
        UpdateUI();
    }

    public int FindItemCount(string itemName)
    {
        return _bagManager.FindItemCount(itemName);
    }

    public void Compositing(string composeObjectTag)
    {
        ComposeBase composeBase = null;
        foreach (var compose in composeBaseList)
        {
            string composeTag = compose.item.itemName.ToString();
            if (composeTag == composeObjectTag)
            {
                composeBase=compose;
            }
        }
        if (composeBase != null)
        {
            bool isAllOk = true;
            foreach (var itemPlate in composeBase.itemPlates)
            {
               int count= _bagManager.FindItemCount(itemPlate.item);
               if (count < itemPlate.count)
               {
                   isAllOk = false;
                   break;
               }
            }
            if (isAllOk)
            {
                _bagManager.AddItem(composeBase.item);
                foreach (var itemPlate in composeBase.itemPlates)
                {
                    for (int i = 0; i < itemPlate.count; i++)
                    {
                        _bagManager.RemoveItem(itemPlate.item);   
                    }
                }
            }
        }
    }

    public void SetActiveComposePage(bool isActive)
    {
        if (isActive)
        {
            composeUI.SetActive(true);
            AddAction.instance.OpenPageBySize(composeUI);
        }
        else
        {
            AddAction.instance.ClosePageBySize(composeUI, () =>
            {
                composeUI.SetActive(false);
            });
        }
    }

    private void UpdateUI()
    {
        foreach (var composeBase in  composeBaseList)
        {
            GameObject button = Instantiate(composePrefab, composeParent);
            button.GetComponent<ComposeUIButton>().Init(composeBase);
        }
    }
}

[System.Serializable]
public class ComposeBase
{
    public ItemBase item;
    public List<ItemPlate> itemPlates;
}