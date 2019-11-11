﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BossSelectRotate : MonoBehaviour
{
    public GameObject[] BossSprite;
    public TitleButton _title;
    public LevelSelect _level;
    public GameObject foothold;
    public Material shiny;
    private Material default_matarial;

    private Vector3 pos;
    private const float Distance = 5f;
    private const float Distance_y = 2.5f;
    private const float Y_Size = 0.4f;

    private Vector2 Post_pos;
    private Vector2 Cur_pos;

    private Vector2 Save_pos;
    private Vector2 Moving_pos;
    private float Move_distance;
    private const float MoveSpeed=0.5f;
    private float[] RotateSave;

    private bool is_Swape;
    public bool Is_Adjust;

    private float power;

    private int count;
    private Vector2[] Post_count;
    public Image outline;
    private Vector2 _pos_ray;
    private bool is_touching_layer = false;
    // Start is called before the first frame update

    void Start()
    {
        count = 0;
        is_Swape = false;
        RotateSave = new float[BossSprite.Length];
        Post_count = new Vector2[20];
        SetDefault();
        Is_Adjust = true;
        default_matarial = foothold.GetComponent<SpriteRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.main.GetComponent<TitleButton>().warning_panel1.activeSelf || Camera.main.GetComponent<TitleButton>().warning_panel2.activeSelf)
        {
            return;
        }


        if (Input.GetMouseButtonDown(0)) //누르는 순간
        {
            _pos_ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Physics2D.Raycast(_pos_ray, Vector2.zero, 1000, 1 << 26))
            {
                is_touching_layer = true;
                return;
            }
            is_Swape = false;
            Is_Adjust = false;
            count = 0;

            Save_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            outline.GetComponent<Image>().color = Color.black;
            for (int i = 0; i < Post_count.Length; i++)
            {
                Post_count[i] = Save_pos;
            }
            for (int i = 0; i < BossSprite.Length; i++)
            {
                BossSprite[i].GetComponentInChildren<Animator>().enabled = false;
                BossSprite[i].GetComponentInChildren<Animator>().GetComponent<Image>().color = new Color(0.2f,0.2f,0.2f,1f);
            }

            foothold.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
            foothold.GetComponent<SpriteRenderer>().material = default_matarial;
        }
        if (Input.GetMouseButton(0) && !is_touching_layer) //누르는동안
        {
            Moving_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Post_count[count++] = Moving_pos;
            if (count > Post_count.Length - 1)
                count = 0;
            outline.GetComponent<Image>().color = Color.black;
            Move_distance = Moving_pos.x - Save_pos.x;
            Move_distance *= MoveSpeed;
            for (int i = 0; i < BossSprite.Length; i++)
            {
                pos.x = Distance * Mathf.Cos(Move_distance + RotateSave[i]);
                pos.y = Distance_y * Mathf.Sin(Move_distance + RotateSave[i]);
                pos.y *= Y_Size;
                pos.z = pos.y;
                BossSprite[i].transform.position = pos;
                BossSprite[i].transform.localScale = 0.5f * Vector3.one * (4 - BossSprite[i].transform.position.y);

            }
            _title.boss_is_selected = false;
            _level.ResetButtons();
        }
        if (Input.GetMouseButtonUp(0)) //마우스 때는순간
        {
            for (int i = 0; i < BossSprite.Length; i++)
            {
                RotateSave[i] = RotateSave[i] + Move_distance;
            }
            Cur_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (count == Post_count.Length - 1)
                count = -1;

            power = Cur_pos.x - Post_count[count + 1].x; //힘
            power *= 0.05f;
            if(!is_touching_layer)
                is_Swape = true;
            is_touching_layer = false;
        }
        if (is_Swape)
        {
            if (power<0)
                power += Time.deltaTime;
            else
                power -= Time.deltaTime;

            if (Mathf.Abs(power)<= 0.1f)
            {
                is_Swape = false;
                Is_Adjust = true;
            }
            for (int i = 0; i < BossSprite.Length; i++)
            {
                pos.x = Distance * Mathf.Cos(power + RotateSave[i]);
                pos.y = Distance_y * Mathf.Sin(power + RotateSave[i]);
                pos.y *= Y_Size;
                pos.z = pos.y;
                BossSprite[i].transform.position = pos;
                BossSprite[i].transform.localScale = 0.5f * Vector3.one * (4 - BossSprite[i].transform.position.y);
            }
            for (int i = 0; i < BossSprite.Length; i++)
            {
                RotateSave[i] = RotateSave[i] + power;
            }
        }
        if(Is_Adjust)
        {
            int below=0;
            float min_y = 99999;    
            float dir = 0.02f;
            for(int i=0;i<BossSprite.Length;i++) //가장 아래것 뽑음
            {
                if(BossSprite[i].transform.position.y< min_y)
                {
                    below = i;
                    min_y = BossSprite[i].transform.position.y;
                    outline.GetComponent<Image>().color = Color.red;
                }
            }

            if (BossSprite[below].transform.position.x>0) //오른쪽에 있으면 왼쪽으로 보내야함
            {
                dir = -0.02f;
            }
            else //왼쪽에 있으니 오른쪽으로
            {

            }
            for (int i = 0; i < BossSprite.Length; i++) //i 가 완전 아래로 가도록
            {
                pos.x = Distance * Mathf.Cos(dir+RotateSave[i]);
                pos.y = Distance_y * Mathf.Sin(dir+RotateSave[i]);
                pos.y *= Y_Size;
                pos.z = pos.y;
                BossSprite[i].transform.position = pos;
                BossSprite[i].transform.localScale = 0.5f * Vector3.one * (4 - BossSprite[i].transform.position.y);
            }
            for (int i = 0; i < BossSprite.Length; i++)
            {
                RotateSave[i] = RotateSave[i] + dir;
            }
            
            BossSprite[below].GetComponentInChildren<Animator>().enabled = true;
            BossSprite[below].GetComponentInChildren<Animator>().GetComponent<Image>().color = Color.white;

            if (Mathf.Abs(BossSprite[below].transform.position.x) < 0.1f)
            {
                Is_Adjust = false;
            }
            foothold.GetComponent<SpriteRenderer>().color = Color.white;
            foothold.GetComponent<SpriteRenderer>().material = shiny;
            _title.boss_is_selected = true;
            _title.boss_index = below;
            _level.LockLevelButtons();
        }
    }

    private void SetDefault()
    {
        for (int i = 0; i < BossSprite.Length; i++)
        {   
            pos.x = Distance * Mathf.Cos(2 * (i - 2) * Mathf.PI / (BossSprite.Length));
            pos.y = Distance_y * Mathf.Sin(2 * (i - 2) * Mathf.PI / (BossSprite.Length));
            pos.y *= Y_Size;
            pos.z = pos.y;
            BossSprite[i].transform.position = pos;
            BossSprite[i].transform.localScale = 0.5f * Vector3.one * (4 - BossSprite[i].transform.position.y);
            RotateSave[i] = 2 * (i - 2) * Mathf.PI / (BossSprite.Length);
            BossSprite[i].GetComponentInChildren<Animator>().GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
        }
        outline.enabled = true;
    }

    public void SetActive()
    {
        for (int i = 0; i < BossSprite.Length; i++)
        {
            BossSprite[i].SetActive(true);
        }
        SetDefault();
        Is_Adjust = true;
        is_touching_layer = false;
        outline.enabled = true;
    }

    public void DeActive()
    {
        for (int i = 0; i < BossSprite.Length; i++)
        {
            BossSprite[i].SetActive(false);
        }
        outline.enabled = false;
    }
}
