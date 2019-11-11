using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    // 오디오 매니저 필요

    public TitleButton _title;
    public int max_item_count = 60;
    
    // 인벤토리 슬롯들
    public InventorySlot[] slots;
    public InventorySlot[] Eq_Slots;

    // 캐릭터 원본 이미지를 저장할 배열
    public Sprite[] character_Images;

    // 플레이어가 소지한 아이템 리스트
    public List<Item> inventoryItemList;
    // 선택한 탭에 따라 다르게 보여질 아이템 리스트
    private List<Item> inventoryTabList;
    // 플레이어가 장착한 아이템 리스트
    public List<Item> EquipmentItemList;

    public GameObject item_panel;

    // 캐릭터 초상화
    public Image character_Portrait;

    // 부연 설명
    public Text Description_Text;
    public Text EQ_Description_Text;

    // 탭 부연 설명
    public string[] tabDesecription;
    public Text price;

    // slot 부모 객체
    public Transform tf;
    public Transform tf_1;
    public GameObject selectedButton;

    public GameObject[] selectedTabImage;

    private GameObject[] selectedItemImage;

    public GameObject[] selectedEquipImage;

    public GameObject SellAsk;
    public GameObject SellEffect;
    private GameObject SellEffect_prefab;
    private GameObject[] itemArray;
    public GameObject Ask_pannel;


    public Text count_text;
    private int count = 1;

    public Text Sell_count_text;
    private int sell_count = 1;

    private Equipment theEquip;

    // 선택된 아이템
    private int selectedItem = 0;
    // 선택된 탭
    public int selectedTab = 0;  //0 장비   1 소비   2 ETC

    private const int WEAPON = 0, ARMOR = 1, LEFT_ACCESSORY = 2, RIGHT_ACCESSORY = 3;
    // 장비 아이콘
    //public Image[] equip_icon;
    // 장착된 장비 리스트
    public Item[] equipItemList;

    //private int character_num = 8;

    private int character_index = 0;

    public Image itemImage;
    public Image Equip_itemImage;

    public Text item_name;
    public Text Equip_item_name;

    public bool item_clicked = false;

    public GameObject obj;

    TitleButton title;

    public Button Scroll_Up;
    public Button Scroll_Down;

    public Text[] Cur_Stat;
    public Text[] Plus_Stat;

    private WaitForSeconds waitTime = new WaitForSeconds(0.01f);

    private int flag = 8;
    private int temp = 0;

    public Text current_item_count;

    // 아이템 테두리 색깔
    private Color white = new Color(1, 1, 1, 0.5f);
    private Color Blue = new Color(0.25f, 0.7f, 1f, 0.5f);
    private Color Purple = new Color(1f, 0.5f, 1f, 0.5f);
    private Color Orange = new Color(1f, 0.8f, 0.5f, 0.5f);

    public void RemoveSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].RemoveItem();
            slots[i].gameObject.SetActive(false);
        }
    }   // 인벤토리 슬롯 초기화

    public void SelectedItem(int index)
    {
        if (inventoryTabList.Count > 0)
        {
            itemImage.color = new Color(255, 255, 255, 255);
            Description_Text.text = inventoryTabList[temp*flag+index].itemDescription;
            itemImage.sprite = inventoryTabList[temp * flag + index].itemIcon;
            item_name.text = inventoryTabList[temp * flag + index].itemName;
        }
    }   // 선택된 아이템을 제외하고, 다른 모든 탭의 컬러 알파값을 0으로 조정


    public void ShowItem()
    {
        inventoryTabList.Clear();
        RemoveSlot();
        Cur_Stat[0].text = DatabaseManager.instance.Char_HP[character_index].ToString();
        Cur_Stat[1].text = DatabaseManager.instance.Char_MP[character_index].ToString();
        Cur_Stat[2].text = DatabaseManager.instance.Char_PhysicalAttackPower[character_index].ToString();
        Cur_Stat[3].text = DatabaseManager.instance.Char_MagicAttackPower[character_index].ToString();
        Cur_Stat[4].text = DatabaseManager.instance.Char_PhysicalArmor[character_index].ToString();
        Cur_Stat[5].text = DatabaseManager.instance.Char_MagicArmorPower[character_index].ToString();
        Cur_Stat[6].text = DatabaseManager.instance.Char_NormalAttackRange[character_index].ToString();
        Cur_Stat[7].text = DatabaseManager.instance.Char_NormalAttackSpeed[character_index].ToString();
        Cur_Stat[8].text = DatabaseManager.instance.Char_MoveSpeed[character_index].ToString();
        Cur_Stat[9].text = DatabaseManager.instance.Char_ciritical[character_index].ToString();
        Cur_Stat[10].text = DatabaseManager.instance.Char_HP_Regenerate[character_index].ToString();
        Cur_Stat[11].text = DatabaseManager.instance.Char_MP_Regenerate[character_index].ToString();

        switch (selectedTab)
        {
            case 0:
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.Equip == inventoryItemList[i].itemType)
                        inventoryTabList.Add(inventoryItemList[i]);
                }
                break;
            case 1:
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.Use == inventoryItemList[i].itemType)
                        inventoryTabList.Add(inventoryItemList[i]);
                }
                break;
            case 2:
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.ETC == inventoryItemList[i].itemType)
                        inventoryTabList.Add(inventoryItemList[i]);
                }
                break;
            default:
                break;
        }  // 탭에 따른 아이템 분류, 그것을 인벤토리 탭 리스트에 추가

        for (int i = 0; i < flag; i++)
        {
            if(inventoryTabList.Count > 0)
            {
                if ((temp * flag) +i<inventoryTabList.Count)
                {
                    slots[i].gameObject.SetActive(true);
                    slots[i].AddItem(inventoryTabList[(temp * flag) + i]);
                    switch(inventoryTabList[(temp * flag) + i]._Rare)
                    {
                        case 0:
                            slots[i].GetComponent<Image>().color = white;
                            break;
                        case 1:
                            slots[i].GetComponent<Image>().color = Blue;
                            break;
                        case 2:
                            slots[i].GetComponent<Image>().color = Purple;
                            break;
                        case 3:
                            slots[i].GetComponent<Image>().color = Orange;
                            break;
                    }
                }
                else
                {
                    break;
                }
            }


        }   // 인벤토리 탭 리스트의 내용을, 인벤토리 슬롯에 추가

    }       // 아이템 활성화


    public void GetCharacterIndex1()
    {
        theEquip.ItemEquip(_title.character_index[0]);
        theEquip.ShowEquip();
        theEquip.ShowUse();
        character_index = _title.character_index[0];
        character_Portrait.sprite = character_Images[character_index];
        switch (character_index)
        {
            case 0:
                EquipmentItemList = DatabaseManager.instance.equipList0;
                break;
            case 1:
                EquipmentItemList = DatabaseManager.instance.equipList1;
                break;
            case 2:
                EquipmentItemList = DatabaseManager.instance.equipList2;
                break;
            case 3:
                EquipmentItemList = DatabaseManager.instance.equipList3;
                break;
            case 4:
                EquipmentItemList = DatabaseManager.instance.equipList4;
                break;
            case 5:
                EquipmentItemList = DatabaseManager.instance.equipList5;
                break;
            case 6:
                EquipmentItemList = DatabaseManager.instance.equipList6;
                break;
            case 7:
                EquipmentItemList = DatabaseManager.instance.equipList7;
                break;
            default:
                break;
        }
    }

    public void GetCharacterIndex2()
    {
        theEquip.ItemEquip(_title.character_index[1]);
        theEquip.ShowEquip();
        theEquip.ShowUse();
        character_index = _title.character_index[1];
        character_Portrait.sprite = character_Images[character_index];
        switch (character_index)
        {
            case 0:
                EquipmentItemList = DatabaseManager.instance.equipList0;
                break;
            case 1:
                EquipmentItemList = DatabaseManager.instance.equipList1;
                break;
            case 2:
                EquipmentItemList = DatabaseManager.instance.equipList2;
                break;
            case 3:
                EquipmentItemList = DatabaseManager.instance.equipList3;
                break;
            case 4:
                EquipmentItemList = DatabaseManager.instance.equipList4;
                break;
            case 5:
                EquipmentItemList = DatabaseManager.instance.equipList5;
                break;
            case 6:
                EquipmentItemList = DatabaseManager.instance.equipList6;
                break;
            case 7:
                EquipmentItemList = DatabaseManager.instance.equipList7;
                break;
            default:
                break;
        }
    }

    public void GetItemIndex(int index)
    {
        item_clicked = true;
        StopAllCoroutines();
        selectedItem = index;
        Color color = slots[0].selected_Item.GetComponent<Image>().color;
        color.a = 1f;
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].selected_Item.GetComponent<Image>().color = color;
        }
        item_panel.SetActive(true);
        SelectedItem(selectedItem);
        StartCoroutine(SelectedItemEffect(selectedItem));
    }

    //public void GetEquipIndex(int index)
    //{
    //    StopAllCoroutines();
    //    selectedEquipItem = index;

    //    Color color = Eq_Slots[0].selected_Item.GetComponent<Image>().color;
    //    color.a = 0.2f;
    //    for (int i = 0; i < Eq_Slots.Length; i++)
    //    {
    //        Eq_Slots[i].selected_Item.GetComponent<Image>().color = color;
    //    }
    //    item_panel.SetActive(false);
    //    SelectedEquipItemp(selectedEquipItem);
    //    StartCoroutine(SelectedEquipEffect(selectedEquipItem));
    //}


    public void GetTabIndex(int index)
    {
        StopAllCoroutines();
        selectedTab = index;
        Color color = selectedTabImage[selectedTab].GetComponent<Image>().color;
        color.a = 0f;
        for (int i = 0; i < selectedTabImage.Length; i++) 
        {
            selectedTabImage[i].GetComponent<Image>().color = color;
        }
        StartCoroutine(SelectedTabEffect(selectedTab));
    }

    public void EquipToInventory(Item _item)
    {
        inventoryItemList.Add(_item);
    }

    //public void ShowEquipItem()
    //{
    //    EquipRemoveSlot();
    //    for (int i = 0; i < EquipmentItemList.Count; i++)
    //    {
    //        Eq_Slots[i].gameObject.SetActive(true);
    //        Eq_Slots[i].AddItem(EquipmentItemList[i]);
    //    }   // 인벤토리 탭 리스트의 내용을, 인벤토리 슬롯에 추가
    //}


    IEnumerator SelectedTabEffect(int index)
    {
        while (true)
        {
            Color color = selectedTabImage[index].GetComponent<Image>().color;
            while (color.a < 0.5)
            {
                color.a += 0.03f;
                selectedTabImage[index].GetComponent<Image>().color = color;
                yield return waitTime;
            }
            while (color.a > 0f)
            {
                color.a -= 0.03f;
                selectedTabImage[index].GetComponent<Image>().color = color;
                yield return waitTime;
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator SelectedItemEffect(int index)
    {
        while (true)
        {
            Color color = selectedItemImage[index].GetComponent<Image>().color;
            while (color.a < 1)
            {
                color.a += 0.02f;
                selectedItemImage[index].GetComponent<Image>().color = color;
                yield return waitTime;
            }
            while (color.a > 0.7f)
            {
                color.a -= 0.02f;
                selectedItemImage[index].GetComponent<Image>().color = color;
                yield return waitTime;
            }
        }
    }

    IEnumerator SelectedEquipEffect(int index)
    {

        while (true)
        {
            Color color = selectedEquipImage[index].GetComponent<Image>().color;
            while (color.a < 0.5)
            {
                color.a += 0.03f;
                selectedEquipImage[index].GetComponent<Image>().color = color;
                yield return waitTime;
            }
            while (color.a > 0f)
            {
                color.a -= 0.03f;
                selectedEquipImage[index].GetComponent<Image>().color = color;
                yield return waitTime;
            }

            yield return new WaitForSeconds(0.3f);

        }
    }


    private void Start()
    {
        title = obj.GetComponent<TitleButton>();
        theEquip = this.gameObject.GetComponent<Equipment>();
        slots = tf.GetComponentsInChildren<InventorySlot>();
        Eq_Slots = tf_1.GetComponentsInChildren<InventorySlot>();
        inventoryItemList = new List<Item>();
        inventoryTabList = new List<Item>();
        EquipmentItemList = new List<Item>();
        selectedItemImage = new GameObject[slots.Length];

        for (int i = 0; i < slots.Length; i++)
        {
            selectedItemImage[i] = slots[i].selected_Item;
        }
        inventoryItemList = DatabaseManager.instance.itemList;

        Description_Text.text = "";
        itemImage.color = new Color(0,0,0,0);
        item_name.text = "";
        count_text.text = "1";
    }

    private GameObject Image_temp;
    private Vector2 pos;
    private RaycastHit2D hit;

    private void Update()
    {
        if(itemImage.sprite == null)
        {
            itemImage.color = new Color(0, 0, 0, 0);
        }
        ShowItem();
        //ShowEquipItem();
        DisplayItemCount();
        if (Input.GetMouseButtonDown(0)) //누르는 순간
        {
            if (item_clicked) //아이템 클릭하면 
            {
                theEquip.Equip_num = -1;
                Image_temp = Instantiate(selectedButton, selectedButton.transform);
                Image_temp.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 100f);


                price.text = "+" + inventoryTabList[temp * flag + selectedItem].money.ToString();
                price.color = Color.green;
            }
        }

        else if (Input.GetMouseButton(0)) //누르는동안
        {
            pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hit = Physics2D.Raycast(pos, Vector2.zero, 1000f);
            if (hit && item_clicked)
            {
                if (hit.transform.tag == "Player") //장비창에 갔다댐
                {
                    theEquip.Equip_num = int.Parse(hit.transform.name);

                    if (theEquip.equipItemList[theEquip.Equip_num].itemID != 0) //있는경우
                    {
                        if (inventoryTabList[temp * flag + selectedItem]._HP - theEquip.equipItemList[theEquip.Equip_num]._HP > 0)
                        {
                            Plus_Stat[0].text = "+ " + (inventoryTabList[temp * flag + selectedItem]._HP - theEquip.equipItemList[theEquip.Equip_num]._HP).ToString();
                            Plus_Stat[0].color = Color.green;
                        }
                        else if(inventoryTabList[temp * flag + selectedItem]._HP - theEquip.equipItemList[theEquip.Equip_num]._HP < 0)
                        {
                            Plus_Stat[0].text = (inventoryTabList[temp * flag + selectedItem]._HP - theEquip.equipItemList[theEquip.Equip_num]._HP).ToString();
                            Plus_Stat[0].color = Color.red;
                        }
                        else
                        {
                            Plus_Stat[0].text = (inventoryTabList[temp * flag + selectedItem]._HP - theEquip.equipItemList[theEquip.Equip_num]._HP).ToString();
                            Plus_Stat[0].color = Color.black;
                        }

                        if ((inventoryTabList[temp * flag + selectedItem]._MP - theEquip.equipItemList[theEquip.Equip_num]._MP) > 0)
                        {
                            Plus_Stat[1].text = "+ " + (inventoryTabList[temp * flag + selectedItem]._MP - theEquip.equipItemList[theEquip.Equip_num]._MP).ToString();
                            Plus_Stat[1].color = Color.green;
                        }
                        else if((inventoryTabList[temp * flag + selectedItem]._MP - theEquip.equipItemList[theEquip.Equip_num]._MP) < 0)
                        {
                            Plus_Stat[1].text = (inventoryTabList[temp * flag + selectedItem]._MP - theEquip.equipItemList[theEquip.Equip_num]._MP).ToString();
                            Plus_Stat[1].color = Color.red;
                        }
                        else
                        {
                            Plus_Stat[1].text = (inventoryTabList[temp * flag + selectedItem]._MP - theEquip.equipItemList[theEquip.Equip_num]._MP).ToString();
                            Plus_Stat[1].color = Color.black;
                        }

                        if ((inventoryTabList[temp * flag + selectedItem]._PhysicalAttackPower - theEquip.equipItemList[theEquip.Equip_num]._PhysicalAttackPower) > 0)
                        {
                            Plus_Stat[2].text = "+ " + (inventoryTabList[temp * flag + selectedItem]._PhysicalAttackPower - theEquip.equipItemList[theEquip.Equip_num]._PhysicalAttackPower).ToString();
                            Plus_Stat[2].color = Color.green;
                        }
                        else if((inventoryTabList[temp * flag + selectedItem]._PhysicalAttackPower - theEquip.equipItemList[theEquip.Equip_num]._PhysicalAttackPower) < 0)
                        {
                            Plus_Stat[2].text = (inventoryTabList[temp * flag + selectedItem]._PhysicalAttackPower - theEquip.equipItemList[theEquip.Equip_num]._PhysicalAttackPower).ToString();
                            Plus_Stat[2].color = Color.red;
                        }
                        else
                        {
                            Plus_Stat[2].text = (inventoryTabList[temp * flag + selectedItem]._PhysicalAttackPower - theEquip.equipItemList[theEquip.Equip_num]._PhysicalAttackPower).ToString();
                            Plus_Stat[2].color = Color.black;
                        }

                        if ((inventoryTabList[temp * flag + selectedItem]._MagicAttackPower - theEquip.equipItemList[theEquip.Equip_num]._MagicAttackPower) > 0)
                        {
                            Plus_Stat[3].text = "+ " + (inventoryTabList[temp * flag + selectedItem]._MagicAttackPower - theEquip.equipItemList[theEquip.Equip_num]._MagicAttackPower).ToString();
                            Plus_Stat[3].color = Color.green;
                        }
                        else if((inventoryTabList[temp * flag + selectedItem]._MagicAttackPower - theEquip.equipItemList[theEquip.Equip_num]._MagicAttackPower) < 0)
                        {
                            Plus_Stat[3].text = (inventoryTabList[temp * flag + selectedItem]._MagicAttackPower - theEquip.equipItemList[theEquip.Equip_num]._MagicAttackPower).ToString();
                            Plus_Stat[3].color = Color.red;
                        }
                        else
                        {
                            Plus_Stat[3].text = (inventoryTabList[temp * flag + selectedItem]._MagicAttackPower - theEquip.equipItemList[theEquip.Equip_num]._MagicAttackPower).ToString();
                            Plus_Stat[3].color = Color.black;
                        }

                        if ((inventoryTabList[temp * flag + selectedItem]._PhysicalArmor - theEquip.equipItemList[theEquip.Equip_num]._PhysicalArmor) > 0)
                        {
                            Plus_Stat[4].text = "+ " + (inventoryTabList[temp * flag + selectedItem]._PhysicalArmor - theEquip.equipItemList[theEquip.Equip_num]._PhysicalArmor).ToString();
                            Plus_Stat[4].color = Color.green;
                        }
                        else if((inventoryTabList[temp * flag + selectedItem]._PhysicalArmor - theEquip.equipItemList[theEquip.Equip_num]._PhysicalArmor) < 0)
                        {
                            Plus_Stat[4].text = (inventoryTabList[temp * flag + selectedItem]._PhysicalArmor - theEquip.equipItemList[theEquip.Equip_num]._PhysicalArmor).ToString();
                            Plus_Stat[4].color = Color.red;
                        }
                        else
                        {
                            Plus_Stat[4].text = (inventoryTabList[temp * flag + selectedItem]._PhysicalArmor - theEquip.equipItemList[theEquip.Equip_num]._PhysicalArmor).ToString();
                            Plus_Stat[4].color = Color.black;
                        }

                        if((inventoryTabList[temp * flag + selectedItem]._MagicArmorPower - theEquip.equipItemList[theEquip.Equip_num]._MagicArmorPower) > 0)
                        {
                            Plus_Stat[5].text = "+ " + (inventoryTabList[temp * flag + selectedItem]._MagicArmorPower - theEquip.equipItemList[theEquip.Equip_num]._MagicArmorPower).ToString();
                            Plus_Stat[5].color = Color.green;
                        }
                        else if((inventoryTabList[temp * flag + selectedItem]._MagicArmorPower - theEquip.equipItemList[theEquip.Equip_num]._MagicArmorPower) < 0)
                        {
                            Plus_Stat[5].text = (inventoryTabList[temp * flag + selectedItem]._MagicArmorPower - theEquip.equipItemList[theEquip.Equip_num]._MagicArmorPower).ToString();
                            Plus_Stat[5].color = Color.red;
                        }
                        else
                        {
                            Plus_Stat[5].text = (inventoryTabList[temp * flag + selectedItem]._MagicArmorPower - theEquip.equipItemList[theEquip.Equip_num]._MagicArmorPower).ToString();
                            Plus_Stat[5].color = Color.black;
                        }

                        if((inventoryTabList[temp * flag + selectedItem]._NormalAttackRange - theEquip.equipItemList[theEquip.Equip_num]._NormalAttackRange) > 0)
                        {
                            Plus_Stat[6].text = "+ " + (inventoryTabList[temp * flag + selectedItem]._NormalAttackRange - theEquip.equipItemList[theEquip.Equip_num]._NormalAttackRange).ToString();
                            Plus_Stat[6].color = Color.green;
                        }
                        else if((inventoryTabList[temp * flag + selectedItem]._NormalAttackRange - theEquip.equipItemList[theEquip.Equip_num]._NormalAttackRange) < 0)
                        {
                            Plus_Stat[6].text = (inventoryTabList[temp * flag + selectedItem]._NormalAttackRange - theEquip.equipItemList[theEquip.Equip_num]._NormalAttackRange).ToString();
                            Plus_Stat[6].color = Color.red;
                        }
                        else
                        {
                            Plus_Stat[6].text = (inventoryTabList[temp * flag + selectedItem]._NormalAttackRange - theEquip.equipItemList[theEquip.Equip_num]._NormalAttackRange).ToString();
                            Plus_Stat[6].color = Color.black;
                        }

                        if((inventoryTabList[temp * flag + selectedItem]._NormalAttackSpeed - theEquip.equipItemList[theEquip.Equip_num]._NormalAttackSpeed) > 0)
                        {
                            Plus_Stat[7].text = "+ " + (inventoryTabList[temp * flag + selectedItem]._NormalAttackSpeed - theEquip.equipItemList[theEquip.Equip_num]._NormalAttackSpeed).ToString();
                            Plus_Stat[7].color = Color.green;
                        }
                        else if((inventoryTabList[temp * flag + selectedItem]._NormalAttackSpeed - theEquip.equipItemList[theEquip.Equip_num]._NormalAttackSpeed) < 0)
                        {
                            Plus_Stat[7].text = (inventoryTabList[temp * flag + selectedItem]._NormalAttackSpeed - theEquip.equipItemList[theEquip.Equip_num]._NormalAttackSpeed).ToString();
                            Plus_Stat[7].color = Color.red;
                        }
                        else
                        {
                            Plus_Stat[7].text = (inventoryTabList[temp * flag + selectedItem]._NormalAttackSpeed - theEquip.equipItemList[theEquip.Equip_num]._NormalAttackSpeed).ToString();
                            Plus_Stat[7].color = Color.black;
                        }

                        if((inventoryTabList[temp * flag + selectedItem].__MoveSpeed - theEquip.equipItemList[theEquip.Equip_num].__MoveSpeed) > 0)
                        {
                            Plus_Stat[8].text = "+ " + (inventoryTabList[temp * flag + selectedItem].__MoveSpeed - theEquip.equipItemList[theEquip.Equip_num].__MoveSpeed).ToString();
                            Plus_Stat[8].color = Color.green;
                        }
                        else if((inventoryTabList[temp * flag + selectedItem].__MoveSpeed - theEquip.equipItemList[theEquip.Equip_num].__MoveSpeed) < 0)
                        {
                            Plus_Stat[8].text = (inventoryTabList[temp * flag + selectedItem].__MoveSpeed - theEquip.equipItemList[theEquip.Equip_num].__MoveSpeed).ToString();
                            Plus_Stat[8].color = Color.red;
                        }
                        else
                        {
                            Plus_Stat[8].text = (inventoryTabList[temp * flag + selectedItem].__MoveSpeed - theEquip.equipItemList[theEquip.Equip_num].__MoveSpeed).ToString();
                            Plus_Stat[8].color = Color.black;
                        }

                        if((inventoryTabList[temp * flag + selectedItem]._ciritical - theEquip.equipItemList[theEquip.Equip_num]._ciritical) > 0)
                        {
                            Plus_Stat[9].text = "+ " + (inventoryTabList[temp * flag + selectedItem]._ciritical - theEquip.equipItemList[theEquip.Equip_num]._ciritical).ToString();
                            Plus_Stat[9].color = Color.green;
                        }
                        else if((inventoryTabList[temp * flag + selectedItem]._ciritical - theEquip.equipItemList[theEquip.Equip_num]._ciritical) < 0)
                        {
                            Plus_Stat[9].text = (inventoryTabList[temp * flag + selectedItem]._ciritical - theEquip.equipItemList[theEquip.Equip_num]._ciritical).ToString();
                            Plus_Stat[9].color = Color.red;
                        }
                        else
                        {
                            Plus_Stat[9].text = (inventoryTabList[temp * flag + selectedItem]._ciritical - theEquip.equipItemList[theEquip.Equip_num]._ciritical).ToString();
                            Plus_Stat[9].color = Color.black;
                        }

                        if((inventoryTabList[temp * flag + selectedItem]._HP_Regenerate - theEquip.equipItemList[theEquip.Equip_num]._HP_Regenerate) > 0)
                        {
                            Plus_Stat[10].text = "+ " + (inventoryTabList[temp * flag + selectedItem]._HP_Regenerate - theEquip.equipItemList[theEquip.Equip_num]._HP_Regenerate).ToString();
                            Plus_Stat[10].color = Color.green;
                        }
                        else if((inventoryTabList[temp * flag + selectedItem]._HP_Regenerate - theEquip.equipItemList[theEquip.Equip_num]._HP_Regenerate) < 0)
                        {
                            Plus_Stat[10].text = (inventoryTabList[temp * flag + selectedItem]._HP_Regenerate - theEquip.equipItemList[theEquip.Equip_num]._HP_Regenerate).ToString();
                            Plus_Stat[10].color = Color.red;
                        }
                        else
                        {
                            Plus_Stat[10].text = (inventoryTabList[temp * flag + selectedItem]._HP_Regenerate - theEquip.equipItemList[theEquip.Equip_num]._HP_Regenerate).ToString();
                            Plus_Stat[10].color = Color.black;
                        }

                        if ((inventoryTabList[temp * flag + selectedItem]._MP_Regenerate - theEquip.equipItemList[theEquip.Equip_num]._MP_Regenerate) > 0)
                        {
                            Plus_Stat[11].text = "+ " + (inventoryTabList[temp * flag + selectedItem]._MP_Regenerate - theEquip.equipItemList[theEquip.Equip_num]._MP_Regenerate).ToString();
                            Plus_Stat[11].color = Color.green;
                        }
                        else if((inventoryTabList[temp * flag + selectedItem]._MP_Regenerate - theEquip.equipItemList[theEquip.Equip_num]._MP_Regenerate) < 0)
                        {
                            Plus_Stat[11].text = (inventoryTabList[temp * flag + selectedItem]._MP_Regenerate - theEquip.equipItemList[theEquip.Equip_num]._MP_Regenerate).ToString();
                            Plus_Stat[11].color = Color.red;
                        }
                        else
                        {
                            Plus_Stat[11].text = (inventoryTabList[temp * flag + selectedItem]._MP_Regenerate - theEquip.equipItemList[theEquip.Equip_num]._MP_Regenerate).ToString();
                            Plus_Stat[11].color = Color.black;
                        }

                    }
                    else //없는경우
                    {
                        // 더 만들어야 함
                        if(inventoryTabList[temp * flag + selectedItem]._HP > 0)
                        {
                            Plus_Stat[0].text = "+ " + inventoryTabList[temp * flag + selectedItem]._HP.ToString();
                            Plus_Stat[0].color = Color.green;
                        }
                        else if(inventoryTabList[temp * flag + selectedItem]._HP < 0)
                        {
                            Plus_Stat[0].text = inventoryTabList[temp * flag + selectedItem]._HP.ToString();
                            Plus_Stat[0].color = Color.red;
                        }
                        else
                        {
                            Plus_Stat[0].text = inventoryTabList[temp * flag + selectedItem]._HP.ToString();
                            Plus_Stat[0].color = Color.black;
                        }

                        if (inventoryTabList[temp * flag + selectedItem]._MP > 0)
                        {
                            Plus_Stat[1].text = "+ " + inventoryTabList[temp * flag + selectedItem]._MP.ToString();
                            Plus_Stat[1].color = Color.green;
                        }
                        else if(inventoryTabList[temp * flag + selectedItem]._MP < 0)
                        {
                            Plus_Stat[1].text = inventoryTabList[temp * flag + selectedItem]._MP.ToString();
                            Plus_Stat[1].color = Color.red;
                        }
                        else
                        {
                            Plus_Stat[1].text = inventoryTabList[temp * flag + selectedItem]._MP.ToString();
                            Plus_Stat[1].color = Color.black;
                        }
                        
                        if(inventoryTabList[temp * flag + selectedItem]._PhysicalAttackPower > 0)
                        {
                            Plus_Stat[2].text = "+ " + inventoryTabList[temp * flag + selectedItem]._PhysicalAttackPower.ToString();
                            Plus_Stat[2].color = Color.green;
                        }
                        else if(inventoryTabList[temp * flag + selectedItem]._PhysicalAttackPower < 0)
                        {
                            Plus_Stat[2].text = inventoryTabList[temp * flag + selectedItem]._PhysicalAttackPower.ToString();
                            Plus_Stat[2].color = Color.red;
                        }
                        else
                        {
                            Plus_Stat[2].text = inventoryTabList[temp * flag + selectedItem]._PhysicalAttackPower.ToString();
                            Plus_Stat[2].color = Color.black;
                        }

                        if(inventoryTabList[temp * flag + selectedItem]._MagicAttackPower > 0)
                        {
                            Plus_Stat[3].text = "+ " + inventoryTabList[temp * flag + selectedItem]._MagicAttackPower.ToString();
                            Plus_Stat[3].color = Color.green;
                        }
                        else if(inventoryTabList[temp * flag + selectedItem]._MagicAttackPower < 0)
                        {
                            Plus_Stat[3].text = inventoryTabList[temp * flag + selectedItem]._MagicAttackPower.ToString();
                            Plus_Stat[3].color = Color.red;
                        }
                        else
                        {
                            Plus_Stat[3].text = inventoryTabList[temp * flag + selectedItem]._MagicAttackPower.ToString();
                            Plus_Stat[3].color = Color.black;
                        }

                        if(inventoryTabList[temp * flag + selectedItem]._PhysicalArmor > 0)
                        {
                            Plus_Stat[4].text = "+ " + inventoryTabList[temp * flag + selectedItem]._PhysicalArmor.ToString();
                            Plus_Stat[4].color = Color.green;
                        }
                        else if(inventoryTabList[temp * flag + selectedItem]._PhysicalArmor < 0)
                        {
                            Plus_Stat[4].text = inventoryTabList[temp * flag + selectedItem]._PhysicalArmor.ToString();
                            Plus_Stat[4].color = Color.red;
                        }
                        else
                        {
                            Plus_Stat[4].text = inventoryTabList[temp * flag + selectedItem]._PhysicalArmor.ToString();
                            Plus_Stat[4].color = Color.black;
                        }

                        if(inventoryTabList[temp * flag + selectedItem]._MagicArmorPower > 0)
                        {
                            Plus_Stat[5].text = "+ " + inventoryTabList[temp * flag + selectedItem]._MagicArmorPower.ToString();
                            Plus_Stat[5].color = Color.green;
                        }
                        else if(inventoryTabList[temp * flag + selectedItem]._MagicArmorPower < 0)
                        {
                            Plus_Stat[5].text = inventoryTabList[temp * flag + selectedItem]._MagicArmorPower.ToString();
                            Plus_Stat[5].color = Color.red;
                        }
                        else
                        {
                            Plus_Stat[5].text = inventoryTabList[temp * flag + selectedItem]._MagicArmorPower.ToString();
                            Plus_Stat[5].color = Color.black;
                        }

                        if(inventoryTabList[temp * flag + selectedItem]._NormalAttackRange > 0)
                        {
                            Plus_Stat[6].text = "+ " + inventoryTabList[temp * flag + selectedItem]._NormalAttackRange.ToString();
                            Plus_Stat[6].color = Color.green;
                        }
                        else if(inventoryTabList[temp * flag + selectedItem]._NormalAttackRange < 0)
                        {
                            Plus_Stat[6].text = inventoryTabList[temp * flag + selectedItem]._NormalAttackRange.ToString();
                            Plus_Stat[6].color = Color.red;
                        }
                        else
                        {
                            Plus_Stat[6].text = inventoryTabList[temp * flag + selectedItem]._NormalAttackRange.ToString();
                            Plus_Stat[6].color = Color.black;
                        }

                        if(inventoryTabList[temp * flag + selectedItem]._NormalAttackSpeed > 0)
                        {
                            Plus_Stat[7].text = "+ " + inventoryTabList[temp * flag + selectedItem]._NormalAttackSpeed.ToString();
                            Plus_Stat[7].color = Color.green;
                        }
                        else if(inventoryTabList[temp * flag + selectedItem]._NormalAttackSpeed < 0)
                        {
                            Plus_Stat[7].text = inventoryTabList[temp * flag + selectedItem]._NormalAttackSpeed.ToString();                        
                            Plus_Stat[7].color = Color.red;
                        }
                        else
                        {
                            Plus_Stat[7].text = inventoryTabList[temp * flag + selectedItem]._NormalAttackSpeed.ToString();
                            Plus_Stat[7].color = Color.black;
                        }

                        if(inventoryTabList[temp * flag + selectedItem].__MoveSpeed > 0)
                        {
                            Plus_Stat[8].text = "+ " + inventoryTabList[temp * flag + selectedItem].__MoveSpeed.ToString();
                            Plus_Stat[8].color = Color.green;
                        }
                        else if(inventoryTabList[temp * flag + selectedItem].__MoveSpeed < 0)
                        {
                            Plus_Stat[8].text = inventoryTabList[temp * flag + selectedItem].__MoveSpeed.ToString();
                            Plus_Stat[8].color = Color.red;
                        }
                        else
                        {
                            Plus_Stat[8].text = inventoryTabList[temp * flag + selectedItem].__MoveSpeed.ToString();
                            Plus_Stat[8].color = Color.black;
                        }

                        if(inventoryTabList[temp * flag + selectedItem]._ciritical > 0)
                        {
                            Plus_Stat[9].text = "+ " + inventoryTabList[temp * flag + selectedItem]._ciritical.ToString();
                            Plus_Stat[9].color = Color.green;
                        }
                        else if(inventoryTabList[temp * flag + selectedItem]._ciritical < 0)
                        {
                            Plus_Stat[9].text = inventoryTabList[temp * flag + selectedItem]._ciritical.ToString();
                            Plus_Stat[9].color = Color.red;
                        }
                        else
                        {
                            Plus_Stat[9].text = inventoryTabList[temp * flag + selectedItem]._ciritical.ToString();
                            Plus_Stat[9].color = Color.black;
                        }

                        if(inventoryTabList[temp * flag + selectedItem]._HP_Regenerate > 0)
                        {
                            Plus_Stat[10].text = "+ " + inventoryTabList[temp * flag + selectedItem]._HP_Regenerate.ToString();
                            Plus_Stat[10].color = Color.green;
                        }
                        else if(inventoryTabList[temp * flag + selectedItem]._HP_Regenerate < 0)
                        {
                            Plus_Stat[10].text = inventoryTabList[temp * flag + selectedItem]._HP_Regenerate.ToString();
                            Plus_Stat[10].color = Color.red;
                        }
                        else
                        {
                            Plus_Stat[10].text = inventoryTabList[temp * flag + selectedItem]._HP_Regenerate.ToString();
                            Plus_Stat[10].color = Color.black;
                        }

                        if(inventoryTabList[temp * flag + selectedItem]._MP_Regenerate > 0)
                        {
                            Plus_Stat[11].text = "+ " + inventoryTabList[temp * flag + selectedItem]._MP_Regenerate.ToString();
                            Plus_Stat[11].color = Color.green;
                        }
                        else if(inventoryTabList[temp * flag + selectedItem]._MP_Regenerate < 0)
                        {
                            Plus_Stat[11].text = inventoryTabList[temp * flag + selectedItem]._MP_Regenerate.ToString();
                            Plus_Stat[11].color = Color.red;
                        }
                        else
                        {
                            Plus_Stat[11].text = inventoryTabList[temp * flag + selectedItem]._MP_Regenerate.ToString();
                            Plus_Stat[11].color = Color.black;
                        }

                    }
                }
                else if(hit.transform.tag == "Boss") //소비창에 가져다댐
                {
                    theEquip.Use_num = int.Parse(hit.transform.name);
                }
                
            }
            if (item_clicked)
            {
                Image_temp.transform.position = pos;
            }
        }

        else if(Input.GetMouseButtonUp(0))//때면
        {
            price.text = "";

            pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hit = Physics2D.Raycast(pos, Vector2.zero, 1000f);
            if (hit && item_clicked)
            {
                if (hit.transform.tag == "Player")
                {
                    SoundManager.instance.Play(40);

                    hit.transform.gameObject.GetComponent<Image>().material = null;
                }

                if (hit.transform.tag == "GameController") //상점에 갔다대고 땐경우
                {
                    SellAsk.SetActive(true);
                }

                if (hit.transform.tag == "Boss") //사용아이템 창에 가져다대면
                {
                    SoundManager.instance.Play(40);
                    Ask_pannel.SetActive(true);                
                }

            }
            for (int i = 0; i < Plus_Stat.Length; i++)
            {
                Plus_Stat[i].text = null;
                Plus_Stat[i].color = Color.black;
            }
            theEquip.Equip_num = -1;
            character_Portrait.material = null;
            Destroy(Image_temp);
            item_clicked = false;
            selectedButton = null;
        }
    }

    public void EquipItem()
    {
        theEquip.EquipItem(inventoryTabList[temp * flag + selectedItem]); //들고있는 아이템
        int i = 0;

        for (i=0;i< inventoryItemList.Count;i++)
        {
            if (inventoryItemList[i] == inventoryTabList[temp * flag + selectedItem]) 
            {
                selectedItem = i;
                break;
            }
        }
        inventoryItemList.RemoveAt(selectedItem);
    }


    public void ScrollUp()
    {
        if (temp > 0)
        {
            SoundManager.instance.Play(37);
            temp--;
        }
        else
        {
            SoundManager.instance.Play(36);

            return;
        }
    }

    public void ScrollDown()
    {
        if(temp < 17)
        {
            SoundManager.instance.Play(37);
            temp++;
        }
        else
        {
            SoundManager.instance.Play(36);

            return;
        }
    }

    public void SellYes()
    {
        SoundManager.instance.Play(39);
        Sell_count_text.text = count.ToString();
        if (DatabaseManager.instance.Money > 9999999)
        {
            DatabaseManager.instance.Money = 9999999;
        }
        if(sell_count == inventoryTabList[temp * flag + selectedItem].itemCount)
        {
            DatabaseManager.instance.Money += sell_count * inventoryTabList[temp * flag + selectedItem].money;
            DatabaseManager.instance.itemList.Remove(DatabaseManager.instance.itemList.Find(item => item.itemID == inventoryTabList[temp * flag + selectedItem].itemID));
        }
        else
        {

            DatabaseManager.instance.Money += sell_count * inventoryTabList[temp * flag + selectedItem].money;
            DatabaseManager.instance.itemList.Find(item => item.itemID == inventoryTabList[temp * flag + selectedItem].itemID).itemCount -= sell_count;
        }   

        //돈이펙트 ...
        pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        theEquip.ShowUse();

        ShowItem();
        SellEffect_prefab = Instantiate(SellEffect, pos,Quaternion.identity);
        Destroy(SellEffect_prefab, 3f);
        sell_count = 1;
        Sell_count_text.text = sell_count.ToString();
        SellAsk.SetActive(false);
        itemImage.sprite = null;
        Description_Text.text = "";
        itemImage.sprite = null;
        item_name.text = "";
        price.text = "";

    }
    public void SellNo()
    {
        SoundManager.instance.Play(36);

        SellAsk.SetActive(false);
    }
    public GameObject[] tab;
    public void EquipTab()
    {
        tab[0].SetActive(true);
        tab[1].SetActive(false);
    }
    public void UseTab()
    {
        tab[0].SetActive(false);
        tab[1].SetActive(true);
        theEquip.ShowUse();
    }

    private void DisplayItemCount()
    {
        current_item_count.text = DatabaseManager.instance.itemList.Count.ToString();

        if (DatabaseManager.instance.itemList.Count > max_item_count - 10)
        {
            current_item_count.color = Color.red;
        }
    }

    public void CountDown()
    {
        if (count <= 1)
        {
            SoundManager.instance.Play(36);
            count = 1;
            return;
        }

        else
        {
            count--;
        }
        count_text.text = count.ToString();

    }

    public void CountUp()
    {
        if(count >= inventoryTabList[temp * flag + selectedItem].itemCount)
        {
            SoundManager.instance.Play(36);
            count = inventoryTabList[temp * flag + selectedItem].itemCount;
            return;
        }
        else
        {
            count++;
        }
        count_text.text = count.ToString();

    }

    public void Cancel()
    {
        count = 1;
        count_text.text = count.ToString();
        Ask_pannel.SetActive(false);
    }

    public void Min()
    {
        count = 1;
        count_text.text = count.ToString();

    }

    public void Max()
    {
        count = inventoryTabList[temp * flag + selectedItem].itemCount;
        count_text.text = count.ToString();
    }

    public void OK()
    {
        bool Isalready = false;

        for (int i = 0; i < theEquip.useitemList.Length; i++)
        {
            if (theEquip.useitemList[i].itemID == inventoryTabList[temp * flag + selectedItem].itemID)//넣으려는 템이랑 같은게 있으면
            {
                theEquip.useitemList[i].itemCount += count;
                DatabaseManager.instance.itemList.Find(item => item.itemID == inventoryTabList[temp * flag + selectedItem].itemID).itemCount -= count;
                theEquip.ShowUse();
                ShowItem();
                if (DatabaseManager.instance.itemList.Find(item => item.itemID == inventoryTabList[temp * flag + selectedItem].itemID).itemCount == 0)
                {
                    DatabaseManager.instance.itemList.Remove(inventoryTabList[temp * flag + selectedItem]);
                    inventoryItemList.RemoveAt(selectedItem); //넣은 템은 인벤에서 삭제
                    theEquip.ShowUse();
                    ShowItem();
                }
                Isalready = true;
                break;
            }
        }


        if (!Isalready)
        {
            if (theEquip.useitemList[theEquip.Use_num].itemID != 0)//원래 템이있으면
            {
                EquipToInventory(theEquip.useitemList[theEquip.Use_num]); //있던템은 인벤으로 보내주고
                if (count == inventoryTabList[temp * flag + selectedItem].itemCount)
                {
                    DatabaseManager.instance.itemList.Remove(inventoryTabList[temp * flag + selectedItem]);
                    theEquip.useitemList[theEquip.Use_num] = inventoryTabList[temp * flag + selectedItem];
                    theEquip.ShowUse();
                    ShowItem();
                }
                else
                {
                    Item _item = (Item)inventoryTabList[temp * flag + selectedItem].Clone();
                    theEquip.useitemList[theEquip.Use_num] = _item;
                    theEquip.useitemList[theEquip.Use_num].itemCount = count;
                    inventoryItemList.Find(item => item.itemID == inventoryTabList[temp * flag + selectedItem].itemID).itemCount -= count;
                    theEquip.ShowUse();

                }
            }
            else
            {
                if(count == inventoryTabList[temp * flag + selectedItem].itemCount)
                {
                    DatabaseManager.instance.itemList.Remove(inventoryTabList[temp * flag + selectedItem]);
                    theEquip.useitemList[theEquip.Use_num] = inventoryTabList[temp * flag + selectedItem];
                    theEquip.ShowUse();
                    ShowItem();
                }
                else
                {
                    Item _item = (Item)inventoryTabList[temp * flag + selectedItem].Clone();
                    theEquip.useitemList[theEquip.Use_num] = _item;
                    theEquip.useitemList[theEquip.Use_num].itemCount = count;
                    inventoryItemList.Find(item => item.itemID == inventoryTabList[temp * flag + selectedItem].itemID).itemCount -= count;
                    theEquip.ShowUse();

                }
            }

        }

        selectedItem = 0;
     
        //데이터베이스에 넣어주고
        switch (character_index)
        {
            case 0:
                DatabaseManager.instance.UseList0[theEquip.Use_num] = theEquip.useitemList[theEquip.Use_num];
                break;
            case 1:
                DatabaseManager.instance.UseList1[theEquip.Use_num] = theEquip.useitemList[theEquip.Use_num];
                break;
            case 2:
                DatabaseManager.instance.UseList2[theEquip.Use_num] = theEquip.useitemList[theEquip.Use_num];
                break;
            case 3:
                DatabaseManager.instance.UseList3[theEquip.Use_num] = theEquip.useitemList[theEquip.Use_num];
                break;
            case 4:
                DatabaseManager.instance.UseList4[theEquip.Use_num] = theEquip.useitemList[theEquip.Use_num];
                break;
            case 5:
                DatabaseManager.instance.UseList5[theEquip.Use_num] = theEquip.useitemList[theEquip.Use_num];
                break;
            case 6:
                DatabaseManager.instance.UseList6[theEquip.Use_num] = theEquip.useitemList[theEquip.Use_num];
                break;
            case 7:
                DatabaseManager.instance.UseList7[theEquip.Use_num] = theEquip.useitemList[theEquip.Use_num];
                break;

        }
        count = 1;
        count_text.text = count.ToString();
        Ask_pannel.SetActive(false);
    }


    public void SellCountDown()
    {
        if (sell_count <= 1)
        {
            SoundManager.instance.Play(36);
            sell_count = 1;
            return;
        }

        else
        {
            sell_count--;
        }
        Sell_count_text.text = sell_count.ToString();

    }

    public void SellCountUp()
    {
        if (sell_count >= inventoryTabList[temp * flag + selectedItem].itemCount)
        {
            SoundManager.instance.Play(36);
            sell_count = inventoryTabList[temp * flag + selectedItem].itemCount;
            return;
        }
        else
        {
            sell_count++;
        }
        Sell_count_text.text = sell_count.ToString();
    }

    public void SellCancel()
    {
        sell_count = 1;
        Sell_count_text.text = sell_count.ToString();
        SellAsk.SetActive(false);
    }

    public void SellMin()
    {
        sell_count = 1;
        Sell_count_text.text = sell_count.ToString();

    }

    public void SellMax()
    {
        sell_count = inventoryTabList[temp * flag + selectedItem].itemCount;
        Sell_count_text.text = sell_count.ToString();
    }


}

