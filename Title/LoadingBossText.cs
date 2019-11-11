using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBossText : MonoBehaviour
{
    public Text Bosstext;

    public Text TextTitle;

    public Text TextDiscription;

    private int num;
    // Start is called before the first frame update
    void Start()
    {
        Bosstext.text = "Boss " + (UpLoadData.boss_index+1).ToString();


        num=Random.Range(0, 6);

        switch(num)
        {
            case 0:
                TextTitle.text = "저장";
                TextDiscription.text = "저장은 자동입니다";
                break;
            case 1:
                TextTitle.text = "아이템";
                TextDiscription.text = "캐릭터 선택창에서 캐릭터를 길게 누르면 아이템창으로 갈 수 있습니다";
                break;
            case 2:
                TextTitle.text = "소비 아이템";
                TextDiscription.text = "캐릭터마다 각각 장비 할 수 있습니다";
                break;
            case 3:
                TextTitle.text = "게임";
                TextDiscription.text = "어려울 수 있습니다";
                break;
            case 4:
                TextTitle.text = "캔슬";
                TextDiscription.text = "Boss가 빨간색으로 변했을 때 캔슬 스킬을 사용하면 스킬을 멈출 수 있습니다";
                break;
            case 5:
                TextTitle.text = "시간 제한";
                TextDiscription.text = "시간이 지나면 Boss가 화가 납니다";
                break;
        }
    }
}
