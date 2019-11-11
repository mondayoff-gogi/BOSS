using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ButtonClick : MonoBehaviour
{
    private Button button;
    //private int boss_index = 0;
    //private int character_index = 0;
    private int item_index = 0;
    private int tab_index = 0;
    private int Equip_index = 0;
    TitleButton title_Button;
    Inventory inventory;
    Equipment Equip;

    public GameObject main_camera;
    public GameObject inven;

    // Start is called before the first frame update
    void Start()
    {
        button = this.gameObject.GetComponent<Button>();
        title_Button = main_camera.GetComponent<TitleButton>();
        inventory = inven.GetComponent<Inventory>();
        Equip = inven.GetComponent<Equipment>();
    }
    
    public void CharacterIndex1()
    {
        DatabaseManager.instance.EquipSort(title_Button.character_index[0]);
        inventory.GetCharacterIndex1();
        Equip.GetCharacterIndex(title_Button.character_index[0]);
    }

    public void CharacterIndex2()
    {
        DatabaseManager.instance.EquipSort(title_Button.character_index[1]);
        inventory.GetCharacterIndex2();
        Equip.GetCharacterIndex(title_Button.character_index[1]);
    }

    public void ItemIndex()
    {
        SoundManager.instance.Play(38);


        if (tab_index == 0) //장비창인경우
        {
            inventory.selectedButton = button.gameObject;
        }

        item_index = Convert.ToInt32(button.name);
        inventory.GetItemIndex(item_index);        
    }

    public void InvenToEquip()
    {
        SoundManager.instance.Play(38);

        if (tab_index == 0) //장비창인경우
        {
            inventory.selectedButton = button.transform.Find("ItemIcon").gameObject;
        }

        item_index = Convert.ToInt32(button.name);
        inventory.GetItemIndex(item_index);
    }

    public void TabIndex()
    {
        SoundManager.instance.Play(37);

        tab_index = Convert.ToInt32(button.name);
        inventory.GetTabIndex(tab_index);        
    }

    public void EquipIndex()
    {
        SoundManager.instance.Play(38);

        Equip_index = Convert.ToInt32(button.name);
        //inventory.GetEquipIndex(Equip_index);
    }

}
