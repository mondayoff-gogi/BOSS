﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss4_Bomb_Together : MonoBehaviour
{
    private List<GameObject> players;
    private Vector2 destination;
    private Vector2 dir;
    private GameObject[] _players;

    public GameObject explosion_effect;
    private GameObject explosion_temp;
    private float damage;

    private void Start()
    {
        players = new List<GameObject>();
        _players = GameObject.FindGameObjectsWithTag("Player");
        switch (UpLoadData.boss_level)
        {
            case 0:
                damage = 2000;
                break;
            case 1:
                damage = 3000;
                break;
            case 2:
                damage = 3500;
                break;
            case 3:
                damage = 4000;
                break;
        }

    }

    private void Update()
    {
        this.transform.position =  Vector2.Lerp(this.transform.position, destination, Time.deltaTime * 2f);

        if(Vector2.Distance(destination, this.transform.position) <= 0.2f)
        {
            GiveDamage();
            explosion_temp = Instantiate(explosion_effect, this.transform.position, Quaternion.identity);
            if(BossStatus.instance.gameObject != null)
            {
                BossStatus.instance.gameObject.GetComponent<Boss4Move>().main.GetComponent<Camera_move>().VivrateForTime(1f);
            }
            Destroy(explosion_temp, 0.5f);
            Destroy(this.gameObject);

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            players.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            players.Remove(collision.gameObject);
        }
    }

    private void GiveDamage()
    {
        if(players.Count <= 0)
        {
            for(int i = 0; i < _players.Length; i++)
            {
                if(_players[i].tag == "Player")
                _players[i].GetComponent<CharacterStat>().GetDamage(damage, true);
            }
        }
        for(int i = 0; i < players.Count; i++)
        {
            if (_players[i].tag == "Player")
                players[i].GetComponent<CharacterStat>().GetDamage(damage / players.Count, true);
        }
    }

    public void GetDestination(Vector2 des)
    {
        destination = des;
    }
}
