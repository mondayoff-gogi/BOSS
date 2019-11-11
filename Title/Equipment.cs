using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Equipment : MonoBehaviour
{
    public GameObject[] shop;
    public GameObject[] equip;
    public Text Money;
    public ShopSlot[] shopSlots;
    public EquipmentSlot[] slots;
    public UseSlot[] Useslots;

    private List<Item> ShopList = new List<Item>();
    private int flag = 0;
    // 플레이어가 장착한 아이템 리스트
    public Item[] equipItemList;
    // 플레이어가 장착한 사용아이템 리스트
    public Item[] useitemList;

    public int Equip_num = -1;
    public int Use_num = -1;

    public Image inven_Icon;
    public Image shop_Icon;


    private int character_index = 0;
    private Inventory inven;

    public Text Description_Text;
    public Image itemImage;
    public Text item_name;

    public Text[] text;
    public Image[] item_Slot_Image;
    public Image[] Shop_Slot_Image;
    public Image[] use_Slot_Image;
    public Text price;
    private int selected_Slot;

    private Vector2 pos;
    private RaycastHit2D hit;

    private float[] Char_Stat;
    private bool IsTakeOff = false;
    private bool IsThrow = false;
    private bool isMove = false;
    private bool IsShopping = false;
    private bool IsBuying = false;
    private bool IsUseOff = false;
    private bool IsUseThrow = false;
    private bool IsUseMove = false;
    private int slot_num;

    private GameObject Image_temp;
    private GameObject selectedButton;

    // 아이템 테두리 색깔
    private Color white = new Color(1, 1, 1, 0.5f);
    private Color Blue = new Color(0.25f, 0.7f, 1f, 0.5f);
    private Color Purple = new Color(1f, 0.5f, 1f, 0.5f);
    private Color Orange = new Color(1f, 0.8f, 0.5f, 0.5f);

    public Text[] Change_stat;

    public GameObject warning_pannel;
    private Text warning_text;

    public Text Buy_count_text;
    private int buy_count = 1;
    public GameObject BuyAskPannel;

    private int max_count;

    private void Update()
    {
        if (itemImage.sprite == null)
        {
            itemImage.color = new Color(0, 0, 0, 0);
        }

        ShowShop();

        if (Input.GetMouseButtonDown(0)) //누르는 순간
        {
            if (IsTakeOff || IsShopping || IsUseOff) //벗는게 맞거나 사는게 맞으면
            {
                itemImage.color = new Color(255, 255, 255, 255);
                //selectedItem  의 그림을 움직임
                pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                Image_temp = Instantiate(selectedButton, selectedButton.transform);

                Image_temp.transform.position = pos;
                Image_temp.transform.SetParent(selectedButton.transform.parent.parent.parent.parent.parent);


                Image_temp.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 100f);
            }
        }


        else if (Input.GetMouseButton(0)) //누르는동안
        {
            pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hit = Physics2D.Raycast(pos, Vector2.zero, 1000f);

            if (hit && (IsTakeOff || IsUseOff))
            {

                if (hit.transform.tag == "Boundary" && inven.selectedTab == 0) //장비옮기는경우
                {
                    IsThrow = true;
                }
                else
                    IsThrow = false;

                if (hit.transform.tag == "Boundary" && inven.selectedTab == 1) //소비옮기는경우
                {
                    IsUseThrow = true;
                }
                else
                    IsUseThrow = false;


                if (hit.transform.tag == "Player") //장비칸으로 옮기는경우
                {
                    isMove = true;
                    slot_num = int.Parse(hit.transform.name);
                }
                else
                {
                    isMove = false;
                }


                if (hit.transform.tag == "Boss") //소비칸으로 옮기는경우
                {
                    IsUseMove = true;
                    slot_num = int.Parse(hit.transform.name);
                }
                else
                {
                    IsUseMove = false;
                }

            }
            else
            {
                IsThrow = false;
                isMove = false;
                IsUseThrow = false;
            }

            if (hit && IsShopping)
            {
                if (hit.transform.tag == "Boundary") //사는거면
                {
                    //탭을 옮겨줌
                    if (ShopList[selected_Slot + flag * shopSlots.Length].itemType == Item.ItemType.Equip)
                    {
                        inven.GetTabIndex(0);
                    }
                    else if (ShopList[selected_Slot + flag * shopSlots.Length].itemType == Item.ItemType.Use)
                    {
                        inven.GetTabIndex(1);
                    }
                    else if (ShopList[selected_Slot + flag * shopSlots.Length].itemType == Item.ItemType.ETC)
                    {
                        inven.GetTabIndex(2);
                    }
                    IsBuying = true;
                }
                else
                    IsBuying = false;
            }
            else
                IsBuying = false;


            if (Image_temp)
                Image_temp.transform.position = pos;
            if (IsTakeOff) //장비아이템클릭된상태
            {
                inven.Plus_Stat[0].text = (-equipItemList[selected_Slot]._HP).ToString();
                inven.Plus_Stat[1].text = (-equipItemList[selected_Slot]._MP).ToString();
                inven.Plus_Stat[2].text = (-equipItemList[selected_Slot]._PhysicalAttackPower).ToString();
                inven.Plus_Stat[3].text = (-equipItemList[selected_Slot]._MagicAttackPower).ToString();
                inven.Plus_Stat[4].text = (-equipItemList[selected_Slot]._PhysicalArmor).ToString();
                inven.Plus_Stat[5].text = (-equipItemList[selected_Slot]._MagicArmorPower).ToString();
                inven.Plus_Stat[6].text = (-equipItemList[selected_Slot]._NormalAttackRange).ToString();
                inven.Plus_Stat[7].text = (-equipItemList[selected_Slot]._NormalAttackSpeed).ToString();
                inven.Plus_Stat[8].text = (-equipItemList[selected_Slot].__MoveSpeed).ToString();
                inven.Plus_Stat[9].text = (-equipItemList[selected_Slot]._ciritical).ToString();
                inven.Plus_Stat[10].text = (-equipItemList[selected_Slot]._HP_Regenerate).ToString();
                inven.Plus_Stat[11].text = (-equipItemList[selected_Slot]._MP_Regenerate).ToString();

                for (int i = 0; i < 12; i++)
                {
                    if (float.Parse(inven.Plus_Stat[i].text) < 0) //0보다 작으면
                        inven.Plus_Stat[i].color = Color.red;
                    else if (float.Parse(inven.Plus_Stat[i].text) > 0) //0보다 크면
                    {
                        inven.Plus_Stat[i].text = "+" + inven.Plus_Stat[i].text;
                        inven.Plus_Stat[i].color = Color.green;
                    }
                    else
                        inven.Plus_Stat[i].color = Color.black;
                }
            }

        }
        else if (Input.GetMouseButtonUp(0))//때면
        {
            price.text = "";
            if (Image_temp)
                Destroy(Image_temp);
            if (IsTakeOff)
            {
                IsTakeOff = false;
                selectedButton = null;
                if (IsThrow)
                {
                    SoundManager.instance.Play(41);
                    TakeOffItem();
                }
                if (isMove) //교체 또는 옮기기
                { //slot은 가는곳    selected_Slot 는 기존위치
                    SoundManager.instance.Play(38);

                    Item temp = equipItemList[selected_Slot];
                    equipItemList[selected_Slot] = equipItemList[slot_num];
                    equipItemList[slot_num] = temp;
                    ShowEquip();


                    switch (character_index)
                    {
                        case 0:
                            DatabaseManager.instance.equipList0[selected_Slot] = DatabaseManager.instance.equipList0[slot_num];
                            DatabaseManager.instance.equipList0[slot_num] = temp;
                            break;
                        case 1:
                            DatabaseManager.instance.equipList1[selected_Slot] = DatabaseManager.instance.equipList1[slot_num];
                            DatabaseManager.instance.equipList1[slot_num] = temp;
                            break;
                        case 2:
                            DatabaseManager.instance.equipList2[selected_Slot] = DatabaseManager.instance.equipList2[slot_num];
                            DatabaseManager.instance.equipList2[slot_num] = temp;
                            break;
                        case 3:
                            DatabaseManager.instance.equipList3[selected_Slot] = DatabaseManager.instance.equipList3[slot_num];
                            DatabaseManager.instance.equipList3[slot_num] = temp;
                            break;
                        case 4:
                            DatabaseManager.instance.equipList4[selected_Slot] = DatabaseManager.instance.equipList4[slot_num];
                            DatabaseManager.instance.equipList4[slot_num] = temp;
                            break;
                        case 5:
                            DatabaseManager.instance.equipList5[selected_Slot] = DatabaseManager.instance.equipList5[slot_num];
                            DatabaseManager.instance.equipList5[slot_num] = temp;
                            break;
                        case 6:
                            DatabaseManager.instance.equipList6[selected_Slot] = DatabaseManager.instance.equipList6[slot_num];
                            DatabaseManager.instance.equipList6[slot_num] = temp;
                            break;
                        case 7:
                            DatabaseManager.instance.equipList7[selected_Slot] = DatabaseManager.instance.equipList7[slot_num];
                            DatabaseManager.instance.equipList7[slot_num] = temp;
                            break;
                    }
                }
            }
            if (IsUseOff)
            {

                IsUseOff = false;
                selectedButton = null;

                if (IsUseMove) //소비창 템 옮기는거면
                {
                    SoundManager.instance.Play(38);

                    IsUseMove = false;

                    Item temp = useitemList[selected_Slot];
                    useitemList[selected_Slot] = useitemList[slot_num];
                    useitemList[slot_num] = temp;
                    ShowUse();


                    switch (character_index)
                    {
                        case 0:
                            DatabaseManager.instance.UseList0[selected_Slot] = DatabaseManager.instance.UseList0[slot_num];
                            DatabaseManager.instance.UseList0[slot_num] = temp;
                            break;
                        case 1:
                            DatabaseManager.instance.UseList1[selected_Slot] = DatabaseManager.instance.UseList1[slot_num];
                            DatabaseManager.instance.UseList1[slot_num] = temp;
                            break;
                        case 2:
                            DatabaseManager.instance.UseList2[selected_Slot] = DatabaseManager.instance.UseList2[slot_num];
                            DatabaseManager.instance.UseList2[slot_num] = temp;
                            break;
                        case 3:
                            DatabaseManager.instance.UseList3[selected_Slot] = DatabaseManager.instance.UseList3[slot_num];
                            DatabaseManager.instance.UseList3[slot_num] = temp;
                            break;
                        case 4:
                            DatabaseManager.instance.UseList4[selected_Slot] = DatabaseManager.instance.UseList4[slot_num];
                            DatabaseManager.instance.UseList4[slot_num] = temp;
                            break;
                        case 5:
                            DatabaseManager.instance.UseList5[selected_Slot] = DatabaseManager.instance.UseList5[slot_num];
                            DatabaseManager.instance.UseList5[slot_num] = temp;
                            break;
                        case 6:
                            DatabaseManager.instance.UseList6[selected_Slot] = DatabaseManager.instance.UseList6[slot_num];
                            DatabaseManager.instance.UseList6[slot_num] = temp;
                            break;
                        case 7:
                            DatabaseManager.instance.UseList7[selected_Slot] = DatabaseManager.instance.UseList7[slot_num];
                            DatabaseManager.instance.UseList7[slot_num] = temp;
                            break;


                    }

                }


                if (IsUseThrow)//소비창 빼는거면
                {
                    SoundManager.instance.Play(41);

                    IsUseThrow = false;

                    if (useitemList[selected_Slot].itemID == 0) return;

                    Useslots[selected_Slot].RemoveItem();

                    Item _item = new Item(0, "", "", Item.ItemType.Use);

                    switch (character_index)
                    {
                        case 0:
                            DatabaseManager.instance.UseList0[selected_Slot] = _item;
                            break;
                        case 1:
                            DatabaseManager.instance.UseList1[selected_Slot] = _item;
                            break;
                        case 2:
                            DatabaseManager.instance.UseList2[selected_Slot] = _item;
                            break;
                        case 3:
                            DatabaseManager.instance.UseList3[selected_Slot] = _item;
                            break;
                        case 4:
                            DatabaseManager.instance.UseList4[selected_Slot] = _item;
                            break;
                        case 5:
                            DatabaseManager.instance.UseList5[selected_Slot] = _item;
                            break;
                        case 6:
                            DatabaseManager.instance.UseList6[selected_Slot] = _item;
                            break;
                        case 7:
                            DatabaseManager.instance.UseList7[selected_Slot] = _item;
                            break;
                    }

                    Item temp = DatabaseManager.instance.itemList.Find(item => item.itemID == useitemList[selected_Slot].itemID);
                    //비교해서 원래 있던 아이의 개수만 늘려주기
                    if (temp != null)//있으면
                        temp.itemCount += useitemList[selected_Slot].itemCount;
                    else//없으면
                    {
                        //선택한거 넣고
                        inven.EquipToInventory(useitemList[selected_Slot]);
                    }
                    //선택한거 삭제하고
                    useitemList[selected_Slot] = new Item(0, "", "", Item.ItemType.Use);

                    ShowUse();
                }
            }
            if (IsShopping)
            {
                selectedButton = null;
                IsShopping = false;

                if (!IsBuying) return;
                IsBuying = false;
                Description_Text.text = null;
                itemImage.sprite = Resources.Load("ItemIcon/Inven/0", typeof(Sprite)) as Sprite;

                item_name.text = null;
                price.text = null;
                if (ShopList[selected_Slot + flag * shopSlots.Length].money * 1f > DatabaseManager.instance.Money || DatabaseManager.instance.Money == 0)
                {
                    warning_pannel.SetActive(true);
                    warning_text.text = "금액이 부족합니다.";
                    buy_count = 0;
                }
                else if (DatabaseManager.instance.itemList.Count == max_count)
                {
                    warning_pannel.SetActive(true);
                    warning_text.text = "아이템 소지 한도를 넘었습니다.";
                    buy_count = 0;
                }
                else
                {
                    buy_count = 1;
                    BuyAskPannel.SetActive(true);
                }
                Buy_count_text.text = buy_count.ToString();
            }
        }
    }


    void Start()
    {
        equipItemList = new Item[4];

        useitemList = new Item[2];

        inven = FindObjectOfType<Inventory>();

        Char_Stat = new float[14];

        ShopList = new List<Item>();

        ShopList = DatabaseManager.instance.ShopItem;
        Buy_count_text.text = "0";
        warning_text = warning_pannel.GetComponentInChildren<Text>();
        max_count = DatabaseManager.instance.max_item_count;
    }

    public void ItemEquip(int CharIndex)
    {
        for (int i = 0; i < equipItemList.Length; i++)
        {
            equipItemList[i] = new Item(0, "", "", Item.ItemType.Equip);//맨처음 초기화
        }

        for (int i = 0; i < useitemList.Length; i++)
        {
            useitemList[i] = new Item(0, "", "", Item.ItemType.Use);
        }

        for (int i = 0; i < DatabaseManager.instance.equipList.Count; i++)
        {
            equipItemList[i] = DatabaseManager.instance.equipList[i];
        }

        for (int i = 0; i < DatabaseManager.instance.UseList.Count; i++)
        {
            useitemList[i] = DatabaseManager.instance.UseList[i];
        }
    }

    public void EquipButtonClick(int num) //장비 해제
    {
        SoundManager.instance.Play(38);

        IsTakeOff = true;
        selectedButton = item_Slot_Image[num].gameObject;
        selected_Slot = num;

        Description_Text.text = equipItemList[selected_Slot].itemDescription;
        itemImage.sprite = equipItemList[selected_Slot].itemIcon;
        item_name.text = equipItemList[selected_Slot].itemName;

    }
    public void UseButtonClick(int num) //소모품 해제
    {
        SoundManager.instance.Play(38);

        IsUseOff = true;
        selectedButton = use_Slot_Image[num].gameObject;
        selected_Slot = num;

        Description_Text.text = useitemList[selected_Slot].itemDescription;
        itemImage.sprite = useitemList[selected_Slot].itemIcon;
        item_name.text = useitemList[selected_Slot].itemName;
    }



    public void EquipButtonUp() //장비 장착
    {
        if (inven.selectedTab != 0) return;

        if (Equip_num == -1) return;

        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 1000f);
        if (hit)
        {
            if (hit.transform.tag != "Player")
            {
                return;
            }
        }
        else
        {
            return;
        }


        if (inven.item_clicked)
        {
            selected_Slot = Equip_num;
            if (selected_Slot >= slots.Length) return;

            if (equipItemList[Equip_num].itemID != 0)//장비를 착용하고있는경우
            {
                TakeOffItem();
                inven.EquipItem();
                return;
            }

            inven.EquipItem();
        }

    }


    public void GetCharacterIndex(int index)
    {
        character_index = index;
    }



    public void EquipItem(Item _item)
    {

        slots[selected_Slot].AddItem(_item);

        equipItemList[selected_Slot] = _item;
        EquipEffect(_item);

        switch (character_index)
        {
            case 0:
                DatabaseManager.instance.equipList0[selected_Slot] = _item;
                break;
            case 1:
                DatabaseManager.instance.equipList1[selected_Slot] = _item;
                break;
            case 2:
                DatabaseManager.instance.equipList2[selected_Slot] = _item;
                break;
            case 3:
                DatabaseManager.instance.equipList3[selected_Slot] = _item;
                break;
            case 4:
                DatabaseManager.instance.equipList4[selected_Slot] = _item;
                break;
            case 5:
                DatabaseManager.instance.equipList5[selected_Slot] = _item;
                break;
            case 6:
                DatabaseManager.instance.equipList6[selected_Slot] = _item;
                break;
            case 7:
                DatabaseManager.instance.equipList7[selected_Slot] = _item;
                break;
        }

    }


    public void ClearEquiq()
    {
        Color color = item_Slot_Image[0].color;
        color.a = 0f;

        for (int i = 0; i < item_Slot_Image.Length; i++)
        {
            item_Slot_Image[i].sprite = null;
            item_Slot_Image[i].color = color;
        }
    }

    public void ShowEquip()
    {
        ClearEquiq();

        Color color = item_Slot_Image[0].color;
        color.a = 1f;
        for (int i = 0; i < item_Slot_Image.Length; i++)
        {
            if (equipItemList[i].itemID != 0)
            {
                item_Slot_Image[i].sprite = equipItemList[i].itemIcon;
                item_Slot_Image[i].color = color;
            }
        }
    }

    public void ClearUse()
    {
        Color color = use_Slot_Image[0].color;
        color.a = 0f;

        for (int i = 0; i < use_Slot_Image.Length; i++)
        {
            use_Slot_Image[i].sprite = null;
            use_Slot_Image[i].color = color;
            Useslots[i].RemoveItem();
        }
    }
    public void ShowUse()
    {
        ClearUse();
        Color color = use_Slot_Image[0].color;
        color.a = 1f;
        for (int i = 0; i < Useslots.Length; i++)
        {
            if (useitemList[i].itemID != 0)
            {
                use_Slot_Image[i].sprite = useitemList[i].itemIcon;
                use_Slot_Image[i].color = color;
                Useslots[i].AddItem(useitemList[i]);
            }
        }
    }

    public void EquipEffect(Item item)
    {
        DatabaseManager.instance.GetCharacterStat(character_index, item);

    }
    public void TakeOffEffect(Item item)
    {
        Item _item = new Item(item.itemID, item.itemName, item.itemDescription, item.itemType, 0, -item._HP, -item._MP, -item._PhysicalAttackPower, -item._MagicAttackPower,
            -item._PhysicalArmor, -item._MagicArmorPower, -item._NormalAttackRange, -item._NormalAttackSpeed, -item.__MoveSpeed, -item._ciritical, -item._HP_Regenerate, -item._MP_Regenerate);

        DatabaseManager.instance.GetCharacterStat(character_index, _item);
    }

    public void TakeOffItem()   // equip -> inven
    {
        if (equipItemList[selected_Slot].itemID == 0) return;

        TakeOffEffect(equipItemList[selected_Slot]);

        slots[selected_Slot].RemoveItem();

        inven.EquipToInventory(equipItemList[selected_Slot]);

        Description_Text.text = null;
        itemImage.sprite = Resources.Load("ItemIcon/Inven/0", typeof(Sprite)) as Sprite;
        item_name.text = null;

        equipItemList[selected_Slot] = new Item(0, "", "", Item.ItemType.Equip);


        switch (character_index)
        {
            case 0:
                DatabaseManager.instance.equipList0[selected_Slot] = new Item(0, "", "", Item.ItemType.Equip);
                break;
            case 1:
                DatabaseManager.instance.equipList1[selected_Slot] = new Item(0, "", "", Item.ItemType.Equip);
                break;
            case 2:
                DatabaseManager.instance.equipList2[selected_Slot] = new Item(0, "", "", Item.ItemType.Equip);
                break;
            case 3:
                DatabaseManager.instance.equipList3[selected_Slot] = new Item(0, "", "", Item.ItemType.Equip);
                break;
            case 4:
                DatabaseManager.instance.equipList4[selected_Slot] = new Item(0, "", "", Item.ItemType.Equip);
                break;
            case 5:
                DatabaseManager.instance.equipList5[selected_Slot] = new Item(0, "", "", Item.ItemType.Equip);
                break;
            case 6:
                DatabaseManager.instance.equipList6[selected_Slot] = new Item(0, "", "", Item.ItemType.Equip);
                break;
            case 7:
                DatabaseManager.instance.equipList7[selected_Slot] = new Item(0, "", "", Item.ItemType.Equip);
                break;
        }
        ClearEquiq();

        ShowEquip();
    }
    public void ShopOn()
    {
        SoundManager.instance.Play(37);

        shop_Icon.material = Resources.Load("Material/kogi") as Material;
        inven_Icon.material = null;
        for (int i = 0; i < shop.Length; i++)
        {
            shop[i].SetActive(true);
        }
        for (int i = 0; i < equip.Length; i++)
        {
            equip[i].SetActive(false);
        }
    }
    public void EquipOn()
    {
        SoundManager.instance.Play(37);

        inven_Icon.material = Resources.Load("Material/kogi") as Material;
        shop_Icon.material = null;
        for (int i = 0; i < shop.Length; i++)
        {
            shop[i].SetActive(false);
        }
        for (int i = 0; i < equip.Length; i++)
        {
            equip[i].SetActive(true);

            switch (inven.selectedTab)
            {
                case 0://열려있는게 장비창인경우
                    inven.EquipTab();
                    break;
                case 1://열려있는게 소비창인경우
                    inven.UseTab();
                    break;

            }
        }
    }
    public void ShopButtonClick(int i)
    {
        SoundManager.instance.Play(38);


        selectedButton = Shop_Slot_Image[i].gameObject;
        selected_Slot = i;

        Description_Text.text = ShopList[i + flag * shopSlots.Length].itemDescription;
        itemImage.sprite = ShopList[i + flag * shopSlots.Length].itemIcon;
        item_name.text = ShopList[i + flag * shopSlots.Length].itemName;
        price.text = "-" + ShopList[i + flag * shopSlots.Length].money.ToString();
        price.color = Color.red;
        IsShopping = true;
    }
    private void ShowShop()
    {
        for (int i = 0; i < shopSlots.Length; i++)
        {
            if (i + flag * shopSlots.Length < ShopList.Count)
            {
                shopSlots[i].gameObject.SetActive(true);
                switch (ShopList[i + flag * shopSlots.Length]._Rare)
                {
                    case 0:
                        shopSlots[i].gameObject.GetComponent<Image>().color = white;
                        break;
                    case 1:
                        shopSlots[i].gameObject.GetComponent<Image>().color = Blue;
                        break;
                    case 2:
                        shopSlots[i].gameObject.GetComponent<Image>().color = Purple;
                        break;
                    case 3:
                        shopSlots[i].gameObject.GetComponent<Image>().color = Orange;
                        break;
                }
                shopSlots[i].AddItem(ShopList[i + flag * shopSlots.Length]);
            }
        }
        Money.text = DatabaseManager.instance.Money.ToString();
    }
    private void RemoveShop()
    {
        for (int i = 0; i < shopSlots.Length; i++)
        {
            shopSlots[i].RemoveItem();
            shopSlots[i].gameObject.SetActive(false);
        }
    }
    public void ShopLeft()
    {
        if (flag <= 0)
        {
            SoundManager.instance.Play(36);
            return;
        }
        else
        {
            SoundManager.instance.Play(37);

            flag--;

            for (int i = 0; i < shopSlots.Length; i++)
            {
                shopSlots[i].enabled = true;
            }

            RemoveShop();
        }
    }
    public void ShopRight()
    {

        if (flag >= (ShopList.Count - 1) / shopSlots.Length)
        {
            SoundManager.instance.Play(36);
            return;
        }
        else
        {
            SoundManager.instance.Play(37);
            flag++;
            RemoveShop();
        }
    }

    public void BuyCountDown()
    {
        if (buy_count <= 1)
        {
            SoundManager.instance.Play(36);
            buy_count = 1;
            return;
        }

        else
        {
            buy_count--;
        }
        Buy_count_text.text = buy_count.ToString();
    }

    public void BuyCountUp()
    {
        int temp = buy_count;
        if ((ShopList[selected_Slot + flag * shopSlots.Length].money * (++temp)) > DatabaseManager.instance.Money)
        {
            SoundManager.instance.Play(36);
            return;
        }
        else
        {
            buy_count++;
        }
        Buy_count_text.text = buy_count.ToString();
    }

    public void BuyCancel()
    {
        buy_count = 1;
        Buy_count_text.text = buy_count.ToString();
        BuyAskPannel.SetActive(false);
    }

    public void BuyMin()
    {
        buy_count = 1;
        Buy_count_text.text = buy_count.ToString();
    }

    public void BuyMax()
    {
        while (true)
        {
            buy_count++;
            if (ShopList[selected_Slot + flag * shopSlots.Length].money * buy_count > DatabaseManager.instance.Money)
            {
                SoundManager.instance.Play(36);
                buy_count--;
                break;
            }
        }

        Buy_count_text.text = buy_count.ToString();
    }

    public void BuyOK()
    {

        if (DatabaseManager.instance.Money >= ShopList[selected_Slot + flag * shopSlots.Length].money)
        {
            DatabaseManager.instance.Money -= (ShopList[selected_Slot + flag * shopSlots.Length].money * buy_count);
            SoundManager.instance.Play(39);

            //데이터베이스에 넣으면 끝
            //Item temp = DatabaseManager.instance.itemList.Find(item => item == ShopList[selected_Slot + flag * shopSlots.Length]);


            bool isOK = false;
            //비교해서 원래 있던 아이의 개수만 늘려주기
            for (int i = 0; i < DatabaseManager.instance.itemList.Count; i++)
            {
                if (DatabaseManager.instance.itemList[i].itemID == ShopList[selected_Slot + flag * shopSlots.Length].itemID)
                {
                    DatabaseManager.instance.itemList[i].itemCount += buy_count;//있으면
                    isOK = true;
                    break;
                }
            }
            if (!isOK)
            {
                Item _itemtemp = (Item)ShopList[selected_Slot + flag * shopSlots.Length].Clone();
                _itemtemp.itemCount = buy_count;
                if (_itemtemp.itemType == Item.ItemType.Equip)
                {
                    UpLoadData.item_is_gained[_itemtemp.itemID - 10000] = true;
                }
                else if (_itemtemp.itemType == Item.ItemType.Use)
                {
                    UpLoadData.item_is_gained[_itemtemp.itemID - 19900] = true;
                }
                else if (_itemtemp.itemType == Item.ItemType.ETC)
                {
                    UpLoadData.item_is_gained[_itemtemp.itemID - 29800] = true;
                }
                DatabaseManager.instance.itemList.Add(_itemtemp);
                UpLoadData.new_item_count++;

                isOK = false;
            }
        }

        buy_count = 1;
        Buy_count_text.text = buy_count.ToString();
        BuyAskPannel.SetActive(false);
    }

    public void CloseWarning()
    {
        warning_pannel.SetActive(false);
    }


}
