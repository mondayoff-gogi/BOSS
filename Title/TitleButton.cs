using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleButton : MonoBehaviour
{
    // 타이틀에서 보스 선택창으로 넘어가는 애니메이션
    public Sprite[] _ani_title_to_boss;
    public Sprite[] _ani_title_to_info;
    LevelSelect _leveltemp;
    BossSelectRotate _bossSelect;
    CharacterSelectRotate _characterSelect;
    Inventory _inven;
    Equipment _Equip;
    // 캔버스를 관리할 배열
    public Canvas[] canvas;

    // 캔버스의 버튼들을 관리할 배열
    public GameObject[] boss_Buttons;
    public GameObject boss_select_UI;
    public GameObject[] character_Buttons;

    // 캔버스 인덱스
    private int canvas_index = 0;

    // 타이들 배경화면
    private Image title_image;
    // 보스 선택 배경화면
    private Image boss_select_image;
    private Image info_image;

    // 타이틀의 버튼을 관리할 배열
    private Button[] title_buttons;
    public GameObject title_title;
    // 보스 선택 캔버스의 버튼을 관리할 배열
    public Button[] boss_selection_buttons;
    public GameObject boss_title;
    // 캐릭터 선택 캔버스의 버튼을 관리할 배열
    public GameObject character_title;

    // 인벤토리 캔버스의 버튼을 관리할 배열
    private Button[] inventory_Buttons;
    // 도감 선택 캔버스의 버튼을 관리할 배열
    private Button[] info_buttons;
    // 옵션 캔버스의 버튼을 관리할 배열
    private Button option_button;
    // 타이틀 버튼 이미지
    public Image[] title_buttons_image;

    // 8개의 보스 애니메이션을 보관하는 배열
    private Animator[] boss_animator;
    // 8개의 캐릭터 애니메이션을 보관하는 배열
    private Animator[] character_animator;

    // 보스 인덱스
    public int boss_index = 9;
    // 보스가 선택됬는지 알 수 있는 불리언 배열
    public bool boss_is_selected = false;

    private int temp = 0;

    // 첫 보스의 이미지들을 보관하는 배열
    private Sprite[] boss_image;
    // 첫 캐릭터들의 이미지를 보관하는 배열
    public Sprite[] character_image;

    // 버튼이 눌린것을 확인하는 변수
    //private bool is_Pressed = false;
    public bool IsLevelSelect = false;
    //// 캐릭터 인덱스
    public int[] character_index;

    public GameObject warning_panel1;
    public GameObject warning_panel2;
    public GameObject warning_panel3;

    public GameObject fade_out;

    public LevelSelect level_select;

    public Slider[] _sliders;

    public GameObject[] level;

    private float max_item_count;

    public GameObject warning_pannel;
    private Text warning_text;

    private MoveTip boss_tip;
    private MoveTip character_tip;

    public Image[] character_backGround;
    public GameObject new_icon;

    public GameObject boss_foothold;

    public GameObject multiplay_panel;
    public GameObject multiplay_lobby_panel;
    public GameObject Error_panel;

    public GameObject boss_level_select;
    public GameObject NetWorkobject;

    public Text player_name;
    public Text[] player_name_text;
    public Text cur_player;
    private void Start()
    {
        Debug.Log("title again?");
        NetworkManager.instance.player_name = player_name;
        NetworkManager.instance.players_name_text = player_name_text;
        NetworkManager.instance.current_player_count = cur_player;

        _inven = canvas[3].GetComponentInChildren<Inventory>();
        _Equip = canvas[3].GetComponentInChildren<Equipment>();

        boss_animator = new Animator[boss_Buttons.Length];
        boss_selection_buttons = canvas[1].GetComponentsInChildren<Button>();
        boss_select_image = canvas[1].GetComponentInChildren<Image>();
        boss_image = new Sprite[boss_Buttons.Length];
        _leveltemp = canvas[1].GetComponentInChildren<LevelSelect>();
        boss_tip = canvas[1].GetComponentInChildren<MoveTip>();
        _bossSelect = canvas[1].GetComponent<BossSelectRotate>();
        character_index = new int[2];
        character_image = new Sprite[character_Buttons.Length];
        character_animator = new Animator[character_Buttons.Length];
        character_tip = canvas[2].GetComponentInChildren<MoveTip>();
        _characterSelect = canvas[2].GetComponent<CharacterSelectRotate>();
        max_item_count = DatabaseManager.instance.max_item_count;
        title_buttons = canvas[0].GetComponentsInChildren<Button>();
        title_image = canvas[0].GetComponentInChildren<Image>();
        inventory_Buttons = canvas[3].GetComponentsInChildren<Button>();
        info_image = canvas[4].GetComponentInChildren<Image>();
        info_buttons = canvas[4].GetComponentsInChildren<Button>();
        option_button = canvas[5].GetComponentInChildren<Button>();
        warning_text = warning_pannel.GetComponentInChildren<Text>();
        // 캔버스 초기화
        for (int i = 1; i < canvas.Length; i++)
        {
            canvas[i].enabled = false;
        }

        // 보스 버튼과 캐릭터 버튼 초기화
        for (int i = 0; i < boss_selection_buttons.Length; i++)
        {
            boss_selection_buttons[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < inventory_Buttons.Length; i++)
        {
            inventory_Buttons[i].enabled = false;
        }

        // 나머지 초기화
        for (int i = 0; i < boss_Buttons.Length; i++)
        {
            boss_animator[i] = boss_Buttons[i].GetComponentInChildren<Animator>();
            boss_animator[i].enabled = false;
            //boss_level[i].SetActive(false);

            boss_image[i] = boss_Buttons[i].GetComponentInChildren<Image>().sprite;
            
            character_animator[i] = character_Buttons[i].GetComponentInChildren<Animator>();
            character_animator[i].enabled = false;
            character_image[i] = character_Buttons[i].GetComponentInChildren<Image>().sprite;
        }

        // 캐릭터 인덱스 초기화
        for (int i = 0; i < character_index.Length; i++)
        {
            character_index[i] = 0;
        }
        SetDeActivateInfoButtons();

        if (title_buttons[0].IsActive() == false)
        {
            SetDeActivateTitleButtons();
        }
        BGMManager.instance.Play(0);
        ActivateNewIcon();
    }

    public void StartGame()
    {
        SoundManager.instance.Play(34);

        SetDeActivateTitleButtons();

        StartCoroutine(TitleToBoss(0.1f));

        canvas[1].gameObject.SetActive(true);
    }

    public void GoToInfo()
    {
        SoundManager.instance.Play(34);
        SetActivateInfoButtons();

        StartCoroutine(TitleToInfo(0.1f));
    }

    public void InfoToTitle()
    {
        SoundManager.instance.Play(35);
        SetDeActivateInfoButtons();
        SetDeActivateTitleButtons();
        DatabaseManager.instance.SaveData();
        StartCoroutine(InfoToTitle(0.1f));
    }

    public void BackToTitleButton()
    {
        SoundManager.instance.Play(35);
        SetDeActivateBossButtons();

        StartCoroutine(BossToTitle(0.1f));
    }

    public void BossToCharacterButton()
    {
        StartCoroutine(BossToCharacter());
    }

    public void CharacterToBossButton()
    {
        SoundManager.instance.Play(35);        
        

        StartCoroutine(CharacterToBoss());
    }

    public void InventoryToCharacterButton()
    {
        SoundManager.instance.Play(35);

        SetDeActivateInventoryButtons();
        StartCoroutine(InventoryToCharacter());
    }

    public void CharaterToInventory()
    {
        SoundManager.instance.Play(35);

        StartCoroutine(CharacterToInventory1());
    }

    public void CharaterToInventory2()
    {
        SoundManager.instance.Play(35);

        StartCoroutine(CharacterToInventory2());
    }

    public void CharacterToBattle()
    {
        StartCoroutine(GoToNextScene());
    }

    public void TitleToOption()
    {
        SoundManager.instance.Play(34);

        SetActivateOptionButtons();
        StartCoroutine(GoToOption(0.1f));
    }

    public void OptionToTitle()
    {
        SoundManager.instance.Play(35);

        SetDeActivateOptionButtons();
        StartCoroutine(OptionToTitle(0.1f));
    }

    public void StartBossAnimation()
    {
        SoundManager.instance.Play(38);

        IsLevelSelect = false;
        for (int i = 0; i < boss_animator.Length; i++)
        {
            if (boss_is_selected)
            {
                boss_animator[i].enabled = true;

            }
            else
            {
                boss_animator[i].enabled = false;
                //boss_level[i].SetActive(false);
                boss_Buttons[i].GetComponentInChildren<Image>().sprite = boss_image[i];

            }
        }
    }


    IEnumerator TitleToBoss(float speed)
    {
        temp = canvas_index;
        for (int i = 0; i < _ani_title_to_boss.Length; i++)
        {
            title_image.sprite = _ani_title_to_boss[i];
            yield return new WaitForSeconds(speed);
        }
        title_image.sprite = _ani_title_to_boss[0];
		_bossSelect.SetActive();

        canvas[temp].enabled = false;
        temp += 1;
        if (NetworkManager.instance.is_multi)
        {
            warning_panel3.SetActive(true);
        }
        else
        {
            warning_panel3.SetActive(false);
        }
        // 캔버스 이미지 초기화
        SetActivateBossButtons();
        boss_tip.enabled = true;
        canvas[temp].enabled = true;
        boss_foothold.SetActive(true);
        canvas_index = temp;
    }

    IEnumerator TitleToInfo(float speed)
    {
        temp = canvas_index;
        for (int i = 0; i < _ani_title_to_info.Length; i++)
        {
            title_image.sprite = _ani_title_to_info[i];
            yield return new WaitForSeconds(speed);
        }
        title_image.sprite = _ani_title_to_boss[0];
        canvas[temp].enabled = false;
        SetDeActivateTitleButtons();
        temp = 4;
        canvas[temp].enabled = true;
        canvas[temp].GetComponent<Information>().InitializeObjects();
        canvas_index = temp;
    }

    IEnumerator GoToOption(float speed)
    {
        temp = canvas_index;
        for (int i = 0; i < _ani_title_to_info.Length; i++)
        {
            title_image.sprite = _ani_title_to_info[i];
            yield return new WaitForSeconds(speed);
        }
        title_image.sprite = _ani_title_to_boss[0];
        canvas[temp].enabled = false;
        SetDeActivateTitleButtons();
        temp = 5;
        canvas[temp].enabled = true;
        canvas_index = temp;

    }

    IEnumerator InfoToTitle(float speed)
    {
        SetActivateTitleButtons();
        canvas[4].GetComponent<Information>().InitializeObjects();
        temp = canvas_index;
        canvas[temp].enabled = false;
        temp = 0;
        canvas[temp].enabled = true;
        canvas_index = temp;
        ActivateNewIcon();
        yield return 0;
    }

    IEnumerator OptionToTitle(float speed)
    {
        SetActivateTitleButtons();
        temp = canvas_index;
        canvas[temp].enabled = false;
        temp = 0;
        canvas[temp].enabled = true;
        canvas_index = temp;
        ActivateNewIcon();
        yield return 0;
    }

    IEnumerator BossToTitle(float speed)
    {
        temp = canvas_index;

        boss_foothold.SetActive(false);

        for (int i = 0; i < boss_Buttons.Length; i++)
        {
            // 보스 애니메이션 끄기
            boss_animator[i].enabled = false;
            //boss_level[i].SetActive(false);
            boss_is_selected = false;

            // 보스 이미지 초기화
        }
        if (boss_index < 8)
            boss_Buttons[boss_index].GetComponentInChildren<Image>().sprite = boss_image[boss_index];
        else
            yield return 0;
            
         _bossSelect.DeActive();
            
        // 클릭 확인 배열 초기화
        
        for (int i = _ani_title_to_boss.Length - 1; i >= 0; --i)
        {
            yield return new WaitForSeconds(speed);
            boss_select_image.sprite = _ani_title_to_boss[i];
        }
        canvas[temp].enabled = false;
        // 캔버스 이미지 초기화
        boss_select_image.sprite = _ani_title_to_boss[_ani_title_to_boss.Length - 1];
        temp -= 1;
        canvas[temp].enabled = true;
        level_select.ResetButtons();
        SetActivateTitleButtons();
        canvas_index = temp;
        boss_tip.StopMove();
        ActivateNewIcon();
        boss_tip.enabled = false;

        boss_index = 9;
    }

    // 나중에 책장 넘기는 이펙트 넣어 줄것
    IEnumerator BossToCharacter()
    {
        temp = canvas_index;
        //if (NetworkManager.instance.is_multi)
        //    IsLevelSelect = true;
        if (boss_is_selected && IsLevelSelect)  // 보스와 레벨이 선택 되었을때
        {
            SoundManager.instance.Play(34);

            IsLevelSelect = false;
            canvas[temp].enabled = false;
            temp += 1;
            canvas[temp].enabled = true;
            if (boss_index < 8)
            	boss_Buttons[boss_index].GetComponentInChildren<Image>().sprite = boss_image[boss_index];
            else
                yield return 0;
            boss_animator[boss_index].enabled = false;
            SetDeActivateBossButtons();
            canvas_index = temp;
			_leveltemp.DeActivate();
            boss_tip.enabled = false;
            boss_tip.StopMove();
            _characterSelect.SetActive();
            _bossSelect.DeActive();
            character_tip.enabled = true;
            canvas[1].gameObject.SetActive(false);
            canvas[2].gameObject.SetActive(true);
            if (NetworkManager.instance.is_multi)
            {
                NetworkManager.instance.BossindexPlus(true);
            }
                

            yield return new WaitForSeconds(0);
        }
        else if(boss_is_selected && !IsLevelSelect)
        {
            SoundManager.instance.Play(36);

            warning_panel1.SetActive(true);
            warning_panel1.GetComponentInChildren<Text>().text = "난이도를 선택해주세요.";
            warning_panel1.GetComponent<Canvas>().overrideSorting = true;
        }
        else
        {
            SoundManager.instance.Play(36);

            warning_panel1.SetActive(true);
            warning_panel1.GetComponentInChildren<Text>().text = "보스를 선택해 주세요.";
        }
    
    }

    IEnumerator CharacterToBoss()
    {
        temp = canvas_index;
        if (NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.BossindexPlus(false);
        }

        //// 캐릭터 인덱스 초기화
        //first_character_index = 0;
        //second_character_index = 0;
        canvas[temp].enabled = false;
        temp -= 1;
        canvas[temp].enabled = true;
        _characterSelect.SetDeActive();
        for (int i = 0; i < character_index.Length; i++)
        {
            character_index[i] = 9;
        }

        canvas[1].gameObject.SetActive(true);
        canvas[2].gameObject.SetActive(false);
        _bossSelect.SetActive();
        SetActivateBossButtons();
        _bossSelect.SetActive();

        canvas_index = temp;
        level_select.ResetButtons();
        character_tip.StopMove();
        character_tip.enabled = false;

        yield return new WaitForSeconds(0);
    }

    IEnumerator GoToNextScene()
    {
        if (character_index[0] != character_index[1])
        {
            if(DatabaseManager.instance.itemList.Count == max_item_count)
            {
                warning_text.text = "아이템 소지 개수가 아이템 소지 한도를 넘어서 더 이상 아이템을 획득 할 수 없습니다.\n 계속 하시겠습니까?";
                warning_pannel.SetActive(true);
                yield return 0;
            }
            else
            {
                NextScene();
            }
        }
        else
        {
            SoundManager.instance.Play(36);

            warning_panel2.SetActive(true);
            yield return 0;
        }
    }

    IEnumerator CharacterToInventory1()
    {
        SoundManager.instance.Play(34);

        SetDeActivateInventoryButtons();
        temp = canvas_index;
        canvas[temp].gameObject.SetActive(false);
        temp += 1;
        canvas[temp].gameObject.SetActive(true);
        canvas[temp].GetComponentInChildren<Equipment>().EquipOn();
        canvas[temp].enabled = true;
        DatabaseManager.instance.EquipSort(character_index[0]);
        _inven.GetCharacterIndex1();
        _Equip.GetCharacterIndex(character_index[0]);
        canvas_index = temp;
        //is_Pressed = false;
        yield return 0;
    }

    IEnumerator CharacterToInventory2()
    {
        SoundManager.instance.Play(34);

        SetDeActivateInventoryButtons();
        temp = canvas_index;
        canvas[temp].gameObject.SetActive(false);
        temp += 1;
        canvas[temp].gameObject.SetActive(true);
        canvas[temp].GetComponentInChildren<Equipment>().EquipOn();
        canvas[temp].enabled = true;
        DatabaseManager.instance.EquipSort(character_index[1]);
        _inven.GetCharacterIndex2();
        _Equip.GetCharacterIndex(character_index[1]);
        canvas_index = temp;
        //is_Pressed = false;
        yield return 0;
    }


    IEnumerator InventoryToCharacter()
    {
        temp = canvas_index;
        canvas[temp].enabled = false;
        canvas[temp].gameObject.SetActive(false);
        temp -= 1;
        canvas[temp].gameObject.SetActive(true);
        canvas[temp].enabled = true;
        canvas_index = temp;
        yield return new WaitForSeconds(0);

    }

    private void SetActivateTitleButtons()
    {
        title_title.SetActive(true);
        for (int i = 0; i < title_buttons.Length; i++)
        {
            title_buttons[i].gameObject.SetActive(true);
        }
        new_icon.SetActive(true);
    }

    private void SetDeActivateTitleButtons()
    {
        title_title.SetActive(false);

        for (int i = 0; i < title_buttons.Length; i++)
        {
            title_buttons[i].gameObject.SetActive(false);
        }
        new_icon.SetActive(false);
    }

    private void SetActivateBossButtons()
    {
        boss_title.SetActive(true);
        for (int i = 0; i < boss_selection_buttons.Length; i++)
        {
            boss_selection_buttons[i].gameObject.SetActive(true);
        }
       
    }

    private void SetDeActivateBossButtons()
    {
        boss_title.SetActive(false);
       
        for (int i = 0; i < boss_selection_buttons.Length; i++)
        {
            boss_selection_buttons[i].gameObject.SetActive(false);
        }

    }



    private void SetDeActivateInventoryButtons()
    {
        for (int i = 0; i < inventory_Buttons.Length; i++)
        {
            inventory_Buttons[i].enabled = !inventory_Buttons[i].IsActive();
        }
    }

    private void SetDeActivateInfoButtons()
    {
        for (int i = 0; i < info_buttons.Length; i++)
        {
            info_buttons[i].enabled = false;
        }
    }
    private void SetActivateInfoButtons()
    {
        for (int i = 0; i < info_buttons.Length; i++)
        {
            info_buttons[i].enabled = true;
        }
    }
    private void SetActivateOptionButtons()
    {
        option_button.gameObject.SetActive(true);
        for (int i = 0; i < _sliders.Length; i++)
        {
            _sliders[i].gameObject.SetActive(true);
        }
    }

    private void SetDeActivateOptionButtons()
    {
        option_button.gameObject.SetActive(false);

        for (int i = 0; i < _sliders.Length; i++)
        {
            _sliders[i].gameObject.SetActive(false);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void CloseBossWarning()
    {
        warning_panel1.SetActive(false);
    }

    public void CloseMultiWarning()
    {
        warning_panel3.SetActive(false);
    }

    public void CloseCharacterWarning()
    {
        warning_panel2.SetActive(false);
    }

    public void NextScene()
    {
        if(NetworkManager.instance.is_multi) //싱글플레이
        {

            StartCoroutine(OKNextScene());
        }
        else //멀티플레이
        {

            StartCoroutine(OKNextScene());
        }
    }

    public void Cancel()
    {
        warning_pannel.SetActive(false);
    }

    public void StartMulti()
    {
        //NetworkManager.instance.ConnectToUDPServer(NetworkManager.instance.serverPort, NetworkManager.instance.clientPort);
        NetworkManager.instance.is_multi = true;
        multiplay_panel.SetActive(true);
    }

    public void CancleMulti()
    {
        NetworkManager.instance.is_multi = false;
        multiplay_panel.SetActive(false);
    }

    public void MultiLobby()
    {
        if(player_name.text.Length<=0|| player_name.text.Length > 10)
        {
            Error_panel.SetActive(true);
            return;
        }
        multiplay_panel.SetActive(false);
        multiplay_lobby_panel.SetActive(true);
        NetworkManager.instance.EmitJoin();
    }
    public void CancleErrorPanel()
    {
        Error_panel.SetActive(false);        
    }
    public void CancleMultiLobby()
    {
        NetworkManager.instance.GameOver();
        NetworkManager.instance.is_multi = false;
        multiplay_lobby_panel.SetActive(false);
        
    }

    public void MutiPlayStart()
    {
        multiplay_lobby_panel.SetActive(false);
    }
    private void ActivateNewIcon()
    {
        if (UpLoadData.new_boss_skill_count + UpLoadData.new_item_count >= 1)
        {
            new_icon.SetActive(true);
        }
        else
        {
            new_icon.SetActive(false);
        }
    }


    IEnumerator OKNextScene()
    {
        SoundManager.instance.Play(34);

        WaitForSeconds waittime;
        waittime = new WaitForSeconds(0.1f);

        if (!NetworkManager.instance.is_multi) //싱글플레이
        {
            canvas_index = 0;
            UpLoadData.boss_index = this.boss_index;
            UpLoadData.character_index = this.character_index;
            fade_out.gameObject.SetActive(true);
            fade_out.GetComponent<Canvas>().sortingLayerName = "BackGround";
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene("Loading");
            SetDeActivateTitleButtons();
            yield return 0;
        }
        else //멀티플레이시
        {
            bool ready_On;
            while(true)
            {
                ready_On = true;
                for (int i=0;i<NetworkManager.instance.Player_num;i++)
                {
                    //ready_On = NetworkManager.instance.CharSelect[i] && ready_On;
                }

                if(ready_On)//모두 ㄹㄷ인가
                {
                    canvas_index = 0;
                    UpLoadData.boss_index = NetworkManager.instance.boss_index;
                    UpLoadData.character_index = this.character_index;

                    NetworkManager.instance.char_index[NetworkManager.instance.my_index,0] = this.character_index[0];
                    NetworkManager.instance.char_index[NetworkManager.instance.my_index,1] = this.character_index[1];

                    UpLoadData.boss_level = NetworkManager.instance.level_index+4;   // 4부터 멀티플레이 보스 전용 레벨
                    fade_out.gameObject.SetActive(true);
                    fade_out.GetComponent<Canvas>().sortingLayerName = "BackGround";
                    yield return new WaitForSeconds(1f);
                    SceneManager.LoadScene("Loading");
                    SetDeActivateTitleButtons();
                    yield return 0;
                }

                yield return waittime;
            }
        }
    }
}
