using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss7_TurnRed : MonoBehaviour
{
    public GameObject[] players;
    private float player1_hp;
    private float player2_hp;
    private float maxhp;
    public GameObject panel;
    private float currenthp;
    // Start is called before the first frame update
    void Start()
    {
        //players = GameObject.FindGameObjectsWithTag("Player");
        DatabaseManager.instance.Init_char_stat(UpLoadData.character_index[0]);
        player1_hp = DatabaseManager.instance.MaxHP;
        DatabaseManager.instance.Init_char_stat(UpLoadData.character_index[1]);
        player2_hp = DatabaseManager.instance.MaxHP;
        maxhp = player1_hp + player2_hp;
    }

    // Update is called once per frame
    void Update()
    {
        currenthp = players[0].GetComponent<CharacterStat>().HP + players[1].GetComponent<CharacterStat>().HP;
        Color color = panel.GetComponent<Image>().color;
        color.a = 0.5f - (currenthp / (maxhp * 2f));
        if(currenthp <= 0)
        {
            currenthp = 1f;
        }
        if(color.a >= 0.5)
        {
            color.a = 0.5f;
        }
        panel.GetComponent<Image>().color = color;
    }
}
