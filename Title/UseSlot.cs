using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UseSlot : MonoBehaviour
{
    public Image icon;
    public Text Item_counter;
    //public Text itemName_Text;
    //public GameObject selected_Item;

    public void AddItem(Item _item)
    {
        //itemName_Text.text = _item.itemName;
        icon.sprite = _item.itemIcon;
        icon.color = new Color(1, 1, 1, 1);

        if (_item.itemCount > 0)
            Item_counter.text = "x" + _item.itemCount.ToString();
        else
            Item_counter.text = "";
    }

    public void RemoveItem()
    {
        //itemName_Text.text = "";
        icon.sprite = null;
        Item_counter.text = "";

    }
}
