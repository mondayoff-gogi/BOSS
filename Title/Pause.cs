using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    private bool is_pause = false;
    public Canvas canvas;
    public Canvas information;
    public GameObject UIRoot;
    public GameObject[] UIRoots;
    private FloatingText[] texts;
    public UIButton pause_button;

    public Canvas waiting;
    public Image numbers;

    private GameObject boss;
    private GameObject player;

    public Sprite[] images;

    public bool is_restarted = false;

    private void Start()
    {
        boss = BossStatus.instance.gameObject;
        player = GameManage.instance.player[0];
        canvas.gameObject.SetActive(false);
        if (NetworkManager.instance.is_multi)
            pause_button.gameObject.SetActive(false);
        else
            pause_button.gameObject.SetActive(true);

    }

    public void PauseButton()
    {
        for (int i = 0; i < UIRoots.Length; i++)
        {
            UIRoots[i].SetActive(false);
        }
        is_pause = !is_pause;
        if (is_pause)
        {
            pause_button.gameObject.SetActive(false);
            StartCoroutine(PausePanel());
        }
        else
        {
            StartCoroutine(ReStartGame());
        }
    }

    public void InformationButton()
    {
        UIRoot.SetActive(false);
        information.gameObject.SetActive(true);
    }

    public void CloseInformation()
    {
        UIRoot.SetActive(true);
        information.gameObject.SetActive(false);
    }

    public void GoToTitle()
    {
        if(GameUI.instance.gameObject != null)
        {
            Destroy(GameUI.instance.gameObject);
            Destroy(GameManage.instance.gameObject);
            Destroy(BossStatus.instance.gameObject);
        }
        SceneManager.LoadScene("Title");
        Time.timeScale = 1f;
    }

    IEnumerator PausePanel()
    {
        texts = UIRoot.GetComponentsInChildren<FloatingText>();
        if (texts.Length > 0)
        {
            for (int i = 0; i < texts.Length; i++)
            {
                Destroy(texts[i].gameObject);
            }
        }
        canvas.gameObject.SetActive(true);
        canvas.GetComponent<Canvas>().enabled = true;

        player.GetComponent<Character_Control>().enabled = false;

        Color color = canvas.GetComponentInChildren<Image>().color;
        while (color.a < 0.5)
        {
            color.a += 3f * Time.deltaTime;
            canvas.GetComponentInChildren<Image>().color = color;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        Time.timeScale = 0;
        yield return 0;
    }

    IEnumerator ReStartGame()
    {
        canvas.GetComponent<Canvas>().enabled = false;

        SceneManager.LoadScene("Test", LoadSceneMode.Additive);
        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 1f;

        for (int i = 0; i < UIRoots.Length; i++)
        {
            UIRoots[i].SetActive(true);
        }

        player.GetComponent<Character_Control>().enabled = true;


        pause_button.gameObject.SetActive(true);

        yield return 0;
    }
}
