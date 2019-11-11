using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{

    static public GameUI instance;

    private void Awake()
    {
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

    public GameObject healthBar;
    public GameObject ManaBar;

    // 화면에 표시할 버프 디버프 개수.
    private const int SkillNum = 6;
    private Vector3 position_ui;
    // 버프 스킬 창
    public Image[] Skill;

    public GameObject red_text;
    public GameObject green_text;
    public GameObject white_text;
    public GameObject yellow_text;
    public GameObject orange_text;
    public GameObject green_bold_text;
    public GameObject blue_text;

    public Camera worldCam;
    public Camera uiCam;

    public UILabel HpCount;
    public UILabel MpCount;


    public GameObject Potion;

    public GameObject TagButton;
    
    private BossStatus BossStatus;

    public GameObject[] refer;
    public GameObject[] skillimage;


    void Start()
    {
        Skill = new Image[SkillNum];
        
        BossStatus = BossStatus.instance.GetComponent<BossStatus>();
    }

    // Update is called once per frame
    void Update()
    {
        if (BossStatus == null) return;
        healthBar.GetComponent<UISprite>().fillAmount = (float)BossStatus.HP / (float)BossStatus.MaxHP;
        ManaBar.GetComponent<UISprite>().fillAmount = (float)BossStatus.MP / (float)BossStatus.MaxMp;

        if(GameManage.instance.selected_player == null)
        {
            return;
        }
        else
        {
            if (GameManage.instance.selected_player.tag != "Player")
            {
                Potion.GetComponent<PotionCooltime>()._uisprite[0].enabled = false;
                Potion.GetComponent<PotionCooltime>()._uisprite[1].enabled = false;
            }
            else
            {
                Potion.GetComponent<PotionCooltime>()._uisprite[0].enabled = true;
                Potion.GetComponent<PotionCooltime>()._uisprite[1].enabled = true;
            }
        }


        if (GameManage.instance.selected_player != null)
        {
            if (DatabaseManager.instance.UseList[0].itemID != 0)
                HpCount.text = "x" + DatabaseManager.instance.UseList[0].itemCount.ToString();
            else
                HpCount.text = "";
            if (DatabaseManager.instance.UseList[1].itemID != 0)
                MpCount.text = "x" + DatabaseManager.instance.UseList[1].itemCount.ToString();
            else
                MpCount.text = "";
        }
        if (GameManage.instance.player[0].GetComponent<CharacterStat>().Tag_HP < 0)
        {
            TagButton.GetComponent<UISprite>().color = Color.gray;
        }
    }

    public void FloatingWhiteDamage(Vector3 position, float damage)
    {
        Vector3 position_UI = worldCam.WorldToViewportPoint(position);
        GameObject clone = Instantiate(white_text, GameUI.instance.transform);
        clone.transform.position = uiCam.ViewportToWorldPoint(position_UI);

        position_UI = clone.transform.localPosition;
        position_UI.x = Mathf.RoundToInt(position_UI.x);
        position_UI.y = Mathf.RoundToInt(position_UI.y) + 250;
        position_UI.z = 0.0f;

        clone.transform.localPosition = position_UI;
        clone.GetComponent<FloatingText>().text = damage.ToString();
        clone.transform.SetParent(this.transform);
    }
    public void FloatingYellowDamage(Vector3 position, float damage)
    {
        Vector3 position_UI = worldCam.WorldToViewportPoint(position);
        GameObject clone = Instantiate(yellow_text, GameUI.instance.transform);
        clone.transform.position = uiCam.ViewportToWorldPoint(position_UI);

        position_UI = clone.transform.localPosition;
        position_UI.x = Mathf.RoundToInt(position_UI.x);
        position_UI.y = Mathf.RoundToInt(position_UI.y) + 250;
        position_UI.z = 0.0f;

        clone.transform.localPosition = position_UI;
        clone.GetComponent<FloatingText>().text = damage.ToString();
        clone.transform.SetParent(this.transform);
    }

    public void FloatingOrangeDamage(Vector3 position, float damage)
    {
        Vector3 position_UI = worldCam.WorldToViewportPoint(position);
        GameObject clone = Instantiate(orange_text, GameUI.instance.transform);
        clone.transform.position = uiCam.ViewportToWorldPoint(position_UI);

        position_UI = clone.transform.localPosition;
        position_UI.x = Mathf.RoundToInt(position_UI.x);
        position_UI.y = Mathf.RoundToInt(position_UI.y) + 250;
        position_UI.z = 0.0f;

        clone.transform.localPosition = position_UI;
        clone.GetComponent<FloatingText>().text = damage.ToString();
        clone.transform.SetParent(this.transform);
    }


    public void FloatingRedDamage(Vector3 position, float damage)
    {
        Vector3 position_UI = worldCam.WorldToViewportPoint(position);
        GameObject clone = Instantiate(red_text, GameUI.instance.transform);
        clone.transform.position = uiCam.ViewportToWorldPoint(position_UI);

        position_UI = clone.transform.localPosition;
        position_UI.x = Mathf.RoundToInt(position_UI.x);
        position_UI.y = Mathf.RoundToInt(position_UI.y) + 50;
        position_UI.z = 0.0f;

        clone.transform.localPosition = position_UI;
        clone.GetComponent<FloatingText>().text = damage.ToString();
        clone.transform.SetParent(this.transform);
    }
    public void FloatingRedEffect(Vector3 position, string effect)
    {
        Vector3 position_UI = worldCam.WorldToViewportPoint(position);
        GameObject clone = Instantiate(red_text, GameUI.instance.transform);
        clone.transform.position = uiCam.ViewportToWorldPoint(position_UI);

        position_UI = clone.transform.localPosition;
        position_UI.x = Mathf.RoundToInt(position_UI.x);
        position_UI.y = Mathf.RoundToInt(position_UI.y) + 50;
        position_UI.z = 0.0f;

        clone.transform.localPosition = position_UI;
        clone.GetComponent<FloatingText>().text = effect;
        clone.transform.SetParent(this.transform);
    }
    public void FloatingGreenDamage(Vector3 position, float damage)
    {
        Vector3 position_UI = worldCam.WorldToViewportPoint(position);
        GameObject clone = Instantiate(green_text, GameUI.instance.transform);
        clone.transform.position = uiCam.ViewportToWorldPoint(position_UI);

        position_UI = clone.transform.localPosition;
        position_UI.x = Mathf.RoundToInt(position_UI.x);
        position_UI.y = Mathf.RoundToInt(position_UI.y) + 50;
        position_UI.z = 0.0f;

        clone.transform.localPosition = position_UI;
        clone.GetComponent<FloatingText>().text = damage.ToString();
        clone.transform.SetParent(this.transform);
    }
    public void FloatingBlueDamage(Vector3 position, float damage)
    {
        Vector3 position_UI = worldCam.WorldToViewportPoint(position);
        GameObject clone = Instantiate(blue_text, GameUI.instance.transform);
        clone.transform.position = uiCam.ViewportToWorldPoint(position_UI);
        
        position_UI = clone.transform.localPosition;
        position_UI.x = Mathf.RoundToInt(position_UI.x);
        position_UI.y = Mathf.RoundToInt(position_UI.y) + 50;
        position_UI.z = 0.0f;

        clone.transform.localPosition = position_UI;
        clone.GetComponent<FloatingText>().text = damage.ToString();
        clone.transform.SetParent(this.transform);
    }
    public void FloatingGreenDamageBold(Vector3 position, float damage)
    {
        Vector3 position_UI = worldCam.WorldToViewportPoint(position);
        GameObject clone = Instantiate(green_bold_text, GameUI.instance.transform);
        clone.transform.position = uiCam.ViewportToWorldPoint(position_UI);

        position_UI = clone.transform.localPosition;
        position_UI.x = Mathf.RoundToInt(position_UI.x);
        position_UI.y = Mathf.RoundToInt(position_UI.y) + 50;
        position_UI.z = 0.0f;

        clone.transform.localPosition = position_UI;
        clone.GetComponent<FloatingText>().text = damage.ToString();
        clone.transform.SetParent(this.transform);
    }

    public void FloatingGreenEffect(Vector3 position, string effect)
    {
        Vector3 position_UI = worldCam.WorldToViewportPoint(position);
        GameObject clone = Instantiate(green_text, GameUI.instance.transform);
        clone.transform.position = uiCam.ViewportToWorldPoint(position_UI);

        position_UI = clone.transform.localPosition;
        position_UI.x = Mathf.RoundToInt(position_UI.x);
        position_UI.y = Mathf.RoundToInt(position_UI.y)+50;
        position_UI.z = 0.0f;

        clone.transform.localPosition = position_UI;
        clone.GetComponent<FloatingText>().text = effect;
        clone.transform.SetParent(this.transform);
    }

    public void HpPotionEnter()
    {
        int temp;
        if (GameManage.instance.selected_player.GetComponent<CharacterStat>().char_num == UpLoadData.character_index[0])
            temp = 0;
        else
            temp = 1;

        if (GameManage.instance.selected_player != null)
        {
            if (DatabaseManager.instance.UseList[0].itemID == 0 || (DatabaseManager.instance.UseList[0].itemCount == 0)) //물약있는지 체크
                return;
            
            //쿨타임인지 아닌지 체크
            if (Potion.GetComponent<PotionCooltime>().timer[temp * 2] >= Potion.GetComponent<PotionCooltime>().cooltime[temp * 2])
            {
                DatabaseManager.instance.UseList[0].itemCount--;//아이템 카운트 줄이기
                
                PotionEffect(GameManage.instance.selected_player, DatabaseManager.instance.UseList[0].itemID);
                if (DatabaseManager.instance.UseList[0].itemCount <= 0) //아이템 개수 다 없어지면
                    DatabaseManager.instance.UseList[0] = new Item(0, "", "", Item.ItemType.Use); //없어짐
                Potion.GetComponent<PotionCooltime>().timer[temp * 2] = 0;               
            }
        }
    }
    public void MpPotionEnter()
    {
        int temp;
        if (GameManage.instance.selected_player.GetComponent<CharacterStat>().char_num == UpLoadData.character_index[0])
            temp = 0;
        else
            temp = 1;

        if (GameManage.instance.selected_player != null)
        {
            if (DatabaseManager.instance.UseList[1].itemID == 0 || (DatabaseManager.instance.UseList[1].itemCount == 0)) //물약있는지 체크
                return;
            //쿨타임인지 아닌지 체크
            if (Potion.GetComponent<PotionCooltime>().timer[temp * 2+1] >= Potion.GetComponent<PotionCooltime>().cooltime[temp * 2 + 1])
            {
                DatabaseManager.instance.UseList[1].itemCount--;//아이템 카운트 줄이기
                
                PotionEffect(GameManage.instance.selected_player, DatabaseManager.instance.UseList[1].itemID);

                if (DatabaseManager.instance.UseList[1].itemCount <= 0) //아이템 개수 다 없어지면
                    DatabaseManager.instance.UseList[1] = new Item(0, "", "", Item.ItemType.Use); //없어짐
                Potion.GetComponent<PotionCooltime>().timer[temp * 2+1] = 0;
            }
        }
    }



    public void PotionEffect(GameObject Player,int itemID)
    {
        CharacterStat stat_temp = Player.GetComponent<CharacterStat>();

        switch(itemID)
        {
            case 20001:
                stat_temp.GetHeal(1000, Player);
                break;
            case 20002:
                stat_temp.GetHeal(2000, Player);
                break;
            case 20003:
                stat_temp.GetHeal(3000,Player);
                break;
            case 20004:
                stat_temp.GetHeal(3000, Player);
                break;
            case 20005:
                stat_temp.GetHeal(3000, Player);
                break;
            case 20006:
                stat_temp.GetMana(1000);
                break;
            case 20007:
                stat_temp.GetMana(2000);
                break;
            case 20008:
                stat_temp.GetMana(3000);
                break;
            case 20009:
                stat_temp.GetMana(4000);
                break;
            case 20010:
                stat_temp.GetMana(5000);
                break;
            case 20011:
                StartCoroutine(BuffPotion_Delay(15, stat_temp, 100, 20011));
                break;
            case 20012:
                StartCoroutine(BuffPotion_Delay(15, stat_temp, 100, 20012));
                break;
            case 20013:
                StartCoroutine(BuffPotion_Delay(15, stat_temp, 100, 20013));
                break;
            case 20014:
                StartCoroutine(BuffPotion_Delay(15, stat_temp, 100, 20014));
                break;
            case 20015:
                StartCoroutine(BuffPotion_Delay(15, stat_temp, 40, 20015));
                break;
            case 20016:
                StartCoroutine(BuffPotion_Delay(15, stat_temp, 30, 20016));
                break;
            case 20017:
                StartCoroutine(BuffPotion_Delay(15, stat_temp, 30, 20017));
                break;
            case 20018:
                StartCoroutine(BuffPotion_Delay(3, stat_temp, 100, 20018));
                break;

        }
    }

    IEnumerator BuffPotion_Delay(float timer, CharacterStat stat, float effect,int num)
    {
        float temp,temp1;
        GameObject prefab_temp;
        switch (num)
        {
            case 20011:
                temp = stat.PhysicalAttackPower;
                temp *= effect / 100f;
                stat.PhysicalAttackPower += temp;
                prefab_temp = Instantiate(SkillManager.instance.skill_prefab[100], stat.gameObject.transform);

                yield return new WaitForSeconds(timer);

                Destroy(prefab_temp);
                stat.PhysicalAttackPower -= temp;

                    
                break;
            case 20012:
                temp = stat.MagicAttackPower;
                temp *= effect / 100f;
                stat.MagicAttackPower += temp;
                prefab_temp = Instantiate(SkillManager.instance.skill_prefab[100], stat.gameObject.transform);

                yield return new WaitForSeconds(timer);

                Destroy(prefab_temp);
                stat.MagicAttackPower -= temp;

                    
                break;
            case 20013:
                temp = stat.PhysicalArmor;
                temp *= effect / 100f;
                stat.PhysicalArmor += temp;

                temp1 = stat.MagicArmorPower;
                temp1 *= effect / 100f;
                stat.MagicArmorPower += temp1;

                prefab_temp = Instantiate(SkillManager.instance.skill_prefab[100], stat.gameObject.transform);

                yield return new WaitForSeconds(timer);

                Destroy(prefab_temp);
                stat.PhysicalArmor -= temp;
                stat.MagicArmorPower -= temp1;
                break;
            case 20014:
                temp = stat.move_speed;
                temp *= effect / 200f;
                stat.move_speed += temp;

                temp1 = stat.Attack_speed;
                temp1 *= effect / 100f;
                stat.Attack_speed += temp1;


                prefab_temp = Instantiate(SkillManager.instance.skill_prefab[100], stat.gameObject.transform);

                yield return new WaitForSeconds(timer);

                Destroy(prefab_temp);
                stat.move_speed -= temp;
                stat.Attack_speed -= temp1;
                break;
            case 20015:
                temp = effect;
                stat.ciritical += temp;
                prefab_temp = Instantiate(SkillManager.instance.skill_prefab[100], stat.gameObject.transform);

                yield return new WaitForSeconds(timer);

                Destroy(prefab_temp);
                stat.ciritical -= temp;


                break;
            case 20016:
                temp = stat.MaxHP * effect/100;
                stat.HP_Regenerate += temp;
                prefab_temp = Instantiate(SkillManager.instance.skill_prefab[100], stat.gameObject.transform);

                yield return new WaitForSeconds(timer);

                Destroy(prefab_temp);
                stat.HP_Regenerate -= temp;


                break;
            case 20017:
                temp = stat.MaxMp * effect/100;
                stat.MP_Regenerate += temp;
                prefab_temp = Instantiate(SkillManager.instance.skill_prefab[100], stat.gameObject.transform);

                yield return new WaitForSeconds(timer);

                Destroy(prefab_temp);
                stat.MP_Regenerate -= temp;


                break;
            case 20018:
                prefab_temp = Instantiate(SkillManager.instance.skill_prefab[67], stat.gameObject.transform);

                yield return new WaitForSeconds(timer);

                Destroy(prefab_temp);


                break;
           
        }        
    }
    public bool SwitchPlayer() //바꿀때 data저장하고 
    {
        if (GameManage.instance.player[0].GetComponent<CharacterStat>().Tag_HP < 0)
        {
            return false;
        }
        StartCoroutine(SwitchPlayerCor());
        return true;
    }
    IEnumerator SwitchPlayerCor()
    {
        GameObject prefab_temp;
        GameManage.instance.player[0].tag = "DeadPlayer";
        refer[5].GetComponent<BoxCollider2D>().enabled = false;
        refer[5].GetComponent<TagPlayerHP>().timer = 0;

        float timer = 0;
        WaitForSeconds waittime;
        waittime = new WaitForSeconds(0.01f);

        GameManage.instance.player[0].GetComponent<Character_Control>().Runable = false;
        GameManage.instance.player[0].GetComponent<Character_Control>()._animator.SetBool("Running", false);
        
        refer[1].SetActive(false);
        refer[4].SetActive(false);
        GameManage.instance.player[0].GetComponent<SpriteRenderer>().material = Resources.Load("Material/HolographicTint") as Material;

        if (NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.MaterialChange("Material/AlphaIntensity","Material/HolographicTint",1.5f);
        }

        prefab_temp = Instantiate(SkillManager.instance.skill_prefab[81], GameManage.instance.player[0].transform);

        if (NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.InstantiateOtherPlayerSkill(81, prefab_temp.transform, 1.5f);
        }

        while (timer < 0.5f)
        {
            timer += Time.deltaTime;
            yield return waittime;
        }

        int[] Player_index = UpLoadData.character_index;
        BossStatus.instance.player[0].GetComponent<Animator>().SetBool("IsDead", false);


        if (/*BossStatus.instance.player[0].GetComponent<CharacterStat>().char_num == Player_index[0]*/BossStatus.instance.player[0].GetComponent<CharacterStat>().is_firstCharacter)
        {
            GameManage.instance.player[0].GetComponent<CharacterStat>().char_num = Player_index[1];
            GameManage.instance.player[0].GetComponent<Character_Control>().SetSkill(Player_index[1]);
            GameManage.instance.player[0].GetComponent<Character_Control>().PotionChange();
            SkillManager.instance.player_1_num = Player_index[1];
            GameManage.instance.player[0].GetComponent<CharacterStat>().SwitchPlayer();
        }
        else
        {
            GameManage.instance.player[0].GetComponent<CharacterStat>().char_num = Player_index[0];
            GameManage.instance.player[0].GetComponent<Character_Control>().SetSkill(Player_index[0]);
            GameManage.instance.player[0].GetComponent<Character_Control>().PotionChange();
            SkillManager.instance.player_1_num = Player_index[0];
            GameManage.instance.player[0].GetComponent<CharacterStat>().SwitchPlayer();
        }
        this.GetComponentInChildren<TagPlayerHP>().SwitchPlayerSprite();
        GameManage.instance.player[0].GetComponent<Character_Control>().SwitchPlayerStat();

        if (NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.SwitchPlayer();
        }
        //팡! 등장!
        Destroy(prefab_temp);
        prefab_temp = Instantiate(SkillManager.instance.skill_prefab[82], GameManage.instance.player[0].transform);
        if (NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.InstantiateOtherPlayerSkill(82, prefab_temp.transform, 1);
        }
        Destroy(prefab_temp, 1f);

        while (timer < 1)
        {
            timer += Time.deltaTime;
            yield return waittime;
        }
        GameManage.instance.player[0].GetComponent<Character_Control>().Runable = true;
        refer[1].SetActive(true);
        refer[4].SetActive(true);
        GameManage.instance.player[0].GetComponent<SpriteRenderer>().material = Resources.Load("Material/AlphaIntensity") as Material;
        GameManage.instance.player[0].tag = "Player";

 
    }
    public void UI_ON()
    {
        for (int i = 0; i < refer.Length; i++)
            refer[i].SetActive(true);

        Vector2 pos;
        float ratio = (float)1/540;
        switch(OptionManager.instance.SkillPos)
        {
            case 0: //left
                pos.x = 750* ratio;
                pos.y = -430 * ratio;
                refer[1].transform.position = pos; //potion

                pos.x = -850 * ratio;
                pos.y = 0;
                refer[4].transform.position = pos; //skill

                pos.x = 850 * ratio;
                pos.y = -220 * ratio;
                refer[5].transform.position = pos; //tag

                for(int i=0;i< skillimage.Length;i++)
                {
                    skillimage[i].transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                break;
            case 1://middle
                pos.x = 750 * ratio;
                pos.y = -430 * ratio;
                refer[1].transform.position = pos; //potion

                pos.x = 0;
                pos.y = -370 * ratio;                
                refer[4].transform.position = pos; //skill
                refer[4].transform.rotation = Quaternion.Euler(0, 0, 90);

                for (int i = 0; i < skillimage.Length; i++)
                {
                    skillimage[i].transform.rotation = Quaternion.Euler(0, 0, 0);
                }

                pos.x = 850 * ratio;
                pos.y = -220 * ratio;
                refer[5].transform.position = pos; //tag
                break;
            case 2://right
                pos.x = -750 * ratio;
                pos.y = -430 * ratio;
                refer[1].transform.position = pos; //potion

                pos.x = 850 * ratio;
                pos.y = 0;
                refer[4].transform.position = pos; //skill

                pos.x = -850 * ratio;
                pos.y = -220 * ratio;
                refer[5].transform.position = pos; //tag

                for (int i = 0; i < skillimage.Length; i++)
                {
                    skillimage[i].transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                break;
        }
        
    }
    public void UI_OFF()
    {
        for (int i = 0; i < refer.Length; i++)
            refer[i].SetActive(false);
    }
    public void SkillOff()
    {
        for(int i=0;i< skillimage.Length;i++)
        {
            skillimage[i].GetComponent<ButtonScript>().SkillOff();
        }
    }
}
