using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    public Image panel;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MakeFadeOut());
    }

    IEnumerator MakeFadeOut()
    {
        Color color = panel.GetComponent<Image>().color;
        while(color.a < 1)
        {
            color.a += 2f * Time.deltaTime;
            panel.GetComponent<Image>().color = color;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

}
