using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectId : MonoBehaviour
{
    [SerializeField] private Id id;
    [SerializeField] private Vector2 range = Vector2.one;

    public Id GetId()
    {
        return id;
    }

    public bool CompareId(string id)
    {
        string idStr = this.id.ToString();
        return idStr == id;
    }
    public Vector2 GetRange()
    {
        return range;
    }

    public enum Id
    {
        None = -1,
        Player = 0,
        Wall = 1,
        Tree,
        Rock,
        Signboard,
        GoddessStatue,
        Wood,
        WoodenBarrel,
        Box,
        Shrub,
        StonePillar,
        Monuments,
        House,
        Water,
        RewardChest,
        
    }
}