using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2_ULT_Check : MonoBehaviour
{
    private GameObject[] blocks;
    private int count = 0;
    private GameObject boss;
    private GameObject[] players;
    private GameObject[] players_magnetic;
    private GameObject[] obj_temp;

    public GameObject normal_move;
    private GameObject normal_temp;

    // Start is called before the first frame update
    void Start()
    {
        blocks = GameObject.FindGameObjectsWithTag("Magnetic");
        players = GameObject.FindGameObjectsWithTag("Player");
        players_magnetic = new GameObject[players.Length];
        boss = GameObject.FindGameObjectWithTag("Boss");

        for (int i = 0; i < players.Length; i++)
        {
            players_magnetic[i] = players[i].GetComponentInChildren<Boss2_Player_Magnetic>().gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (count >= blocks.Length)
        {
            for(int i = 0; i < players_magnetic.Length; i++)
            {
                if(players_magnetic[i] != null)
                {
                    Destroy(players_magnetic[i]);
                }
                if(players[i].GetComponentInChildren<Boss2_Player_Magnetic>()!=null)
                {
                    if (players[i].GetComponentInChildren<Boss2_Player_Magnetic>().magnetic == false)
                    {
                        Destroy(players[i].transform.Find("N(Clone)").gameObject);
                    }
                    else
                    {
                        Destroy(players[i].transform.Find("S(Clone)").gameObject);
                    }
                }
              
            }
            boss.GetComponent<Boss2Move>().StopMagneticCor();
            boss.GetComponent<Boss2Move>().ChangeBossAct(0);
            Destroy(this.gameObject, 0.5f);
        }
    }

    public void CountBlocks()
    {
        count++;
    }
}
