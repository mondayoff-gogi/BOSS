using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayerHPBar : MonoBehaviour
{
    private Camera worldCam;
    public Camera uiCam;
    private Transform target;
    public int index;

    private UISprite[] temp;


    private void Start()
    {
        if(GameManage.instance.Character_other[index]==null)//없으면
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            worldCam = Camera.main;
            temp = this.GetComponentsInChildren<UISprite>();
            target = GameManage.instance.Character_other[index].transform;
        }

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
            SetPositionHUD();

            if (this.CompareTag("HP")) this.GetComponent<UISlider>().value = GameManage.instance.Character_other[index].GetComponent<CharacterStat>().HP/GameManage.instance.Character_other[index].GetComponent<CharacterStat>().MaxHP;
            else if (this.CompareTag("MP")) this.GetComponent<UISlider>().value = GameManage.instance.Character_other[index].GetComponent<CharacterStat>().MP / GameManage.instance.Character_other[index].GetComponent<CharacterStat>().MaxMp;
        }
        if (target.tag == "OtherPlayer")
        {
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i].enabled = true;
            }
        }
        if (target.tag == "DeadPlayer")
        {
            for (int i = 0; i < temp.Length; i++)
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
        position.x = Mathf.RoundToInt(position.x) - 40;
        position.y = Mathf.RoundToInt(position.y) - 85;
        position.z = 0.0f;
        transform.localPosition = position;
    }
}
