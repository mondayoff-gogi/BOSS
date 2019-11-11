using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionCooltime : MonoBehaviour
{
    [HideInInspector]
    public float[] timer;
    [HideInInspector]
    public float[] cooltime;

    private float cooltime_temp;

    public GameObject[] PotionOn;

    public GameObject[] Player;
    private int char_index;

    public UISprite[] _uisprite;
    // Start is called before the first frame update
    
    private void Start()//케릭터 index마다 다른 쿨타임 가져야함
    {
        timer = new float[4];
        cooltime = new float[4];

        char_index = UpLoadData.character_index[0];

        switch (char_index)
        {
            case 0:
                cooltime[0] = DatabaseManager.instance.UseList0[0]._Cooltime;
                cooltime[1] = DatabaseManager.instance.UseList0[1]._Cooltime;
                break;
            case 1:
                cooltime[0] = DatabaseManager.instance.UseList1[0]._Cooltime;
                cooltime[1] = DatabaseManager.instance.UseList1[1]._Cooltime;
                break;
            case 2:
                cooltime[0] = DatabaseManager.instance.UseList2[0]._Cooltime;
                cooltime[1] = DatabaseManager.instance.UseList2[1]._Cooltime;
                break;
            case 3:
                cooltime[0] = DatabaseManager.instance.UseList3[0]._Cooltime;
                cooltime[1] = DatabaseManager.instance.UseList3[1]._Cooltime;
                break;
            case 4:
                cooltime[0] = DatabaseManager.instance.UseList4[0]._Cooltime;
                cooltime[1] = DatabaseManager.instance.UseList4[1]._Cooltime;
                break;
            case 5:
                cooltime[0] = DatabaseManager.instance.UseList5[0]._Cooltime;
                cooltime[1] = DatabaseManager.instance.UseList5[1]._Cooltime;
                break;
            case 6:
                cooltime[0] = DatabaseManager.instance.UseList6[0]._Cooltime;
                cooltime[1] = DatabaseManager.instance.UseList6[1]._Cooltime;
                break;
            case 7:
                cooltime[0] = DatabaseManager.instance.UseList7[0]._Cooltime;
                cooltime[1] = DatabaseManager.instance.UseList7[1]._Cooltime;
                break;
        }
        timer[0] = cooltime[0];
        timer[1] = cooltime[1];


        char_index = UpLoadData.character_index[1];
        switch (char_index)
        {
            case 0:
                cooltime[2] = DatabaseManager.instance.UseList0[0]._Cooltime;
                cooltime[3] = DatabaseManager.instance.UseList0[1]._Cooltime;
                break;
            case 1:
                cooltime[2] = DatabaseManager.instance.UseList1[0]._Cooltime;
                cooltime[3] = DatabaseManager.instance.UseList1[1]._Cooltime;
                break;
            case 2:
                cooltime[2] = DatabaseManager.instance.UseList2[0]._Cooltime;
                cooltime[3] = DatabaseManager.instance.UseList2[1]._Cooltime;
                break;
            case 3:
                cooltime[2] = DatabaseManager.instance.UseList3[0]._Cooltime;
                cooltime[3] = DatabaseManager.instance.UseList3[1]._Cooltime;
                break;
            case 4:
                cooltime[2] = DatabaseManager.instance.UseList4[0]._Cooltime;
                cooltime[3] = DatabaseManager.instance.UseList4[1]._Cooltime;
                break;
            case 5:
                cooltime[2] = DatabaseManager.instance.UseList5[0]._Cooltime;
                cooltime[3] = DatabaseManager.instance.UseList5[1]._Cooltime;
                break;
            case 6:
                cooltime[2] = DatabaseManager.instance.UseList6[0]._Cooltime;
                cooltime[3] = DatabaseManager.instance.UseList6[1]._Cooltime;
                break;
            case 7:
                cooltime[2] = DatabaseManager.instance.UseList7[0]._Cooltime;
                cooltime[3] = DatabaseManager.instance.UseList7[1]._Cooltime;
                break;
        }
        timer[2] = cooltime[2];
        timer[3] = cooltime[3];
    }

    // Update is called once per frame
    void Update()
    {
        for(int i=0;i<timer.Length;i++)
        {
            timer[i] += Time.deltaTime;
        }

        if(UpLoadData.character_index[0]==Player[0].GetComponent<CharacterStat>().char_num)
        {
            char_index = 0;
        }
        else
        {
            char_index = 1;
        }

        _uisprite[0].fillAmount = timer[char_index * 2] / cooltime[char_index * 2];
        _uisprite[1].fillAmount = timer[char_index * 2+1] / cooltime[char_index * 2 + 1];


        if (_uisprite[0].fillAmount >= 1&& cooltime[char_index * 2]!=0/*&& Player[0].GetComponent<CharacterStat>().HP>0*/)
            PotionOn[0].SetActive(true);
        else
            PotionOn[0].SetActive(false);
        if (_uisprite[1].fillAmount >= 1 && cooltime[char_index * 2+1] != 0 /*&& Player[0].GetComponent<CharacterStat>().HP > 0*/)
            PotionOn[1].SetActive(true);
        else
            PotionOn[1].SetActive(false);
      

        
    }
}
