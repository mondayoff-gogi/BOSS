using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{   
    public Image progressBar;

    public Image panel;

    public Text ready_text;

    int flag;

    Color _color;
    Color text_color;
    WaitForSeconds waittime;

    private void Start()
    {
        _color = new Color(0, 0, 0, 0);
        text_color = ready_text.color;
        ready_text.text = "준비 중";
        flag = 1;
        waittime = new WaitForSeconds(0.01f);
        StartCoroutine(LoadScene());
        
        BGMManager.instance.FadeOutMusic();
    }



    IEnumerator LoadScene()
    {
        yield return null;
        AsyncOperation oper = new AsyncOperation();
        switch (UpLoadData.boss_index)
        {
            case 0:
                oper = SceneManager.LoadSceneAsync("boss_1");
                break;
                
            case 1:
                 oper = SceneManager.LoadSceneAsync("boss_2");
                break;
                
            case 2:
                 oper = SceneManager.LoadSceneAsync("boss_3");
                break;
                
            case 3:
                 oper = SceneManager.LoadSceneAsync("boss_4");
                break;
                
            case 4:
                 oper = SceneManager.LoadSceneAsync("boss_5");
                break;
                
            case 5:
                 oper = SceneManager.LoadSceneAsync("boss_6");
                break;
                
            case 6:
                 oper = SceneManager.LoadSceneAsync("boss_7");
                break;
                
            case 7:
                 oper = SceneManager.LoadSceneAsync("boss_8");
                break;

        }
        oper.allowSceneActivation = false;

        float timer = 0.0f;
        while (!oper.isDone)
        {
            yield return null;

            timer += Time.deltaTime;

            if (oper.progress >= 0.9f)
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);

                if (progressBar.fillAmount == 1.0f)
                {
                    if(!NetworkManager.instance.is_multi)
                    {
                        ready_text.text = "시작하려면 아무곳이나 누르세요";

                        while (true)
                        {
                            if (Input.GetMouseButtonDown(0))
                            {
                                break;
                            }
                            yield return waittime;

                            text_color.a -= flag * Time.deltaTime;
                            ready_text.color = text_color;
                            if (text_color.a < 0 || text_color.a > 1)
                            {
                                flag *= -1;
                                text_color.a -= flag * Time.deltaTime;
                            }
                        }


                        while (_color.a < 1)
                        {
                            _color.a += 3f * Time.deltaTime;
                            panel.color = _color;
                            yield return new WaitForSeconds(Time.deltaTime);
                        }
                        oper.allowSceneActivation = true;
                    }
                    else //멀티플레이시
                    {
                        ready_text.text = "동료를 기다리는중 입니다.";

                        NetworkManager.instance.GameReady();

                        while (true)
                        {
                            if (NetworkManager.instance.EveryOneGameReady)
                            {
                                break;
                            }
                            yield return waittime;

                            text_color.a -= flag * Time.deltaTime;
                            ready_text.color = text_color;
                            if (text_color.a < 0 || text_color.a > 1)
                            {
                                flag *= -1;
                                text_color.a -= flag * Time.deltaTime;
                            }
                        }


                        while (_color.a < 1)
                        {
                            _color.a += 3f * Time.deltaTime;
                            panel.color = _color;
                            yield return new WaitForSeconds(Time.deltaTime);
                        }
                        oper.allowSceneActivation = true;
                    }

                }
            }
            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, oper.progress, timer);
                if (progressBar.fillAmount >= oper.progress)
                {
                    timer = 0f;
                }
            }
        }
    }
}
