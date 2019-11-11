using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    // 아이템 고유 ID값.
    public int itemID=0;  //0인경우는 없다고 처리
    // 아이템의 이름
    public string itemName;
    // 아이템 설명
    public string itemDescription;
    // 소지 개수
    public int itemCount;
    // 아이템의 아이콘
    public Sprite itemIcon;
    // 아이템 타입
    public ItemType itemType;
    //얼마
    public int money;
    public enum ItemType
    {
        Use,
        Equip,
        Quest,
        ETC
    }

    public float _HP;
    public float _MP;
    public float _PhysicalAttackPower;
    public float _MagicAttackPower;
    public float _PhysicalArmor;
    public float _MagicArmorPower;
    public float _NormalAttackRange;
    public float _NormalAttackSpeed;
    public float __MoveSpeed;
    public float _ciritical;
    public float _HP_Regenerate;
    public float _MP_Regenerate;
    public int _Rare;
    public float _Cooltime;
    public float _drop_chance;

    public object Clone()
    {
        Item _item = new Item(0, "", "", ItemType.Use);
        _item.itemID = this.itemID;
        _item.itemName = this.itemName;
        _item.itemDescription = this.itemDescription;
        _item.itemCount = this.itemCount;
        _item.itemIcon = this.itemIcon;
        _item.itemType = this.itemType;
        _item.money = this.money;

        _item._HP = this._HP;
        _item._MP = this._MP;
        _item._PhysicalAttackPower = this._PhysicalAttackPower;
        _item._MagicAttackPower = this._MagicAttackPower;
        _item._PhysicalArmor = this._PhysicalArmor;
        _item._MagicArmorPower = this._MagicArmorPower;
        _item._NormalAttackRange = this._NormalAttackRange;
        _item._NormalAttackSpeed = this._NormalAttackSpeed;
        _item.__MoveSpeed = this.__MoveSpeed;
        _item._ciritical = this._ciritical;
        _item._HP_Regenerate = this._HP_Regenerate;
        _item._MP_Regenerate = this._MP_Regenerate;
        _item._Rare = this._Rare;
        _item._Cooltime = this._Cooltime;
        _item._drop_chance = this._drop_chance;


        return _item;
    }

    public Item(int _itemID=0, string _itemName="", string _itemDes="", ItemType _itemType=0,int _money=0, float HP = 0, float MP = 0, float PhysicalAttackPower = 0, float MagicAttackPower = 0, 
        float PhysicalArmorPower = 0, float MagicArmorPower = 0, float NormalAttackRange = 0, float NormalAttackSpeed = 0, float MoveSpeed = 0, float Critical = 0, 
        float HP_Generate=0, float MP_Generate = 0, int _itemCount = 1,float Cooltime = 0, float Drop_chance = 0,int Rare = 0)  //장비
    {
        itemID = _itemID;
        itemName = _itemName;
        itemDescription = _itemDes;
        itemType = _itemType;
        money = _money;

        _HP = HP;
        _MP = MP;
        _PhysicalAttackPower = PhysicalAttackPower;
        _MagicAttackPower = MagicAttackPower;
        _PhysicalArmor = PhysicalArmorPower;
        _MagicArmorPower = MagicArmorPower;
        _NormalAttackRange = NormalAttackRange;
        _NormalAttackSpeed = NormalAttackSpeed;
        __MoveSpeed = MoveSpeed;
        _ciritical = Critical;
        _HP_Regenerate = HP_Generate;
        _MP_Regenerate = MP_Generate;
        _Cooltime = Cooltime;
        _drop_chance = Drop_chance;
        itemCount = _itemCount;
        itemIcon = Resources.Load("ItemIcon/Inven/" + _itemID.ToString(), typeof(Sprite)) as Sprite;
        _Rare = Rare;
    }

}
