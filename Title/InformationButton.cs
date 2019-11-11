using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class InformationButton : MonoBehaviour
{
    private Button button;
    private int boss_index;
    private int boss_level;
    private int char_index;
    private int item_type_index;
    private int item_index;
    Information information;
    public GameObject canvas;
    // Start is called before the first frame update
    void Start()
    {
        button = this.gameObject.GetComponent<Button>();
        information = canvas.GetComponent<Information>();
    }

    public void BossButtonClick()
    {
        SoundManager.instance.Play(37);


        boss_index = Convert.ToInt32(button.name);
        information.GetBossIndex(boss_index);
    }

    public void BossLevelButtonClick()
    {
        SoundManager.instance.Play(37);


        boss_level = Convert.ToInt32(button.name);
        information.GetBossLevelIndex(boss_level);
    }

    public void CharacterButtonClick()
    {
        SoundManager.instance.Play(37);

        char_index = Convert.ToInt32(button.name);
        information.GetCharacterIndex(char_index);
    }

    public void ItemTypeButtonClick()
    {
        SoundManager.instance.Play(37);

        item_type_index = Convert.ToInt32(button.name);
        information.GetItemTypeIndex(item_type_index);
    }


}
