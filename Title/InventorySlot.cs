using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public Text itemCount_Text;
    public GameObject selected_Item;
    public Sprite lock_icon;
    public Material Shiny;
    private bool is_Dictionary = false;
    private bool is_Result = false;

    public void AddItem(Item _item,bool dictionary = false, bool result = false)
    {
        is_Dictionary = dictionary;
        is_Result = result;
        if (is_Dictionary)
        {
            if (_item.itemType == Item.ItemType.Equip)
            {
                if (UpLoadData.item_is_gained[_item.itemID - 10001])
                {
                    if(!UpLoadData.item_info_list[_item.itemID - 10001])
                    {
                        icon.sprite = lock_icon;
                        icon.material = Shiny;
                    }
                    else
                    {
                        icon.sprite = _item.itemIcon;
                        icon.material = null;
                    }
                }
                else
                {
                    icon.sprite = lock_icon;
                    icon.material = null;
                }
            }
            else if (_item.itemType == Item.ItemType.Use)
            {
                if (UpLoadData.item_is_gained[_item.itemID - 19901])
                {
                    if(!UpLoadData.item_info_list[_item.itemID - 19901])
                    {
                        icon.sprite = lock_icon;
                        icon.material = Shiny;
                    }
                    else
                    {
                        icon.sprite = _item.itemIcon;
                        icon.material = null;
                    }
                }
                else
                {
                    icon.sprite = lock_icon;
                    icon.material = null;
                }
            }
            else
            {
                if (UpLoadData.item_is_gained[_item.itemID - 29801])
                {
                    if (!UpLoadData.item_info_list[_item.itemID - 29801])
                    {
                        icon.sprite = lock_icon;
                        icon.material = Shiny;
                    }
                    else
                    {
                        icon.sprite = _item.itemIcon;
                        icon.material = null;
                    }
                }
                else
                {
                    icon.sprite = lock_icon;
                    icon.material = null;
                }
            }

            itemCount_Text.text = null;

        }
        if (is_Result)
        {
            icon.sprite = _item.itemIcon;

            if (Item.ItemType.Use == _item.itemType || Item.ItemType.ETC == _item.itemType)
            {
                if (_item.itemCount > 0)
                    itemCount_Text.text = "x" + _item.itemCount.ToString();
                else
                    itemCount_Text.text = "";
            }
            else
            {
                if (_item.itemCount > 0)
                {
                    itemCount_Text.text = "x" + _item.itemCount.ToString();
                }
                else
                    itemCount_Text.text = "";
            }
        }

        if(!is_Dictionary && !is_Result)
        {
            icon.sprite = _item.itemIcon;

            if (Item.ItemType.Use == _item.itemType || Item.ItemType.ETC == _item.itemType)
            {
                if (_item.itemCount > 0)
                    itemCount_Text.text = "x" + _item.itemCount.ToString();
                else
                    itemCount_Text.text = "";
            }
            else
            {
                if (_item.itemCount > 0)
                {
                    itemCount_Text.text = "x" + _item.itemCount.ToString();
                }
                else
                    itemCount_Text.text = "";
            }
        }


    }
    
    public void RemoveItem()    {

        itemCount_Text.text = "";
        icon.sprite = null;
    }

}
