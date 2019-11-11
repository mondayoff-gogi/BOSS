using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 보스, 캐릭터 모든 데이터를 넘겨줄 것임
public static class UpLoadData
{
    public static int boss_index;
    public static int boss_level;
    public static List<bool[]> boss_usedskill_list = new List<bool[]>();
    public static List<bool[]> boss_infoskill_list = new List<bool[]>();
    public static bool[] boss_is_cleared = new bool[64];
    public static int[] character_index;
    public static int new_boss_skill_count;
    public static int new_item_count;
    // 0~99번은 Equip, 100~199번은 Use, 200~299번은 ETC
    public static bool[] item_is_gained = new bool[300];
    public static bool[] item_info_list = new bool[300];
}

public class DatabaseManager : MonoBehaviour
{
    static public DatabaseManager instance;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 40;

        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
    }  //--------------인스턴스화를 위함 ----

    // 케릭터의 체력. 
    [HideInInspector]
    public float HP = 100f;
    [HideInInspector]
    public float MaxHP = 100f;

    // 마나 
    [HideInInspector]
    public float MP = 0f;
    [HideInInspector]
    public float MaxMp = 0f;

    //물리 공격력. 
    [HideInInspector]
    public float PhysicalAttackPower = 0f;

    //마법 공격력. 
    [HideInInspector]
    public float MagicAttackPower = 0f;

    // 물리 방어력. 
    [HideInInspector]
    public float PhysicalArmor = 0f;

    //마법 방어력. 
    [HideInInspector]
    public float MagicArmorPower = 0f;

    //기본공격 사정거리
    [HideInInspector]
    public float NormalAttackRange = 0f;

    //기본공격속도
    [HideInInspector]
    public float NormalAttackSpeed = 0f;

    //이동속도
    [HideInInspector]
    public float MoveSpeed = 0f;

    //크리율
    [HideInInspector]
    public float ciritical = 0f;

    //체젠
    [HideInInspector]
    public float HP_Regenerate = 0f;

    //마젠
    [HideInInspector]
    public float MP_Regenerate = 0f;

    //아이템목록
    [HideInInspector]
    public List<Item> itemDatabase = new List<Item>();

    //상점 아이템목록
    [HideInInspector]
    public List<Item> ShopItem = new List<Item>();

    // 케릭터 소지품
    [HideInInspector]
    public List<Item> itemList = new List<Item>();

    [HideInInspector]
    public int Money;

    [HideInInspector]
    public int max_item_count = 60;

    [HideInInspector]
    public List<Item> UseList = new List<Item>();
    public List<Item> UseList0 = new List<Item>();
    public List<Item> UseList1 = new List<Item>();
    public List<Item> UseList2 = new List<Item>();
    public List<Item> UseList3 = new List<Item>();
    public List<Item> UseList4 = new List<Item>();
    public List<Item> UseList5 = new List<Item>();
    public List<Item> UseList6 = new List<Item>();
    public List<Item> UseList7 = new List<Item>();


    // 캐릭터가 소지하고 있는 아이템 리스트
    [HideInInspector]
    public List<Item> equipList = new List<Item>();

    public List<Item> equipList0 = new List<Item>();
    public List<Item> equipList1 = new List<Item>();
    public List<Item> equipList2 = new List<Item>();
    public List<Item> equipList3 = new List<Item>();
    public List<Item> equipList4 = new List<Item>();
    public List<Item> equipList5 = new List<Item>();
    public List<Item> equipList6 = new List<Item>();
    public List<Item> equipList7 = new List<Item>();

    [HideInInspector]
    public float VolumeSettingValue;


    [HideInInspector]
    public bool JoyStickUse;

    [HideInInspector]
    public int skillpos;

    // 캐릭터용
    public float[] Char_HP;
    public float[] Char_MaxHP;
    public float[] Char_MP;
    public float[] Char_MaxMp;
    public float[] Char_PhysicalAttackPower;
    public float[] Char_MagicAttackPower;
    public float[] Char_PhysicalArmor;
    public float[] Char_MagicArmorPower;
    public float[] Char_NormalAttackRange;
    public float[] Char_NormalAttackSpeed;
    public float[] Char_MoveSpeed;
    public float[] Char_ciritical;
    public float[] Char_HP_Regenerate;
    public float[] Char_MP_Regenerate;

    public List<List<Item>> boss_drop_item = new List<List<Item>>();

    private List<Item> Boss1Item = new List<Item>();
    private List<Item> Boss2Item = new List<Item>();
    private List<Item> Boss3Item = new List<Item>();
    private List<Item> Boss4Item = new List<Item>();
    private List<Item> Boss5Item = new List<Item>();
    private List<Item> Boss6Item = new List<Item>();
    private List<Item> Boss7Item = new List<Item>();
    private List<Item> Boss8Item = new List<Item>();
    private List<Item> Boss9Item = new List<Item>();
    private List<Item> Boss10Item = new List<Item>();
    private List<Item> Boss11Item = new List<Item>();
    private List<Item> Boss12Item = new List<Item>();
    private List<Item> Boss13Item = new List<Item>();
    private List<Item> Boss14Item = new List<Item>();
    private List<Item> Boss15Item = new List<Item>();
    private List<Item> Boss16Item = new List<Item>();
    private List<Item> Boss17Item = new List<Item>();
    private List<Item> Boss18Item = new List<Item>();
    private List<Item> Boss19Item = new List<Item>();
    private List<Item> Boss20Item = new List<Item>();
    private List<Item> Boss21Item = new List<Item>();
    private List<Item> Boss22Item = new List<Item>();
    private List<Item> Boss23Item = new List<Item>();
    private List<Item> Boss24Item = new List<Item>();
    private List<Item> Boss25Item = new List<Item>();
    private List<Item> Boss26Item = new List<Item>();
    private List<Item> Boss27Item = new List<Item>();
    private List<Item> Boss28Item = new List<Item>();
    private List<Item> Boss29Item = new List<Item>();
    private List<Item> Boss30Item = new List<Item>();
    private List<Item> Boss31Item = new List<Item>();
    private List<Item> Boss32Item = new List<Item>();
    private List<Item> Boss33Item = new List<Item>();
    private List<Item> Boss34Item = new List<Item>();
    private List<Item> Boss35Item = new List<Item>();
    private List<Item> Boss36Item = new List<Item>();
    private List<Item> Boss37Item = new List<Item>();
    private List<Item> Boss38Item = new List<Item>();
    private List<Item> Boss39Item = new List<Item>();
    private List<Item> Boss40Item = new List<Item>();
    private List<Item> Boss41Item = new List<Item>();
    private List<Item> Boss42Item = new List<Item>();
    private List<Item> Boss43Item = new List<Item>();
    private List<Item> Boss44Item = new List<Item>();
    private List<Item> Boss45Item = new List<Item>();
    private List<Item> Boss46Item = new List<Item>();
    private List<Item> Boss47Item = new List<Item>();
    private List<Item> Boss48Item = new List<Item>();
    private List<Item> Boss49Item = new List<Item>();
    private List<Item> Boss50Item = new List<Item>();
    private List<Item> Boss51Item = new List<Item>();
    private List<Item> Boss52Item = new List<Item>();
    private List<Item> Boss53Item = new List<Item>();
    private List<Item> Boss54Item = new List<Item>();
    private List<Item> Boss55Item = new List<Item>();
    private List<Item> Boss56Item = new List<Item>();
    private List<Item> Boss57Item = new List<Item>();
    private List<Item> Boss58Item = new List<Item>();
    private List<Item> Boss59Item = new List<Item>();
    private List<Item> Boss60Item = new List<Item>();
    private List<Item> Boss61Item = new List<Item>();
    private List<Item> Boss62Item = new List<Item>();
    private List<Item> Boss63Item = new List<Item>();
    private List<Item> Boss64Item = new List<Item>();

    public void GetCharacterStat(int char_num, Item item)
    {
        Char_HP[char_num] += item._HP;
        Char_MaxHP[char_num] += item._HP;
        Char_MP[char_num] += item._MP;
        Char_MaxMp[char_num] += item._MP;
        Char_PhysicalAttackPower[char_num] += item._PhysicalAttackPower;
        Char_MagicAttackPower[char_num] += item._MagicAttackPower;
        Char_PhysicalArmor[char_num] += item._PhysicalArmor;
        Char_MagicArmorPower[char_num] += item._MagicArmorPower;
        Char_NormalAttackRange[char_num] += item._NormalAttackRange;
        Char_NormalAttackSpeed[char_num] += item._NormalAttackSpeed;
        Char_MoveSpeed[char_num] += item.__MoveSpeed;
        Char_ciritical[char_num] += item._ciritical;
        Char_HP_Regenerate[char_num] += item._HP_Regenerate;
        Char_MP_Regenerate[char_num] += item._MP_Regenerate;
    }

    public void Init_char_stat(int char_num)
    {
        HP = Char_HP[char_num];
        MaxHP = Char_HP[char_num];
        MP = Char_MP[char_num];
        MaxMp = Char_MP[char_num];
        PhysicalAttackPower = Char_PhysicalAttackPower[char_num];
        MagicAttackPower = Char_MagicAttackPower[char_num];
        PhysicalArmor = Char_PhysicalArmor[char_num];
        MagicArmorPower = Char_MagicArmorPower[char_num];
        NormalAttackRange = Char_NormalAttackRange[char_num];
        NormalAttackSpeed = Char_NormalAttackSpeed[char_num];
        MoveSpeed = Char_MoveSpeed[char_num];
        ciritical = Char_ciritical[char_num];
        HP_Regenerate = Char_HP_Regenerate[char_num];
        MP_Regenerate = Char_MP_Regenerate[char_num];
    }

    public void EquipSort(int char_num)
    {
        switch (char_num)
        {
            case 0:
                equipList = equipList0;
                UseList = UseList0;
                break;
            case 1:
                equipList = equipList1;
                UseList = UseList1;
                break;
            case 2:
                equipList = equipList2;
                UseList = UseList2;
                break;
            case 3:
                equipList = equipList3;
                UseList = UseList3;
                break;
            case 4:
                equipList = equipList4;
                UseList = UseList4;
                break;
            case 5:
                equipList = equipList5;
                UseList = UseList5;
                break;
            case 6:
                equipList = equipList6;
                UseList = UseList6;
                break;
            case 7:
                equipList = equipList7;
                UseList = UseList7;
                break;
        }
    }


    private void Start()
    {

        Screen.SetResolution(1920, 1080, true);

        //for(int i = 0; i < UpLoadData.item_is_gained.Length; i++)   // 테스트용
        //{
        //    UpLoadData.item_is_gained[i] = true;
        //}

        if (ES3.KeyExists("BossClear") == false)
        {
            SetDefaultSetting();
            SaveData();
        }
        else
        {
            LoadData();
        }


        Char_HP = new float[8];
        Char_MaxHP = new float[8];
        Char_MP = new float[8];
        Char_MaxMp = new float[8];
        Char_PhysicalAttackPower = new float[8];
        Char_MagicAttackPower = new float[8];
        Char_PhysicalArmor = new float[8];
        Char_MagicArmorPower = new float[8];
        Char_NormalAttackRange = new float[8];
        Char_NormalAttackSpeed = new float[8];
        Char_MoveSpeed = new float[8];
        Char_ciritical = new float[8];
        Char_HP_Regenerate = new float[8];
        Char_MP_Regenerate = new float[8];

        // 캐릭터 스텟 초기화
        Init_CharBase();

        //전체 아이템들-------------------------//
        // 장비 드롭 확률 - 일반 : 30, 희귀 : 10, 영웅 : 3, 전설 : 0.5
        // 장비 값어치 - 일반 : 5, 희귀 : 15, 영웅 : 50, 전설 : 100

        //10000~10199 무기//
        itemDatabase.Add(new Item(10001, "수련용 검", "기본적인 검으로써 모험을 시작하기에 알맞은 장비이다.", Item.ItemType.Equip, 10, 0, 0, 50, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 30f));
        itemDatabase.Add(new Item(10002, "수련용 활", "기본적인 활으로써 사거리를 늘려주는 효과가 있다.", Item.ItemType.Equip, 10, 0, 0, 30, 0, 0, 0, 0.5f, 0, 0, 0, 0, 0, 1, 0, 30f));
        itemDatabase.Add(new Item(10003, "수련용 단검", "기본적인 단검로써 공격력은 약하지만 빠르게 공격할 수 있다..", Item.ItemType.Equip, 10, 0, 0, 25, 0, 0, 0, 0, 0.5f, 0, 0, 0, 0, 1, 0, 30f));
        itemDatabase.Add(new Item(10004, "수련용 지팡이", "기본적인 지팡이로써 마법 공격력을 증가시켜준다.", Item.ItemType.Equip, 10, 0, 0, 0, 50f, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 30f));
        itemDatabase.Add(new Item(10005, "수련용 망치", "기본적인 망치로써 공격력을 크게 늘려주지만 느리게 공격한다.", Item.ItemType.Equip, 10, 0, 0, 60, 0, 0, 0, 0, -0.3f, 0, 0, 0, 0, 1, 0, 30f));
        itemDatabase.Add(new Item(10006, "수련용 창", "기본적인 창로써 사거리가 늘어난다는 장점이 있다.", Item.ItemType.Equip, 10, 0, 0, 45, 0, 0, 0, 0.3f, 0, 0, 0, 0, 0, 1, 0, 30f));

        //10200~10299 방어구//
        itemDatabase.Add(new Item(10007, "나무방패", "기초적인 나무로 만든 튼튼한 방패이다.", Item.ItemType.Equip, 10, 0, 0, 0, 0, 15, 0, 0, 0, 0, 0, 0, 0, 1, 0, 30f));
        itemDatabase.Add(new Item(10008, "가죽갑옷", "가죽을 덧대어 만든 값옷이다.", Item.ItemType.Equip, 10, 0, 0, 0, 0, 20, 15, 0, 0, 0.5f, 0, 0, 0, 0, 0, 30f));
        itemDatabase.Add(new Item(10009, "구리갑옷", "제일 값싼 구리로 만든 기본적인 갑옷이다.", Item.ItemType.Equip, 10, 0, 0, 0, 0, 30, 0, 0, 0, -0.5f, 0, 0, 0, 1, 0, 30f));
        itemDatabase.Add(new Item(10010, "천갑옷", "싸구려 천으로 기워 만든 갑옷이다.", Item.ItemType.Equip, 10, 0, 0, 0, 0, 15f, 15f, 0, 0, 0, 0, 0, 0, 1, 0, 30f));
        itemDatabase.Add(new Item(10011, "털모자", "뜨게질로 만든 투구로써 최대체력을 증가시켜준다.", Item.ItemType.Equip, 10, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 30f));
        itemDatabase.Add(new Item(10012, "가죽장갑", "가죽으로 만든 장갑이다. 공격속도를 증가시켜준다.", Item.ItemType.Equip, 10, 0, 0, 0, 0, 0, 5, 5, 0, 0.3f, 0, 0, 0, 1, 0, 30f));
        itemDatabase.Add(new Item(10013, "누더기장화", "누더기 장화이다. 방어능력은 낮지만 이동속도를 증가시켜준다.", Item.ItemType.Equip, 10, 0, 0, 0, 0, 5, 5, 0, 0, 0.3f, 0, 0, 0, 1, 0, 30f));
        itemDatabase.Add(new Item(10014, "백금반지", "백금으로 만든 반지로써 체력 회복량을 증가시킨다.", Item.ItemType.Equip, 10, 100, 0, 0, 0, 0, 0, 0, 0.3f, 0.3f, 0, 30f, 0, 1, 0, 30f));
        itemDatabase.Add(new Item(10015, "팬던트", "기본 팬던트로써 마나 회복량을 증가시킨다.", Item.ItemType.Equip, 10, 100, 0, 0, 0, 0, 0, 0, 0, 0.3f, 0, 0, 5f, 1, 0, 30f));
        itemDatabase.Add(new Item(10016, "사파이어보석", "크리티컬률을 증가시켜주는 신비한 보석이다.", Item.ItemType.Equip, 10, 100, 0, 0, 0, 0, 0, 0, 0.3f, 0, 0.05f, 0, 5f, 1, 0, 30f));

        //itemDatabase.Add(new Item(10003, "test", "test.", Item.ItemType.Equip, 777));
        //itemDatabase.Add(new Item(10004, "2", "2.", Item.ItemType.Equip, 777));
        //itemDatabase.Add(new Item(10005, "3", "3.", Item.ItemType.Equip, 777));
        //itemDatabase.Add(new Item(10006, "4", "4.", Item.ItemType.Equip, 777));

        itemDatabase.Add(new Item(20001, "Red Potion1", "HP +1000, 2G", Item.ItemType.Use, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 60f, 30f));
        itemDatabase.Add(new Item(20002, "Red Potion2", "HP +2000, 5G", Item.ItemType.Use, 50, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 60f, 25f));
        itemDatabase.Add(new Item(20003, "Red Potion3", "HP +3000, 10G", Item.ItemType.Use, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 60f, 20f));
        itemDatabase.Add(new Item(20004, "Red Potion4", "HP +4000, 20G", Item.ItemType.Use, 200, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 60f, 15f));
        itemDatabase.Add(new Item(20005, "Fresh Meat", "HP +5000, 40G", Item.ItemType.Use, 500, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 60f, 10f));
        itemDatabase.Add(new Item(20006, "Blue Potion1", "MP +1000, 10G", Item.ItemType.Use, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 60, 10f));
        itemDatabase.Add(new Item(20007, "Blue Potion2", "MP +2000, 20G", Item.ItemType.Use, 50, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 60, 10f));
        itemDatabase.Add(new Item(20008, "Blue Potion3", "MP +3000, 40G", Item.ItemType.Use, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 60, 10f));
        itemDatabase.Add(new Item(20009, "Blue Potion4", "MP +4000, 80G", Item.ItemType.Use, 200, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 60, 10f));
        itemDatabase.Add(new Item(20010, "Cheese", "MP +5000, 160G", Item.ItemType.Use, 160, 500, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 60, 10f));
        itemDatabase.Add(new Item(20011, "Strength Potion", "Phy.Attack +100% ,duration 15s, cooltime 120s, 400G", Item.ItemType.Use, 400, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 120, 10f));
        itemDatabase.Add(new Item(20012, "MagicPower Potion", "Magic.Attack +100% ,duration 15s, cooltime 120s, 400G", Item.ItemType.Use, 400, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 120, 10f));
        itemDatabase.Add(new Item(20013, "Armor Potion", "Phy.Magic Armor +100% ,duration 15s, cooltime 120s, 200G", Item.ItemType.Use, 200, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 120, 10f));
        itemDatabase.Add(new Item(20014, "Speed Potion", "MoveSpeed +50%, Attackpeed +100% ,duration 15s, cooltime 120s, 200G", Item.ItemType.Use, 200, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 120, 10f));
        itemDatabase.Add(new Item(20015, "Critical Potion", "Critical +40 ,duration 15s, cooltime 120s, 400G", Item.ItemType.Use, 400, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 120, 10f));
        itemDatabase.Add(new Item(20016, "HpRegenerate Potion", "HP_Regenerate MaxHP +30%, duration 15s, cooltime 120s, 400G", Item.ItemType.Use, 400, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 120, 10f));
        itemDatabase.Add(new Item(20017, "MpRegenerate Potion", "MP_Regenerate MaxMP +30%, duration 15s, cooltime 120s, 800G", Item.ItemType.Use, 800, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 120, 10f));
        itemDatabase.Add(new Item(20018, "Invincible Potion", "Make User Invincible,duration 3s, cooltime 120s, 1200G", Item.ItemType.Use, 1200, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 120, 10f));

        itemDatabase.Add(new Item(30001, "고문서", "알 수 없는 언어로 적혀진 고문서이다.", Item.ItemType.ETC, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 50));      // 1원, 희귀, 50퍼
        itemDatabase.Add(new Item(30002, "뼈다귀", "파라오가 사라진 자리에서 나온 조각이다.", Item.ItemType.ETC, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 40));
        itemDatabase.Add(new Item(30003, "생선조각", "오아시스가 파라오의 분노에의해 말라버린후 죽은 물고기의 시체이다.", Item.ItemType.ETC, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 45));
        itemDatabase.Add(new Item(30004, "바위조각", "드워프가 소환한 바위에서 떨어져 나온 조각이다.", Item.ItemType.ETC, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 50));
        itemDatabase.Add(new Item(30005, "황토버섯", "채식주의자 드워프가 제일 좋아하는 간식이다.", Item.ItemType.ETC, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 45));
        itemDatabase.Add(new Item(30006, "당근", "오이를 싫어하는 드워프는 당근으로 수분을 충전한다.", Item.ItemType.ETC, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 40));
        itemDatabase.Add(new Item(30007, "메두사의\n이빨", "죽고난 메두사에게서 나온 이빨이다.", Item.ItemType.ETC, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 50));
        itemDatabase.Add(new Item(30008, "메두사의\n독", "쓸모있을것 같지만 생각보다 쓸모 없는 치명적인 독이다.", Item.ItemType.ETC, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 45));
        itemDatabase.Add(new Item(30009, "메두사의\n눈", "이 눈에 마주치면 돌이되어 죽는다. 간담이 서늘하다.", Item.ItemType.ETC, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 40));
        itemDatabase.Add(new Item(30010, "돌덩이\n조각", "골렘의 몸에더 뜯겨나온 조각. 룬문자가 적혀져있지만 내용은 알 수 없다.", Item.ItemType.ETC, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 50));
        itemDatabase.Add(new Item(30011, "핑핑이집", "골렘의 친구였던 핑핑이가 죽고남긴 그의 집이다.", Item.ItemType.ETC, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 45));
        itemDatabase.Add(new Item(30012, "이끼섞인\n잡초", "골렘의 몸을 뒤덮던 이끼와 잡초이다. 골렘의 몸을 무르게했지만 덕분에 골렘은 더위를 피할 수 있었다고 한다.", Item.ItemType.ETC, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 40));
        itemDatabase.Add(new Item(30013, "군인복무\n규율", "과거 최정예 군인이였던 X-23이 가슴속 깊이 보관하는 수첩이다.", Item.ItemType.ETC, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 50));
        itemDatabase.Add(new Item(30014, "비상식량", "군에서 보급된 것은 맛없어서 버리고 생선을 말려 소지하고다녔다.", Item.ItemType.ETC, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 45));
        itemDatabase.Add(new Item(30015, "가족의편지", "꼭 돌아와 달라는 애절한 내용이 적힌 편지. 임무후 여행을 떠나자던 아내의 문구에 얼룩이 져있다.", Item.ItemType.ETC, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 40));
        itemDatabase.Add(new Item(30016, "집게발", "용왕을 수호하던 집게대신의 집게발이다. 살이 꽉차있어 맛있을듯 하다.", Item.ItemType.ETC, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 50));
        itemDatabase.Add(new Item(30017, "문어발", "용왕의 여러 팔중 하나의 조각이다. 꿈틀대는것이 아직 살아있는 듯 하다.", Item.ItemType.ETC, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 45));
        itemDatabase.Add(new Item(30018, "용왕의\n비늘", "용왕을 보호하던 비늘이다. 매우 반짝이고 아름답다.", Item.ItemType.ETC, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 40));
        itemDatabase.Add(new Item(30019, "보관용\n병", "급하게 피가 필요할 때를 위해 피를 보관하기 위한 병이다.", Item.ItemType.ETC, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 50));
        itemDatabase.Add(new Item(30020, "의식용\n해골", "어둠의 의식을 거행하기 위한 제물이다. 소문에의하면 그의 옛 연인의 두개골이라고...", Item.ItemType.ETC, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 45));
        itemDatabase.Add(new Item(30021, "성경책", "뱀파이어는 종교를 싫어하지만, 지피지기의 신념으로 그가 들고다니던 성경책.", Item.ItemType.ETC, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 40));
        itemDatabase.Add(new Item(30022, "버킷리스트", "인간으로 환생하면 할 것들을 빼곡히 적은 리스트. 1순위는 연애라고 적혀있다.", Item.ItemType.ETC, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 50));
        itemDatabase.Add(new Item(30023, "손거울", "거울에 비치지 않는 자신의 모습을 볼 때마다 자신이 귀신임을 자각했다.", Item.ItemType.ETC, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 45));
        itemDatabase.Add(new Item(30024, "마법서", "다시 인간으로 환생하기위한 의식의 절차들이 적혀있는 마법서이다.", Item.ItemType.ETC, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 40));
        //20000~29999 소모품//


        //30000~39999 ETC  //

        //소지 아이템들-------------------------//

        //List<Item> Copy = new List<Item>(itemDatabase);

        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());
        //itemList.Add((Item)itemDatabase.Find(item => item.itemID == 10001).Clone());


        //소지 아이템들 끝----------------------//


        //전체 아이템들 끝------------------------//


        Init_Shop_Item(); //상점 리스트


        InitiateBossItem();

        for (int i = 0; i < equipList.Count; i++)
        {
            if (equipList0[i].itemID != 0)
            {
                GetCharacterStat(0, equipList0[i]);
            }
        }
        for (int i = 0; i < equipList.Count; i++)
        {
            if (equipList1[i].itemID != 0)
            {
                GetCharacterStat(1, equipList0[i]);
            }
        }
        for (int i = 0; i < equipList.Count; i++)
        {
            if (equipList2[i].itemID != 0)
            {
                GetCharacterStat(2, equipList0[i]);
            }
        }
        for (int i = 0; i < equipList.Count; i++)
        {
            if (equipList3[i].itemID != 0)
            {
                GetCharacterStat(3, equipList0[i]);
            }
        }
        for (int i = 0; i < equipList.Count; i++)
        {
            if (equipList4[i].itemID != 0)
            {
                GetCharacterStat(4, equipList0[i]);
            }
        }
        for (int i = 0; i < equipList.Count; i++)
        {
            if (equipList5[i].itemID != 0)
            {
                GetCharacterStat(5, equipList0[i]);
            }
        }
        for (int i = 0; i < equipList.Count; i++)
        {
            if (equipList6[i].itemID != 0)
            {
                GetCharacterStat(6, equipList0[i]);
            }
        }
        for (int i = 0; i < equipList.Count; i++)
        {
            if (equipList7[i].itemID != 0)
            {
                GetCharacterStat(7, equipList0[i]);
            }
        }

    }

    private void Init_CharBase()
    {
        //Assassin
        Char_HP[0] = 1000f;
        Char_MaxHP[0] = 1000f;
        Char_MP[0] = 1000f;
        Char_MaxMp[0] = 1000f;
        Char_PhysicalAttackPower[0] = 90f;
        Char_MagicAttackPower[0] = 60f;
        Char_PhysicalArmor[0] = 20f;
        Char_MagicArmorPower[0] = 20f;
        Char_NormalAttackRange[0] = 2f;
        Char_NormalAttackSpeed[0] = 3f;
        Char_MoveSpeed[0] = 4;
        Char_ciritical[0] = 50f;
        Char_HP_Regenerate[0] = 1f;
        Char_MP_Regenerate[0] = 1f;
        //Arrow
        Char_HP[1] = 1100f;
        Char_MaxHP[1] = 1100f;
        Char_MP[1] = 1200f;
        Char_MaxMp[1] = 1200f;
        Char_PhysicalAttackPower[1] = 80f;
        Char_MagicAttackPower[1] = 80f;
        Char_PhysicalArmor[1] = 15f;
        Char_MagicArmorPower[1] = 15f;
        Char_NormalAttackRange[1] = 8f;
        Char_NormalAttackSpeed[1] = 2f;
        Char_MoveSpeed[1] = 3;
        Char_ciritical[1] = 25f;
        Char_HP_Regenerate[1] = 1.5f;
        Char_MP_Regenerate[1] = 1f;
        //Fan
        Char_HP[2] = 1300f;
        Char_MaxHP[2] = 1300f;
        Char_MP[2] = 1900f;
        Char_MaxMp[2] = 1900f;
        Char_PhysicalAttackPower[2] = 60;
        Char_MagicAttackPower[2] = 80;
        Char_PhysicalArmor[2] = 30f;
        Char_MagicArmorPower[2] = 30f;
        Char_NormalAttackRange[2] = 10f;
        Char_NormalAttackSpeed[2] = 1f;
        Char_MoveSpeed[2] = 2.5f;
        Char_ciritical[2] = 0f;
        Char_HP_Regenerate[2] = 2.5f;
        Char_MP_Regenerate[2] = 4f;
        //Hammer
        Char_HP[3] = 1800f;
        Char_MaxHP[3] = 1800f;
        Char_MP[3] = 1200f;
        Char_MaxMp[3] = 1200f;
        Char_PhysicalAttackPower[3] = 100f;
        Char_MagicAttackPower[3] = 65f;
        Char_PhysicalArmor[3] = 35f;
        Char_MagicArmorPower[3] = 35f;
        Char_NormalAttackRange[3] = 3.5f;
        Char_NormalAttackSpeed[3] = 1f;
        Char_MoveSpeed[3] = 3.25f;
        Char_ciritical[3] = 0f;
        Char_HP_Regenerate[3] = 2.5f;
        Char_MP_Regenerate[3] = 2f;
        //Healer
        Char_HP[4] = 1300f;
        Char_MaxHP[4] = 1300f;
        Char_MP[4] = 2000f;
        Char_MaxMp[4] = 2000f;
        Char_PhysicalAttackPower[4] = 60f;
        Char_MagicAttackPower[4] = 85f;
        Char_PhysicalArmor[4] = 25f;
        Char_MagicArmorPower[4] = 25f;
        Char_NormalAttackRange[4] = 10f;
        Char_NormalAttackSpeed[4] = 1f;
        Char_MoveSpeed[4] = 2;
        Char_ciritical[4] = 0f;
        Char_HP_Regenerate[4] = 2f;
        Char_MP_Regenerate[4] = 5f;
        //Mage
        Char_HP[5] = 1200f;
        Char_MaxHP[5] = 1200f;
        Char_MP[5] = 1800f;
        Char_MaxMp[5] = 1800f;
        Char_PhysicalAttackPower[5] = 60f;
        Char_MagicAttackPower[5] = 100f;
        Char_PhysicalArmor[5] = 10f;
        Char_MagicArmorPower[5] = 10f;
        Char_NormalAttackRange[5] = 10f;
        Char_NormalAttackSpeed[5] = 1.25f;
        Char_MoveSpeed[5] = 2.5f;
        Char_ciritical[5] = 0f;
        Char_HP_Regenerate[5] = 2f;
        Char_MP_Regenerate[5] = 4f;
        //Shield
        Char_HP[6] = 2000f;
        Char_MaxHP[6] = 2000f;
        Char_MP[6] = 1100f;
        Char_MaxMp[6] = 1100f;
        Char_PhysicalAttackPower[6] = 65f;
        Char_MagicAttackPower[6] = 65f;
        Char_PhysicalArmor[6] = 50f;
        Char_MagicArmorPower[6] = 50f;
        Char_NormalAttackRange[6] = 2.75f;
        Char_NormalAttackSpeed[6] = 1.25f;
        Char_MoveSpeed[6] = 2.75f;
        Char_ciritical[6] = 0f;
        Char_HP_Regenerate[6] = 5f;
        Char_MP_Regenerate[6] = 1.5f;
        //Spear
        Char_HP[7] = 1600f;
        Char_MaxHP[7] = 1600f;
        Char_MP[7] = 1200f;
        Char_MaxMp[7] = 1200f;
        Char_PhysicalAttackPower[7] = 85f;
        Char_MagicAttackPower[7] = 65f;
        Char_PhysicalArmor[7] = 25f;
        Char_MagicArmorPower[7] = 25f;
        Char_NormalAttackRange[7] = 3.5f;
        Char_NormalAttackSpeed[7] = 2.0f;
        Char_MoveSpeed[7] = 3.5f;
        Char_ciritical[7] = 12.5f;
        Char_HP_Regenerate[7] = 2f;
        Char_MP_Regenerate[7] = 2f;

    }


    public void Init_Boss_Stat(int Boss_num, int level)
    {
        switch (Boss_num)
        {
            case 0:  //DesertBoss
                switch (level)
                {
                    case 0:
                        BossStatus.instance.HP = 150f; //이걸 기준으로 새롭게 바꾸어야함 기존에 23000
                        BossStatus.instance.MaxHP = 15000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 60f;
                        BossStatus.instance.PhysicalAttackPower = 300f;
                        BossStatus.instance.MagicAttackPower = 300f;
                        BossStatus.instance.PhysicalArmor = 0f;
                        BossStatus.instance.MagicArmorPower = 0f;
                        BossStatus.instance.moveSpeed = 3f;
                        break;
                    case 1:
                        BossStatus.instance.HP = 60000f;
                        BossStatus.instance.MaxHP = 60000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 80f;
                        BossStatus.instance.PhysicalAttackPower = 500f;
                        BossStatus.instance.MagicAttackPower = 500f;
                        BossStatus.instance.PhysicalArmor = 0f;
                        BossStatus.instance.MagicArmorPower = 0f;
                        BossStatus.instance.moveSpeed = 2f;
                        break;
                    case 2:
                        BossStatus.instance.HP = 100000f;
                        BossStatus.instance.MaxHP = 100000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 100f;
                        BossStatus.instance.PhysicalAttackPower = 500f;
                        BossStatus.instance.MagicAttackPower = 500f;
                        BossStatus.instance.PhysicalArmor = 0f;
                        BossStatus.instance.MagicArmorPower = 0f;
                        BossStatus.instance.moveSpeed = 2f;
                        break;
                    case 3:
                        BossStatus.instance.HP = 200000f;
                        BossStatus.instance.MaxHP = 200000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 120f;
                        BossStatus.instance.PhysicalAttackPower = 500f;
                        BossStatus.instance.MagicAttackPower = 500f;
                        BossStatus.instance.PhysicalArmor = 0f;
                        BossStatus.instance.MagicArmorPower = 0f;
                        BossStatus.instance.moveSpeed = 2f;
                        break;
                    case 4:
                        BossStatus.instance.HP = 2000f;
                        BossStatus.instance.MaxHP = 200000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 60;
                        BossStatus.instance.PhysicalAttackPower = 100f;
                        BossStatus.instance.MagicAttackPower = 100f;
                        BossStatus.instance.PhysicalArmor = 0f;
                        BossStatus.instance.MagicArmorPower = 0f;
                        BossStatus.instance.moveSpeed = 2f;
                        break;
                    case 5:
                        BossStatus.instance.HP = 200000f;
                        BossStatus.instance.MaxHP = 200000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 60;
                        BossStatus.instance.PhysicalAttackPower = 100f;
                        BossStatus.instance.MagicAttackPower = 100f;
                        BossStatus.instance.PhysicalArmor = 0f;
                        BossStatus.instance.MagicArmorPower = 0f;
                        BossStatus.instance.moveSpeed = 2f;
                        break;
                    case 6:
                        BossStatus.instance.HP = 200000f;
                        BossStatus.instance.MaxHP = 200000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 60;
                        BossStatus.instance.PhysicalAttackPower = 100f;
                        BossStatus.instance.MagicAttackPower = 100f;
                        BossStatus.instance.PhysicalArmor = 0f;
                        BossStatus.instance.MagicArmorPower = 0f;
                        BossStatus.instance.moveSpeed = 2f;
                        break;
                    case 7:
                        BossStatus.instance.HP = 200000f;
                        BossStatus.instance.MaxHP = 200000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 60;
                        BossStatus.instance.PhysicalAttackPower = 100f;
                        BossStatus.instance.MagicAttackPower = 100f;
                        BossStatus.instance.PhysicalArmor = 0f;
                        BossStatus.instance.MagicArmorPower = 0f;
                        BossStatus.instance.moveSpeed = 2f;
                        break;
                }
                break;
            case 1:  //boss_2
                switch (level)
                {
                    case 0:
                        BossStatus.instance.HP = 900f;
                        BossStatus.instance.MaxHP = 9000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 25f;
                        BossStatus.instance.PhysicalAttackPower = 400;
                        BossStatus.instance.MagicAttackPower = 200;
                        BossStatus.instance.PhysicalArmor = 20f;
                        BossStatus.instance.MagicArmorPower = -20f;
                        BossStatus.instance.moveSpeed = 2f;
                        break;
                    case 1:
                        BossStatus.instance.HP = 10000f;
                        BossStatus.instance.MaxHP = 10000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 50f;
                        BossStatus.instance.PhysicalAttackPower = 500f;
                        BossStatus.instance.MagicAttackPower = 500f;
                        BossStatus.instance.PhysicalArmor = 0f;
                        BossStatus.instance.MagicArmorPower = 0f;
                        BossStatus.instance.moveSpeed = 2f;
                        break;
                    case 2:
                        BossStatus.instance.HP = 15000f;
                        BossStatus.instance.MaxHP = 15000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 60f;
                        BossStatus.instance.PhysicalAttackPower = 500f;
                        BossStatus.instance.MagicAttackPower = 500f;
                        BossStatus.instance.PhysicalArmor = 0f;
                        BossStatus.instance.MagicArmorPower = 0f;
                        BossStatus.instance.moveSpeed = 2f;
                        break;
                    case 3:
                        BossStatus.instance.HP = 20000f;
                        BossStatus.instance.MaxHP = 20000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 120f;
                        BossStatus.instance.PhysicalAttackPower = 500f;
                        BossStatus.instance.MagicAttackPower = 500f;
                        BossStatus.instance.PhysicalArmor = 0f;
                        BossStatus.instance.MagicArmorPower = 0f;
                        BossStatus.instance.moveSpeed = 2f;
                        break;
                    case 4:
                        BossStatus.instance.HP = 200000f;
                        BossStatus.instance.MaxHP = 200000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 60;
                        BossStatus.instance.PhysicalAttackPower = 100f;
                        BossStatus.instance.MagicAttackPower = 100f;
                        BossStatus.instance.PhysicalArmor = 0f;
                        BossStatus.instance.MagicArmorPower = 0f;
                        BossStatus.instance.moveSpeed = 2f;
                        break;
                }
                break;
            case 2:  //boss_3
                switch (level)
                {
                    case 0:
                        BossStatus.instance.HP = 12500f;
                        BossStatus.instance.MaxHP = 12500f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 60f;
                        BossStatus.instance.PhysicalAttackPower = 200f;
                        BossStatus.instance.MagicAttackPower = 400f;
                        BossStatus.instance.PhysicalArmor = -20f;
                        BossStatus.instance.MagicArmorPower = 20f;
                        BossStatus.instance.moveSpeed = 3f;
                        break;
                    case 1:
                        BossStatus.instance.HP = 10000f;
                        BossStatus.instance.MaxHP = 10000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 50f;
                        BossStatus.instance.PhysicalAttackPower = 500f;
                        BossStatus.instance.MagicAttackPower = 500f;
                        BossStatus.instance.PhysicalArmor = 0f;
                        BossStatus.instance.MagicArmorPower = 0f;
                        BossStatus.instance.moveSpeed = 2f;
                        break;
                    case 2:
                        BossStatus.instance.HP = 15000f;
                        BossStatus.instance.MaxHP = 15000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 60f;
                        BossStatus.instance.PhysicalAttackPower = 500f;
                        BossStatus.instance.MagicAttackPower = 500f;
                        BossStatus.instance.PhysicalArmor = 0f;
                        BossStatus.instance.MagicArmorPower = 0f;
                        BossStatus.instance.moveSpeed = 2f;
                        break;
                    case 3:
                        BossStatus.instance.HP = 20000f;
                        BossStatus.instance.MaxHP = 20000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 120f;
                        BossStatus.instance.PhysicalAttackPower = 500f;
                        BossStatus.instance.MagicAttackPower = 500f;
                        BossStatus.instance.PhysicalArmor = 0f;
                        BossStatus.instance.MagicArmorPower = 0f;
                        BossStatus.instance.moveSpeed = 2f;
                        break;
                }
                break;
            case 3:  //boss_4
                switch (level)
                {
                    case 0:
                        BossStatus.instance.HP = 8000f;
                        BossStatus.instance.MaxHP = 8000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 25f;
                        BossStatus.instance.PhysicalAttackPower = 200f;
                        BossStatus.instance.MagicAttackPower = 200f;
                        BossStatus.instance.PhysicalArmor = 60f;
                        BossStatus.instance.MagicArmorPower = 40f;
                        BossStatus.instance.moveSpeed = 1f;
                        break;
                    case 1:
                        BossStatus.instance.HP = 10000f;
                        BossStatus.instance.MaxHP = 10000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 30f;
                        BossStatus.instance.PhysicalAttackPower = 500f;
                        BossStatus.instance.MagicAttackPower = 500f;
                        BossStatus.instance.PhysicalArmor = 0f;
                        BossStatus.instance.MagicArmorPower = 0f;
                        BossStatus.instance.moveSpeed = 2f;
                        break;
                    case 2:
                        BossStatus.instance.HP = 15000f;
                        BossStatus.instance.MaxHP = 15000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 30f;
                        BossStatus.instance.PhysicalAttackPower = 500f;
                        BossStatus.instance.MagicAttackPower = 500f;
                        BossStatus.instance.PhysicalArmor = 0f;
                        BossStatus.instance.MagicArmorPower = 0f;
                        BossStatus.instance.moveSpeed = 2f;
                        break;
                    case 3:
                        BossStatus.instance.HP = 20000f;
                        BossStatus.instance.MaxHP = 20000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 30f;
                        BossStatus.instance.PhysicalAttackPower = 500f;
                        BossStatus.instance.MagicAttackPower = 500f;
                        BossStatus.instance.PhysicalArmor = 0f;
                        BossStatus.instance.MagicArmorPower = 0f;
                        BossStatus.instance.moveSpeed = 2f;
                        break;
                }
                break;
            case 4:  //boss_5
                switch (level)
                {
                    case 0:
                        BossStatus.instance.HP = 15000f;
                        BossStatus.instance.MaxHP = 15000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 40f;
                        BossStatus.instance.PhysicalAttackPower = 300f;
                        BossStatus.instance.MagicAttackPower = 300f;
                        BossStatus.instance.PhysicalArmor = 30f;
                        BossStatus.instance.MagicArmorPower = 30f;
                        BossStatus.instance.moveSpeed = 0f;
                        break;
                    case 1:
                        BossStatus.instance.HP = 10000f;
                        BossStatus.instance.MaxHP = 10000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 30f;
                        BossStatus.instance.PhysicalAttackPower = 500f;
                        BossStatus.instance.MagicAttackPower = 500f;
                        BossStatus.instance.PhysicalArmor = 0f;
                        BossStatus.instance.MagicArmorPower = 0f;
                        BossStatus.instance.moveSpeed = 2f;
                        break;
                    case 2:
                        BossStatus.instance.HP = 15000f;
                        BossStatus.instance.MaxHP = 15000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 30f;
                        BossStatus.instance.PhysicalAttackPower = 500f;
                        BossStatus.instance.MagicAttackPower = 500f;
                        BossStatus.instance.PhysicalArmor = 0f;
                        BossStatus.instance.MagicArmorPower = 0f;
                        BossStatus.instance.moveSpeed = 2f;
                        break;
                    case 3:
                        BossStatus.instance.HP = 20000f;
                        BossStatus.instance.MaxHP = 20000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 30f;
                        BossStatus.instance.PhysicalAttackPower = 500f;
                        BossStatus.instance.MagicAttackPower = 500f;
                        BossStatus.instance.PhysicalArmor = 0f;
                        BossStatus.instance.MagicArmorPower = 0f;
                        BossStatus.instance.moveSpeed = 2f;
                        break;
                }
                break;
            case 5:  //boss_6
                switch (level)
                {
                    case 0:
                        BossStatus.instance.HP = 15000f;
                        BossStatus.instance.MaxHP = 15000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 80f;
                        BossStatus.instance.PhysicalAttackPower = 200f;
                        BossStatus.instance.MagicAttackPower = 400f;
                        BossStatus.instance.PhysicalArmor = -20f;
                        BossStatus.instance.MagicArmorPower = 20f;
                        BossStatus.instance.moveSpeed = 2f;
                        break;
                    case 1:
                        BossStatus.instance.HP = 10000f;
                        BossStatus.instance.MaxHP = 10000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 30f;
                        BossStatus.instance.PhysicalAttackPower = 500f;
                        BossStatus.instance.MagicAttackPower = 500f;
                        BossStatus.instance.PhysicalArmor = 0f;
                        BossStatus.instance.MagicArmorPower = 0f;
                        BossStatus.instance.moveSpeed = 2f;
                        break;
                    case 2:
                        BossStatus.instance.HP = 15000f;
                        BossStatus.instance.MaxHP = 15000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 30f;
                        BossStatus.instance.PhysicalAttackPower = 500f;
                        BossStatus.instance.MagicAttackPower = 500f;
                        BossStatus.instance.PhysicalArmor = 0f;
                        BossStatus.instance.MagicArmorPower = 0f;
                        BossStatus.instance.moveSpeed = 2f;
                        break;
                    case 3:
                        BossStatus.instance.HP = 20000f;
                        BossStatus.instance.MaxHP = 20000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 30f;
                        BossStatus.instance.PhysicalAttackPower = 500f;
                        BossStatus.instance.MagicAttackPower = 500f;
                        BossStatus.instance.PhysicalArmor = 0f;
                        BossStatus.instance.MagicArmorPower = 0f;
                        BossStatus.instance.moveSpeed = 2f;
                        break;
                }
                break;
            case 6:  //boss_7
                switch (level)
                {
                    case 0:
                        BossStatus.instance.HP = 10000f;
                        BossStatus.instance.MaxHP = 10000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 100f;
                        BossStatus.instance.PhysicalAttackPower = 100f;
                        BossStatus.instance.MagicAttackPower = 600f;
                        BossStatus.instance.PhysicalArmor = 40f;
                        BossStatus.instance.MagicArmorPower = -40f;
                        BossStatus.instance.moveSpeed = 1f;
                        break;
                    case 1:
                        BossStatus.instance.HP = 10000f;
                        BossStatus.instance.MaxHP = 10000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 30f;
                        BossStatus.instance.PhysicalAttackPower = 500f;
                        BossStatus.instance.MagicAttackPower = 500f;
                        BossStatus.instance.PhysicalArmor = 0f;
                        BossStatus.instance.MagicArmorPower = 0f;
                        BossStatus.instance.moveSpeed = 2f;
                        break;
                    case 2:
                        BossStatus.instance.HP = 15000f;
                        BossStatus.instance.MaxHP = 15000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 30f;
                        BossStatus.instance.PhysicalAttackPower = 500f;
                        BossStatus.instance.MagicAttackPower = 500f;
                        BossStatus.instance.PhysicalArmor = 0f;
                        BossStatus.instance.MagicArmorPower = 0f;
                        BossStatus.instance.moveSpeed = 2f;
                        break;
                    case 3:
                        BossStatus.instance.HP = 20000f;
                        BossStatus.instance.MaxHP = 20000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 30f;
                        BossStatus.instance.PhysicalAttackPower = 500f;
                        BossStatus.instance.MagicAttackPower = 500f;
                        BossStatus.instance.PhysicalArmor = 0f;
                        BossStatus.instance.MagicArmorPower = 0f;
                        BossStatus.instance.moveSpeed = 2f;
                        break;
                }
                break;
            case 7:  //boss_8
                switch (level)
                {
                    case 0:
                        BossStatus.instance.HP = 10000f;
                        BossStatus.instance.MaxHP = 10000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 50f;
                        BossStatus.instance.PhysicalAttackPower = 500;
                        BossStatus.instance.MagicAttackPower = 100f;
                        BossStatus.instance.PhysicalArmor = 40f;
                        BossStatus.instance.MagicArmorPower = -40f;
                        BossStatus.instance.moveSpeed = 3.5f;
                        break;
                    case 1:
                        BossStatus.instance.HP = 10000f;
                        BossStatus.instance.MaxHP = 10000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 60f;
                        BossStatus.instance.PhysicalAttackPower = 500f;
                        BossStatus.instance.MagicAttackPower = 500f;
                        BossStatus.instance.PhysicalArmor = 0f;
                        BossStatus.instance.MagicArmorPower = 0f;
                        BossStatus.instance.moveSpeed = 2f;
                        break;
                    case 2:
                        BossStatus.instance.HP = 15000f;
                        BossStatus.instance.MaxHP = 15000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 60f;
                        BossStatus.instance.PhysicalAttackPower = 500f;
                        BossStatus.instance.MagicAttackPower = 500f;
                        BossStatus.instance.PhysicalArmor = 0f;
                        BossStatus.instance.MagicArmorPower = 0f;
                        BossStatus.instance.moveSpeed = 2f;
                        break;
                    case 3:
                        BossStatus.instance.HP = 20000f;
                        BossStatus.instance.MaxHP = 20000f;
                        BossStatus.instance.MP = 0f;
                        BossStatus.instance.MaxMp = 60f;
                        BossStatus.instance.PhysicalAttackPower = 500f;
                        BossStatus.instance.MagicAttackPower = 500f;
                        BossStatus.instance.PhysicalArmor = 0f;
                        BossStatus.instance.MagicArmorPower = 0f;
                        BossStatus.instance.moveSpeed = 2f;
                        break;
                }
                break;
        }
        BossStatus.instance.moveSpeed /= 1.5f;
    }

    public void Init_Shop_Item()
    {
        ShopItem.Clear();

        List<Item> Copy = new List<Item>(itemDatabase);

        ShopItem.Add(Copy.Find(item => item.itemID == 20001));
        ShopItem.Add(Copy.Find(item => item.itemID == 20002));
        ShopItem.Add(Copy.Find(item => item.itemID == 20003));
        ShopItem.Add(Copy.Find(item => item.itemID == 20004));
        ShopItem.Add(Copy.Find(item => item.itemID == 20005));
        ShopItem.Add(Copy.Find(item => item.itemID == 20006));
        ShopItem.Add(Copy.Find(item => item.itemID == 20007));
        ShopItem.Add(Copy.Find(item => item.itemID == 20008));
        ShopItem.Add(Copy.Find(item => item.itemID == 20009));
        ShopItem.Add(Copy.Find(item => item.itemID == 20010));
        ShopItem.Add(Copy.Find(item => item.itemID == 20011));
        ShopItem.Add(Copy.Find(item => item.itemID == 20012));
        ShopItem.Add(Copy.Find(item => item.itemID == 20013));
        ShopItem.Add(Copy.Find(item => item.itemID == 20014));
        ShopItem.Add(Copy.Find(item => item.itemID == 20015));
        ShopItem.Add(Copy.Find(item => item.itemID == 20016));
        ShopItem.Add(Copy.Find(item => item.itemID == 20017));
        ShopItem.Add(Copy.Find(item => item.itemID == 20018));


        //ShopItem.Add(new Item(20001, "포션1", "포션1", Item.ItemType.Use, 1));
        //ShopItem.Add(new Item(20002, "포션2", "포션2", Item.ItemType.Use, 2));
        //ShopItem.Add(itemDatabase.Find(item => item.itemID == 20003));


    }

    public void InitiateBossItem()
    {
        // 8 개 이하로 넣어야함
        // 1~8 보스 1

        // 잡템
        Boss1Item.Add(itemDatabase.Find(item => item.itemID == 30001));
        Boss1Item.Add(itemDatabase.Find(item => item.itemID == 30002));
        Boss1Item.Add(itemDatabase.Find(item => item.itemID == 30003));

        // 물약
        Boss1Item.Add(itemDatabase.Find(item => item.itemID == 20001)); // 빨포1
        Boss1Item.Add(itemDatabase.Find(item => item.itemID == 20006)); // 파포1

        // 무기
        Boss1Item.Add(itemDatabase.Find(item => item.itemID == 10001));
        Boss1Item.Add(itemDatabase.Find(item => item.itemID == 10002));

        // 임시 테스트용
        Boss5Item.Add(itemDatabase.Find(item => item.itemID == 10001));
        Boss5Item.Add(itemDatabase.Find(item => item.itemID == 10002));

        Boss5Item.Add(itemDatabase.Find(item => item.itemID == 30001));
        Boss5Item.Add(itemDatabase.Find(item => item.itemID == 30002));
        Boss5Item.Add(itemDatabase.Find(item => item.itemID == 30003));

        // 물약
        Boss5Item.Add(itemDatabase.Find(item => item.itemID == 20001)); // 빨포1
        Boss5Item.Add(itemDatabase.Find(item => item.itemID == 20006)); // 파포1 

        boss_drop_item.Add(Boss1Item);
        boss_drop_item.Add(Boss2Item);
        boss_drop_item.Add(Boss3Item);
        boss_drop_item.Add(Boss4Item);
        boss_drop_item.Add(Boss5Item);
        boss_drop_item.Add(Boss6Item);
        boss_drop_item.Add(Boss7Item);
        boss_drop_item.Add(Boss8Item);

        // 9~16 보스 2

        // 잡템
        Boss9Item.Add(itemDatabase.Find(item => item.itemID == 30004));
        Boss9Item.Add(itemDatabase.Find(item => item.itemID == 30005));
        Boss9Item.Add(itemDatabase.Find(item => item.itemID == 30006));

        // 물약
        Boss9Item.Add(itemDatabase.Find(item => item.itemID == 20001)); // 빨포1
        Boss9Item.Add(itemDatabase.Find(item => item.itemID == 20006)); // 파포1

        // 무기
        Boss9Item.Add(itemDatabase.Find(item => item.itemID == 10003));
        Boss9Item.Add(itemDatabase.Find(item => item.itemID == 10004));

        boss_drop_item.Add(Boss9Item);
        boss_drop_item.Add(Boss10Item);
        boss_drop_item.Add(Boss11Item);
        boss_drop_item.Add(Boss12Item);
        boss_drop_item.Add(Boss13Item);
        boss_drop_item.Add(Boss14Item);
        boss_drop_item.Add(Boss15Item);
        boss_drop_item.Add(Boss16Item);


        // 17~24 보스 3

        // 잡템
        Boss17Item.Add(itemDatabase.Find(item => item.itemID == 30007));
        Boss17Item.Add(itemDatabase.Find(item => item.itemID == 30008));
        Boss17Item.Add(itemDatabase.Find(item => item.itemID == 30009));

        // 물약
        Boss17Item.Add(itemDatabase.Find(item => item.itemID == 20001)); // 빨포1
        Boss17Item.Add(itemDatabase.Find(item => item.itemID == 20006)); // 파포1

        // 무기
        Boss17Item.Add(itemDatabase.Find(item => item.itemID == 10005));
        Boss17Item.Add(itemDatabase.Find(item => item.itemID == 10006));

        boss_drop_item.Add(Boss17Item);
        boss_drop_item.Add(Boss18Item);
        boss_drop_item.Add(Boss19Item);
        boss_drop_item.Add(Boss20Item);
        boss_drop_item.Add(Boss21Item);
        boss_drop_item.Add(Boss22Item);
        boss_drop_item.Add(Boss23Item);
        boss_drop_item.Add(Boss24Item);

        // 25~32 보스 4

        // 잡템
        Boss25Item.Add(itemDatabase.Find(item => item.itemID == 30010));
        Boss25Item.Add(itemDatabase.Find(item => item.itemID == 30011));
        Boss25Item.Add(itemDatabase.Find(item => item.itemID == 30012));

        // 물약
        Boss25Item.Add(itemDatabase.Find(item => item.itemID == 20001)); // 빨포1
        Boss25Item.Add(itemDatabase.Find(item => item.itemID == 20006)); // 파포1

        // 무기
        Boss25Item.Add(itemDatabase.Find(item => item.itemID == 10007));
        Boss25Item.Add(itemDatabase.Find(item => item.itemID == 10008));

        boss_drop_item.Add(Boss25Item);
        boss_drop_item.Add(Boss26Item);
        boss_drop_item.Add(Boss27Item);
        boss_drop_item.Add(Boss28Item);
        boss_drop_item.Add(Boss29Item);
        boss_drop_item.Add(Boss30Item);
        boss_drop_item.Add(Boss31Item);
        boss_drop_item.Add(Boss32Item);

        // 33~40 보스 5

        // 잡템
        Boss33Item.Add(itemDatabase.Find(item => item.itemID == 30013));
        Boss33Item.Add(itemDatabase.Find(item => item.itemID == 30014));
        Boss33Item.Add(itemDatabase.Find(item => item.itemID == 30015));

        // 물약
        Boss33Item.Add(itemDatabase.Find(item => item.itemID == 20001)); // 빨포1
        Boss33Item.Add(itemDatabase.Find(item => item.itemID == 20006)); // 파포1

        // 무기
        Boss33Item.Add(itemDatabase.Find(item => item.itemID == 10009));
        Boss33Item.Add(itemDatabase.Find(item => item.itemID == 10010));

        boss_drop_item.Add(Boss33Item);
        boss_drop_item.Add(Boss34Item);
        boss_drop_item.Add(Boss35Item);
        boss_drop_item.Add(Boss36Item);
        boss_drop_item.Add(Boss37Item);
        boss_drop_item.Add(Boss38Item);
        boss_drop_item.Add(Boss39Item);
        boss_drop_item.Add(Boss40Item);

        // 41~48 보스 6

        // 잡템
        Boss41Item.Add(itemDatabase.Find(item => item.itemID == 30016));
        Boss41Item.Add(itemDatabase.Find(item => item.itemID == 30017));
        Boss41Item.Add(itemDatabase.Find(item => item.itemID == 30018));

        // 물약
        Boss41Item.Add(itemDatabase.Find(item => item.itemID == 20001)); // 빨포1
        Boss41Item.Add(itemDatabase.Find(item => item.itemID == 20006)); // 파포1

        // 무기
        Boss41Item.Add(itemDatabase.Find(item => item.itemID == 10011));
        Boss41Item.Add(itemDatabase.Find(item => item.itemID == 10012));

        boss_drop_item.Add(Boss41Item);
        boss_drop_item.Add(Boss42Item);
        boss_drop_item.Add(Boss43Item);
        boss_drop_item.Add(Boss44Item);
        boss_drop_item.Add(Boss45Item);
        boss_drop_item.Add(Boss46Item);
        boss_drop_item.Add(Boss47Item);
        boss_drop_item.Add(Boss48Item);


        // 49~56 보스 7

        // 잡템
        Boss49Item.Add(itemDatabase.Find(item => item.itemID == 30019));
        Boss49Item.Add(itemDatabase.Find(item => item.itemID == 30020));
        Boss49Item.Add(itemDatabase.Find(item => item.itemID == 30021));

        // 물약
        Boss49Item.Add(itemDatabase.Find(item => item.itemID == 20001)); // 빨포1
        Boss49Item.Add(itemDatabase.Find(item => item.itemID == 20006)); // 파포1

        // 무기
        Boss49Item.Add(itemDatabase.Find(item => item.itemID == 10013));
        Boss49Item.Add(itemDatabase.Find(item => item.itemID == 10014));

        boss_drop_item.Add(Boss49Item);
        boss_drop_item.Add(Boss50Item);
        boss_drop_item.Add(Boss51Item);
        boss_drop_item.Add(Boss52Item);
        boss_drop_item.Add(Boss53Item);
        boss_drop_item.Add(Boss54Item);
        boss_drop_item.Add(Boss55Item);
        boss_drop_item.Add(Boss56Item);

        // 57~64 보스 8

        // 잡템
        Boss57Item.Add(itemDatabase.Find(item => item.itemID == 30022));
        Boss57Item.Add(itemDatabase.Find(item => item.itemID == 30023));
        Boss57Item.Add(itemDatabase.Find(item => item.itemID == 30024));

        // 물약
        Boss57Item.Add(itemDatabase.Find(item => item.itemID == 20001)); // 빨포1
        Boss57Item.Add(itemDatabase.Find(item => item.itemID == 20006)); // 파포1

        // 무기
        Boss57Item.Add(itemDatabase.Find(item => item.itemID == 10015));
        Boss57Item.Add(itemDatabase.Find(item => item.itemID == 10016));

        boss_drop_item.Add(Boss57Item);
        boss_drop_item.Add(Boss58Item);
        boss_drop_item.Add(Boss59Item);
        boss_drop_item.Add(Boss60Item);
        boss_drop_item.Add(Boss61Item);
        boss_drop_item.Add(Boss62Item);
        boss_drop_item.Add(Boss63Item);
        boss_drop_item.Add(Boss64Item);

    }


    public void SaveData()
    {
        ES3.Save<bool[]>("BossClear", UpLoadData.boss_is_cleared);
        ES3.Save<bool[]>("ItemGained", UpLoadData.item_is_gained);
        ES3.Save<bool[]>("ItemUnlock", UpLoadData.item_info_list);

        // 보스 스킬 확인 배열

        ES3.Save<bool[]>("boss0_0_usedskill_list", UpLoadData.boss_usedskill_list[0]);
        ES3.Save<bool[]>("boss0_1_usedskill_list", UpLoadData.boss_usedskill_list[1]);
        ES3.Save<bool[]>("boss0_2_usedskill_list", UpLoadData.boss_usedskill_list[2]);
        ES3.Save<bool[]>("boss0_3_usedskill_list", UpLoadData.boss_usedskill_list[3]);
        ES3.Save<bool[]>("boss0_4_usedskill_list", UpLoadData.boss_usedskill_list[4]);
        ES3.Save<bool[]>("boss0_5_usedskill_list", UpLoadData.boss_usedskill_list[5]);
        ES3.Save<bool[]>("boss0_6_usedskill_list", UpLoadData.boss_usedskill_list[6]);
        ES3.Save<bool[]>("boss0_7_usedskill_list", UpLoadData.boss_usedskill_list[7]);

        ES3.Save<bool[]>("boss1_0_usedskill_list", UpLoadData.boss_usedskill_list[8]);
        ES3.Save<bool[]>("boss1_1_usedskill_list", UpLoadData.boss_usedskill_list[9]);
        ES3.Save<bool[]>("boss1_2_usedskill_list", UpLoadData.boss_usedskill_list[10]);
        ES3.Save<bool[]>("boss1_3_usedskill_list", UpLoadData.boss_usedskill_list[11]);
        ES3.Save<bool[]>("boss1_4_usedskill_list", UpLoadData.boss_usedskill_list[12]);
        ES3.Save<bool[]>("boss1_5_usedskill_list", UpLoadData.boss_usedskill_list[13]);
        ES3.Save<bool[]>("boss1_6_usedskill_list", UpLoadData.boss_usedskill_list[14]);
        ES3.Save<bool[]>("boss1_7_usedskill_list", UpLoadData.boss_usedskill_list[15]);

        ES3.Save<bool[]>("boss2_0_usedskill_list", UpLoadData.boss_usedskill_list[16]);
        ES3.Save<bool[]>("boss2_1_usedskill_list", UpLoadData.boss_usedskill_list[17]);
        ES3.Save<bool[]>("boss2_2_usedskill_list", UpLoadData.boss_usedskill_list[18]);
        ES3.Save<bool[]>("boss2_3_usedskill_list", UpLoadData.boss_usedskill_list[19]);
        ES3.Save<bool[]>("boss2_4_usedskill_list", UpLoadData.boss_usedskill_list[20]);
        ES3.Save<bool[]>("boss2_5_usedskill_list", UpLoadData.boss_usedskill_list[21]);
        ES3.Save<bool[]>("boss2_6_usedskill_list", UpLoadData.boss_usedskill_list[22]);
        ES3.Save<bool[]>("boss2_7_usedskill_list", UpLoadData.boss_usedskill_list[23]);

        ES3.Save<bool[]>("boss3_0_usedskill_list", UpLoadData.boss_usedskill_list[24]);
        ES3.Save<bool[]>("boss3_1_usedskill_list", UpLoadData.boss_usedskill_list[25]);
        ES3.Save<bool[]>("boss3_2_usedskill_list", UpLoadData.boss_usedskill_list[26]);
        ES3.Save<bool[]>("boss3_3_usedskill_list", UpLoadData.boss_usedskill_list[27]);
        ES3.Save<bool[]>("boss3_4_usedskill_list", UpLoadData.boss_usedskill_list[28]);
        ES3.Save<bool[]>("boss3_5_usedskill_list", UpLoadData.boss_usedskill_list[29]);
        ES3.Save<bool[]>("boss3_6_usedskill_list", UpLoadData.boss_usedskill_list[30]);
        ES3.Save<bool[]>("boss3_7_usedskill_list", UpLoadData.boss_usedskill_list[31]);

        ES3.Save<bool[]>("boss4_0_usedskill_list", UpLoadData.boss_usedskill_list[32]);
        ES3.Save<bool[]>("boss4_1_usedskill_list", UpLoadData.boss_usedskill_list[33]);
        ES3.Save<bool[]>("boss4_2_usedskill_list", UpLoadData.boss_usedskill_list[34]);
        ES3.Save<bool[]>("boss4_3_usedskill_list", UpLoadData.boss_usedskill_list[35]);
        ES3.Save<bool[]>("boss4_4_usedskill_list", UpLoadData.boss_usedskill_list[36]);
        ES3.Save<bool[]>("boss4_5_usedskill_list", UpLoadData.boss_usedskill_list[37]);
        ES3.Save<bool[]>("boss4_6_usedskill_list", UpLoadData.boss_usedskill_list[38]);
        ES3.Save<bool[]>("boss4_7_usedskill_list", UpLoadData.boss_usedskill_list[39]);

        ES3.Save<bool[]>("boss5_0_usedskill_list", UpLoadData.boss_usedskill_list[40]);
        ES3.Save<bool[]>("boss5_1_usedskill_list", UpLoadData.boss_usedskill_list[41]);
        ES3.Save<bool[]>("boss5_2_usedskill_list", UpLoadData.boss_usedskill_list[42]);
        ES3.Save<bool[]>("boss5_3_usedskill_list", UpLoadData.boss_usedskill_list[43]);
        ES3.Save<bool[]>("boss5_4_usedskill_list", UpLoadData.boss_usedskill_list[44]);
        ES3.Save<bool[]>("boss5_5_usedskill_list", UpLoadData.boss_usedskill_list[45]);
        ES3.Save<bool[]>("boss5_6_usedskill_list", UpLoadData.boss_usedskill_list[46]);
        ES3.Save<bool[]>("boss5_7_usedskill_list", UpLoadData.boss_usedskill_list[47]);

        ES3.Save<bool[]>("boss6_0_usedskill_list", UpLoadData.boss_usedskill_list[48]);
        ES3.Save<bool[]>("boss6_1_usedskill_list", UpLoadData.boss_usedskill_list[49]);
        ES3.Save<bool[]>("boss6_2_usedskill_list", UpLoadData.boss_usedskill_list[50]);
        ES3.Save<bool[]>("boss6_3_usedskill_list", UpLoadData.boss_usedskill_list[51]);
        ES3.Save<bool[]>("boss6_4_usedskill_list", UpLoadData.boss_usedskill_list[52]);
        ES3.Save<bool[]>("boss6_5_usedskill_list", UpLoadData.boss_usedskill_list[53]);
        ES3.Save<bool[]>("boss6_6_usedskill_list", UpLoadData.boss_usedskill_list[54]);
        ES3.Save<bool[]>("boss6_7_usedskill_list", UpLoadData.boss_usedskill_list[55]);

        ES3.Save<bool[]>("boss7_0_usedskill_list", UpLoadData.boss_usedskill_list[56]);
        ES3.Save<bool[]>("boss7_1_usedskill_list", UpLoadData.boss_usedskill_list[57]);
        ES3.Save<bool[]>("boss7_2_usedskill_list", UpLoadData.boss_usedskill_list[58]);
        ES3.Save<bool[]>("boss7_3_usedskill_list", UpLoadData.boss_usedskill_list[59]);
        ES3.Save<bool[]>("boss7_4_usedskill_list", UpLoadData.boss_usedskill_list[60]);
        ES3.Save<bool[]>("boss7_5_usedskill_list", UpLoadData.boss_usedskill_list[61]);
        ES3.Save<bool[]>("boss7_6_usedskill_list", UpLoadData.boss_usedskill_list[62]);
        ES3.Save<bool[]>("boss7_7_usedskill_list", UpLoadData.boss_usedskill_list[63]);


        // 보스 스킬 인포 배열

        ES3.Save<bool[]>("boss0_0_infoskill_list", UpLoadData.boss_usedskill_list[0]);
        ES3.Save<bool[]>("boss0_1_infoskill_list", UpLoadData.boss_usedskill_list[1]);
        ES3.Save<bool[]>("boss0_2_infoskill_list", UpLoadData.boss_usedskill_list[2]);
        ES3.Save<bool[]>("boss0_3_infoskill_list", UpLoadData.boss_usedskill_list[3]);
        ES3.Save<bool[]>("boss0_4_infoskill_list", UpLoadData.boss_usedskill_list[4]);
        ES3.Save<bool[]>("boss0_5_infoskill_list", UpLoadData.boss_usedskill_list[5]);
        ES3.Save<bool[]>("boss0_6_infoskill_list", UpLoadData.boss_usedskill_list[6]);
        ES3.Save<bool[]>("boss0_7_infoskill_list", UpLoadData.boss_usedskill_list[7]);
                                  
        ES3.Save<bool[]>("boss1_0_infoskill_list", UpLoadData.boss_usedskill_list[8]);
        ES3.Save<bool[]>("boss1_1_infoskill_list", UpLoadData.boss_usedskill_list[9]);
        ES3.Save<bool[]>("boss1_2_infoskill_list", UpLoadData.boss_usedskill_list[10]);
        ES3.Save<bool[]>("boss1_3_infoskill_list", UpLoadData.boss_usedskill_list[11]);
        ES3.Save<bool[]>("boss1_4_infoskill_list", UpLoadData.boss_usedskill_list[12]);
        ES3.Save<bool[]>("boss1_5_infoskill_list", UpLoadData.boss_usedskill_list[13]);
        ES3.Save<bool[]>("boss1_6_infoskill_list", UpLoadData.boss_usedskill_list[14]);
        ES3.Save<bool[]>("boss1_7_infoskill_list", UpLoadData.boss_usedskill_list[15]);
                                  
        ES3.Save<bool[]>("boss2_0_infoskill_list", UpLoadData.boss_usedskill_list[16]);
        ES3.Save<bool[]>("boss2_1_infoskill_list", UpLoadData.boss_usedskill_list[17]);
        ES3.Save<bool[]>("boss2_2_infoskill_list", UpLoadData.boss_usedskill_list[18]);
        ES3.Save<bool[]>("boss2_3_infoskill_list", UpLoadData.boss_usedskill_list[19]);
        ES3.Save<bool[]>("boss2_4_infoskill_list", UpLoadData.boss_usedskill_list[20]);
        ES3.Save<bool[]>("boss2_5_infoskill_list", UpLoadData.boss_usedskill_list[21]);
        ES3.Save<bool[]>("boss2_6_infoskill_list", UpLoadData.boss_usedskill_list[22]);
        ES3.Save<bool[]>("boss2_7_infoskill_list", UpLoadData.boss_usedskill_list[23]);
                                  
        ES3.Save<bool[]>("boss3_0_infoskill_list", UpLoadData.boss_usedskill_list[24]);
        ES3.Save<bool[]>("boss3_1_infoskill_list", UpLoadData.boss_usedskill_list[25]);
        ES3.Save<bool[]>("boss3_2_infoskill_list", UpLoadData.boss_usedskill_list[26]);
        ES3.Save<bool[]>("boss3_3_infoskill_list", UpLoadData.boss_usedskill_list[27]);
        ES3.Save<bool[]>("boss3_4_infoskill_list", UpLoadData.boss_usedskill_list[28]);
        ES3.Save<bool[]>("boss3_5_infoskill_list", UpLoadData.boss_usedskill_list[29]);
        ES3.Save<bool[]>("boss3_6_infoskill_list", UpLoadData.boss_usedskill_list[30]);
        ES3.Save<bool[]>("boss3_7_infoskill_list", UpLoadData.boss_usedskill_list[31]);
                                  
        ES3.Save<bool[]>("boss4_0_infoskill_list", UpLoadData.boss_usedskill_list[32]);
        ES3.Save<bool[]>("boss4_1_infoskill_list", UpLoadData.boss_usedskill_list[33]);
        ES3.Save<bool[]>("boss4_2_infoskill_list", UpLoadData.boss_usedskill_list[34]);
        ES3.Save<bool[]>("boss4_3_infoskill_list", UpLoadData.boss_usedskill_list[35]);
        ES3.Save<bool[]>("boss4_4_infoskill_list", UpLoadData.boss_usedskill_list[36]);
        ES3.Save<bool[]>("boss4_5_infoskill_list", UpLoadData.boss_usedskill_list[37]);
        ES3.Save<bool[]>("boss4_6_infoskill_list", UpLoadData.boss_usedskill_list[38]);
        ES3.Save<bool[]>("boss4_7_infoskill_list", UpLoadData.boss_usedskill_list[39]);
                                  
        ES3.Save<bool[]>("boss5_0_infoskill_list", UpLoadData.boss_usedskill_list[40]);
        ES3.Save<bool[]>("boss5_1_infoskill_list", UpLoadData.boss_usedskill_list[41]);
        ES3.Save<bool[]>("boss5_2_infoskill_list", UpLoadData.boss_usedskill_list[42]);
        ES3.Save<bool[]>("boss5_3_infoskill_list", UpLoadData.boss_usedskill_list[43]);
        ES3.Save<bool[]>("boss5_4_infoskill_list", UpLoadData.boss_usedskill_list[44]);
        ES3.Save<bool[]>("boss5_5_infoskill_list", UpLoadData.boss_usedskill_list[45]);
        ES3.Save<bool[]>("boss5_6_infoskill_list", UpLoadData.boss_usedskill_list[46]);
        ES3.Save<bool[]>("boss5_7_infoskill_list", UpLoadData.boss_usedskill_list[47]);
                                  
        ES3.Save<bool[]>("boss6_0_infoskill_list", UpLoadData.boss_usedskill_list[48]);
        ES3.Save<bool[]>("boss6_1_infoskill_list", UpLoadData.boss_usedskill_list[49]);
        ES3.Save<bool[]>("boss6_2_infoskill_list", UpLoadData.boss_usedskill_list[50]);
        ES3.Save<bool[]>("boss6_3_infoskill_list", UpLoadData.boss_usedskill_list[51]);
        ES3.Save<bool[]>("boss6_4_infoskill_list", UpLoadData.boss_usedskill_list[52]);
        ES3.Save<bool[]>("boss6_5_infoskill_list", UpLoadData.boss_usedskill_list[53]);
        ES3.Save<bool[]>("boss6_6_infoskill_list", UpLoadData.boss_usedskill_list[54]);
        ES3.Save<bool[]>("boss6_7_infoskill_list", UpLoadData.boss_usedskill_list[55]);
                                  
        ES3.Save<bool[]>("boss7_0_infoskill_list", UpLoadData.boss_usedskill_list[56]);
        ES3.Save<bool[]>("boss7_1_infoskill_list", UpLoadData.boss_usedskill_list[57]);
        ES3.Save<bool[]>("boss7_2_infoskill_list", UpLoadData.boss_usedskill_list[58]);
        ES3.Save<bool[]>("boss7_3_infoskill_list", UpLoadData.boss_usedskill_list[59]);
        ES3.Save<bool[]>("boss7_4_infoskill_list", UpLoadData.boss_usedskill_list[60]);
        ES3.Save<bool[]>("boss7_5_infoskill_list", UpLoadData.boss_usedskill_list[61]);
        ES3.Save<bool[]>("boss7_6_infoskill_list", UpLoadData.boss_usedskill_list[62]);
        ES3.Save<bool[]>("boss7_7_infoskill_list", UpLoadData.boss_usedskill_list[63]);

        ES3.Save<List<Item>>("itemList", itemList);
        ES3.Save<List<Item>>("UseList", UseList);
        ES3.Save<List<Item>>("UseList0", UseList0);
        ES3.Save<List<Item>>("UseList1", UseList1);
        ES3.Save<List<Item>>("UseList2", UseList2);
        ES3.Save<List<Item>>("UseList3", UseList3);
        ES3.Save<List<Item>>("UseList4", UseList4);
        ES3.Save<List<Item>>("UseList5", UseList5);
        ES3.Save<List<Item>>("UseList6", UseList6);
        ES3.Save<List<Item>>("UseList7", UseList7);
        ES3.Save<List<Item>>("equipList", equipList);
        ES3.Save<List<Item>>("equipList0", equipList0);
        ES3.Save<List<Item>>("equipList1", equipList1);
        ES3.Save<List<Item>>("equipList2", equipList2);
        ES3.Save<List<Item>>("equipList3", equipList3);
        ES3.Save<List<Item>>("equipList4", equipList4);
        ES3.Save<List<Item>>("equipList5", equipList5);
        ES3.Save<List<Item>>("equipList6", equipList6);
        ES3.Save<List<Item>>("equipList7", equipList7);
        ES3.Save<int>("Money", Money);
        ES3.Save<int>("new_boss_skill_count", UpLoadData.new_boss_skill_count);
        ES3.Save<int>("new_item_count", UpLoadData.new_item_count);

        ES3.Save<float>("BGMVolume", VolumeSettingValue);
        ES3.Save<bool>("JoyStickUse", JoyStickUse);
        ES3.Save<int>("skillpos", skillpos);
    }
    public void LoadData()
    {
        UpLoadData.boss_is_cleared = ES3.Load<bool[]>("BossClear");
        UpLoadData.item_is_gained = ES3.Load<bool[]>("ItemGained");
        UpLoadData.item_info_list = ES3.Load<bool[]>("ItemUnlock");

        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss0_0_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss0_1_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss0_2_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss0_3_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss0_4_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss0_5_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss0_6_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss0_7_usedskill_list"));

        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss1_0_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss1_1_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss1_2_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss1_3_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss1_4_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss1_5_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss1_6_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss1_7_usedskill_list"));

        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss2_0_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss2_1_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss2_2_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss2_3_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss2_4_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss2_5_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss2_6_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss2_7_usedskill_list"));

        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss3_0_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss3_1_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss3_2_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss3_3_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss3_4_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss3_5_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss3_6_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss3_7_usedskill_list"));

        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss4_0_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss4_1_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss4_2_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss4_3_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss4_4_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss4_5_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss4_6_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss4_7_usedskill_list"));

        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss5_0_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss5_1_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss5_2_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss5_3_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss5_4_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss5_5_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss5_6_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss5_7_usedskill_list"));

        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss6_0_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss6_1_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss6_2_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss6_3_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss6_4_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss6_5_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss6_6_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss6_7_usedskill_list"));

        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss7_0_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss7_1_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss7_2_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss7_3_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss7_4_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss7_5_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss7_6_usedskill_list"));
        UpLoadData.boss_usedskill_list.Add(ES3.Load<bool[]>("boss7_7_usedskill_list"));

        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss0_0_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss0_1_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss0_2_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss0_3_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss0_4_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss0_5_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss0_6_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss0_7_infoskill_list"));
                                                                     
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss1_0_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss1_1_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss1_2_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss1_3_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss1_4_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss1_5_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss1_6_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss1_7_infoskill_list"));
                                                                     
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss2_0_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss2_1_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss2_2_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss2_3_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss2_4_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss2_5_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss2_6_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss2_7_infoskill_list"));
                                                                     
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss3_0_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss3_1_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss3_2_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss3_3_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss3_4_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss3_5_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss3_6_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss3_7_infoskill_list"));
                                                                     
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss4_0_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss4_1_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss4_2_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss4_3_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss4_4_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss4_5_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss4_6_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss4_7_infoskill_list"));
                                                                     
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss5_0_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss5_1_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss5_2_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss5_3_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss5_4_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss5_5_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss5_6_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss5_7_infoskill_list"));
                                                                     
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss6_0_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss6_1_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss6_2_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss6_3_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss6_4_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss6_5_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss6_6_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss6_7_infoskill_list"));
                                                                     
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss7_0_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss7_1_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss7_2_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss7_3_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss7_4_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss7_5_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss7_6_infoskill_list"));
        UpLoadData.boss_infoskill_list.Add(ES3.Load<bool[]>("boss7_7_infoskill_list"));


        itemList = ES3.Load<List<Item>>("itemList");
        UseList = ES3.Load<List<Item>>("UseList");
        UseList0 = ES3.Load<List<Item>>("UseList0");
        UseList1 = ES3.Load<List<Item>>("UseList1");
        UseList2 = ES3.Load<List<Item>>("UseList2");
        UseList3 = ES3.Load<List<Item>>("UseList3");
        UseList4 = ES3.Load<List<Item>>("UseList4");
        UseList5 = ES3.Load<List<Item>>("UseList5");
        UseList6 = ES3.Load<List<Item>>("UseList6");
        UseList7 = ES3.Load<List<Item>>("UseList7");
        equipList = ES3.Load<List<Item>>("equipList");
        equipList0 = ES3.Load<List<Item>>("equipList0");
        equipList1 = ES3.Load<List<Item>>("equipList1");
        equipList2 = ES3.Load<List<Item>>("equipList2");
        equipList3 = ES3.Load<List<Item>>("equipList3");
        equipList4 = ES3.Load<List<Item>>("equipList4");
        equipList5 = ES3.Load<List<Item>>("equipList5");
        equipList6 = ES3.Load<List<Item>>("equipList6");
        equipList7 = ES3.Load<List<Item>>("equipList7");
        Money = ES3.Load<int>("Money");
        UpLoadData.new_boss_skill_count = ES3.Load<int>("new_boss_skill_count");
        UpLoadData.new_item_count = ES3.Load<int>("new_item_count");
        VolumeSettingValue = ES3.Load<float>("BGMVolume");
        JoyStickUse = ES3.Load<bool>("JoyStickUse");
        skillpos = ES3.Load<int>("skillpos");
    }
    private void SetDefaultSetting()
    {
        // 보스 노말 난이도 해금
        for (int i = 0; i < UpLoadData.boss_is_cleared.Length; i++)
        {
            UpLoadData.boss_is_cleared[i] = false;
        }
        for (int i = 0; i < UpLoadData.item_is_gained.Length; i++)
        {
            UpLoadData.item_is_gained[i] = false;
            UpLoadData.item_info_list[i] = false;
        }

        // 궁극기는 보통 맨 마지막에 놓을려고 함
        // 1번 보스
        UpLoadData.boss_usedskill_list.Add(new bool[4] { false, false, false, false });   // 일반공격, 8방향, 장판, 전범위 폭발,
        UpLoadData.boss_usedskill_list.Add(new bool[5] { false, false, false, false, false });    // 일반공격, 8방향, 로테이트, 전범위 폭발, 장판
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });    // 일반공격, 8방향, 로테이트, 스프레드, 장판, 전범위 폭발, 필라, 스웜프 중복
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });    // 일반공격, 8방향, 로테이트, 스프레드, 장판, 전범위 폭발, 필라, 스웜프 중복
        // 멀티 난이도
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });    // 일반공격, 8방향, 로테이트, 스프레드, 장판, 전범위 폭발, 필라, 스웜프 중복
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });    // 일반공격, 8방향, 로테이트, 스프레드, 장판, 전범위 폭발, 필라, 스웜프 중복
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });    // 일반공격, 8방향, 로테이트, 스프레드, 장판, 전범위 폭발, 필라, 스웜프 중복
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });    // 일반공격, 8방향, 로테이트, 스프레드, 장판, 전범위 폭발, 필라, 스웜프 중복

        // 2번 보스
        UpLoadData.boss_usedskill_list.Add(new bool[4] { false, false, false, false });    // 일반공격, 임의폭발, 불소환, 자석
        UpLoadData.boss_usedskill_list.Add(new bool[5] { false, false, false, false, false});    // 일반공격, 임의폭발, 불소환, 반대방향, 자석
        UpLoadData.boss_usedskill_list.Add(new bool[7] { false, false, false, false, false, false, false });    // 일반공격, 임의폭발, 불소환, 반대방향, 같은방향, 시계방향, 자석
        UpLoadData.boss_usedskill_list.Add(new bool[7] { false, false, false, false, false, false, false });    // 일반공격, 임의폭발, 불소환, 반대방향, 같은방향, 시계방향, 자석
        // 멀티 난이도
        UpLoadData.boss_usedskill_list.Add(new bool[7] { false, false, false, false, false, false, false });    // 일반공격, 임의폭발, 불소환, 반대방향, 같은방향, 시계방향, 자석
        UpLoadData.boss_usedskill_list.Add(new bool[7] { false, false, false, false, false, false, false });    // 일반공격, 임의폭발, 불소환, 반대방향, 같은방향, 시계방향, 자석
        UpLoadData.boss_usedskill_list.Add(new bool[7] { false, false, false, false, false, false, false });    // 일반공격, 임의폭발, 불소환, 반대방향, 같은방향, 시계방향, 자석
        UpLoadData.boss_usedskill_list.Add(new bool[7] { false, false, false, false, false, false, false });    // 일반공격, 임의폭발, 불소환, 반대방향, 같은방향, 시계방향, 자석

        // 3번 보스
        UpLoadData.boss_usedskill_list.Add(new bool[4] { false, false, false, false });    // 일반공격, 8방향, 핀볼, 말뚝
        UpLoadData.boss_usedskill_list.Add(new bool[5] { false, false, false, false, false }); // 일반공격, 8방향, 석화, 핀볼, 말뚝
        UpLoadData.boss_usedskill_list.Add(new bool[7] { false, false, false, false, false, false, false });    // 일반공격, 8방향, 석화, 회전 공격, 비, 핀볼, 말뚝
        UpLoadData.boss_usedskill_list.Add(new bool[7] { false, false, false, false, false, false, false });    // 일반공격, 8방향, 석화, 회전 공격, 비, 핀볼, 말뚝
        // 멀티 난이도
        UpLoadData.boss_usedskill_list.Add(new bool[7] { false, false, false, false, false, false, false });    // 일반공격, 8방향, 석화, 회전 공격, 비, 핀볼, 말뚝
        UpLoadData.boss_usedskill_list.Add(new bool[7] { false, false, false, false, false, false, false });    // 일반공격, 8방향, 석화, 회전 공격, 비, 핀볼, 말뚝
        UpLoadData.boss_usedskill_list.Add(new bool[7] { false, false, false, false, false, false, false });    // 일반공격, 8방향, 석화, 회전 공격, 비, 핀볼, 말뚝
        UpLoadData.boss_usedskill_list.Add(new bool[7] { false, false, false, false, false, false, false });    // 일반공격, 8방향, 석화, 회전 공격, 비, 핀볼, 말뚝

        // 4번 보스
        UpLoadData.boss_usedskill_list.Add(new bool[4] { false, false, false, false });     // 일반공격, 롤링 락,  bombtogether, 메테오 
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });     // 일반공격, 롤링 락, bombtogether, 레이저, 돌굴리기, 돌쏘기, 메테오
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });     // 일반공격, 롤링 락, bombtogether, 레이저, 돌굴리기, 돌쏘기, 메테오
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });     // 일반공격, 롤링 락, bombtogether, 레이저, 돌굴리기, 돌쏘기, 메테오
        // 멀티 난이도                                                                                                  
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });     // 일반공격, 롤링 락, bombtogether, 레이저, 돌굴리기, 돌쏘기, 메테오
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });     // 일반공격, 롤링 락, bombtogether, 레이저, 돌굴리기, 돌쏘기, 메테오
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });     // 일반공격, 롤링 락, bombtogether, 레이저, 돌굴리기, 돌쏘기, 메테오
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });     // 일반공격, 롤링 락, bombtogether, 레이저, 돌굴리기, 돌쏘기, 메테오

        // 5번 보스
        UpLoadData.boss_usedskill_list.Add(new bool[4] { false, false, false, false });     // 일반공격, 러쉬, 암전, 마인
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });     // 일반공격, 러쉬, 회전미사일, 레이저, 바주카, 암전, 마인
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });     // 일반공격, 러쉬, 회전미사일, 레이저, 바주카, 암전, 마인
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });     // 일반공격, 러쉬, 회전미사일, 레이저, 바주카, 암전, 마인
        // 멀티 난이도                                                                                                   
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });     // 일반공격, 러쉬, 회전미사일, 레이저, 바주카, 암전, 마인
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });     // 일반공격, 러쉬, 회전미사일, 레이저, 바주카, 암전, 마인
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });     // 일반공격, 러쉬, 회전미사일, 레이저, 바주카, 암전, 마인
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });     // 일반공격, 러쉬, 회전미사일, 레이저, 바주카, 암전, 마인

        // 6번 보스
        UpLoadData.boss_usedskill_list.Add(new bool[4] { false, false, false, false });     // 일반공격, 토네이도, 화산폭발, 빨아들이기
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });     // 일반공격, 토네이도, 긱, 회전흐름, 화산폭발, 빨아들이기, 물방울
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });     // 일반공격, 토네이도, 긱, 회전흐름, 화산폭발, 빨아들이기, 물방울
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });     // 일반공격, 토네이도, 긱, 회전흐름, 화산폭발, 빨아들이기, 물방울
        // 멀티 난이도                                                                                                   
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });     // 일반공격, 토네이도, 긱, 회전흐름, 화산폭발, 빨아들이기, 물방울
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });     // 일반공격, 토네이도, 긱, 회전흐름, 화산폭발, 빨아들이기, 물방울
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });     // 일반공격, 토네이도, 긱, 회전흐름, 화산폭발, 빨아들이기, 물방울
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });     // 일반공격, 토네이도, 긱, 회전흐름, 화산폭발, 빨아들이기, 물방울

        // 7번 보스
        UpLoadData.boss_usedskill_list.Add(new bool[4] { false, false, false, false });     // 일반공격, 씨클, 모기, 토템
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });     // 일반공격, 씨클, 흡수, 벳템프, 피 웅덩이, 모기, 토템
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });     // 일반공격, 씨클, 흡수, 벳템프, 피 웅덩이, 모기, 토템
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });     // 일반공격, 씨클, 흡수, 벳템프, 피 웅덩이, 모기, 토템
        // 멀티 난이도                                                                                                   
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });     // 일반공격, 씨클, 흡수, 벳템프, 피 웅덩이, 모기, 토템
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });     // 일반공격, 씨클, 흡수, 벳템프, 피 웅덩이, 모기, 토템
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });     // 일반공격, 씨클, 흡수, 벳템프, 피 웅덩이, 모기, 토템
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });     // 일반공격, 씨클, 흡수, 벳템프, 피 웅덩이, 모기, 토템

        // 8번 보스
        UpLoadData.boss_usedskill_list.Add(new bool[4] { false, false, false, false });
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        // 멀티 난이도
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_usedskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });


        // 1번 보스
        UpLoadData.boss_infoskill_list.Add(new bool[4] { false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[5] { false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        // 멀티 난이도
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });

        // 2번 보스
        UpLoadData.boss_infoskill_list.Add(new bool[4] { false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[5] { false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[7] { false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[7] { false, false, false, false, false, false, false });
        // 멀티 난이도
        UpLoadData.boss_infoskill_list.Add(new bool[7] { false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[7] { false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[7] { false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[7] { false, false, false, false, false, false, false });


        // 3번 보스
        UpLoadData.boss_infoskill_list.Add(new bool[4] { false, false, false, false });    // 일반공격, 8방향, 핀볼, 말뚝
        UpLoadData.boss_infoskill_list.Add(new bool[5] { false, false, false, false, false }); // 일반공격, 8방향, 석화, 핀볼, 말뚝
        UpLoadData.boss_infoskill_list.Add(new bool[7] { false, false, false, false, false, false, false });    // 일반공격, 8방향, 석화, 회전 공격, 비, 핀볼, 말뚝
        UpLoadData.boss_infoskill_list.Add(new bool[7] { false, false, false, false, false, false, false });    // 일반공격, 8방향, 석화, 회전 공격, 비, 핀볼, 말뚝
        // 멀티 난이도
        UpLoadData.boss_infoskill_list.Add(new bool[7] { false, false, false, false, false, false, false });    // 일반공격, 8방향, 석화, 회전 공격, 비, 핀볼, 말뚝
        UpLoadData.boss_infoskill_list.Add(new bool[7] { false, false, false, false, false, false, false });    // 일반공격, 8방향, 석화, 회전 공격, 비, 핀볼, 말뚝
        UpLoadData.boss_infoskill_list.Add(new bool[7] { false, false, false, false, false, false, false });    // 일반공격, 8방향, 석화, 회전 공격, 비, 핀볼, 말뚝
        UpLoadData.boss_infoskill_list.Add(new bool[7] { false, false, false, false, false, false, false });    // 일반공격, 8방향, 석화, 회전 공격, 비, 핀볼, 말뚝

        // 4번 보스
        UpLoadData.boss_infoskill_list.Add(new bool[4] { false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        // 멀티 난이도
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });

        // 5번 보스                        
        UpLoadData.boss_infoskill_list.Add(new bool[4] { false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        // 멀티 난이도
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });

        // 6번 보스                       
        UpLoadData.boss_infoskill_list.Add(new bool[4] { false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        // 멀티 난이도
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });


        // 7번 보스                        
        UpLoadData.boss_infoskill_list.Add(new bool[4] { false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        // 멀티 난이도
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });

        // 8번 보스                        
        UpLoadData.boss_infoskill_list.Add(new bool[4] { false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        // 멀티 난이도
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });
        UpLoadData.boss_infoskill_list.Add(new bool[8] { false, false, false, false, false, false, false, false });

        VolumeSettingValue = 1;
        JoyStickUse = false;
        skillpos = 2;
        Money = 0;
        UpLoadData.new_boss_skill_count = 0;
        UpLoadData.new_item_count = 0;
        equipList.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList0.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList0.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList0.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList0.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList1.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList1.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList1.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList1.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList2.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList2.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList2.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList2.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList3.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList3.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList3.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList3.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList4.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList4.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList4.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList4.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList5.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList5.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList5.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList5.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList6.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList6.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList6.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList6.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList7.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList7.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList7.Add(new Item(0, "", "", Item.ItemType.ETC));
        equipList7.Add(new Item(0, "", "", Item.ItemType.ETC));

        UseList.Add(new Item(0, "", "", Item.ItemType.ETC));
        UseList.Add(new Item(0, "", "", Item.ItemType.ETC));
        UseList0.Add(new Item(0, "", "", Item.ItemType.ETC));
        UseList0.Add(new Item(0, "", "", Item.ItemType.ETC));
        UseList1.Add(new Item(0, "", "", Item.ItemType.ETC));
        UseList1.Add(new Item(0, "", "", Item.ItemType.ETC));
        UseList2.Add(new Item(0, "", "", Item.ItemType.ETC));
        UseList2.Add(new Item(0, "", "", Item.ItemType.ETC));
        UseList3.Add(new Item(0, "", "", Item.ItemType.ETC));
        UseList3.Add(new Item(0, "", "", Item.ItemType.ETC));
        UseList4.Add(new Item(0, "", "", Item.ItemType.ETC));
        UseList4.Add(new Item(0, "", "", Item.ItemType.ETC));
        UseList5.Add(new Item(0, "", "", Item.ItemType.ETC));
        UseList5.Add(new Item(0, "", "", Item.ItemType.ETC));
        UseList6.Add(new Item(0, "", "", Item.ItemType.ETC));
        UseList6.Add(new Item(0, "", "", Item.ItemType.ETC));
        UseList7.Add(new Item(0, "", "", Item.ItemType.ETC));
        UseList7.Add(new Item(0, "", "", Item.ItemType.ETC));
    }

}
