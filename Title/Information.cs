using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Information : MonoBehaviour
{
    // 보스, 캐릭터, 아이템 순의 판넬들을 보관
    public GameObject[] panels;

    public GameObject destroy_effect;
    private GameObject destroy_effect_temp;

    // 보스용
    // 보스 이름들을 저장
    public string[] boss_names;
    // 32개의 보스 스테이터스를 저장
    private float[,] boss_status_data;

    // 보스 이름 텍스트
    public Text boss_name_text;
    // 보스 이미지 전시
    public GameObject[] boss_portrait;
    // 보스 스테이터스 전시
    public Text[] boss_status;

    public GameObject[] boss_skill_panel;
    // 보스 인덱스용 버튼
    public Button[] boss_index_buttons;
    // 보스 스킬 잠금 머티리얼
    public Material Shiny;
    // 보스 인덱스 버튼 머티리얼
    public Material innerGlow;
    // 보스별 새로운 스킬 해금 표시
    public GameObject[] new_skill_each_boss;

    private WaitForSeconds waitTime = new WaitForSeconds(0.01f);

    public Image[] boss_skill_lock;

    public Button[] title_buttons;

    private bool is_multi = false;

    //캐릭터용
    // 캐릭터 판텔의 캐릭터 이미지들을 보관
    public Sprite[] character_images;
    public string[] character_names;

    // 캐릭터 스테이터스를 데이터 베이스에서 불러 온다.
    private float[] Char_MaxHP;
    private float[] Char_MaxMp;
    private float[] Char_PhysicalAttackPower;
    private float[] Char_MagicAttackPower;
    private float[] Char_PhysicalArmor;
    private float[] Char_MagicArmorPower;
    private float[] Char_NormalAttackRange;
    private float[] Char_NormalAttackSpeed;
    private float[] Char_MoveSpeed;
    private float[] Char_ciritical;
    private float[] Char_HP_Regenerate;
    private float[] Char_MP_Regenerate;

    // 보스 스킬 아이콘
    public Image[] boss_skill_icons;


    // 캐릭터 스킬 이름
    public Text[] boss_skill_names;
    // 캐릭터 스킬 설명
    public Text[] boss_skill_descriptions;
    // 캐릭터 스킬 아이콘
    public Image[] skill_icon;
    // 캐릭터 스킬 이름
    public Text[] skill_name;
    // 캐릭터 스킬 설명
    public Text[] skill_description;

    // 캐릭터 이미지 전시
    public Image character_portrait;
    // 캐릭터 이름 텍스트
    public Text character_name_text;
    // 캐릭터 스테이터스 전시
    public Text[] character_status;

    public GameObject Multi_Single_Button;

    // 아이템용
    // 슬롯의 상위 오브젝트
    public GameObject[] content;
    // 생성할 슬롯
    private InventorySlot[] equip_item;
    private InventorySlot[] use_item;
    private InventorySlot[] etc_item;

    private List<Item> equip_item_list;
    private List<Item> use_item_list;
    private List<Item> etc_item_list;

    public GameObject blank_slot;
    // 임시 슬롯을 저장할 오브젝트
    private GameObject slot_temp;
    // 아이템 이미지
    public Image item_image;
    // 아이템 이름
    public Text item_name;
    // 아이템 설명
    public Text item_description;
    // 아이템 스크롤 뷰
    public GameObject[] scroll_view;
    // 아이템 스테이터스 표시
    public Text[] item_status;
    // 아이템 타입 버튼
    public Image[] item_type_buttons;
    public GameObject[] new_item_type_icon;

    public int panel_index = 0;
    private int boss_index = 0;
    private int boss_level = 0;
    private int char_index = 0;
    private int item_index = 0;
    private int item_type_index = 0;



    // 데이터 베이스의 모든 아이템을 가져옴
    private List<Item> itemList;
    // Start is called before the first frame \

    // 아이템 테두리 색깔
    private Color white = new Color(1, 1, 1, 0.5f);
    private Color Blue = new Color(0.25f, 0.7f, 1f, 0.5f);
    private Color Purple = new Color(1f, 0.5f, 1f, 0.5f);
    private Color Orange = new Color(1f, 0.8f, 0.5f, 0.5f);

    private Color boss_skill_name_color;
    private Color boss_skill_info_color;
    private Color boss_skill_icon_color;

    private Color item_name_color;
    private Color item_info_color;
    private Color item_stat_color;

    // 보스 설명창 스크롤바
    public Scrollbar _boss_scrollbar;
    // 아이템 설명창 스크롤바
    public Scrollbar[] _item_scrollbar;

    public Sprite lock_icon;

    public GameObject new_boss_skill_icon;
    public GameObject new_item_icon;

    void Start()
    {
        itemList = new List<Item>();
        equip_item_list = new List<Item>();
        use_item_list = new List<Item>();
        etc_item_list = new List<Item>();
        _boss_scrollbar = panels[1].GetComponentInChildren<Scrollbar>();
        InstanceSlots();
        InitializeObjects();
        boss_skill_name_color = boss_skill_names[0].color;
        boss_skill_info_color = boss_skill_descriptions[0].color;
        boss_skill_icon_color = boss_skill_icons[0].color;
        item_name_color = item_name.color;
        item_info_color = item_description.color;
        item_stat_color = item_status[0].color;
        ActivateNewSkillIcon();
        ActivateNewItemIcon();
    }

    //------------------ 타이틀 판넬 캐릭터, 보스, 아이템 순
    public void TurnOn0Panel()
    {
        SoundManager.instance.Play(37);

        char_index = 0;
        panel_index = 0;
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }
        panels[panel_index].SetActive(true);

        ChangeCharacterContenet(char_index);
        ChangeCharacterStatus(char_index);
        StopAllCoroutines();
        StartCoroutine(SelectedTabEffect(panel_index));
    }

    public void TurnOn1Panel()
    {
        SoundManager.instance.Play(37);

        boss_index = 0;
        panel_index = 1;
        boss_level = 0;
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }
        panels[panel_index].SetActive(true);

        ChangeBossContent(boss_index);
        ChangeStatusContent(boss_level);
        StopAllCoroutines();
        StartCoroutine(SelectedTabEffect(panel_index));
        ClickedBossButton(boss_index);
        //for(int i = 0; i < 8; i++)
        //{
        //    ControlEachBossNewSkillIcon(i);
        //}
        ControlEachBossNewSkillIcon(0);
        ControlEachBossNewSkillIcon(1);
        ControlEachBossNewSkillIcon(2);

    }


    public void TurnOn2Panel()
    {
        SoundManager.instance.Play(37);

        item_type_index = 0;
        panel_index = 2;
        StopAllCoroutines();
        StartCoroutine(SelectedTabEffect(panel_index));
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }
        panels[panel_index].SetActive(true);
        itemList = DatabaseManager.instance.itemDatabase;
        item_index = 0;
        ItemInitiate();
        for (int i = 0; i < content.Length; i++)
        {
            scroll_view[i].SetActive(false);
        }
        scroll_view[item_type_index].SetActive(true);
        if (equip_item.Length == 0)
        {
            InstanceSlots();
        }
        else
        {
            return;
        }
        ActivateNewSkillIcon();
        for(int i = 0; i < new_item_type_icon.Length; i++)
        {
            SetActiveNewItemIcon(i);
        }
        ClickedItemTypeButton(item_type_index);
    }
    //------------------ 타이틀 판넬 끝




    //------------------ 버튼 클릭용 함수

    // 보스 인덱스를 얻기 위한 함수
    public void GetBossIndex(int index)
    {
        boss_index = index;
        boss_level = 0;
        ChangeBossContent(index);
        ChangeStatusContent(boss_level);
        ClickedBossButton(index);
    }

    // 보스 레벨 인덱스를 얻기 위한 함수
    public void GetBossLevelIndex(int index)
    {
        boss_level = index;
        if (is_multi)
        {
            boss_level += 4;
        }
        ChangeStatusContent(boss_level);
    }

    // 캐릭터 인덱스를 얻기 위한 함수
    public void GetCharacterIndex(int index)
    {
        char_index = index;
        ChangeCharacterContenet(char_index);
        ChangeCharacterStatus(char_index);
    }

    // 아이템 타입 인덱스를 얻기 위한 함수
    public void GetItemTypeIndex(int index)
    {
        item_type_index = index;
        for (int i = 0; i < content.Length; i++)
        {
            _item_scrollbar[i].value = 1;
            scroll_view[i].SetActive(false);
        }
        scroll_view[item_type_index].SetActive(true);
        SetDefaultItem(item_type_index);
        ClickedItemTypeButton(item_type_index);
        ItemInitiate();
    }

    // 아이템 인덱스를 얻기 위한 함수
    public void GetItemIndex(int index, GameObject button)
    {
        item_index = index;
        DisplayItem(item_index, button);
    }

    //------------------ 버튼 클릭용 함수 끝


    // 보스 인덱스를 얻을 때마다 오브젝트 내용을 변경할 함수
    private void ChangeBossContent(int index)
    {
        _boss_scrollbar.value = 1;
        for (int i = 0; i < boss_portrait.Length; i++)
        {
            boss_portrait[i].SetActive(false);
        }
        boss_portrait[index].SetActive(true);
        boss_name_text.text = boss_names[index];
    }

    // 레벨에따른 보스 내용 변경하는 함수
    private void ChangeStatusContent(int level)
    {
        if (UpLoadData.boss_is_cleared[level + boss_index * 8] != true)
        {
            for (int i = 0; i < boss_status.Length; i++)
            {
                boss_status[i].text = "???";
            }

        }
        else
        {
            boss_status[0].text = DatabaseManager.instance.MaxHP.ToString();
            boss_status[1].text = DatabaseManager.instance.MaxMp.ToString();
            boss_status[2].text = DatabaseManager.instance.PhysicalAttackPower.ToString();
            boss_status[3].text = DatabaseManager.instance.MagicAttackPower.ToString();
            boss_status[4].text = DatabaseManager.instance.PhysicalArmor.ToString();
            boss_status[5].text = DatabaseManager.instance.MagicArmorPower.ToString();
            boss_status[6].text = DatabaseManager.instance.MoveSpeed.ToString();
        }


        // 레벨에 따라 스킬 자체에 변화가 없다면...
        switch (level + boss_index * 8)
        {
            case 0:
                ControlPanels(4);
                IconClose(4);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "모래바람";
                    boss_skill_descriptions[0].text = "태양신이 플레이어를 향하여 직선으로 모래바람을 일으킵니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "퍼지는\n 분노";
                    boss_skill_descriptions[1].text = "태양신의 분노가 8방향으로 2번에 걸쳐 퍼져나갑니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }

                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }

                    boss_skill_icons[2].sprite = null;
                    boss_skill_names[2].text = "분노의\n 절정";
                    boss_skill_descriptions[2].text = "태양신의 분노가 절정에 이르러 전역에 폭발을 일으킵니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "도망칠 수\n없다";
                    boss_skill_descriptions[3].text = "태양신이 외곽에 저주를 시전하여 플레이어들이 도망갈 수 없게 합니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                break;
            case 1:
                ControlPanels(5);
                IconClose(5);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "모래바람";
                    boss_skill_descriptions[0].text = "태양신이 플레이어를 향하여 직선으로 모래바람을 일으킵니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "퍼지는\n 분노";
                    boss_skill_descriptions[1].text = "태양신의 분노가 8방향으로 2번에 걸쳐 퍼져나갑니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }
                    boss_skill_icons[2].sprite = null;
                    boss_skill_names[2].text = "휘감는\n 불꽃";
                    boss_skill_descriptions[2].text = "태양신을 중심으로 4방향의 불꽃들이\n회전하여 플레이어를 공격합니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "분노의\n 절정";
                    boss_skill_descriptions[3].text = "태양신의 분노가 절정에 이르러 전역에 폭발을 일으킵니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "도망칠 수\n없다";
                    boss_skill_descriptions[4].text = "태양신이 외곽에 저주를 시전하여 플레이어들이 도망갈 수 없게 합니다.";
                }
                else
                {
                    NullSkillPanel(4);
                }
                break;
            case 2:
                ControlPanels(8);
                IconClose(8);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "모래바람";
                    boss_skill_descriptions[0].text = "태양신이 플레이어를 향하여 직선으로 모래바람을 일으킵니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "퍼지는\n 분노";
                    boss_skill_descriptions[1].text = "태양신의 분노가 8방향으로 2번에 걸쳐 퍼져나갑니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }
                    boss_skill_icons[2].sprite = null;
                    boss_skill_names[2].text = "휘감는\n 불꽃";
                    boss_skill_descriptions[2].text = "태양신을 중심으로 4방향의 불꽃들이\n회전하여 플레이어를 공격합니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "목죄는\n 저주";
                    boss_skill_descriptions[3].text = "저주에 걸린 플레이어의 위치에 3초마다 함정이 설치됩니다. \n함정은 곂쳐지는 순간 폭발합니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "도망칠 수\n없다";
                    boss_skill_descriptions[4].text = "태양신이 외곽에 저주를 시전하여 플레이어들이 도망갈 수 없게 합니다.";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "분노의\n 절정";
                    boss_skill_descriptions[5].text = "태양신의 분노가 절정에 이르러 전역에 폭발을 일으킵니다.";
                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "왕가의\n 석상";
                    boss_skill_descriptions[6].text = "태양신이 폭발하는 석상을 n개 소환합니다. 일정시간 지난뒤 지정된 방향으로 힘을 발산하여\n 석상을 폭발시킵니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][7])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][7])
                    {
                        BlockSkillPanel(7);
                    }
                    else
                    {
                        UnlockSkillPanel(7);
                    }
                    boss_skill_icons[7].sprite = null;
                    boss_skill_names[7].text = "유혹하는\n 죽음";
                    boss_skill_descriptions[7].text = "함정의 폭발로 생성된 지역은 플레이어들을 끌어들이며\n큰 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(7);
                }
                break;
            case 3:
                ControlPanels(8);
                IconClose(8);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "모래바람";
                    boss_skill_descriptions[0].text = "태양신이 플레이어를 향하여 직선으로 모래바람을 일으킵니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "퍼지는\n 분노";
                    boss_skill_descriptions[1].text = "태양신의 분노가 8방향으로 2번에 걸쳐 퍼져나갑니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }
                    boss_skill_icons[2].sprite = null;
                    boss_skill_names[2].text = "휘감는\n 불꽃";
                    boss_skill_descriptions[2].text = "태양신을 중심으로 4방향의 불꽃들이\n회전하여 플레이어를 공격합니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "목죄는\n 저주";
                    boss_skill_descriptions[3].text = "저주에 걸린 플레이어의 위치에 3초마다 함정이 설치됩니다. \n함정은 곂쳐지는 순간 폭발합니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "도망칠 수\n없다";
                    boss_skill_descriptions[4].text = "태양신이 외곽에 저주를 시전하여 플레이어들이 도망갈 수 없게 합니다.";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "분노의\n 절정";
                    boss_skill_descriptions[5].text = "태양신의 분노가 절정에 이르러 전역에 폭발을 일으킵니다.";
                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "왕가의\n 석상";
                    boss_skill_descriptions[6].text = "태양신이 폭발하는 석상을 n개 소환합니다. 일정시간 지난뒤 지정된 방향으로 힘을 발산하여\n 석상을 폭발시킵니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][7])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][7])
                    {
                        BlockSkillPanel(7);
                    }
                    else
                    {
                        UnlockSkillPanel(7);
                    }
                    boss_skill_icons[7].sprite = null;
                    boss_skill_names[7].text = "유혹하는\n 죽음";
                    boss_skill_descriptions[7].text = "함정의 폭발로 생성된 지역은 플레이어들을 끌어들이며\n큰 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(7);
                }
                break;
            case 4:
                ControlPanels(8);
                IconClose(8);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "모래바람";
                    boss_skill_descriptions[0].text = "태양신이 플레이어를 향하여 직선으로 모래바람을 일으킵니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "퍼지는\n 분노";
                    boss_skill_descriptions[1].text = "태양신의 분노가 8방향으로 2번에 걸쳐 퍼져나갑니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }
                    boss_skill_icons[2].sprite = null;
                    boss_skill_names[2].text = "휘감는\n 불꽃";
                    boss_skill_descriptions[2].text = "태양신을 중심으로 4방향의 불꽃들이\n회전하여 플레이어를 공격합니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "목죄는\n 저주";
                    boss_skill_descriptions[3].text = "저주에 걸린 플레이어의 위치에 3초마다 함정이 설치됩니다. \n함정은 곂쳐지는 순간 폭발합니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "도망칠 수\n없다";
                    boss_skill_descriptions[4].text = "태양신이 외곽에 저주를 시전하여 플레이어들이 도망갈 수 없게 합니다.";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "분노의\n 절정";
                    boss_skill_descriptions[5].text = "태양신의 분노가 절정에 이르러 전역에 폭발을 일으킵니다.";
                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "왕가의\n 석상";
                    boss_skill_descriptions[6].text = "태양신이 폭발하는 석상을 n개 소환합니다. 일정시간 지난뒤 지정된 방향으로 힘을 발산하여\n 석상을 폭발시킵니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][7])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][7])
                    {
                        BlockSkillPanel(7);
                    }
                    else
                    {
                        UnlockSkillPanel(7);
                    }
                    boss_skill_icons[7].sprite = null;
                    boss_skill_names[7].text = "유혹하는\n 죽음";
                    boss_skill_descriptions[7].text = "함정의 폭발로 생성된 지역은 플레이어들을 끌어들이며\n큰 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(7);
                }
                break;
            case 5:
                ControlPanels(8);
                IconClose(8);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "모래바람";
                    boss_skill_descriptions[0].text = "태양신이 플레이어를 향하여 직선으로 모래바람을 일으킵니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "퍼지는\n 분노";
                    boss_skill_descriptions[1].text = "태양신의 분노가 8방향으로 2번에 걸쳐 퍼져나갑니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }
                    boss_skill_icons[2].sprite = null;
                    boss_skill_names[2].text = "휘감는\n 불꽃";
                    boss_skill_descriptions[2].text = "태양신을 중심으로 4방향의 불꽃들이\n회전하여 플레이어를 공격합니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "목죄는\n 저주";
                    boss_skill_descriptions[3].text = "저주에 걸린 플레이어의 위치에 3초마다 함정이 설치됩니다. \n함정은 곂쳐지는 순간 폭발합니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "도망칠 수\n없다";
                    boss_skill_descriptions[4].text = "태양신이 외곽에 저주를 시전하여 플레이어들이 도망갈 수 없게 합니다.";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "분노의\n 절정";
                    boss_skill_descriptions[5].text = "태양신의 분노가 절정에 이르러 전역에 폭발을 일으킵니다.";
                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "왕가의\n 석상";
                    boss_skill_descriptions[6].text = "태양신이 폭발하는 석상을 n개 소환합니다. 일정시간 지난뒤 지정된 방향으로 힘을 발산하여\n 석상을 폭발시킵니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][7])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][7])
                    {
                        BlockSkillPanel(7);
                    }
                    else
                    {
                        UnlockSkillPanel(7);
                    }
                    boss_skill_icons[7].sprite = null;
                    boss_skill_names[7].text = "유혹하는\n 죽음";
                    boss_skill_descriptions[7].text = "함정의 폭발로 생성된 지역은 플레이어들을 끌어들이며\n큰 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(7);
                }
                break;
            case 6:
                ControlPanels(8);
                IconClose(8);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "모래바람";
                    boss_skill_descriptions[0].text = "태양신이 플레이어를 향하여 직선으로 모래바람을 일으킵니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "퍼지는\n 분노";
                    boss_skill_descriptions[1].text = "태양신의 분노가 8방향으로 2번에 걸쳐 퍼져나갑니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }
                    boss_skill_icons[2].sprite = null;
                    boss_skill_names[2].text = "휘감는\n 불꽃";
                    boss_skill_descriptions[2].text = "태양신을 중심으로 4방향의 불꽃들이\n회전하여 플레이어를 공격합니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "목죄는\n 저주";
                    boss_skill_descriptions[3].text = "저주에 걸린 플레이어의 위치에 3초마다 함정이 설치됩니다. \n함정은 곂쳐지는 순간 폭발합니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "도망칠 수\n없다";
                    boss_skill_descriptions[4].text = "태양신이 외곽에 저주를 시전하여 플레이어들이 도망갈 수 없게 합니다.";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "분노의\n 절정";
                    boss_skill_descriptions[5].text = "태양신의 분노가 절정에 이르러 전역에 폭발을 일으킵니다.";
                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "왕가의\n 석상";
                    boss_skill_descriptions[6].text = "태양신이 폭발하는 석상을 n개 소환합니다. 일정시간 지난뒤 지정된 방향으로 힘을 발산하여\n 석상을 폭발시킵니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][7])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][7])
                    {
                        BlockSkillPanel(7);
                    }
                    else
                    {
                        UnlockSkillPanel(7);
                    }
                    boss_skill_icons[7].sprite = null;
                    boss_skill_names[7].text = "유혹하는\n 죽음";
                    boss_skill_descriptions[7].text = "함정의 폭발로 생성된 지역은 플레이어들을 끌어들이며\n큰 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(7);
                }
                break;
            case 7:
                ControlPanels(8);
                IconClose(8);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "모래바람";
                    boss_skill_descriptions[0].text = "태양신이 플레이어를 향하여 직선으로 모래바람을 일으킵니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "퍼지는\n 분노";
                    boss_skill_descriptions[1].text = "태양신의 분노가 8방향으로 2번에 걸쳐 퍼져나갑니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }
                    boss_skill_icons[2].sprite = null;
                    boss_skill_names[2].text = "휘감는\n 불꽃";
                    boss_skill_descriptions[2].text = "태양신을 중심으로 4방향의 불꽃들이\n회전하여 플레이어를 공격합니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "목죄는\n 저주";
                    boss_skill_descriptions[3].text = "저주에 걸린 플레이어의 위치에 3초마다 함정이 설치됩니다. \n함정은 곂쳐지는 순간 폭발합니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "도망칠 수\n없다";
                    boss_skill_descriptions[4].text = "태양신이 외곽에 저주를 시전하여 플레이어들이 도망갈 수 없게 합니다.";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "분노의\n 절정";
                    boss_skill_descriptions[5].text = "태양신의 분노가 절정에 이르러 전역에 폭발을 일으킵니다.";
                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "왕가의\n 석상";
                    boss_skill_descriptions[6].text = "태양신이 폭발하는 석상을 n개 소환합니다. 일정시간 지난뒤 지정된 방향으로 힘을 발산하여\n 석상을 폭발시킵니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][7])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][7])
                    {
                        BlockSkillPanel(7);
                    }
                    else
                    {
                        UnlockSkillPanel(7);
                    }
                    boss_skill_icons[7].sprite = null;
                    boss_skill_names[7].text = "유혹하는\n 죽음";
                    boss_skill_descriptions[7].text = "함정의 폭발로 생성된 지역은 플레이어들을 끌어들이며\n큰 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(7);
                }
                break;
            case 8:     // 2번 보스
                ControlPanels(4);
                IconClose(4);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "대지의\n위협";
                    boss_skill_descriptions[0].text = "플레이어 방향으로 다수의 바위들을 소환하며 공격합니다. 플레이어의 공격에의해 소멸됩니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "폭발하는\n대지";
                    boss_skill_descriptions[1].text = "난쟁이가 전역에 다수의 폭발을 일으켜 플레이어들을 위협합니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }

                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }

                    boss_skill_icons[2].sprite = null;
                    boss_skill_names[2].text = "땅의\n정령";
                    boss_skill_descriptions[2].text = "난쟁이가 대지를 땅의 정령을 소환합니다. 정령은 죽을 때까지 존재하며 대지 불로 뒤덮습니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "접근하는\n위협";
                    boss_skill_descriptions[3].text = "소환된 바위들이 플레이어의 방향으로 직접 날아들어 크게 플레이어들을 위협합니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                break;
            case 9:
                ControlPanels(5);
                IconClose(5);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "대지의\n위협";
                    boss_skill_descriptions[0].text = "플레이어 방향으로 다수의 바위들을 소환하며 공격합니다. 플레이어의 공격에의해 소멸됩니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "폭발하는\n대지";
                    boss_skill_descriptions[1].text = "난쟁이가 전역에 다수의 폭발을 일으켜 플레이어들을 위협합니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }

                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }

                    boss_skill_icons[2].sprite = null;
                    boss_skill_names[2].text = "땅의\n정령";
                    boss_skill_descriptions[2].text = "난쟁이가 대지를 땅의 정령을 소환합니다. 정령은 죽을 때까지 존재하며 대지 불로 뒤덮습니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "대지는\n내편";
                    boss_skill_descriptions[3].text = "전역에 큰 피해를 입히는 화염을 소환합니다. 필할 곳은 짧은 순간에 좁은 공간만 제공됩니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "위협의\n승화";
                    boss_skill_descriptions[4].text = "소환된 바위들이 플레이어의 방향으로 직접 날아들어 크게 플레이어들을 위협합니다.";
                }
                else
                {
                    NullSkillPanel(4);
                }
                break;
            case 10:
                ControlPanels(7);
                IconClose(7);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "대지의\n위협";
                    boss_skill_descriptions[0].text = "플레이어 방향으로 다수의 바위들을 소환하며 공격합니다. 플레이어의 공격에의해 소멸됩니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "폭발하는\n대지";
                    boss_skill_descriptions[1].text = "난쟁이가 전역에 다수의 폭발을 일으켜 플레이어들을 위협합니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }

                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }

                    boss_skill_icons[2].sprite = null;
                    boss_skill_names[2].text = "땅의\n정령";
                    boss_skill_descriptions[2].text = "난쟁이가 대지를 땅의 정령을 소환합니다. 정령은 죽을 때까지 존재하며 대지 불로 뒤덮습니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "대지는\n내편";
                    boss_skill_descriptions[3].text = "전역에 큰 피해를 입히는 화염을 소환합니다. 필할 곳은 짧은 순간에 좁은 공간만 제공됩니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "함께하는\n폭발";
                    boss_skill_descriptions[4].text = "플레이어들의 이동이 제한된 상태로 수많은 폭탄들 중에 2개의 폭탄을 밟아야 합니다.";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "조여오는\n화염";
                    boss_skill_descriptions[5].text = "난쟁이가 거대한 화염을 2방향으로 2번 시간 간격을 두고 소환합니다.(방해 가능)";
                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "위협의\n승화";
                    boss_skill_descriptions[6].text = "소환된 바위들이 플레이어의 방향으로 직접 날아들어 크게 플레이어들을 위협합니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                break;
            case 11:
                ControlPanels(7);
                IconClose(7);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "대지의\n위협";
                    boss_skill_descriptions[0].text = "플레이어 방향으로 다수의 바위들을 소환하며 공격합니다. 플레이어의 공격에의해 소멸됩니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "폭발하는\n대지";
                    boss_skill_descriptions[1].text = "난쟁이가 전역에 다수의 폭발을 일으켜 플레이어들을 위협합니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }

                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }

                    boss_skill_icons[2].sprite = null;
                    boss_skill_names[2].text = "땅의\n정령";
                    boss_skill_descriptions[2].text = "난쟁이가 대지를 땅의 정령을 소환합니다. 정령은 죽을 때까지 존재하며 대지 불로 뒤덮습니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "대지는\n내편";
                    boss_skill_descriptions[3].text = "전역에 큰 피해를 입히는 화염을 소환합니다. 필할 곳은 짧은 순간에 좁은 공간만 제공됩니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "함께하는\n폭발";
                    boss_skill_descriptions[4].text = "플레이어들의 이동이 제한된 상태로 수많은 폭탄들 중에 2개의 폭탄을 밟아야 합니다.";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "조여오는\n화염";
                    boss_skill_descriptions[5].text = "난쟁이가 거대한 화염을 2방향으로 2번 시간 간격을 두고 소환합니다.(방해 가능)";
                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "위협의\n승화";
                    boss_skill_descriptions[6].text = "소환된 바위들이 플레이어의 방향으로 직접 날아들어 크게 플레이어들을 위협합니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                break;
            case 12:
                ControlPanels(7);
                IconClose(7);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "대지의\n위협";
                    boss_skill_descriptions[0].text = "플레이어 방향으로 다수의 바위들을 소환하며 공격합니다. 플레이어의 공격에의해 소멸됩니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "폭발하는\n대지";
                    boss_skill_descriptions[1].text = "난쟁이가 전역에 다수의 폭발을 일으켜 플레이어들을 위협합니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }

                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }

                    boss_skill_icons[2].sprite = null;
                    boss_skill_names[2].text = "땅의\n정령";
                    boss_skill_descriptions[2].text = "난쟁이가 대지를 땅의 정령을 소환합니다. 정령은 죽을 때까지 존재하며 대지 불로 뒤덮습니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "대지는\n내편";
                    boss_skill_descriptions[3].text = "전역에 큰 피해를 입히는 화염을 소환합니다. 필할 곳은 짧은 순간에 좁은 공간만 제공됩니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "함께하는\n폭발";
                    boss_skill_descriptions[4].text = "플레이어들의 이동이 제한된 상태로 수많은 폭탄들 중에 2개의 폭탄을 밟아야 합니다.";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "조여오는\n화염";
                    boss_skill_descriptions[5].text = "난쟁이가 거대한 화염을 2방향으로 2번 시간 간격을 두고 소환합니다.(방해 가능)";
                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "위협의\n승화";
                    boss_skill_descriptions[6].text = "소환된 바위들이 플레이어의 방향으로 직접 날아들어 크게 플레이어들을 위협합니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                break;
            case 13:
                ControlPanels(7);
                IconClose(7);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "대지의\n위협";
                    boss_skill_descriptions[0].text = "플레이어 방향으로 다수의 바위들을 소환하며 공격합니다. 플레이어의 공격에의해 소멸됩니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "폭발하는\n대지";
                    boss_skill_descriptions[1].text = "난쟁이가 전역에 다수의 폭발을 일으켜 플레이어들을 위협합니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }

                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }

                    boss_skill_icons[2].sprite = null;
                    boss_skill_names[2].text = "땅의\n정령";
                    boss_skill_descriptions[2].text = "난쟁이가 대지를 땅의 정령을 소환합니다. 정령은 죽을 때까지 존재하며 대지 불로 뒤덮습니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "대지는\n내편";
                    boss_skill_descriptions[3].text = "전역에 큰 피해를 입히는 화염을 소환합니다. 필할 곳은 짧은 순간에 좁은 공간만 제공됩니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "함께하는\n폭발";
                    boss_skill_descriptions[4].text = "플레이어들의 이동이 제한된 상태로 수많은 폭탄들 중에 2개의 폭탄을 밟아야 합니다.";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "조여오는\n화염";
                    boss_skill_descriptions[5].text = "난쟁이가 거대한 화염을 2방향으로 2번 시간 간격을 두고 소환합니다.(방해 가능)";
                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "위협의\n승화";
                    boss_skill_descriptions[6].text = "소환된 바위들이 플레이어의 방향으로 직접 날아들어 크게 플레이어들을 위협합니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                break;
            case 14:
                ControlPanels(7);
                IconClose(7);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "대지의\n위협";
                    boss_skill_descriptions[0].text = "플레이어 방향으로 다수의 바위들을 소환하며 공격합니다. 플레이어의 공격에의해 소멸됩니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "폭발하는\n대지";
                    boss_skill_descriptions[1].text = "난쟁이가 전역에 다수의 폭발을 일으켜 플레이어들을 위협합니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }

                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }

                    boss_skill_icons[2].sprite = null;
                    boss_skill_names[2].text = "땅의\n정령";
                    boss_skill_descriptions[2].text = "난쟁이가 대지를 땅의 정령을 소환합니다. 정령은 죽을 때까지 존재하며 대지 불로 뒤덮습니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "대지는\n내편";
                    boss_skill_descriptions[3].text = "전역에 큰 피해를 입히는 화염을 소환합니다. 필할 곳은 짧은 순간에 좁은 공간만 제공됩니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "함께하는\n폭발";
                    boss_skill_descriptions[4].text = "플레이어들의 이동이 제한된 상태로 수많은 폭탄들 중에 2개의 폭탄을 밟아야 합니다.";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "조여오는\n화염";
                    boss_skill_descriptions[5].text = "난쟁이가 거대한 화염을 2방향으로 2번 시간 간격을 두고 소환합니다.(방해 가능)";
                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "위협의\n승화";
                    boss_skill_descriptions[6].text = "소환된 바위들이 플레이어의 방향으로 직접 날아들어 크게 플레이어들을 위협합니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                break;
            case 15:
                ControlPanels(7);
                IconClose(7);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "대지의\n위협";
                    boss_skill_descriptions[0].text = "플레이어 방향으로 다수의 바위들을 소환하며 공격합니다. 플레이어의 공격에의해 소멸됩니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "폭발하는\n대지";
                    boss_skill_descriptions[1].text = "난쟁이가 전역에 다수의 폭발을 일으켜 플레이어들을 위협합니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }

                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }

                    boss_skill_icons[2].sprite = null;
                    boss_skill_names[2].text = "땅의\n정령";
                    boss_skill_descriptions[2].text = "난쟁이가 대지를 땅의 정령을 소환합니다. 정령은 죽을 때까지 존재하며 대지 불로 뒤덮습니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "대지는\n내편";
                    boss_skill_descriptions[3].text = "전역에 큰 피해를 입히는 화염을 소환합니다. 필할 곳은 짧은 순간에 좁은 공간만 제공됩니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "함께하는\n폭발";
                    boss_skill_descriptions[4].text = "플레이어들의 이동이 제한된 상태로 수많은 폭탄들 중에 2개의 폭탄을 밟아야 합니다.";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "조여오는\n화염";
                    boss_skill_descriptions[5].text = "난쟁이가 거대한 화염을 2방향으로 2번 시간 간격을 두고 소환합니다.(방해 가능)";
                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "위협의\n승화";
                    boss_skill_descriptions[6].text = "소환된 바위들이 플레이어의 방향으로 직접 날아들어 크게 플레이어들을 위협합니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                break;
            case 16: // 3번 보스
                ControlPanels(4);
                IconClose(4);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "악취의\n땅";
                    boss_skill_descriptions[0].text = "플레이어의 위치에 악취가 진동하는 지역을 생성합니다.\n 일정한 피해를 지속적으로 입힙니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "독의\n폭풍";
                    boss_skill_descriptions[1].text = "메두사가 자신의 독기를 여러 갈래로 내뿜어 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }
                    boss_skill_icons[2].sprite = null;
                    boss_skill_names[2].text = "부유하는\n독";
                    boss_skill_descriptions[2].text = "여러방향으로 튕겨져 나가는 독이 생성되어 플레이어들을 혼돈에 빠뜨리고, 큰 피해를 입힙니다.\n난이도에 따라 갯수가 정해집니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "죽음의\n말뚝";
                    boss_skill_descriptions[3].text = "특정 플레이어에게 말뚝을 박습니다. 말뚝은 사라질 때까지 지속적으로 플레이어를\n 밀어내며 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                break;
            case 17:
                ControlPanels(5);
                IconClose(5);
                Debug.Log(boss_index);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "악취의\n땅";
                    boss_skill_descriptions[0].text = "플레이어의 위치에 악취가 진동하는 지역을 생성합니다.\n 일정한 피해를 지속적으로 입힙니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "독의\n폭풍";
                    boss_skill_descriptions[1].text = "메두사가 자신의 독기를 여러 갈래로 내뿜어 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }
                    boss_skill_icons[2].sprite = null;
                    boss_skill_names[2].text = "죽음의\n시선";
                    boss_skill_descriptions[2].text = "메두사가 순간적으로 자신의 얼굴을 비춥니다. 마주보는 자는 돌이되어 죽습니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "부유하는\n독";
                    boss_skill_descriptions[3].text = "여러방향으로 튕겨져 나가는 독이 생성되어 플레이어들을 혼돈에 빠뜨리고, 큰 피해를 입힙니다.\n난이도에 따라 갯수가 정해집니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "죽음의\n말뚝";
                    boss_skill_descriptions[4].text = "특정 플레이어에게 말뚝을 박습니다. 말뚝은 사라질 때까지 지속적으로 플레이어를\n 밀어내며 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(4);
                }
                break;
            case 18:
                ControlPanels(8);
                IconClose(8);
                Debug.Log(boss_index);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "악취의\n땅";
                    boss_skill_descriptions[0].text = "플레이어의 위치에 악취가 진동하는 지역을 생성합니다.\n 일정한 피해를 지속적으로 입힙니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "독의\n폭풍";
                    boss_skill_descriptions[1].text = "메두사가 자신의 독기를 여러 갈래로 내뿜어 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }
                    boss_skill_icons[2].sprite = null;
                    boss_skill_names[2].text = "죽음의\n시선";
                    boss_skill_descriptions[2].text = "메두사가 순간적으로 자신의 얼굴을 비춥니다. 마주보는 자는 돌이되어 죽습니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "자전하는\n독";
                    boss_skill_descriptions[3].text = "메두사가 자기 자신을 맴도는 독 구슬을 내뿜습니다.\n 이에 맞는 플레이어들은 큰 피해를 입습니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "하늘 독";
                    boss_skill_descriptions[4].text = "메두사가 치명적 독을 흘려보냅니다. 이 독에 맞으면 피해를 입으며\n 악취의 땅을 정화시킵니다. ";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "부유하는\n독";
                    boss_skill_descriptions[5].text = "여러방향으로 튕겨져 나가는 독이 생성되어 플레이어들을 혼돈에 빠뜨리고, 큰 피해를 입힙니다.\n난이도에 따라 갯수가 정해집니다.";
                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "죽음의\n말뚝";
                    boss_skill_descriptions[6].text = "특정 플레이어에게 말뚝을 박습니다. 말뚝은 사라질 때까지 지속적으로 플레이어를\n 밀어내며 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                break;
            case 19:
                ControlPanels(8);
                IconClose(8);
                Debug.Log(boss_index);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "악취의\n땅";
                    boss_skill_descriptions[0].text = "플레이어의 위치에 악취가 진동하는 지역을 생성합니다.\n 일정한 피해를 지속적으로 입힙니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "독의\n폭풍";
                    boss_skill_descriptions[1].text = "메두사가 자신의 독기를 여러 갈래로 내뿜어 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }
                    boss_skill_icons[2].sprite = null;
                    boss_skill_names[2].text = "죽음의\n시선";
                    boss_skill_descriptions[2].text = "메두사가 순간적으로 자신의 얼굴을 비춥니다. 마주보는 자는 돌이되어 죽습니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "자전하는\n독";
                    boss_skill_descriptions[3].text = "메두사가 자기 자신을 맴도는 독 구슬을 내뿜습니다.\n 이에 맞는 플레이어들은 큰 피해를 입습니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "하늘 독";
                    boss_skill_descriptions[4].text = "메두사가 치명적 독을 흘려보냅니다. 이 독에 맞으면 피해를 입으며\n 악취의 땅을 정화시킵니다. ";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "부유하는\n독";
                    boss_skill_descriptions[5].text = "여러방향으로 튕겨져 나가는 독이 생성되어 플레이어들을 혼돈에 빠뜨리고, 큰 피해를 입힙니다.\n난이도에 따라 갯수가 정해집니다.";
                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "죽음의\n말뚝";
                    boss_skill_descriptions[6].text = "특정 플레이어에게 말뚝을 박습니다. 말뚝은 사라질 때까지 지속적으로 플레이어를\n 밀어내며 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                break;
            case 20:
                ControlPanels(8);
                IconClose(8);
                Debug.Log(boss_index);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "악취의\n땅";
                    boss_skill_descriptions[0].text = "플레이어의 위치에 악취가 진동하는 지역을 생성합니다.\n 일정한 피해를 지속적으로 입힙니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "독의\n폭풍";
                    boss_skill_descriptions[1].text = "메두사가 자신의 독기를 여러 갈래로 내뿜어 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }
                    boss_skill_icons[2].sprite = null;
                    boss_skill_names[2].text = "죽음의\n시선";
                    boss_skill_descriptions[2].text = "메두사가 순간적으로 자신의 얼굴을 비춥니다. 마주보는 자는 돌이되어 죽습니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "자전하는\n독";
                    boss_skill_descriptions[3].text = "메두사가 자기 자신을 맴도는 독 구슬을 내뿜습니다.\n 이에 맞는 플레이어들은 큰 피해를 입습니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "하늘 독";
                    boss_skill_descriptions[4].text = "메두사가 치명적 독을 흘려보냅니다. 이 독에 맞으면 피해를 입으며\n 악취의 땅을 정화시킵니다. ";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "부유하는\n독";
                    boss_skill_descriptions[5].text = "여러방향으로 튕겨져 나가는 독이 생성되어 플레이어들을 혼돈에 빠뜨리고, 큰 피해를 입힙니다.\n난이도에 따라 갯수가 정해집니다.";
                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "죽음의\n말뚝";
                    boss_skill_descriptions[6].text = "특정 플레이어에게 말뚝을 박습니다. 말뚝은 사라질 때까지 지속적으로 플레이어를\n 밀어내며 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                break;
            case 21:
                ControlPanels(8);
                IconClose(8);
                Debug.Log(boss_index);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "악취의\n땅";
                    boss_skill_descriptions[0].text = "플레이어의 위치에 악취가 진동하는 지역을 생성합니다.\n 일정한 피해를 지속적으로 입힙니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "독의\n폭풍";
                    boss_skill_descriptions[1].text = "메두사가 자신의 독기를 여러 갈래로 내뿜어 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }
                    boss_skill_icons[2].sprite = null;
                    boss_skill_names[2].text = "죽음의\n시선";
                    boss_skill_descriptions[2].text = "메두사가 순간적으로 자신의 얼굴을 비춥니다. 마주보는 자는 돌이되어 죽습니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "자전하는\n독";
                    boss_skill_descriptions[3].text = "메두사가 자기 자신을 맴도는 독 구슬을 내뿜습니다.\n 이에 맞는 플레이어들은 큰 피해를 입습니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "하늘 독";
                    boss_skill_descriptions[4].text = "메두사가 치명적 독을 흘려보냅니다. 이 독에 맞으면 피해를 입으며\n 악취의 땅을 정화시킵니다. ";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "부유하는\n독";
                    boss_skill_descriptions[5].text = "여러방향으로 튕겨져 나가는 독이 생성되어 플레이어들을 혼돈에 빠뜨리고, 큰 피해를 입힙니다.\n난이도에 따라 갯수가 정해집니다.";
                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "죽음의\n말뚝";
                    boss_skill_descriptions[6].text = "특정 플레이어에게 말뚝을 박습니다. 말뚝은 사라질 때까지 지속적으로 플레이어를\n 밀어내며 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                break;
            case 22:
                ControlPanels(8);
                IconClose(8);
                Debug.Log(boss_index);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "악취의\n땅";
                    boss_skill_descriptions[0].text = "플레이어의 위치에 악취가 진동하는 지역을 생성합니다.\n 일정한 피해를 지속적으로 입힙니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "독의\n폭풍";
                    boss_skill_descriptions[1].text = "메두사가 자신의 독기를 여러 갈래로 내뿜어 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }
                    boss_skill_icons[2].sprite = null;
                    boss_skill_names[2].text = "죽음의\n시선";
                    boss_skill_descriptions[2].text = "메두사가 순간적으로 자신의 얼굴을 비춥니다. 마주보는 자는 돌이되어 죽습니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "자전하는\n독";
                    boss_skill_descriptions[3].text = "메두사가 자기 자신을 맴도는 독 구슬을 내뿜습니다.\n 이에 맞는 플레이어들은 큰 피해를 입습니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "하늘 독";
                    boss_skill_descriptions[4].text = "메두사가 치명적 독을 흘려보냅니다. 이 독에 맞으면 피해를 입으며\n 악취의 땅을 정화시킵니다. ";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "부유하는\n독";
                    boss_skill_descriptions[5].text = "여러방향으로 튕겨져 나가는 독이 생성되어 플레이어들을 혼돈에 빠뜨리고, 큰 피해를 입힙니다.\n난이도에 따라 갯수가 정해집니다.";
                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "죽음의\n말뚝";
                    boss_skill_descriptions[6].text = "특정 플레이어에게 말뚝을 박습니다. 말뚝은 사라질 때까지 지속적으로 플레이어를\n 밀어내며 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                break;
            case 23:
                ControlPanels(8);
                IconClose(8);
                Debug.Log(boss_index);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "악취의\n땅";
                    boss_skill_descriptions[0].text = "플레이어의 위치에 악취가 진동하는 지역을 생성합니다.\n 일정한 피해를 지속적으로 입힙니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "독의\n폭풍";
                    boss_skill_descriptions[1].text = "메두사가 자신의 독기를 여러 갈래로 내뿜어 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }
                    boss_skill_icons[2].sprite = null;
                    boss_skill_names[2].text = "죽음의\n시선";
                    boss_skill_descriptions[2].text = "메두사가 순간적으로 자신의 얼굴을 비춥니다. 마주보는 자는 돌이되어 죽습니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "자전하는\n독";
                    boss_skill_descriptions[3].text = "메두사가 자기 자신을 맴도는 독 구슬을 내뿜습니다.\n 이에 맞는 플레이어들은 큰 피해를 입습니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "하늘 독";
                    boss_skill_descriptions[4].text = "메두사가 치명적 독을 흘려보냅니다. 이 독에 맞으면 피해를 입으며\n 악취의 땅을 정화시킵니다. ";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "부유하는\n독";
                    boss_skill_descriptions[5].text = "여러방향으로 튕겨져 나가는 독이 생성되어 플레이어들을 혼돈에 빠뜨리고, 큰 피해를 입힙니다.\n난이도에 따라 갯수가 정해집니다.";
                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "죽음의\n말뚝";
                    boss_skill_descriptions[6].text = "특정 플레이어에게 말뚝을 박습니다. 말뚝은 사라질 때까지 지속적으로 플레이어를\n 밀어내며 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                break;

            case 24:    // 4번 보스

                break;
            case 25:

                break;
            case 26:

                break;
            case 27:

                break;
            case 28:

                break;
            case 29:

                break;
            case 30:

                break;
            case 31:

                break;

            case 32:    // 5번 보스
                ControlPanels(4);
                IconClose(4);

                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "연발사격";
                    boss_skill_descriptions[0].text = "플레이어를 향해 총을 연발 사격합니다. 사정거리가 생각보다 길지는 않지만 피해는 큽니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "돌격\n폭발";
                    boss_skill_descriptions[1].text = "X-23이 플레이어를 향해 직접 돌격하여 폭발을 일으키고 큰 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }
                    boss_skill_icons[2].sprite = null;
                    boss_skill_names[2].text = "플레어";
                    boss_skill_descriptions[2].text = "X-23이 플레어를 발사하여 플레이어들의 시야를 제한합니다. 플레이어들은 아주 근처의 시야만 허락됩니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }

                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "민감한\n지뢰";
                    boss_skill_descriptions[3].text = "X-23주위에 다수의 지뢰를 설치합니다. 플레이어에 닿은 순간 작동되어 폭발합니다.\n난이도에 따라 갯수가 지정됩니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                break;
            case 33:
                ControlPanels(7);
                IconClose(7);
                Debug.Log(boss_index);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "연발사격";
                    boss_skill_descriptions[0].text = "플레이어를 향해 총을 연발 사격합니다. 사정거리가 생각보다 길지는 않지만 피해는 큽니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "돌격\n폭발";
                    boss_skill_descriptions[1].text = "X-23이 플레이어를 향해 직접 돌격하여 폭발을 일으키고 큰 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "유도\n미사일";
                    boss_skill_descriptions[3].text = "X-23이 특정 플레이어를 향해 유도 미사일을 발사합니다. 미사일은 목표를 맴돌다 순식간에 목표에 도달하여 폭발합니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "레이저\n빔";
                    boss_skill_descriptions[3].text = "X-23이 특정 플레이어에게 강력한 레이져를 발사합니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "사선의\n경계";
                    boss_skill_descriptions[4].text = "X-23가 큰 탄환을 발사합니다. 탄은 지형을 크게 가로질러 지나갑니다.";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "플레어";
                    boss_skill_descriptions[5].text = "X-23이 플레어를 발사하여 플레이어들의 시야를 제한합니다. 플레이어들은 아주 근처의 시야만 허락됩니다.";
                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "민감한\n지뢰";
                    boss_skill_descriptions[6].text = "X-23주위에 다수의 지뢰를 설치합니다. 플레이어에 닿은 순간 작동되어 폭발합니다.\n난이도에 따라 갯수가 지정됩니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                break;
            case 34:
                ControlPanels(7);
                IconClose(7);
                Debug.Log(boss_index);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "연발사격";
                    boss_skill_descriptions[0].text = "플레이어를 향해 총을 연발 사격합니다. 사정거리가 생각보다 길지는 않지만 피해는 큽니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "돌격\n폭발";
                    boss_skill_descriptions[1].text = "X-23이 플레이어를 향해 직접 돌격하여 폭발을 일으키고 큰 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "유도\n미사일";
                    boss_skill_descriptions[3].text = "X-23이 특정 플레이어를 향해 유도 미사일을 발사합니다. 미사일은 목표를 맴돌다 순식간에 목표에 도달하여 폭발합니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "레이저\n빔";
                    boss_skill_descriptions[3].text = "X-23이 특정 플레이어에게 강력한 레이져를 발사합니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "사선의\n경계";
                    boss_skill_descriptions[4].text = "X-23가 큰 탄환을 발사합니다. 탄은 지형을 크게 가로질러 지나갑니다.";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "플레어";
                    boss_skill_descriptions[5].text = "X-23이 플레어를 발사하여 플레이어들의 시야를 제한합니다. 플레이어들은 아주 근처의 시야만 허락됩니다.";
                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "민감한\n지뢰";
                    boss_skill_descriptions[6].text = "X-23주위에 다수의 지뢰를 설치합니다. 플레이어에 닿은 순간 작동되어 폭발합니다.\n난이도에 따라 갯수가 지정됩니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                break;
            case 35:
                ControlPanels(7);
                IconClose(7);
                Debug.Log(boss_index);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "연발사격";
                    boss_skill_descriptions[0].text = "플레이어를 향해 총을 연발 사격합니다. 사정거리가 생각보다 길지는 않지만 피해는 큽니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "돌격\n폭발";
                    boss_skill_descriptions[1].text = "X-23이 플레이어를 향해 직접 돌격하여 폭발을 일으키고 큰 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "유도\n미사일";
                    boss_skill_descriptions[3].text = "X-23이 특정 플레이어를 향해 유도 미사일을 발사합니다. 미사일은 목표를 맴돌다 순식간에 목표에 도달하여 폭발합니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "레이저\n빔";
                    boss_skill_descriptions[3].text = "X-23이 특정 플레이어에게 강력한 레이져를 발사합니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "사선의\n경계";
                    boss_skill_descriptions[4].text = "X-23가 큰 탄환을 발사합니다. 탄은 지형을 크게 가로질러 지나갑니다.";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "플레어";
                    boss_skill_descriptions[5].text = "X-23이 플레어를 발사하여 플레이어들의 시야를 제한합니다. 플레이어들은 아주 근처의 시야만 허락됩니다.";
                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "민감한\n지뢰";
                    boss_skill_descriptions[6].text = "X-23주위에 다수의 지뢰를 설치합니다. 플레이어에 닿은 순간 작동되어 폭발합니다.\n난이도에 따라 갯수가 지정됩니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                break;
            case 36:
                ControlPanels(7);
                IconClose(7);
                Debug.Log(boss_index);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "연발사격";
                    boss_skill_descriptions[0].text = "플레이어를 향해 총을 연발 사격합니다. 사정거리가 생각보다 길지는 않지만 피해는 큽니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "돌격\n폭발";
                    boss_skill_descriptions[1].text = "X-23이 플레이어를 향해 직접 돌격하여 폭발을 일으키고 큰 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "유도\n미사일";
                    boss_skill_descriptions[3].text = "X-23이 특정 플레이어를 향해 유도 미사일을 발사합니다. 미사일은 목표를 맴돌다 순식간에 목표에 도달하여 폭발합니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "레이저\n빔";
                    boss_skill_descriptions[3].text = "X-23이 특정 플레이어에게 강력한 레이져를 발사합니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "사선의\n경계";
                    boss_skill_descriptions[4].text = "X-23가 큰 탄환을 발사합니다. 탄은 지형을 크게 가로질러 지나갑니다.";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "플레어";
                    boss_skill_descriptions[5].text = "X-23이 플레어를 발사하여 플레이어들의 시야를 제한합니다. 플레이어들은 아주 근처의 시야만 허락됩니다.";
                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "민감한\n지뢰";
                    boss_skill_descriptions[6].text = "X-23주위에 다수의 지뢰를 설치합니다. 플레이어에 닿은 순간 작동되어 폭발합니다.\n난이도에 따라 갯수가 지정됩니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                break;
            case 37:
                ControlPanels(7);
                IconClose(7);
                Debug.Log(boss_index);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "연발사격";
                    boss_skill_descriptions[0].text = "플레이어를 향해 총을 연발 사격합니다. 사정거리가 생각보다 길지는 않지만 피해는 큽니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "돌격\n폭발";
                    boss_skill_descriptions[1].text = "X-23이 플레이어를 향해 직접 돌격하여 폭발을 일으키고 큰 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "유도\n미사일";
                    boss_skill_descriptions[3].text = "X-23이 특정 플레이어를 향해 유도 미사일을 발사합니다. 미사일은 목표를 맴돌다 순식간에 목표에 도달하여 폭발합니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "레이저\n빔";
                    boss_skill_descriptions[3].text = "X-23이 특정 플레이어에게 강력한 레이져를 발사합니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "사선의\n경계";
                    boss_skill_descriptions[4].text = "X-23가 큰 탄환을 발사합니다. 탄은 지형을 크게 가로질러 지나갑니다.";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "플레어";
                    boss_skill_descriptions[5].text = "X-23이 플레어를 발사하여 플레이어들의 시야를 제한합니다. 플레이어들은 아주 근처의 시야만 허락됩니다.";
                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "민감한\n지뢰";
                    boss_skill_descriptions[6].text = "X-23주위에 다수의 지뢰를 설치합니다. 플레이어에 닿은 순간 작동되어 폭발합니다.\n난이도에 따라 갯수가 지정됩니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                break;
            case 38:
                ControlPanels(7);
                IconClose(7);
                Debug.Log(boss_index);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "연발사격";
                    boss_skill_descriptions[0].text = "플레이어를 향해 총을 연발 사격합니다. 사정거리가 생각보다 길지는 않지만 피해는 큽니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "돌격\n폭발";
                    boss_skill_descriptions[1].text = "X-23이 플레이어를 향해 직접 돌격하여 폭발을 일으키고 큰 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "유도\n미사일";
                    boss_skill_descriptions[3].text = "X-23이 특정 플레이어를 향해 유도 미사일을 발사합니다. 미사일은 목표를 맴돌다 순식간에 목표에 도달하여 폭발합니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "레이저\n빔";
                    boss_skill_descriptions[3].text = "X-23이 특정 플레이어에게 강력한 레이져를 발사합니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "사선의\n경계";
                    boss_skill_descriptions[4].text = "X-23가 큰 탄환을 발사합니다. 탄은 지형을 크게 가로질러 지나갑니다.";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "플레어";
                    boss_skill_descriptions[5].text = "X-23이 플레어를 발사하여 플레이어들의 시야를 제한합니다. 플레이어들은 아주 근처의 시야만 허락됩니다.";
                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "민감한\n지뢰";
                    boss_skill_descriptions[6].text = "X-23주위에 다수의 지뢰를 설치합니다. 플레이어에 닿은 순간 작동되어 폭발합니다.\n난이도에 따라 갯수가 지정됩니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                break;
            case 39:
                ControlPanels(7);
                IconClose(7);
                Debug.Log(boss_index);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "연발사격";
                    boss_skill_descriptions[0].text = "플레이어를 향해 총을 연발 사격합니다. 사정거리가 생각보다 길지는 않지만 피해는 큽니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "돌격\n폭발";
                    boss_skill_descriptions[1].text = "X-23이 플레이어를 향해 직접 돌격하여 폭발을 일으키고 큰 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "유도\n미사일";
                    boss_skill_descriptions[3].text = "X-23이 특정 플레이어를 향해 유도 미사일을 발사합니다. 미사일은 목표를 맴돌다 순식간에 목표에 도달하여 폭발합니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "레이저\n빔";
                    boss_skill_descriptions[3].text = "X-23이 특정 플레이어에게 강력한 레이져를 발사합니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "사선의\n경계";
                    boss_skill_descriptions[4].text = "X-23가 큰 탄환을 발사합니다. 탄은 지형을 크게 가로질러 지나갑니다.";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "플레어";
                    boss_skill_descriptions[5].text = "X-23이 플레어를 발사하여 플레이어들의 시야를 제한합니다. 플레이어들은 아주 근처의 시야만 허락됩니다.";
                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "민감한\n지뢰";
                    boss_skill_descriptions[6].text = "X-23주위에 다수의 지뢰를 설치합니다. 플레이어에 닿은 순간 작동되어 폭발합니다.\n난이도에 따라 갯수가 지정됩니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                break;

            case 40:    // 6번 보스
                ControlPanels(4);
                IconClose(4);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "용왕의\n분노";
                    boss_skill_descriptions[0].text = "플레이어들을 향해 분노를 쏟아냅니다. 물결을 일으키며 피해를 입히는 방울들이 차례로 생겨납니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "용오름";
                    boss_skill_descriptions[1].text = "용왕이 특정 방향으로 용오름을 보냅니다. 용오름은 플레이어를 빨아들여\n 이동하다 폭발하여 큰 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }
                    boss_skill_icons[2].sprite = null;
                    boss_skill_names[2].text = "화산\n폭발";
                    boss_skill_descriptions[2].text = "용왕의 분노가 극에 달아 화산을 폭발시킵니다. 이 자체에는 피해가 없으나 \n화산재가 수질을 오염시켜 플레이어들에게 지속적 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }

                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "모두\n정화되리라";
                    boss_skill_descriptions[3].text = "용왕이 분화구를 열어 모든 것을 정화하고자합니다. 먼 곳으로부터 빨려오는 구체를 피하면서\n 플레이어들은 분화 중심부에 들어가지 않게 피해야 합니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                break;
            case 41:
                ControlPanels(7);
                IconClose(7);
                Debug.Log(boss_index);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "용왕의\n분노";
                    boss_skill_descriptions[0].text = "플레이어들을 향해 분노를 쏟아냅니다. 물결을 일으키며 피해를 입히는 방울들이 차례로 생겨납니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "용오름";
                    boss_skill_descriptions[1].text = "용왕이 특정 방향으로 용오름을 보냅니다. 용오름은 플레이어를 빨아들여\n 이동하다 폭발하여 큰 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "이리\n오너라";
                    boss_skill_descriptions[3].text = "용왕이 닻을 던집니다. 맞은 플레이어는 용왕앞에 끌려가 큰 피해를 입습니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "만개하는\n물꽃";
                    boss_skill_descriptions[3].text = "용왕이 다수의 물꽃을 소환합니다. 물꽃들은 정해진 나선형의 방식으로 모였다 흩어졌다를 반복합니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "화산\n폭발";
                    boss_skill_descriptions[4].text = "용왕의 분노가 극에 달아 화산을 폭발시킵니다. 이 자체에는 피해가 없으나 \n화산재가 수질을 오염시켜 플레이어들에게 지속적 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "모두\n정화되리라";
                    boss_skill_descriptions[5].text = "용왕이 분화구를 열어 모든 것을 정화하고자합니다. 먼 곳으로부터 빨려오는 구체를 피하면서\n 플레이어들은 분화 중심부에 들어가지 않게 피해야 합니다.";

                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "죽음의\n방울";
                    boss_skill_descriptions[6].text = "분화구에서 생성되는 이 방울은 플레이어들을 5초동안 가둬 \n질식시킵니다. 다른 플레이어에 의해 구출 될 수 있습니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                break;
            case 42:
                ControlPanels(7);
                IconClose(7);
                Debug.Log(boss_index);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "용왕의\n분노";
                    boss_skill_descriptions[0].text = "플레이어들을 향해 분노를 쏟아냅니다. 물결을 일으키며 피해를 입히는 방울들이 차례로 생겨납니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "용오름";
                    boss_skill_descriptions[1].text = "용왕이 특정 방향으로 용오름을 보냅니다. 용오름은 플레이어를 빨아들여\n 이동하다 폭발하여 큰 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "이리\n오너라";
                    boss_skill_descriptions[3].text = "용왕이 닻을 던집니다. 맞은 플레이어는 용왕앞에 끌려가 큰 피해를 입습니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "만개하는\n물꽃";
                    boss_skill_descriptions[3].text = "용왕이 다수의 물꽃을 소환합니다. 물꽃들은 정해진 나선형의 방식으로 모였다 흩어졌다를 반복합니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "화산\n폭발";
                    boss_skill_descriptions[4].text = "용왕의 분노가 극에 달아 화산을 폭발시킵니다. 이 자체에는 피해가 없으나 \n화산재가 수질을 오염시켜 플레이어들에게 지속적 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "모두\n정화되리라";
                    boss_skill_descriptions[5].text = "용왕이 분화구를 열어 모든 것을 정화하고자합니다. 먼 곳으로부터 빨려오는 구체를 피하면서\n 플레이어들은 분화 중심부에 들어가지 않게 피해야 합니다.";

                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "죽음의\n방울";
                    boss_skill_descriptions[6].text = "분화구에서 생성되는 이 방울은 플레이어들을 5초동안 가둬 \n질식시킵니다. 다른 플레이어에 의해 구출 될 수 있습니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                break;
            case 43:
                ControlPanels(7);
                IconClose(7);
                Debug.Log(boss_index);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "용왕의\n분노";
                    boss_skill_descriptions[0].text = "플레이어들을 향해 분노를 쏟아냅니다. 물결을 일으키며 피해를 입히는 방울들이 차례로 생겨납니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "용오름";
                    boss_skill_descriptions[1].text = "용왕이 특정 방향으로 용오름을 보냅니다. 용오름은 플레이어를 빨아들여\n 이동하다 폭발하여 큰 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "이리\n오너라";
                    boss_skill_descriptions[3].text = "용왕이 닻을 던집니다. 맞은 플레이어는 용왕앞에 끌려가 큰 피해를 입습니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "만개하는\n물꽃";
                    boss_skill_descriptions[3].text = "용왕이 다수의 물꽃을 소환합니다. 물꽃들은 정해진 나선형의 방식으로 모였다 흩어졌다를 반복합니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "화산\n폭발";
                    boss_skill_descriptions[4].text = "용왕의 분노가 극에 달아 화산을 폭발시킵니다. 이 자체에는 피해가 없으나 \n화산재가 수질을 오염시켜 플레이어들에게 지속적 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "모두\n정화되리라";
                    boss_skill_descriptions[5].text = "용왕이 분화구를 열어 모든 것을 정화하고자합니다. 먼 곳으로부터 빨려오는 구체를 피하면서\n 플레이어들은 분화 중심부에 들어가지 않게 피해야 합니다.";

                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "죽음의\n방울";
                    boss_skill_descriptions[6].text = "분화구에서 생성되는 이 방울은 플레이어들을 5초동안 가둬 \n질식시킵니다. 다른 플레이어에 의해 구출 될 수 있습니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                break;
            case 44:
                ControlPanels(7);
                IconClose(7);
                Debug.Log(boss_index);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "용왕의\n분노";
                    boss_skill_descriptions[0].text = "플레이어들을 향해 분노를 쏟아냅니다. 물결을 일으키며 피해를 입히는 방울들이 차례로 생겨납니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "용오름";
                    boss_skill_descriptions[1].text = "용왕이 특정 방향으로 용오름을 보냅니다. 용오름은 플레이어를 빨아들여\n 이동하다 폭발하여 큰 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "이리\n오너라";
                    boss_skill_descriptions[3].text = "용왕이 닻을 던집니다. 맞은 플레이어는 용왕앞에 끌려가 큰 피해를 입습니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "만개하는\n물꽃";
                    boss_skill_descriptions[3].text = "용왕이 다수의 물꽃을 소환합니다. 물꽃들은 정해진 나선형의 방식으로 모였다 흩어졌다를 반복합니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "화산\n폭발";
                    boss_skill_descriptions[4].text = "용왕의 분노가 극에 달아 화산을 폭발시킵니다. 이 자체에는 피해가 없으나 \n화산재가 수질을 오염시켜 플레이어들에게 지속적 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "모두\n정화되리라";
                    boss_skill_descriptions[5].text = "용왕이 분화구를 열어 모든 것을 정화하고자합니다. 먼 곳으로부터 빨려오는 구체를 피하면서\n 플레이어들은 분화 중심부에 들어가지 않게 피해야 합니다.";

                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "죽음의\n방울";
                    boss_skill_descriptions[6].text = "분화구에서 생성되는 이 방울은 플레이어들을 5초동안 가둬 \n질식시킵니다. 다른 플레이어에 의해 구출 될 수 있습니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                break;
            case 45:
                ControlPanels(7);
                IconClose(7);
                Debug.Log(boss_index);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "용왕의\n분노";
                    boss_skill_descriptions[0].text = "플레이어들을 향해 분노를 쏟아냅니다. 물결을 일으키며 피해를 입히는 방울들이 차례로 생겨납니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "용오름";
                    boss_skill_descriptions[1].text = "용왕이 특정 방향으로 용오름을 보냅니다. 용오름은 플레이어를 빨아들여\n 이동하다 폭발하여 큰 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "이리\n오너라";
                    boss_skill_descriptions[3].text = "용왕이 닻을 던집니다. 맞은 플레이어는 용왕앞에 끌려가 큰 피해를 입습니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "만개하는\n물꽃";
                    boss_skill_descriptions[3].text = "용왕이 다수의 물꽃을 소환합니다. 물꽃들은 정해진 나선형의 방식으로 모였다 흩어졌다를 반복합니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "화산\n폭발";
                    boss_skill_descriptions[4].text = "용왕의 분노가 극에 달아 화산을 폭발시킵니다. 이 자체에는 피해가 없으나 \n화산재가 수질을 오염시켜 플레이어들에게 지속적 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "모두\n정화되리라";
                    boss_skill_descriptions[5].text = "용왕이 분화구를 열어 모든 것을 정화하고자합니다. 먼 곳으로부터 빨려오는 구체를 피하면서\n 플레이어들은 분화 중심부에 들어가지 않게 피해야 합니다.";

                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "죽음의\n방울";
                    boss_skill_descriptions[6].text = "분화구에서 생성되는 이 방울은 플레이어들을 5초동안 가둬 \n질식시킵니다. 다른 플레이어에 의해 구출 될 수 있습니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                break;
            case 46:
                ControlPanels(7);
                IconClose(7);
                Debug.Log(boss_index);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "용왕의\n분노";
                    boss_skill_descriptions[0].text = "플레이어들을 향해 분노를 쏟아냅니다. 물결을 일으키며 피해를 입히는 방울들이 차례로 생겨납니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "용오름";
                    boss_skill_descriptions[1].text = "용왕이 특정 방향으로 용오름을 보냅니다. 용오름은 플레이어를 빨아들여\n 이동하다 폭발하여 큰 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "이리\n오너라";
                    boss_skill_descriptions[3].text = "용왕이 닻을 던집니다. 맞은 플레이어는 용왕앞에 끌려가 큰 피해를 입습니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "만개하는\n물꽃";
                    boss_skill_descriptions[3].text = "용왕이 다수의 물꽃을 소환합니다. 물꽃들은 정해진 나선형의 방식으로 모였다 흩어졌다를 반복합니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "화산\n폭발";
                    boss_skill_descriptions[4].text = "용왕의 분노가 극에 달아 화산을 폭발시킵니다. 이 자체에는 피해가 없으나 \n화산재가 수질을 오염시켜 플레이어들에게 지속적 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "모두\n정화되리라";
                    boss_skill_descriptions[5].text = "용왕이 분화구를 열어 모든 것을 정화하고자합니다. 먼 곳으로부터 빨려오는 구체를 피하면서\n 플레이어들은 분화 중심부에 들어가지 않게 피해야 합니다.";

                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "죽음의\n방울";
                    boss_skill_descriptions[6].text = "분화구에서 생성되는 이 방울은 플레이어들을 5초동안 가둬 \n질식시킵니다. 다른 플레이어에 의해 구출 될 수 있습니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                break;
            case 47:
                ControlPanels(7);
                IconClose(7);
                Debug.Log(boss_index);
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][0])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][0])
                    {
                        BlockSkillPanel(0);
                    }
                    else
                    {
                        UnlockSkillPanel(0);
                    }
                    boss_skill_icons[0].sprite = null;
                    boss_skill_names[0].text = "용왕의\n분노";
                    boss_skill_descriptions[0].text = "플레이어들을 향해 분노를 쏟아냅니다. 물결을 일으키며 피해를 입히는 방울들이 차례로 생겨납니다.";
                }
                else
                {
                    NullSkillPanel(0);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][1])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][1])
                    {
                        BlockSkillPanel(1);
                    }
                    else
                    {
                        UnlockSkillPanel(1);
                    }
                    boss_skill_icons[1].sprite = null;
                    boss_skill_names[1].text = "용오름";
                    boss_skill_descriptions[1].text = "용왕이 특정 방향으로 용오름을 보냅니다. 용오름은 플레이어를 빨아들여\n 이동하다 폭발하여 큰 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(1);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][2])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][2])
                    {
                        BlockSkillPanel(2);
                    }
                    else
                    {
                        UnlockSkillPanel(2);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "이리\n오너라";
                    boss_skill_descriptions[3].text = "용왕이 닻을 던집니다. 맞은 플레이어는 용왕앞에 끌려가 큰 피해를 입습니다.";
                }
                else
                {
                    NullSkillPanel(2);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][3])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][3])
                    {
                        BlockSkillPanel(3);
                    }
                    else
                    {
                        UnlockSkillPanel(3);
                    }
                    boss_skill_icons[3].sprite = null;
                    boss_skill_names[3].text = "만개하는\n물꽃";
                    boss_skill_descriptions[3].text = "용왕이 다수의 물꽃을 소환합니다. 물꽃들은 정해진 나선형의 방식으로 모였다 흩어졌다를 반복합니다.";
                }
                else
                {
                    NullSkillPanel(3);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][4])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][4])
                    {
                        BlockSkillPanel(4);
                    }
                    else
                    {
                        UnlockSkillPanel(4);
                    }
                    boss_skill_icons[4].sprite = null;
                    boss_skill_names[4].text = "화산\n폭발";
                    boss_skill_descriptions[4].text = "용왕의 분노가 극에 달아 화산을 폭발시킵니다. 이 자체에는 피해가 없으나 \n화산재가 수질을 오염시켜 플레이어들에게 지속적 피해를 입힙니다.";
                }
                else
                {
                    NullSkillPanel(4);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][5])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][5])
                    {
                        BlockSkillPanel(5);
                    }
                    else
                    {
                        UnlockSkillPanel(5);
                    }
                    boss_skill_icons[5].sprite = null;
                    boss_skill_names[5].text = "모두\n정화되리라";
                    boss_skill_descriptions[5].text = "용왕이 분화구를 열어 모든 것을 정화하고자합니다. 먼 곳으로부터 빨려오는 구체를 피하면서\n 플레이어들은 분화 중심부에 들어가지 않게 피해야 합니다.";

                }
                else
                {
                    NullSkillPanel(5);
                }
                if (UpLoadData.boss_usedskill_list[level + boss_index * 8][6])
                {
                    if (!UpLoadData.boss_infoskill_list[level + boss_index * 8][6])
                    {
                        BlockSkillPanel(6);
                    }
                    else
                    {
                        UnlockSkillPanel(6);
                    }
                    boss_skill_icons[6].sprite = null;
                    boss_skill_names[6].text = "죽음의\n방울";
                    boss_skill_descriptions[6].text = "분화구에서 생성되는 이 방울은 플레이어들을 5초동안 가둬 \n질식시킵니다. 다른 플레이어에 의해 구출 될 수 있습니다.";
                }
                else
                {
                    NullSkillPanel(6);
                }
                break;
        }
    }

    // 캐릭터 인덱스를 얻을 때마다 오브젝트 내용을 변경할 함수
    private void ChangeCharacterContenet(int index)
    {
        character_portrait.sprite = character_images[index];
        character_name_text.text = character_names[index];
    }

    private void ChangeCharacterStatus(int index)
    {
        Char_MaxHP = DatabaseManager.instance.Char_MaxHP;
        Char_MaxMp = DatabaseManager.instance.Char_MaxMp;
        Char_PhysicalAttackPower = DatabaseManager.instance.Char_PhysicalAttackPower;
        Char_MagicAttackPower = DatabaseManager.instance.Char_MagicAttackPower;
        Char_PhysicalArmor = DatabaseManager.instance.Char_PhysicalArmor;
        Char_MagicArmorPower = DatabaseManager.instance.Char_MagicArmorPower;
        Char_NormalAttackRange = DatabaseManager.instance.Char_NormalAttackRange;
        Char_NormalAttackSpeed = DatabaseManager.instance.Char_NormalAttackSpeed;
        Char_MoveSpeed = DatabaseManager.instance.Char_MoveSpeed;
        Char_ciritical = DatabaseManager.instance.Char_ciritical;
        Char_HP_Regenerate = DatabaseManager.instance.Char_HP_Regenerate;
        Char_MP_Regenerate = DatabaseManager.instance.Char_MP_Regenerate;

        character_status[0].text = Char_MaxHP[index].ToString();
        character_status[1].text = Char_MaxMp[index].ToString();
        character_status[2].text = Char_PhysicalAttackPower[index].ToString();
        character_status[3].text = Char_MagicAttackPower[index].ToString();
        character_status[4].text = Char_PhysicalArmor[index].ToString();
        character_status[5].text = Char_MagicArmorPower[index].ToString();
        character_status[6].text = Char_NormalAttackRange[index].ToString();
        character_status[7].text = Char_NormalAttackSpeed[index].ToString();
        character_status[8].text = Char_MoveSpeed[index].ToString();
        character_status[9].text = Char_ciritical[index].ToString() + "%";
        character_status[10].text = Char_HP_Regenerate[index].ToString();
        character_status[11].text = Char_MP_Regenerate[index].ToString();

        switch (index)
        {
            case 0:
                skill_icon[0].sprite = Resources.Load<Sprite>("skillicon/1");
                skill_icon[1].sprite = Resources.Load<Sprite>("skillicon/2");
                skill_icon[2].sprite = Resources.Load<Sprite>("skillicon/3");
                skill_icon[3].sprite = Resources.Load<Sprite>("skillicon/4");
                skill_name[0].text = "순간베기";
                skill_name[1].text = "섬광베기";
                skill_name[2].text = "양날 찢기";
                skill_name[3].text = "침묵의\n아우성";
                skill_description[0].text = "빠르게 캐릭터 전방의 모든것을 베어버립니다. 적의 방어력을 감소시킵니다.";
                skill_description[1].text = "원하는 방향으로 빠르게 이동합니다. 이동간에 적을 베어버리고 큰 데미지를 입힙니다.";
                skill_description[2].text = "전방에 있는 적을 힘껏 베어버립니다. 큰 데미지를 입힐 수 있지만 거리가 짧습니다.";
                skill_description[3].text = "큰 데미지를 주지 않지만 보스의 스킬을 취소시킬 수 있는 매우 유용한 기술입니다.";
                break;
            case 1:
                skill_icon[0].sprite = Resources.Load<Sprite>("skillicon/5");
                skill_icon[1].sprite = Resources.Load<Sprite>("skillicon/6");
                skill_icon[2].sprite = Resources.Load<Sprite>("skillicon/7");
                skill_icon[3].sprite = Resources.Load<Sprite>("skillicon/8");
                skill_name[0].text = "독 화살";
                skill_name[1].text = "후퇴사격";
                skill_name[2].text = "최후의\n한 발";
                skill_name[3].text = "회심의\n일격";
                skill_description[0].text = "원하는 방향으로 독 화살을 발사합니다. 독의 피해량은 캐릭터의 물리 공격력에 비례합니다.";
                skill_description[1].text = "캐릭터가 바라보는 방향의 반대 방향으로 빠르게 후퇴이동하면서 공격을 합니다.";
                skill_description[2].text = "온 힘을 화살 끝에 모아 강한 일격을 발사합니다. 이 기술의 피해량은 스킬 시전 시간에 비례합니다.";
                skill_description[3].text = "적의 특정 순간에만 큰 피해를 입힐 수 있는 화살을 원하는 방향에 발사합니다.";
                break;
            case 2:
                skill_icon[0].sprite = Resources.Load<Sprite>("skillicon/9");
                skill_icon[1].sprite = Resources.Load<Sprite>("skillicon/10");
                skill_icon[2].sprite = Resources.Load<Sprite>("skillicon/11");
                skill_icon[3].sprite = Resources.Load<Sprite>("skillicon/12");
                skill_name[0].text = "순풍";
                skill_name[1].text = "영적세계";
                skill_name[2].text = "벼락";
                skill_name[3].text = "기력태우기";
                skill_description[0].text = "원하는 방향으로 치유하는 바람을 생성합니다.";
                skill_description[1].text = "원하는 대상을 영적세계로 잠시 보냅니다. 영의 세계에 들어간 대상은 모든 마법피해를 더 받습니다.";
                skill_description[2].text = "원하는 지점에 벼락을 소환하여 큰 피해를 입힙니다.";
                skill_description[3].text = "적의 특정 순간에 사용하면 적의 기력을 감소시키고 피해를 입힙니다.";
                break;
            case 3:
                skill_icon[0].sprite = Resources.Load<Sprite>("skillicon/13");
                skill_icon[1].sprite = Resources.Load<Sprite>("skillicon/14");
                skill_icon[2].sprite = Resources.Load<Sprite>("skillicon/15");
                skill_icon[3].sprite = Resources.Load<Sprite>("skillicon/16");
                skill_name[0].text = "후려치기";
                skill_name[1].text = "불굴";
                skill_name[2].text = "용맹의\n외침";
                skill_name[3].text = "대지\n가르기";
                skill_description[0].text = "원하는 방향으로 망치를 휘두릅니다. 이에 맞은 적은 이동속도가 감소합니다.";
                skill_description[1].text = "일시적으로 자신의 방어력과 마법방어력을 증가시켜 적의 공격을 방어합니다.";
                skill_description[2].text = "전투에 임하는 모든 캐릭터들의 공격력과 마법 공격력, 체력을 일시적으로 증가시킵니다.";
                skill_description[3].text = "대지를 가를듯한 기세로 땅을 3번 내려칩니다. 매번 피해량과 범위가 증가합니다.";
                break;
            case 4:
                skill_icon[0].sprite = Resources.Load<Sprite>("skillicon/17");
                skill_icon[1].sprite = Resources.Load<Sprite>("skillicon/18");
                skill_icon[2].sprite = Resources.Load<Sprite>("skillicon/19");
                skill_icon[3].sprite = Resources.Load<Sprite>("skillicon/20");
                skill_name[0].text = "샘솓는\n힘";
                skill_name[1].text = "퍼져가는\n힘";
                skill_name[2].text = "폭발의\n흐름";
                skill_name[3].text = "소생";
                skill_description[0].text = "원하는 지점에 치유의 구와 힘의 근원을 생성합니다. 치유의 구는 시간에 따라 치유량이 증가합니다.";
                skill_description[1].text = "힘의 근원을 퍼뜨려 주위의 모든 아군을 치유합니다. 치유량은 캐릭터의 마법공격력에 비례합니다.";
                skill_description[2].text = "힘의 근원을 퍼뜨려 주위의 모든 적을 공격합니다. 피해량은 캐릭터의 마법공격력에 비례합니다.";
                skill_description[3].text = "원하는 지점에 소생의 별을 소환합니다. 죽은 캐릭터는 별에 닿으면 일정한 체력과 기력으로 되살아납니다.";
                break;
            case 5:
                skill_icon[0].sprite = Resources.Load<Sprite>("skillicon/21");
                skill_icon[1].sprite = Resources.Load<Sprite>("skillicon/22");
                skill_icon[2].sprite = Resources.Load<Sprite>("skillicon/23");
                skill_icon[3].sprite = Resources.Load<Sprite>("skillicon/24");
                skill_name[0].text = "불타는\n대지";
                skill_name[1].text = "화염의\n보호";
                skill_name[2].text = "갈래화염";
                skill_name[3].text = "작렬하는\n홍염";
                skill_description[0].text = "원하는 지점에 지속되는 화염을 소환합니다. 이 범위에 존재하는 적들은 일정 시간마다 피해를 입습니다.";
                skill_description[1].text = "원하는 대상에게 화염의 보호막을 생성합니다. 근처에오는 적은 화염 피해를 입습니다.";
                skill_description[2].text = "원하는 방향으로 6개의 화염구를 발사합니다.";
                skill_description[3].text = "원하는 시간만큼 홍염을 시전합니다. 홍염의 피해량은 시전 시간에 비례합니다.";
                break;
            case 6:
                skill_icon[0].sprite = Resources.Load<Sprite>("skillicon/25");
                skill_icon[1].sprite = Resources.Load<Sprite>("skillicon/26");
                skill_icon[2].sprite = Resources.Load<Sprite>("skillicon/27");
                skill_icon[3].sprite = Resources.Load<Sprite>("skillicon/28");
                skill_name[0].text = "방패치기";
                skill_name[1].text = "찬란한\n의지";
                skill_name[2].text = "용기의\n조롱";
                skill_name[3].text = "보호의\n빛";
                skill_description[0].text = "원하는 방향에 크게 방패를 휘두릅니다. 데미지를 입히고 적의 방어력을 감소시킵니다.";
                skill_description[1].text = "일정시간 동안 적의 모든 공격으로부터 면역이 됩니다.";
                skill_description[2].text = "우렁찬 조롱이 울려퍼지면서 적의 이목을 집중시킵니다.";
                skill_description[3].text = "존재하는 모든 아군들에게 적의 공격을 무효시키는 보호막을 생성합니다.";
                break;
            case 7:
                skill_icon[0].sprite = Resources.Load<Sprite>("skillicon/29");
                skill_icon[1].sprite = Resources.Load<Sprite>("skillicon/30");
                skill_icon[2].sprite = Resources.Load<Sprite>("skillicon/31");
                skill_icon[3].sprite = Resources.Load<Sprite>("skillicon/32");
                skill_name[0].text = "고속연격";
                skill_name[1].text = "도약\n찌르기";
                skill_name[2].text = "힘의\n전환";
                skill_name[3].text = "합동일격";
                skill_description[0].text = "원하는 방향으로 빠르게 3번 검기를 발사합니다. 맞은 적은 출혈효과를 얻습니다.";
                skill_description[1].text = "원하는 방향으로 빠르게 이동하여 적을 찌릅니다. 이동중에는 적을 공격할 수 없습니다.";
                skill_description[2].text = "존재하는 모든 아군들에게 힘의 전환 효과를 부여합니다. 적에게 주는 피해만큼 체력을 얻습니다.";
                skill_description[3].text = "분신들을 소환하여 적을 공격합니다. 적을 밀치며 보스의 경우 보스의 기술을 취소시킬 수 있습니다.";
                break;
        }
    }

    private void InstanceSlots()
    {

        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].itemType == Item.ItemType.Equip)
            {
                slot_temp = Instantiate(blank_slot, content[0].transform);
                switch (itemList[i]._Rare)
                {
                    case 0:
                        slot_temp.GetComponent<Image>().color = white;
                        break;
                    case 1:
                        slot_temp.GetComponent<Image>().color = Blue;
                        break;
                    case 2:
                        slot_temp.GetComponent<Image>().color = Purple;
                        break;
                    case 3:
                        slot_temp.GetComponent<Image>().color = Orange;
                        break;
                }
            }
        }

        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].itemType == Item.ItemType.Use)
            {
                slot_temp = Instantiate(blank_slot, content[1].transform);
                switch (itemList[i]._Rare)
                {
                    case 0:
                        slot_temp.GetComponent<Image>().color = white;
                        break;
                    case 1:
                        slot_temp.GetComponent<Image>().color = Blue;
                        break;
                    case 2:
                        slot_temp.GetComponent<Image>().color = Purple;
                        break;
                    case 3:
                        slot_temp.GetComponent<Image>().color = Orange;
                        break;
                }
            }
        }

        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].itemType == Item.ItemType.ETC)
            {
                slot_temp = Instantiate(blank_slot, content[2].transform);
                switch (itemList[i]._Rare)
                {
                    case 0:
                        slot_temp.GetComponent<Image>().color = white;
                        break;
                    case 1:
                        slot_temp.GetComponent<Image>().color = Blue;
                        break;
                    case 2:
                        slot_temp.GetComponent<Image>().color = Purple;
                        break;
                    case 3:
                        slot_temp.GetComponent<Image>().color = Orange;
                        break;
                }
            }
        }

        equip_item = content[0].GetComponentsInChildren<InventorySlot>();
        use_item = content[1].GetComponentsInChildren<InventorySlot>();
        etc_item = content[2].GetComponentsInChildren<InventorySlot>();

        for (int i = 0; i < equip_item.Length; i++)
        {
            equip_item[i].gameObject.name = i.ToString();
        }
        for (int i = 0; i < use_item.Length; i++)
        {
            use_item[i].gameObject.name = i.ToString();
        }
        for (int i = 0; i < etc_item.Length; i++)
        {
            etc_item[i].gameObject.name = i.ToString();
        }
        DisplaySlots();
    }

    private void DisplaySlots()
    {
        int temp = 0;
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].itemType == Item.ItemType.Equip)
            {
                equip_item[temp++].AddItem(itemList[i], true);
            }
        }

        int temp1 = 0;
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].itemType == Item.ItemType.Use)
            {
                use_item[temp1++].AddItem(itemList[i], true);
            }
        }
        int temp2 = 0;
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].itemType == Item.ItemType.ETC)
            {
                etc_item[temp2++].AddItem(itemList[i], true);

            }
        }

    }

    private void SetDefaultItem(int index)
    {
        InformationInven[] items = scroll_view[index].GetComponentsInChildren<InformationInven>();
        switch (index)
        {
            case 0:
                for (int i = 0; i < items.Length; i++)
                {
                    if (UpLoadData.item_is_gained[i] && UpLoadData.item_info_list[i])
                    {
                        items[i].gameObject.transform.GetChild(0).GetComponent<Image>().sprite = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == i + 10001).itemIcon;
                        items[i].gameObject.transform.GetChild(0).GetComponent<Image>().material = null;
                        items[i].gameObject.transform.GetChild(0).GetComponent<Image>().color = Color.white;
                    }
                }
                break;

            case 1:
                for (int i = 0; i < items.Length; i++)
                {
                    if (UpLoadData.item_is_gained[i + 100] && UpLoadData.item_info_list[i + 100])
                    {
                        items[i].gameObject.transform.GetChild(0).GetComponent<Image>().sprite = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == i + 20001).itemIcon;
                        items[i].gameObject.transform.GetChild(0).GetComponent<Image>().material = null;
                        items[i].gameObject.transform.GetChild(0).GetComponent<Image>().color = Color.white;
                    }
                }
                break;

            case 2:
                for (int i = 0; i < items.Length; i++)
                {
                    if (UpLoadData.item_is_gained[i + 200] && UpLoadData.item_info_list[i + 200])
                    {
                        items[i].gameObject.transform.GetChild(0).GetComponent<Image>().sprite = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == i + 30001).itemIcon;
                        items[i].gameObject.transform.GetChild(0).GetComponent<Image>().material = null;
                        items[i].gameObject.transform.GetChild(0).GetComponent<Image>().color = Color.white;
                    }
                }
                break;
        }

    }


    private void DisplayItem(int index, GameObject button)
    {
        switch (item_type_index)
        {
            case 0:
                if (UpLoadData.item_is_gained[index] == false)
                {
                    switch (DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._Rare)
                    {
                        case 0:
                            item_image.color = white;
                            break;
                        case 1:
                            item_image.color = Blue;
                            break;
                        case 2:
                            item_image.color = Purple;
                            break;
                        case 3:
                            item_image.color = Orange;
                            break;
                    }
                    item_name.text = null;
                    item_description.text = null;
                    item_image.sprite = lock_icon;
                    for (int i = 0; i < item_status.Length; i++)
                    {
                        item_status[i].text = null;
                    }
                }
                else
                {
                    if (!UpLoadData.item_info_list[index])
                    {
                        item_name.color = new Color(1f, 1f, 1f, 0f);
                        item_description.color = new Color(1f, 1f, 1f, 0f);
                        item_image.color = new Color(1f, 1f, 1f, 0f);
                        for (int i = 0; i < item_status.Length; i++)
                        {
                            item_status[i].GetComponent<Text>().color = new Color(1f, 1f, 1f, 0f);
                        }
                    }
                    else
                    {
                        item_image.sprite = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001).itemIcon;
                        item_image.color = new Color(1, 1, 1, 1);
                        item_name.text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001).itemName;
                        item_description.text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001).itemDescription;
                        item_name.color = new Color(1f, 1f, 1f, 1f);
                        item_description.color = new Color(1f, 1f, 1f, 1f);
                        item_image.color = new Color(1f, 1f, 1f, 1f);
                    }

                    item_status[0].text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._HP.ToString();
                    if (DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._HP > 0)
                    {
                        item_status[0].color = Color.green;
                    }
                    else if (DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._HP < 0)
                    {
                        item_status[0].color = Color.red;
                    }
                    item_status[1].text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._MP.ToString();
                    if (DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._MP > 0)
                    {
                        item_status[1].color = Color.green;
                    }
                    else if (DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._MP < 0)
                    {
                        item_status[1].color = Color.red;
                    }
                    item_status[2].text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._PhysicalAttackPower.ToString();
                    if (DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._PhysicalAttackPower > 0)
                    {
                        item_status[2].color = Color.green;
                    }
                    else if (DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._PhysicalAttackPower < 0)
                    {
                        item_status[2].color = Color.red;
                    }
                    item_status[3].text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._MagicAttackPower.ToString();
                    if (DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._MagicAttackPower > 0)
                    {
                        item_status[3].color = Color.green;
                    }
                    else if (DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._MagicAttackPower < 0)
                    {
                        item_status[3].color = Color.red;
                    }
                    item_status[4].text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._PhysicalArmor.ToString();
                    if (DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._PhysicalArmor > 0)
                    {
                        item_status[4].color = Color.green;
                    }
                    else if (DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._PhysicalArmor < 0)
                    {
                        item_status[4].color = Color.red;
                    }
                    item_status[5].text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._MagicArmorPower.ToString();
                    if (DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._MagicArmorPower > 0)
                    {
                        item_status[5].color = Color.green;
                    }
                    else if (DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._MagicArmorPower < 0)
                    {
                        item_status[5].color = Color.red;
                    }
                    item_status[6].text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._NormalAttackRange.ToString();
                    if (DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._NormalAttackRange > 0)
                    {
                        item_status[6].color = Color.green;
                    }
                    else if (DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._NormalAttackRange < 0)
                    {
                        item_status[6].color = Color.red;
                    }
                    item_status[7].text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._NormalAttackSpeed.ToString();
                    if (DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._NormalAttackSpeed > 0)
                    {
                        item_status[7].color = Color.green;
                    }
                    else if (DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._NormalAttackSpeed < 0)
                    {
                        item_status[7].color = Color.red;
                    }
                    item_status[8].text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001).__MoveSpeed.ToString();
                    if (DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001).__MoveSpeed > 0)
                    {
                        item_status[8].color = Color.green;
                    }
                    else if (DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001).__MoveSpeed < 0)
                    {
                        item_status[8].color = Color.red;
                    }
                    item_status[9].text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._ciritical.ToString();
                    if (DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._ciritical > 0)
                    {
                        item_status[9].color = Color.green;
                    }
                    else if (DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._ciritical < 0)
                    {
                        item_status[9].color = Color.red;
                    }
                    item_status[10].text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._HP_Regenerate.ToString();
                    if (DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._HP_Regenerate > 0)
                    {
                        item_status[10].color = Color.green;
                    }
                    else if (DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._HP_Regenerate < 0)
                    {
                        item_status[10].color = Color.red;
                    }
                    item_status[11].text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._MP_Regenerate.ToString();
                    if (DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._MP_Regenerate > 0)
                    {
                        item_status[11].color = Color.green;
                    }
                    else if (DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 10001)._MP_Regenerate < 0)
                    {
                        item_status[11].color = Color.red;
                    }
                }
                SetActiveNewItemIcon(0);
                break;

            case 1:
                if (UpLoadData.item_is_gained[index + 100] == false)
                {
                    switch (DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 20001)._Rare)
                    {
                        case 0:
                            item_image.color = white;
                            break;
                        case 1:
                            item_image.color = Blue;
                            break;
                        case 2:
                            item_image.color = Purple;
                            break;
                        case 3:
                            item_image.color = Orange;
                            break;
                    }
                    item_image.sprite = lock_icon;
                    item_name.text = null;
                    item_description.text = null;
                    item_image.sprite = lock_icon;
                    for (int i = 0; i < item_status.Length; i++)
                    {
                        item_status[i].text = null;
                    }

                }
                else
                {
                    if (!UpLoadData.item_info_list[index + 100])
                    {
                        item_name.color = new Color(1f, 1f, 1f, 0f);
                        item_description.color = new Color(1f, 1f, 1f, 0f);
                        item_image.color = new Color(1f, 1f, 1f, 0f);

                        for (int i = 0; i < item_status.Length; i++)
                        {
                            item_status[i].color = new Color(1f, 1f, 1f, 0f);
                        }
                    }
                    else
                    {
                        item_image.sprite = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 20001).itemIcon;
                        item_image.color = new Color(1, 1, 1, 1);
                        item_name.text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 20001).itemName;
                        item_description.text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 20001).itemDescription;
                        item_name.color = new Color(1f, 1f, 1f, 1f);
                        item_description.color = new Color(1f, 1f, 1f, 1f);
                        item_image.color = new Color(1f, 1f, 1f, 1f);
                    }
                    //item_image.sprite = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 20001).itemIcon;
                    //item_name.text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 20001).itemName;
                    //item_description.text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 20001).itemDescription;
                    //item_status[0].text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 20001)._HP.ToString();
                    //item_status[1].text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 20001)._MP.ToString();
                    //item_status[2].text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 20001)._PhysicalAttackPower.ToString();
                    //item_status[3].text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 20001)._MagicAttackPower.ToString();
                    //item_status[4].text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 20001)._PhysicalArmor.ToString();
                    //item_status[5].text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 20001)._MagicArmorPower.ToString();
                    //item_status[6].text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 20001)._NormalAttackRange.ToString();
                    //item_status[7].text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 20001)._NormalAttackSpeed.ToString();
                    //item_status[8].text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 20001).__MoveSpeed.ToString();
                    //item_status[9].text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 20001)._ciritical.ToString();
                    //item_status[10].text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 20001)._HP_Regenerate.ToString();
                    //item_status[11].text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 20001)._MP_Regenerate.ToString();
                }
                SetActiveNewItemIcon(1);
                break;
            case 2:

                if (UpLoadData.item_is_gained[index + 200] == false)
                {
                    switch (DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 30001)._Rare)
                    {
                        case 0:
                            item_image.color = white;
                            break;
                        case 1:
                            item_image.color = Blue;
                            break;
                        case 2:
                            item_image.color = Purple;
                            break;
                        case 3:
                            item_image.color = Orange;
                            break;
                    }
                    item_image.sprite = lock_icon;
                    item_name.text = null;
                    item_description.text = null;
                    item_image.sprite = lock_icon;
                    for (int i = 0; i < item_status.Length; i++)
                    {
                        item_status[i].text = null;
                    }
                }
                else
                {
                    if (!UpLoadData.item_info_list[index + 200])
                    {
                        item_name.color = new Color(1f, 1f, 1f, 0f);
                        item_description.color = new Color(1f, 1f, 1f, 0f);
                        item_image.color = new Color(1f, 1f, 1f, 0f);
                        for (int i = 0; i < item_status.Length; i++)
                        {
                            item_status[i].GetComponent<Text>().color = new Color(1f, 1f, 1f, 0f);
                        }
                    }
                    else
                    {
                        item_image.sprite = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 30001).itemIcon;
                        item_image.color = new Color(1, 1, 1, 1);
                        item_name.text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 30001).itemName;
                        item_description.text = DatabaseManager.instance.itemDatabase.Find(item => item.itemID == index + 30001).itemDescription;
                        item_name.color = new Color(1f, 1f, 1f, 1f);
                        item_description.color = new Color(1f, 1f, 1f, 1f);
                        item_image.color = new Color(1f, 1f, 1f, 1f);
                    }
                    //for (int i = 0; i < item_status.Length; i++)
                    //{
                    //    item_status[i].color = Color.black;
                    //}
                }
                SetActiveNewItemIcon(2);
                break;
            default:
                break;
        }

    }

    // 시작시 초기화 함수
    public void InitializeObjects()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }
        panels[0].SetActive(true);
        ChangeCharacterContenet(0);
        ChangeCharacterStatus(0);
        StopAllCoroutines();
        for (int i = 0; i < title_buttons.Length; i++)
        {
            Color color = title_buttons[i].GetComponent<Image>().color;
            color.a = 1f;
            title_buttons[i].GetComponent<Image>().color = color;
        }
        StartCoroutine(SelectedTabEffect(0));
        boss_index = 0;
        panel_index = 1;
        boss_level = 0;
        item_index = 0;
        item_type_index = 0;
    }

    private void ItemInitiate()
    {
        item_index = 0;
        item_image.sprite = Resources.Load("ItemIcon/Inven/0", typeof(Sprite)) as Sprite;
        item_name.text = "";
        item_description.text = "";
        item_status[0].text = "";
        item_status[1].text = "";
        item_status[2].text = "";
        item_status[3].text = "";
        item_status[4].text = "";
        item_status[5].text = "";
        item_status[6].text = "";
        item_status[7].text = "";
        item_status[8].text = "";
        item_status[9].text = "";
        item_status[10].text = "";
        item_status[11].text = "";
    }

    private void ControlPanels(int count)
    {
        for (int i = 0; i < boss_skill_panel.Length; i++)
        {
            boss_skill_panel[i].SetActive(false);
        }

        for (int i = 0; i < count; i++)
        {
            boss_skill_panel[i].SetActive(true);
        }
    }

    private void IconClose(int count)
    {

        for (int i = 0; i < count; i++)
        {
            Color color = boss_skill_icons[i].color;
            color.a = 0;
            boss_skill_icons[i].color = color;
        }
    }

    private void IconOpen(int index)
    {

        Color color = boss_skill_icons[index].color;
        color.a = 1;
        boss_skill_icons[index].color = color;

    }
    IEnumerator SelectedTabEffect(int index)
    {
        for (int i = 0; i < title_buttons.Length; i++)
        {
            if (i == index)
                continue;

            Color color = title_buttons[i].GetComponent<Image>().color;
            color.a = 1f;
            title_buttons[i].GetComponent<Image>().color = color;
        }

        while (true)
        {
            Color color = title_buttons[index].GetComponent<Image>().color;
            while (color.a < 0.8)
            {
                color.a += 0.03f;
                title_buttons[index].GetComponent<Image>().color = color;
                yield return waitTime;
            }
            while (color.a > 0.3f)
            {
                color.a -= 0.03f;
                title_buttons[index].GetComponent<Image>().color = color;
                yield return waitTime;
            }

            yield return new WaitForSeconds(0.3f);
        }
    }


    public void UnLockSkillIcon(int index)
    {
        if (UpLoadData.boss_usedskill_list[(boss_index * 8) + boss_level][index] && !UpLoadData.boss_infoskill_list[(boss_index * 8) + boss_level][index])        {

            UpLoadData.boss_infoskill_list[(boss_index * 8) + boss_level][index] = true;
            boss_skill_lock[index].GetComponentInChildren<Image>().transform.localPosition = Vector3.zero;
            StartCoroutine(UnLockEffect(index));
            UpLoadData.new_boss_skill_count--;
            Debug.Log(UpLoadData.new_boss_skill_count);
        }
        if (UpLoadData.new_boss_skill_count >= 1)
        {
            new_boss_skill_icon.SetActive(true);
        }
        else
        {
            new_boss_skill_icon.SetActive(false);
        }
        ControlEachBossNewSkillIcon(boss_index);
    }



    IEnumerator UnLockEffect(int index)
    {
        WaitForSeconds wait_time = new WaitForSeconds(0.03f);
        Color color = boss_skill_lock[index].color;
        Color color_name_text = boss_skill_names[index].color;
        Color color_skill_text = boss_skill_descriptions[index].color;
        Color color_skill_icon = boss_skill_icons[index].color;
        float t = 1.5f;
        int flag = 1;
        destroy_effect_temp = Instantiate(destroy_effect, boss_skill_lock[index].transform.GetChild(0).transform.position, Quaternion.identity);
        while (color.a > 0f)
        {
            color.a -= 0.03f;
            boss_skill_lock[index].color = color;
            boss_skill_lock[index].transform.GetChild(0).GetComponent<Transform>().localPosition = new Vector2(0, t * flag);
            boss_skill_lock[index].transform.GetChild(0).GetComponent<Image>().color = color;
            flag *= -1;
            yield return wait_time;
        }

        while (color_name_text.a < 1f)
        {
            color_name_text.a += 0.03f;
            color_skill_text.a += 0.03f;
            color_skill_icon.a += 0.03f;
            boss_skill_names[index].color = color_name_text;
            boss_skill_descriptions[index].color = color_skill_text;
            boss_skill_icons[index].color = color_skill_icon;
            yield return wait_time;
        }
        yield return 0;
    }

    public int GetItemType()
    {
        return this.item_type_index;
    }

    private void ActivateNewSkillIcon()
    {
        if (UpLoadData.new_boss_skill_count >= 1)
        {
            new_boss_skill_icon.SetActive(true);
        }
        else
        {
            new_boss_skill_icon.SetActive(false);
        }
    }

    private void ActivateNewItemIcon()
    {
        if (UpLoadData.new_item_count >= 1)
        {
            new_item_icon.SetActive(true);
        }
        else
        {
            new_item_icon.SetActive(false);
        }
    }

    private void BlockSkillPanel(int index)
    {
        boss_skill_lock[index].gameObject.SetActive(true);
        boss_skill_lock[index].color = new Color(0.5f, 0.4f, 0.4f, 1f);
        boss_skill_lock[index].material = Shiny;
        boss_skill_lock[index].transform.GetChild(0).GetComponent<Image>().material = Shiny;
        boss_skill_lock[index].transform.GetChild(0).GetComponent<Image>().color = new Color(0.7f, 0.6f, 0.6f, 1f);
        boss_skill_name_color.a = 0;
        boss_skill_names[index].color = boss_skill_name_color;
        boss_skill_info_color.a = 0;
        boss_skill_descriptions[index].color = boss_skill_info_color;
        boss_skill_icon_color.a = 0;
        boss_skill_icons[index].color = boss_skill_icon_color;
    }

    private void UnlockSkillPanel(int index)
    {
        boss_skill_lock[index].transform.GetChild(0).GetComponent<Image>().material = null;
        boss_skill_lock[index].material = null;
        boss_skill_name_color.a = 1;
        boss_skill_names[index].color = boss_skill_name_color;
        boss_skill_info_color.a = 1;
        boss_skill_descriptions[index].color = boss_skill_info_color;
        boss_skill_icon_color.a = 1;
        boss_skill_icons[index].color = boss_skill_icon_color;
        boss_skill_lock[index].gameObject.SetActive(false);
    }

    private void NullSkillPanel(int index)
    {
        StopAllCoroutines();
        boss_skill_lock[index].gameObject.SetActive(true);
        boss_skill_icons[index].sprite = null;
        boss_skill_names[index].text = null;
        boss_skill_descriptions[index].text = null;
        boss_skill_lock[index].color = new Color(0.5f, 0.4f, 0.4f, 1f); ;
        boss_skill_lock[index].material = null;
        boss_skill_lock[index].transform.GetChild(0).GetComponent<Image>().color = new Color(0.7f, 0.6f, 0.6f, 1f); 
        boss_skill_lock[index].transform.GetChild(0).GetComponent<Image>().material = null;
    }

    private void ClickedBossButton(int index)
    {
        for(int i = 0; i < boss_index_buttons.Length; i++)
        {
            boss_index_buttons[i].image.material = null;
        }
        boss_index_buttons[index].image.material = innerGlow;
    }

    private void ClickedItemTypeButton(int index)
    {
        for (int i = 0; i < item_type_buttons.Length; i++)
        {
            item_type_buttons[i].material = null;
        }
        item_type_buttons[index].material = innerGlow;
    }

    private void ControlEachBossNewSkillIcon(int index)
    {
        int count = 0;
        for(int j = 0; j < 8; j++)
        {
            for (int i = 0; i < UpLoadData.boss_infoskill_list[(index * 8) + j].Length; i++)
            {
                if (UpLoadData.boss_infoskill_list[(index * 8) + j][i] != UpLoadData.boss_usedskill_list[(index * 8) + j][i])
                {
                    count++;
                }
            }
        }

        if (count > 0)
            new_skill_each_boss[index].SetActive(true);
        else
            new_skill_each_boss[index].SetActive(false);
    }


    public void SetActiveNewItemIcon(int index)
    {
        int count1 = 0;
        for (int i = index * 100; i < index * 100 + 100; i++)
        {
            if (UpLoadData.item_is_gained[i] != UpLoadData.item_info_list[i])
            {
                count1++;
            }
        }

        if (count1 > 0)
        {
            new_item_type_icon[index].SetActive(true);
        }
        else
        {
            new_item_type_icon[index].SetActive(false);
        }

    }

    public void IsMulti()
    {
        is_multi = !is_multi;

        if (!is_multi)
        {
            Multi_Single_Button.GetComponent<Image>().color = Color.white;
            Multi_Single_Button.GetComponentInChildren<Text>().text = "Single";
            Multi_Single_Button.GetComponentInChildren<Text>().color = new Color(0, 1, 0.9f, 1f);
        }
        else
        {
            Multi_Single_Button.GetComponent<Image>().color = new Color(0, 0.1f, 1f, 1f);
            Multi_Single_Button.GetComponentInChildren<Text>().text = "Multi";
            Multi_Single_Button.GetComponentInChildren<Text>().color = new Color(0, 1, 1f, 1f);
        }

    }

}
