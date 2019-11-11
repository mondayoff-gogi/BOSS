using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ButtonScript : MonoBehaviour
{
    public int skill_num;
    public GameObject SkillCooltime;

    private GameObject player;

    private float[] cool_time;
    [HideInInspector]
    public float[] cool_time_temp;

    private int char_num;
    private int[] char_index;
    private float skill_mana;

    private int skill_temp;

    private Color color_temp;

    private bool Is_SkillOn;
    private bool Is_SkillClicked;
    // Start is called before the first frame update
    void Start()
    {
        cool_time = new float[2];
        cool_time_temp = new float[2];
        char_index = UpLoadData.character_index;
        Is_SkillOn = false; Is_SkillClicked = false;
        player = GameObject.FindGameObjectWithTag("Player");
        cool_time_temp[0] = 1000;
        cool_time_temp[1] = 1000;
        char_num = player.GetComponent<CharacterStat>().char_num;

        cool_time[0] = SkillManager.instance.skill_cooltime[4* char_index[0]+ skill_num];
        cool_time[1] = SkillManager.instance.skill_cooltime[4* char_index[1]+ skill_num];

        skill_mana = SkillManager.instance.skill_mana[4 * char_num + skill_num];

        color_temp = this.GetComponentInChildren<SpriteRenderer>().color;

        //if (player.GetComponent<CharacterStat>().is_firstCharacter) skill_temp = 0;
        //else skill_temp = 1;
        //skill_temp = 4 * skill_temp + skill_num;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.GetComponent<UISprite>().fillAmount<1)
        {
            this.GetComponent<UISprite>().color = Color.gray;
            Is_SkillOn = false;
            SkillCooltime.SetActive(false);
        }
        else if(this.gameObject.GetComponent<UISprite>().fillAmount >=1)
        {
            if (skill_mana <= player.GetComponent<CharacterStat>().MP)//마나 충분할때만 흰색
            {
                this.GetComponent<UISprite>().color = Color.white;
            }
            else
            {
                this.GetComponent<UISprite>().color = Color.gray;
            }
            Is_SkillOn = true;
            SkillCooltime.SetActive(true);
        }
        cool_time_temp[0] += Time.deltaTime;
        cool_time_temp[1] += Time.deltaTime;        


        if(char_num == player.GetComponent<CharacterStat>().char_num)
            this.gameObject.GetComponent<UISprite>().fillAmount = cool_time_temp[0] / cool_time[0];
        else
        {
            this.gameObject.GetComponent<UISprite>().fillAmount = cool_time_temp[1] / cool_time[1];
        }

    }
    public void Skillon()
    {
        if (!Is_SkillOn) return;
        if(Is_SkillClicked)
        {
            SkillOff();
            return;
        }
        Is_SkillClicked = true;
        //player.GetComponentInChildren<CharacterClick>()._collider.enabled = false;
        {
            if (player.GetComponent<CharacterStat>().MP < skill_mana) //마나 검사!!
            {
                GameUI.instance.FloatingRedEffect(player.transform.position, "MANA");
                GameUI.instance.FloatingRedEffect(Camera.main.ScreenToWorldPoint(Input.mousePosition), "MANA");
                //못쓴다고 알려줌
                return;
            }

            this.GetComponentInChildren<SpriteRenderer>().color = Color.green;
            //cool_time_temp = 0;    //0이 아니고 이걸 쓴다는 표시 해주기
            //player.GetComponent<CharacterStat>().MP -= skill_mana;  //쓸때 쿨타임 0,마나깍,is skillclicked false

            switch (skill_num)
            {
                case 0:                    
                    SkillManager.instance.Use_player1_1_Skill();
                    break;
                case 1:
                    SkillManager.instance.Use_player1_2_Skill();
                    break;
                case 2:
                    SkillManager.instance.Use_player1_3_Skill();
                    break;
                case 3:
                    SkillManager.instance.Use_player1_4_Skill();
                    break;
                //case 4:
                //    SkillManager.instance.Use_player2_1_Skill();
                //    break;
                //case 5:
                //    SkillManager.instance.Use_player2_2_Skill();
                //    break;
                //case 6:
                //    SkillManager.instance.Use_player2_3_Skill();
                //    break;
                //case 7:
                //    SkillManager.instance.Use_player2_4_Skill();
                //    break;
            }
        }
    }
    public void SkillOff()
    {
        ColorBack();
        Is_SkillClicked = false;
    }

    public void ColorBack()
    {
        this.GetComponentInChildren<SpriteRenderer>().color = color_temp;
    }

    public void Character_Change_skill_off()
    {
        Is_SkillClicked = false;
    }
}
