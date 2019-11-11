using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    public Image panel;
    public GameObject canvas;
    //public Camera main;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MakeFadeOut());
    }

    IEnumerator MakeFadeOut()
    {
        Color color = panel.GetComponent<Image>().color;
        while (color.a > 0)
        {
            color.a -= 1f * Time.deltaTime;
            panel.GetComponent<Image>().color = color;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        //canvas.GetComponent<Canvas>().worldCamera = main;
        canvas.GetComponent<Canvas>().sortingLayerName = "Default";
        canvas.SetActive(false);
    }
}
