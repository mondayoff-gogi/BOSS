using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LoadingLevelText : MonoBehaviour
{
    public bool Is_color;
    // Start is called before the first frame update
    void Start()
    {
        switch(UpLoadData.boss_level)
        {
            case 0:
                if(Is_color)
                    this.GetComponent<Text>().color = Color.green;
                this.GetComponent<Text>().text = "easy";
                break;
            case 1:
                if (Is_color)
                    this.GetComponent<Text>().color = Color.yellow;
                this.GetComponent<Text>().text = "normal";
                break;
            case 2:
                if (Is_color)
                    this.GetComponent<Text>().color = Color.red;
                this.GetComponent<Text>().text = "hard";
                break;
            case 3:
                if (Is_color)
                    this.GetComponent<Text>().color = Color.magenta;
                this.GetComponent<Text>().text = "extream";
                break;
            case 4:
                if (Is_color)
                    this.GetComponent<Text>().color = Color.green;
                this.GetComponent<Text>().text = "multi easy";
                break;
            case 5:
                if (Is_color)
                    this.GetComponent<Text>().color = Color.yellow;
                this.GetComponent<Text>().text = "multi normal";
                break;
            case 6:
                if (Is_color)
                    this.GetComponent<Text>().color = Color.red;
                this.GetComponent<Text>().text = "multi hard";
                break;
            case 7:
                if (Is_color)
                    this.GetComponent<Text>().color = Color.magenta;
                this.GetComponent<Text>().text = "multi extream";
                break;
        }
    }

}
