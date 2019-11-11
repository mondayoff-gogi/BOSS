using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingImage : MonoBehaviour
{
    public Image backgroundImage;
    // Start is called before the first frame update
    void Start()
    {
        backgroundImage.sprite = Resources.Load("Image/"+ UpLoadData.boss_index.ToString(),typeof(Sprite)) as Sprite;
    }

}
