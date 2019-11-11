using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public Image icon;
    //public Text itemName_Text;
    //public GameObject selected_Item;

    public void AddItem(Item _item)
    {
        //itemName_Text.text = _item.itemName;
        icon.sprite = _item.itemIcon;
        icon.color = new Color(1, 1, 1, 1);
    }

    public void RemoveItem()
    {
        //itemName_Text.text = "";
        icon.sprite = null;
    }
}
