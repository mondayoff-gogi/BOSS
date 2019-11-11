using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveTip : MonoBehaviour
{
    private float scrollSpeed = 2f;
    private float tileWidth;
    private GameObject pannel;
    private Vector3 startPosition;
    private Text this_text;
    private string[] arrays;
    private Vector3 this_position = new Vector3(1406,0,0f);
    private bool flag = false;
    private float temp = 0;
    void Start()
    {
        this_text = this.GetComponent<Text>();
        pannel = this.gameObject.transform.parent.parent.gameObject;
        startPosition = transform.position;
        arrays = new string[3];
        arrays[0] = "원하시는 보스와 난이도를 선택해주세요.";
        arrays[1] = "원하시는 2명의 캐릭터를 선택해주세요.";
        arrays[2] = "원하시는 캐릭터를 3초간 누르면 소지품을 확인 할 수 있습니다.";
        if (pannel.name == "BossSelect")
        {
            this_text.text = arrays[0];
            tileWidth = 20f;
        }
        else
        {
            this_text.text = arrays[1];
            tileWidth = 25f;
            flag = true;
        }
    }

    void Update()
    {
        temp += Time.deltaTime;

        
        float newPosition = Mathf.Repeat(Time.time * scrollSpeed, tileWidth);
        transform.position = startPosition + Vector3.left * newPosition;

        if (temp >= 8.6f && flag)
        {
            //ChangeText();
            temp = 0;
        }
        else
        {
            return;
        }
    }

    private void ChangeText()
    {
        if(this_text.text == arrays[1])
        {
            this_text.text = arrays[2];
        }
        else
        {
            this_text.text = arrays[1];
        }
    }

    public void StopMove()
    {
        this.transform.localPosition = this_position;
    }
}
