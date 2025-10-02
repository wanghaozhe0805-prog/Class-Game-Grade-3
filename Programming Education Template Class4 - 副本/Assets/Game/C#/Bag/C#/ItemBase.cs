using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "newItem", menuName = "InventorySystem/Item")]
public class ItemBase : ScriptableObject
{
    public ObjectId.Id itemName;
    [TextArea(3, 10)]
    public string context;
    public Sprite sprite;
    public List<GameObject> prefabs;
}
