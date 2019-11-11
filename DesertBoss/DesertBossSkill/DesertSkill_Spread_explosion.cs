using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertSkill_Spread_explosion : MonoBehaviour
{
    private GameObject[] Player;

    private GameObject _camera;

    void Start()
    {
        _camera = GameObject.FindGameObjectWithTag("MainCamera");

        _camera.GetComponent<Camera_move>().VivrateForTime(0.5f);

        Player = GameObject.FindGameObjectsWithTag("Player");

        for(int i=0;i<Player.Length;i++)
        {
            if (Player[i].tag != "Player")
                continue;
            else
                Player[i].GetComponent<CharacterStat>().GetDamage(BossStatus.instance.MagicAttackPower, true);
        }        
        Destroy(this.gameObject, 2f);
    }
}
