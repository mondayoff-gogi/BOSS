using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    public Button[] level;
    private Color _color;
    public TitleButton _titlebutton;
    public Image next_icon;
    private Vector3 moveposition;
    private float temp = 0f;
    private bool is_clicked = false;
    private Vector3 startposition;
    int _layerMask;
    private Vector2 pos;
    private Sprite default_sprite;
    private Vector2[] button_position;

    private void Awake()
    {
        _layerMask = 1 << LayerMask.NameToLayer("Level");
        button_position = new Vector2[level.Length];
        default_sprite = level[0].GetComponentInChildren<Image>().sprite;
        // 이지 난이도 해금
        startposition = next_icon.transform.localPosition;
        for(int i =0;i <level.Length; i++)
        {
            button_position[i] = level[i].transform.localPosition;
        }
        SetButtonColor();

        if (NetworkManager.instance.is_multi)
        {
            for(int i = 0; i <3; i++)
            {
                Debug.Log(i + 4 + (8 * _titlebutton.boss_index));

                Debug.Log(NetworkManager.instance.boss_level[i + 4 + (8 * _titlebutton.boss_index)]);

                if (NetworkManager.instance.boss_level[i+4 + (8 * _titlebutton.boss_index)])
                {
                    level[i + 1].gameObject.SetActive(true);
                    level[i + 1].gameObject.GetComponent<Image>().enabled = true;
                    level[i + 1].gameObject.GetComponent<Image>().sprite = default_sprite;
                    level[i + 1].gameObject.GetComponentInChildren<Text>().enabled = true;

                }
                else
                {
                    level[i + 1].gameObject.SetActive(true);
                    level[i + 1].gameObject.GetComponentInChildren<Text>().enabled = false;
                    level[i + 1].gameObject.GetComponent<Image>().enabled = true;
                    level[i + 1].gameObject.GetComponent<Image>().sprite = Resources.Load("ItemIcon/Inven/btnIconLock1", typeof(Sprite)) as Sprite;
                }

            }
        }
        else
        {
            // 0번 보스 난이도 해금
            for (int i = 0; i < 3; i++)
            {
                if (UpLoadData.boss_is_cleared[i])
                {
                    level[i + 1].gameObject.SetActive(true);
                    level[i + 1].gameObject.GetComponent<Image>().enabled = true;
                    level[i + 1].gameObject.GetComponent<Image>().sprite = default_sprite;
                    level[i + 1].gameObject.GetComponentInChildren<Text>().enabled = true;

                }
                else
                {
                    level[i + 1].gameObject.SetActive(true);
                    level[i + 1].gameObject.GetComponentInChildren<Text>().enabled = false;
                    level[i + 1].gameObject.GetComponent<Image>().enabled = true;
                    level[i + 1].gameObject.GetComponent<Image>().sprite = Resources.Load("ItemIcon/Inven/btnIconLock1", typeof(Sprite)) as Sprite;
                }
            }
        }


    }
    public void SelectedBoss(int i) //0 easy 3 extream  눌렀을때 색깔 바뀌는 효과
    {
        SoundManager.instance.Play(38);
        StopAllCoroutines();

        if (level[i].GetComponent<Image>().sprite.name == "btnIconLock1")
        {
            StartCoroutine(LockedLevelEffect(i));
            _titlebutton.IsLevelSelect = false;
            SetButtonColor();
            _titlebutton.IsLevelSelect = false;
        }
        else
        {
            SetButtonPosition();
            SetButtonColor();
            StartCoroutine(Blink(i));
            UpLoadData.boss_level = i;
            is_clicked = true;
            _titlebutton.IsLevelSelect = true;
        }

    }

    public void InitBosslevel()
    {
        StopAllCoroutines();
        is_clicked = false;
        SetButtonColor();

    }

    IEnumerator Blink(int i)
    {
        float temp = 0.1f;
        while (true)
        {
            if (_color.a>=1)
            {
                temp = -0.1f;
            }
            else if(_color.a<=0)
            {
                temp = 0.1f;
            }
            _color = level[i].GetComponent<Image>().color;
            _color.a += temp;

            level[i].GetComponent<Image>().color = _color;

            yield return new WaitForSeconds(Time.deltaTime);

        }
    }

    public void ResetButtons()
    {
        StopAllCoroutines();
        SetButtonColor();
        _titlebutton.IsLevelSelect = false;
        is_clicked = false;
    }

    // Start is called before the first frame update

    private void Update()
    {
        if (is_clicked)
        {
            temp += Time.deltaTime * 5f;
            moveposition = new Vector3(Mathf.Cos(temp) * 30f, 6f, next_icon.transform.position.z);
            next_icon.transform.localPosition = moveposition; 
        }
        else
        {
            next_icon.transform.localPosition = startposition;
        }
    }

    public void DeActivate()
    {
        is_clicked = false;
    }

    private void SetButtonColor()
    {
        level[0].gameObject.GetComponent<Image>().color = Color.green;
        level[1].gameObject.GetComponent<Image>().color = Color.yellow;
        level[2].gameObject.GetComponent<Image>().color = Color.red;
        level[3].gameObject.GetComponent<Image>().color = new Color(0.5f,0,0.5f,1f);
    }
    
    private void SetButtonPosition()
    {
        for (int i = 0; i < level.Length; i++)
        {
            level[i].transform.localPosition = button_position[i];
        }
    }

    public void LockLevelButtons()
    {
        if (NetworkManager.instance.is_multi)
        {
            for (int i = 0; i < 3; i++)
            {
                Debug.Log(NetworkManager.instance.boss_level[(i + 4) + (8 * _titlebutton.boss_index)]);
                Debug.Log((i + 4) + (8 * _titlebutton.boss_index));
                

                if (NetworkManager.instance.boss_level[(i+4) + (8 * _titlebutton.boss_index)])
                {
                    level[i + 1].gameObject.SetActive(true);
                    level[i + 1].gameObject.GetComponent<Image>().enabled = true;
                    level[i + 1].gameObject.GetComponent<Image>().sprite = default_sprite;
                    level[i + 1].gameObject.GetComponentInChildren<Text>().enabled = true;

                }
                else
                {
                    level[i + 1].gameObject.SetActive(true);
                    level[i + 1].gameObject.GetComponentInChildren<Text>().enabled = false;
                    level[i + 1].gameObject.GetComponent<Image>().enabled = true;
                    level[i + 1].gameObject.GetComponent<Image>().sprite = Resources.Load("ItemIcon/Inven/btnIconLock1", typeof(Sprite)) as Sprite;
                }

            }
        }
        else
        {
            // 클리어 하면 클리어한 난이도의 다음 난이도 해금
            for (int i = 0; i < 3; i++)
            {
                if (UpLoadData.boss_is_cleared[i + (8 * _titlebutton.boss_index)])
                {
                    level[i + 1].gameObject.SetActive(true);
                    level[i + 1].gameObject.GetComponent<Image>().enabled = true;
                    level[i + 1].gameObject.GetComponent<Image>().sprite = default_sprite;
                    level[i + 1].gameObject.GetComponentInChildren<Text>().enabled = true;
                }
                else
                {
                    level[i + 1].gameObject.SetActive(true);
                    level[i + 1].gameObject.GetComponentInChildren<Text>().enabled = false;
                    level[i + 1].gameObject.GetComponent<Image>().enabled = true;
                    level[i + 1].gameObject.GetComponent<Image>().sprite = Resources.Load("ItemIcon/Inven/btnIconLock1", typeof(Sprite)) as Sprite;
                }
            }
        }

    }

    IEnumerator LockedLevelEffect(int index)
    {
        WaitForSeconds delay_time = new WaitForSeconds(0.01f);
        float time = 0;
        int t = -2;

        for(int i = 0; i < level.Length; i++)
        {
            level[i].transform.localPosition = button_position[i];
            level[i].GetComponent<Image>().transform.localEulerAngles = new Vector3(0, 0, 0);
        }

        while (level[index].GetComponent<Image>().transform.localPosition.y <= 0.5f)
        {
            level[index].GetComponent<Image>().transform.Translate(Vector2.up * 1f * Time.deltaTime);
            yield return delay_time;
        }

        while(time <= 0.5f)
        {
            time += Time.deltaTime;
            level[index].GetComponent<Image>().transform.localEulerAngles = new Vector3(0, 0, t);
            t *= -1;
            yield return delay_time;
        }
        while (level[index].GetComponent<Image>().transform.localPosition.y >= 0f)
        {
            level[index].GetComponent<Image>().transform.Translate(Vector2.down * 1f * Time.deltaTime);
            yield return delay_time;
        }
        level[index].transform.localPosition = button_position[index];
        level[index].GetComponent<Image>().transform.localEulerAngles = new Vector3(0, 0, 0);

        yield return 0;


    }

}
