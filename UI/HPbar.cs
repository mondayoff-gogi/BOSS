using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPbar : MonoBehaviour
{
    public Camera worldCam;
    public Camera uiCam;
    public Transform target;


    private UISprite[] temp;

    private float hp_temp;
    private float mp_temp;

    private void Start()
    {
        temp = this.GetComponentsInChildren<UISprite>();
        hp_temp = target.GetComponent<CharacterStat>().HP;
        mp_temp = target.GetComponent<CharacterStat>().MP;
    }
    // Update is called once per frame
    void Update()
    {
        if (GameManage.instance.IsGameEnd)
        {
            this.gameObject.SetActive(false);
        }
        if (target)
        {
            //SetPositionHUD();
            if (this.CompareTag("HP"))
            {
                if (hp_temp > (int)target.GetComponent<CharacterStat>().HP)
                {
                    hp_temp -= (hp_temp - target.GetComponent<CharacterStat>().HP)/2f;
                }
                else if (hp_temp < (int)target.GetComponent<CharacterStat>().HP)
                {
                    hp_temp += (target.GetComponent<CharacterStat>().HP- hp_temp) / 2f;
                }
                this.GetComponent<UISlider>().value = hp_temp / target.GetComponent<CharacterStat>().MaxHP;
            }
            else if (this.CompareTag("MP"))
            {
                if (mp_temp > (int)target.GetComponent<CharacterStat>().MP)
                {
                    mp_temp -= (mp_temp - target.GetComponent<CharacterStat>().MP) / 2f;
                }
                else if (mp_temp < (int)target.GetComponent<CharacterStat>().MP)
                {
                    mp_temp += (target.GetComponent<CharacterStat>().MP- mp_temp) / 2f;
                }
                this.GetComponent<UISlider>().value = mp_temp / target.GetComponent<CharacterStat>().MaxMp;
            }

        }
       
        if(target.tag=="Player")
        {
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i].enabled = true;
            }
        }

        if (target.tag == "DeadPlayer")
        {
            for(int i = 0; i < temp.Length; i++)
            {
                temp[i].enabled = false;
            }
        }
    }
    void SetPositionHUD()
    {
        //targetposition을 게임카메라의 viewPort 좌표로 변경. 
        Vector3 position = worldCam.WorldToViewportPoint(target.position);

        //해당 좌표를 uiCamera의 World좌표로 변경. 
        transform.position = uiCam.ViewportToWorldPoint(position);
        //값 정리. 
        position = transform.localPosition;
        position.x = Mathf.RoundToInt(position.x)-40;
        position.y = Mathf.RoundToInt(position.y)-85;
        position.z = 0.0f;
        transform.localPosition = position;
    }

    public void SetActive() { }
}
