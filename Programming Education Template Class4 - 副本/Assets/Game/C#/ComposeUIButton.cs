using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ComposeUIButton : MonoBehaviour
{
    public Image iconImage;
    public Text nameText;
    public Text contextText;
    private ComposeBase _composeBase;
    
    public void Init( ComposeBase composeBase)
    {
         _composeBase = composeBase;
         iconImage.sprite = _composeBase.item.sprite;
         nameText.text = _composeBase.item.itemName.ToString();
         string context="";
         foreach (var itemPlate in _composeBase.itemPlates)
         {
             string str=itemPlate.item.itemName+"x"+itemPlate.count;
             context+=str;
             if (itemPlate != _composeBase.itemPlates.Last())
             {
                  context+="\r\n";
             }
         }
         contextText.text = context;

    }
}
