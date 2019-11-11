using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertBossBuff : MonoBehaviour
{
    public GameObject[] Buff_location;

    public GameObject[] buff_text; 

    public GameObject[] buff_image;

    private int buff_count_temp;

    private float[] buff_timer;

    private void Start()
    {
        buff_timer = new float[SkillManager.instance.buff_timer.Length];

        for (int i=0;i<Buff_location.Length;i++)
        {
            Buff_location[i].SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (!BossStatus.instance) return;
        buff_count_temp = 1;
        int temp = 0;
        int last_temp=0;
        if (NetworkManager.instance.is_multi)
        {
            for (int i = 0; i < BossStatus.instance.multi_buff.Count; i++)
            {
                Buff_location[temp].SetActive(true); //버프 칸 켜주고
                Buff_location[temp].GetComponent<UISprite>().spriteName = BossStatus.instance.multi_buff[i].number.ToString(); //이런식으로 번호로 스프라이트 구성하시오  ex 1 ~ 10   버프 이미지 넣어주고


                for (int j = i + 1; j < BossStatus.instance.multi_buff.Count; j++)
                {
                    if (BossStatus.instance.multi_buff[i].number == BossStatus.instance.multi_buff[j].number) //같은거 있는거 찾기
                    {
                        buff_count_temp++;
                        last_temp = j;
                    }
                }
                if (buff_count_temp > 1) //중복이있는경우
                {
                    buff_text[temp].GetComponent<UILabel>().text = buff_count_temp.ToString();

                    buff_image[temp].GetComponent<UISprite>().fillAmount = BossStatus.instance.multi_buff[last_temp].timer / SkillManager.instance.buff_timer[BossStatus.instance.multi_buff[last_temp].number];
                    i += buff_count_temp - 1;  //그 다음 버프로 넘어가주고
                    buff_count_temp = 1;
                    temp++;
                    continue;
                }
                else //중복 없는경우
                {
                    buff_text[temp].GetComponent<UILabel>().text = "";
                    buff_image[temp].GetComponent<UISprite>().fillAmount = BossStatus.instance.multi_buff[i].timer / SkillManager.instance.buff_timer[BossStatus.instance.multi_buff[i].number];
                    //Debug.Log(BossStatus.instance.multi_buff[i].timer);
                    //Debug.Log(SkillManager.instance.buff_timer[BossStatus.instance.multi_buff[i].number]);

                    temp++;
                }
            }
        }
        else
        {

            for (int i = 0; i < BossStatus.instance.buff.Count; i++)
            {
                Buff_location[temp].SetActive(true); //버프 칸 켜주고
                Buff_location[temp].GetComponent<UISprite>().spriteName = BossStatus.instance.buff[i].number.ToString(); //이런식으로 번호로 스프라이트 구성하시오  ex 1 ~ 10   버프 이미지 넣어주고



                for (int j = i + 1; j < BossStatus.instance.buff.Count; j++)
                {
                    if (BossStatus.instance.buff[i].number == BossStatus.instance.buff[j].number) //같은거 있는거 찾기
                    {
                        buff_count_temp++;
                        last_temp = j;
                    }
                }
                if (buff_count_temp > 1) //중복이있는경우
                {
                    buff_text[temp].GetComponent<UILabel>().text = buff_count_temp.ToString();

                    buff_image[temp].GetComponent<UISprite>().fillAmount = BossStatus.instance.buff[last_temp].timer / SkillManager.instance.buff_timer[BossStatus.instance.buff[last_temp].number];
                    i += buff_count_temp - 1;  //그 다음 버프로 넘어가주고
                    buff_count_temp = 1;
                    temp++;
                    continue;
                }
                else //중복 없는경우
                {
                    buff_text[temp].GetComponent<UILabel>().text = "";
                    buff_image[temp].GetComponent<UISprite>().fillAmount = BossStatus.instance.buff[i].timer / SkillManager.instance.buff_timer[BossStatus.instance.buff[i].number];
                    temp++;
                }
            }
        }

         for(int i=temp;i< Buff_location.Length;i++)
            Buff_location[i].SetActive(false); //버프 끄기
    }


    public void Bufftext()
    {

    }

    private string DescriptionBuff(int getbuff)
    {
        string text="";

        switch(getbuff)
        {
            case 0:
                text = "hello";
                break;
            case 1:
                text = "1";
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
                break;
            case 9:
                text = "its 9";
                break;
        }
        return text;
    }

}
