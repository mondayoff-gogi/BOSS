using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DesertBoss_Camera : MonoBehaviour
{
    private Camera _camera;
    //private float plus_size = 1f;
    public UIButton pause_button;
    private Vector3 main_camera_pos;
    private Vector3 this_position;
    private Vector3 boss_position;
    private GameObject player;
    private float startTime;
    private const float totalTime = 3.0f;
    //private float timer;
    private bool flag = false;
    private bool flag1 = false;
    public GameObject _canvas;
    private Text boss_name;
    private Vector2 dir;
    public Image[] cutsceneImage;
    // Start is called before the first frame update
    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 40;
    }
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        boss_name = _canvas.GetComponentInChildren<Text>();
        ChangeBossName();
        boss_position = BossStatus.instance.transform.position;
        //timer = 0;
        _camera = this.gameObject.GetComponent<Camera>();
        this.transform.position = new Vector3(boss_position.x, boss_position.y, -1000f);
        _camera.orthographicSize = 2;
        pause_button.gameObject.SetActive(false);
        boss_name.fontSize = 120;

    }

    private void Update()
    {
        if(_camera.orthographicSize <= 6.5f && !flag1)
        {
            _camera.orthographicSize += Time.deltaTime * 1.5f;
            if(_camera.orthographicSize >= 6.5f)
            {
                _camera.orthographicSize = 6.5f;
                flag1 = true;
                flag = true;
            }
        }

        if (flag)
        {
            cutsceneImage[0].color = Color.clear;
            cutsceneImage[1].color = Color.clear;

            boss_name.text = null;
            dir = player.transform.position - this.transform.position;
            dir.Normalize();
            this.gameObject.transform.Translate(dir * 3 * Time.deltaTime);
        }
        if(Vector2.Distance(this.transform.position, player.transform.position) <= 1f)
        {
            pause_button.gameObject.SetActive(true);
            main_camera_pos.z = -1000f;
            this.gameObject.transform.position = main_camera_pos;
            Destroy(_canvas);
            Destroy(this.gameObject);
        }
    }

    private void ChangeBossName()
    {
        switch (UpLoadData.boss_index)
        {
            case 0:
                boss_name.text = "파라오";
                break;
            case 1:
                boss_name.text = "거대한 드워프";
                break;
            case 2:
                boss_name.text = "겁먹은 메두사";
                break;
            case 3:
                boss_name.text = "이끼낀 골렘";
                break;
            case 4:
                boss_name.text = "퇴역군인 X-23";
                break;
            case 5:
                boss_name.text = "분노한 용왕";
                break;
            case 6:
                boss_name.text = "저주받은 흡혈귀";
                break;
            case 7:
                boss_name.text = "죽지 못하는 자";
                break;
        }
    }

}
