using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlaceManager : MonoBehaviour
{
    private InventoryManager _inventoryManager;
    private PlayerController _playerController;
    public Action<ObjectId.Id> OnPlaceItem;
    private void Awake()
    {
        _inventoryManager = GetComponent<InventoryManager>();
        _playerController = GetComponent<PlayerController>();
    }
    
    public void PlaceItem(GameObject prefab)
    {
        GameObject item= Instantiate(prefab);
        ItemOnWorld itemOnWorld = item.GetComponent<ItemOnWorld>();
        itemOnWorld.OnSpawnAnimation();
        Vector2 dir = _playerController.GetDir();
        ObjectId id = item.GetComponent<ObjectId>();
        Vector2 range = id.GetRange();
        Vector3 pos=Vector3.zero;
        Vector3 posPlayer= _playerController.transform.position;
        pos.y= posPlayer.y;
        if (dir.y > 0)
        {
            pos.y = posPlayer.y + 1;
        }
        else if(dir.y<0)
        {
            pos.y = posPlayer.y - range.y;
        }
        pos.x=posPlayer.x+dir.x*(range.x/2+0.5f);
        item.transform.position = pos;
        OnPlaceItem?.Invoke(id.GetId());
    }
    
}